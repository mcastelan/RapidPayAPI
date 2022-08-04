using RapidPayAPI.Models;

namespace RapidPayAPI.Data
{
    public interface IRapidPayRepo
    {
        bool SaveChanges();

        IEnumerable<BankAccount> GetAllBankAccounts();
         IEnumerable<Bank> GetAllBanks();


        void CreateBankAccount(BankAccount bankAccount);

        bool BankAccountExists(int bankAccountId);
        bool CardNumberExists(string cardNumber);
        BankAccount GetBankAccountById(int id);
        BankAccount GetBankAccountByCardNumber(string cardNumber);
        void UpdateBankAccount(BankAccount bankAccount);

        void CreatePaymentHistory(PaymentHistory paymentHistory);
    }
}