using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RapidPayAPI.Models;

namespace RapidPayAPI.Data
{
    public class AppDbContext:IdentityDbContext<User>//DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt)
        {
            
        }
         
         
        public DbSet<Bank> Banks{get;set;}
        public  DbSet<BankAccount> BankAccounts{get;set;}

        public  DbSet<PaymentHistory> PaymentHistories{get;set;}
      
    }
}