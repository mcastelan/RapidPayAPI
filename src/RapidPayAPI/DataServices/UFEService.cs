
using System;
using System.Threading.Tasks;


namespace RapidPayAPI.Services
{

    public class UFEService :IDisposable
    {
        private decimal feeRate=0.0m;
        private decimal feeDecimal;
        private PeriodicTimer _timer;
        Random random = new Random();
        private readonly ILogger<UFEService> _logger;

        public  decimal LastFeeRate{
            get{return feeRate;}
            set{feeRate=value;}
        }
        
        
        public UFEService(ILogger<UFEService> logger)
        {
            this._logger =logger;
            int randomBetween0And2 = random.Next(1, 20);  
            feeDecimal = randomBetween0And2*0.1m;
            _logger.LogInformation($"Fee Decimal - {feeDecimal}");
            
        }
        public async void StartUFEService()
        {
            _logger.LogInformation("UFE Service is starting.");

          
            
             
              
             _timer= new PeriodicTimer(TimeSpan.FromMinutes(60));
    
            while (await _timer.WaitForNextTickAsync())
            {
                  var rndBetween0And2 = random.Next(0, 20);  
                feeDecimal = rndBetween0And2*0.1m;
                
                feeRate = feeRate*feeDecimal;
                _logger.LogInformation($"Fee Decimal - {feeDecimal}, Fee Rate - {feeRate}");
            }
        }
       

       
       

        public void Dispose()
        {
           
           _timer?.Dispose();
        }
    }
}