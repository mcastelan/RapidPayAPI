using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidPayAPI.Data;
using RapidPayAPI.Dtos;
using RapidPayAPI.Models;
using RapidPayAPI.Services;

namespace RapidPayAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class RapidPayController:ControllerBase
    {
        private readonly IRapidPayRepo _repository;
        private readonly ILogger<RapidPayController> _logger;
        private readonly IMapper _mapper;
        private readonly UFEService _ufeService;

        public RapidPayController(IRapidPayRepo repository
                                    , IMapper mapper
                                    ,ILogger<RapidPayController> logger
                                    ,UFEService ufeService)
        {
            this._repository = repository;
            this._logger=logger;
            this._mapper = mapper;
            this._ufeService=ufeService;
        }
        [HttpGet()]
       
        public async Task<ActionResult<IEnumerable<BankAccountReadDto>>> GetAllBankAccounts()
        {
            _logger.LogDebug("--> Getting BankAccounts....");

            var bankAccountItems= _repository.GetAllBankAccounts();
            
            return Ok(_mapper.Map<IEnumerable<BankAccountReadDto>>(bankAccountItems));
        }
        [HttpGet()]
        public async Task <ActionResult<IEnumerable<BankReadDto>>> GetAllBanks()
        {
            _logger.LogDebug("--> Getting Banks....");

            var bankItems= _repository.GetAllBanks();
            
            return Ok(_mapper.Map<IEnumerable<BankReadDto>>(bankItems));
        }

         [HttpGet("{bankaccountid}")]
        public async Task<ActionResult<BankAccountReadDto>> GetBankAccountById(int id)
        {
              var platformItem= _repository.GetBankAccountById(id);

            if(platformItem!=null)
            {
                return Ok(_mapper.Map<BankAccountReadDto>(platformItem));
            }
            else
                return NotFound();
        }
        [HttpGet("{bankaccountid}")]
        public async Task<ActionResult<AccountBalanceDto>> GetBalance(int id)
        {
            var bankAccountItem= _repository.GetBankAccountById(id);

            if(bankAccountItem!=null)
            {
                return Ok(_mapper.Map<AccountBalanceDto>(bankAccountItem));
            }
            else
                return NotFound();
        }

        [HttpPost()]
        public async Task<ActionResult<BankAccountReadDto>> CreateBankAccount(BankAccountCreateDto bankAccountCreateDto)
        {
            var bankAccount = _mapper.Map<BankAccount>(bankAccountCreateDto);
            if(!_repository.CardNumberExists(bankAccount.CardNumber))
            {
                _repository.CreateBankAccount(bankAccount);
                _repository.SaveChanges();
                var bankAccountReadDto = _mapper.Map<BankAccountReadDto>(bankAccount);
            
            
                return CreatedAtRoute(nameof(GetBankAccountById),new {Id=bankAccount.Id},bankAccount);
            }
            else
            {
                return NotFound();
            }
            
        }

        [HttpPut("{bankaccountid}")]
        public async Task<ActionResult> Pay(int bankaccountid, BankAccountPayDto bankAccountPayDto)
        {
           
           var paymentHistory = new PaymentHistory(); 
            var bankAccount = _repository.GetBankAccountById(bankaccountid);//GetBankAccountByCardNumber(bankAccountPayDto.CardNumber);
            if (bankAccount == null)
            {
                return NotFound();
            }
            var balanceBeforePay= bankAccount.Balance;
            
            if(bankAccountPayDto.PayType==PayType.Deposit)
            {
                             
                bankAccount.Balance+=bankAccountPayDto.Amount;
                
            }
            else if(bankAccountPayDto.PayType == PayType.Withdraw)
            {
                             
                bankAccount.Balance-=bankAccountPayDto.Amount;
            }
            else
            {
                 return BadRequest("Invalid Payment Type");
            }
            var feeRateAmountApplied = bankAccountPayDto.Amount*_ufeService.LastFeeRate;
          
            //Applying feeRateAmount to the Balance
            bankAccount.Balance -= feeRateAmountApplied;


            //Creating Payment History
            paymentHistory.BalanceBeforePayment = balanceBeforePay;
            paymentHistory.BalanceAfterPayment = bankAccount.Balance;
            paymentHistory.FeeAmountApplied = feeRateAmountApplied;
            paymentHistory.FeeRate = _ufeService.LastFeeRate;
            paymentHistory.PaymentAmount = bankAccountPayDto.Amount;
            paymentHistory.InsDateTime = DateTimeOffset.UtcNow;
           
         
             try
             {
             _logger.LogInformation($"LastFeeRate={_ufeService.LastFeeRate}, LastFeeAmountApplied = {feeRateAmountApplied}");
            _repository.UpdateBankAccount(bankAccount);
             paymentHistory.BankAccountId = bankAccount.Id;
             _repository.CreatePaymentHistory(paymentHistory);
            _repository.SaveChanges();
             }
             catch(Exception ex)
             {
                _logger.LogError(ex.Message);
                return BadRequest();
             }
            
            return NoContent();
        }


    }
}