namespace TechMoveMVC.Interfaces
{
    public interface ICurrencyStrategy
    {
        Task<decimal> Convert(decimal amount);
    }
}