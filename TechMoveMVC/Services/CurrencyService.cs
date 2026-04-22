using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using TechMoveMVC.Data;
using TechMoveMVC.Services;

namespace TechMoveMVC.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRate()
        {
            var response = await _httpClient.GetStringAsync("https://api.exchangerate-api.com/v4/latest/USD");
            dynamic data = JsonConvert.DeserializeObject(response);
            return (decimal)data.rates.ZAR;
        }
    }
}