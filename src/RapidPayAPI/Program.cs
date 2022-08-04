using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RapidPayAPI;
using RapidPayAPI.Data;
using RapidPayAPI.Models;
using RapidPayAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.SqlConfiguration(builder.Configuration,builder.Environment.IsProduction());

builder.Services.JwtConfiguration(builder.Configuration);
builder.Services.AddScoped<IRapidPayRepo, RapidPayRepo>();
builder.Services.AddSingleton<UFEService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

var frontendurl = builder.Configuration.GetValue<string>("frontend_url");
builder.Services.CorsConfiguration();
//builder.Services.CorsConfiguration(frontendurl); //for specify an unique frontendurl




builder.Services.AddEndpointsApiExplorer();

builder.Services.SwaggerConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.ConfigureExceptionMiddleware();//Handling errors globally

//Seed BankAccountData
PrepDb.PrepPopulation(app, app.Environment.IsProduction());

//Seed Identity Data In Production Environment
await PrepDb.SeedIdentityData(app);


var ufeService = app.Services.GetRequiredService<UFEService>();

ufeService.StartUFEService();


app.Run();



