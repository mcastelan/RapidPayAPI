using RapidPayAPI.Models;

namespace RapidPayAPI.Data
{
    public class RapidPayRepo : IRapidPayRepo
    {
        private readonly AppDbContext _context;

        public RapidPayRepo(AppDbContext context)
        {
            this._context = context;
        }
        public bool BankAccountExists(int bankAccountId)
        {
             return _context.BankAccounts.Any(p => p.Id == bankAccountId);
        }
        public bool CardNumberExists(string cardNumber)
        {
           return _context.BankAccounts.Any(p => p.CardNumber == cardNumber);
        }
        public void CreateBankAccount(BankAccount bankAccount)
        {
            if(bankAccount==null)
            {
                throw new ArgumentNullException(nameof(bankAccount));
            }
            _context.BankAccounts.Add(bankAccount);
        }

        public IEnumerable<BankAccount> GetAllBankAccounts()
        {
           return _context.BankAccounts.ToList();
        }
        public IEnumerable<Bank> GetAllBanks()
        {
           return _context.Banks.ToList();
        }

        public BankAccount GetBankAccountById(int id)
        {
           return _context.BankAccounts.FirstOrDefault(p=>p.Id==id);
        }

        public BankAccount GetBankAccountByCardNumber(string cardNumber)
        {
           return _context.BankAccounts.FirstOrDefault(p=>p.CardNumber==cardNumber);
        }

        public bool SaveChanges()
        {
            return(_context.SaveChanges()>=0);
        }

         public void UpdateBankAccount(BankAccount bankAccount)
        {
            if (bankAccount == null)
            {
                throw new ArgumentNullException(nameof(bankAccount));
            }
            _context.BankAccounts.Update (bankAccount);
        }

        public void CreatePaymentHistory(PaymentHistory paymentHistory)
        {
            if(paymentHistory==null)
            {
                throw new ArgumentNullException(nameof(paymentHistory));
            }
            _context.PaymentHistories.Add(paymentHistory);
        }
    }
}