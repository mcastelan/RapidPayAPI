using System.ComponentModel.DataAnnotations;

namespace RapidPayAPI.Models
{
    public class BankAccount
    {
        [Key]
        [Required]
        public int Id { get; set; } 


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Bank Bank { get; set; }

        [Required]
        public int BankId { get; set; }

        public AccountType AccountType{get;set;}

        [Required]
        public decimal Balance {get;set;}

     

        [CreditCard]
        public string CardNumber{get;set;}

         public virtual IList<PaymentHistory> PaymentHistories { get; set; }  

        public BankAccount(string firstName, string lastName, decimal initialBalance)
        {
            this.FirstName = firstName;
            this.LastName  =lastName;
            this.Balance = Balance;    
        }
        public BankAccount()
        {
            
        }
    }
}