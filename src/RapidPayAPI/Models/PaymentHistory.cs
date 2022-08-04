using System.ComponentModel.DataAnnotations;

namespace RapidPayAPI.Models
{
    public class PaymentHistory
    {
        [Key]
        [Required]
        public int Id { get; set; } 

       public BankAccount BankAccount { get; set; }
        [Required]
        public int BankAccountId { get; set; }

       
        [Required]
        public decimal BalanceBeforePayment {get;set;}

         [Required]
        public decimal BalanceAfterPayment {get;set;}

       [Required]
        public decimal FeeRate{get;set;}

        [Required]
        public decimal FeeAmountApplied{get;set;}

       [Required]
        public decimal PaymentAmount{get;set;}

    }
}