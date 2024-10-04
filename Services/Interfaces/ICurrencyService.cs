using Schimb_Valutar_API.Models;

namespace Schimb_Valutar_API.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task CreateCurrencyAsync(CurrencyModel currency);
        Task<List<CurrencyModel>> GetAllCurrenciesAsync();
        Task UpdateCurrencyAsync(CurrencyModel currency);
        Task UpdateAllCurrenciesFromBnrAsync();
        Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount);
    }

}
