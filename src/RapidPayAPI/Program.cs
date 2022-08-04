using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RapidPayAPI.Data;
using RapidPayAPI.Models;
using RapidPayAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

 if(builder.Environment.IsProduction())
 {
Console.WriteLine("-->using  Sql Db");
builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("RapidPayConn")))
.AddIdentityCore<User>()
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();



 }
 else
 {
    Console.WriteLine("-->using in Mem Db" );
     builder.Services.AddDbContext<AppDbContext>(
                 opt=>opt.UseInMemoryDatabase("InMem"))
                  .AddIdentityCore<User>()
                  .AddRoles<IdentityRole>()
                 .AddEntityFrameworkStores<AppDbContext>();
 }
builder.Services

    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddScoped<IRapidPayRepo, RapidPayRepo>();
builder.Services.AddSingleton<UFEService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RapidPayAPI",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



//Seed BankAccountData
PrepDb.PrepPopulation(app, app.Environment.IsProduction());

//Seed Identity Data In Production Environment
await SeedIdentityData();


var ufeService = app.Services.GetRequiredService<UFEService>();

ufeService.StartUFEService();


app.Run();



async Task SeedIdentityData()
{
    var scopeFactory = app!.Services.GetRequiredService<IServiceScopeFactory>();
    using var scope = scopeFactory.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    //context.Database.EnsureCreated();

    if (!userManager.Users.Any())
    {
        logger.LogInformation("Creating test user");

        var newUser = new User
        {
            Email = "mcastelan@rapidpay.com",
            FirstName = "Test",
            LastName = "User",
            UserName = "mcastelan.rapidpay"
        };

        await userManager.CreateAsync(newUser, "P@55.W0rd");
        await roleManager.CreateAsync(new IdentityRole
        {
            Name = "Admin"
        });
        await roleManager.CreateAsync(new IdentityRole
        {
            Name = "AnotherRole"
        });

        await userManager.AddToRoleAsync(newUser, "Admin");
        await userManager.AddToRoleAsync(newUser, "AnotherRole");

    }
    else
    {
        logger.LogInformation("-->We already have identity data");
    }

}
