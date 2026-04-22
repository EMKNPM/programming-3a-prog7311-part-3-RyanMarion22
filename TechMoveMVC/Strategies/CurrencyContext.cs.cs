using TechMoveMVC.Interfaces;

namespace TechMoveMVC.Strategies
{
    public class CurrencyContext
    {
        private ICurrencyStrategy _strategy;

        public void SetStrategy(ICurrencyStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<decimal> Convert(decimal amount)
        {
            return await _strategy.Convert(amount);
        }
    }
}