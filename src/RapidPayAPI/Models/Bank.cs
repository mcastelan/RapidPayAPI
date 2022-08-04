namespace RapidPayAPI.Models
{
    public class Bank
    {
        public int Id { get; set; }

        public string Name { get; set; }  

        public virtual IList<BankAccount> BankAccounts { get; set; }  
    }
}