using System.ComponentModel.DataAnnotations;

namespace RapidPayAPI.Dtos
{
    public class BankAccountCreateDto{
         
       

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber{get;set;}

        [Required]
        public int BankId { get; set; }

        [Required]
        public decimal  InitialBalance { get; set; }

    }
}