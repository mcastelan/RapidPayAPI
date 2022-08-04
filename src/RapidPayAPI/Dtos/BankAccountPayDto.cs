using System.ComponentModel.DataAnnotations;

namespace RapidPayAPI.Dtos
{
    public class BankAccountPayDto{
         
         [Required]
         public string CardNumber { get; set; } 

       
       [Required]
        public Decimal Amount{get;set;}


        [Required]
        public PayType PayType{get;set;}

    }

    
}