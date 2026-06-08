using TechMoveMVC.Interfaces;
using TechMoveMVC.Services;

namespace TechMoveMVC.Strategies
{
    public class USDStrategy : ICurrencyStrategy
    {
        private readonly CurrencyService _service;

        public USDStrategy(CurrencyService service)
        {
            _service = service;
        }

        public async Task<decimal> Convert(decimal amount)
        {
            var rate = await _service.GetUsdToZarRate();
            return amount * rate;
        }
    }
}