namespace RapidPayAPI.Dtos
{
    public class BankAccountReadDto{
         
        public int Id{get;set;}
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CardNumber{get;set;}

        public decimal Balance {get;set;}

    }
}