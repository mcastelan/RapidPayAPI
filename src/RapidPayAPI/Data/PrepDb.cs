using RapidPayAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace RapidPayAPI.Data
{
    public class PrepDb
    {
        private static bool _isProduction;

       
        public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProduction)
        {
             var scopeFactory = applicationBuilder!.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = scopeFactory.CreateScope();
            _isProduction = isProduction;
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var dbContext =serviceScope.ServiceProvider.GetService<AppDbContext>();
                Migrate(dbContext, logger);
                SeedBankAccountData(dbContext, logger);
           

        }
        private static void Migrate(AppDbContext context, ILogger<Program> logger)
        {
            logger.LogInformation($"Seeding Data , IsProduction - {_isProduction}...");
            if (_isProduction)
            {
                logger.LogInformation("Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();

                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Could not migrate the database{ex.Message}");
                }

            }
        }
        
public async static  Task SeedIdentityData(IApplicationBuilder applicationBuilder)
{
    var scopeFactory = applicationBuilder!.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
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


        // private async static void SeedIdentityData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<Program> logger)
        // {
        //     if (!userManager.Users.Any())
        //     {
        //         logger.LogInformation("Creating test user");

        //         var newUser = new User
        //         {
        //             Email = "mcastelan@rapidpay.com",
        //             FirstName = "Test",
        //             LastName = "User",
        //             UserName = "mcastelan.rapidpay"
        //         };

        //         await userManager.CreateAsync(newUser, "P@55.W0rd");
        //         await roleManager.CreateAsync(new IdentityRole
        //         {
        //             Name = "Admin"
        //         });
        //         await roleManager.CreateAsync(new IdentityRole
        //         {
        //             Name = "AnotherRole"
        //         });

        //         await userManager.AddToRoleAsync(newUser, "Admin");
        //         await userManager.AddToRoleAsync(newUser, "AnotherRole");

        //     }
        //     else
        //     {
        //         logger.LogInformation("-->We already have identity data");
        //     }
        // }

        private static void SeedBankAccountData(AppDbContext context, ILogger<Program> logger)
        {
            if (!context.BankAccounts.Any())
            {
                Bank centralBank = new Bank { Name = "Central Bank" };
                Bank bankOfAmerica = new Bank { Name = "Bank Of America" };
                context.Banks.AddRange(centralBank, bankOfAmerica);
                logger.LogInformation("-->Seeding Bank Account data ...");
                context.BankAccounts.AddRange(
                    new BankAccount()
                    {
                        //Id = 1,
                        Bank = centralBank,
                        FirstName = "John",
                        LastName = "Doe",
                        Balance = 100,
                        CardNumber = "4111111111111111",
                        AccountType = AccountType.Savings
                    },
                     new BankAccount()
                     {
                         //Id = 2,
                         Bank = centralBank,
                         FirstName = "Jane",
                         LastName = "Doe",
                         Balance = 50,
                         CardNumber = "4111111111111112",
                         AccountType = AccountType.Savings
                     },
                     new BankAccount()
                     {
                         //Id = 3,
                         Bank = bankOfAmerica,
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                     }
                );
                context.SaveChanges();


            }
            else
            {
                logger.LogInformation("-->We already have bank account data");
            }

        }
    }
}