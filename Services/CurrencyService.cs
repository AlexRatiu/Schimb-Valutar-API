using Schimb_Valutar_API.DAL.DBO;
using Schimb_Valutar_API.DAL.Repositories.Interfaces;
using Schimb_Valutar_API.Models;
using Schimb_Valutar_API.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ICurrencyRateService _currencyRateService;

    public CurrencyService(ICurrencyRepository currencyRepository, ICurrencyRateService currencyRateService)
    {
        _currencyRepository = currencyRepository;
        _currencyRateService = currencyRateService;
    }

    public async Task CreateCurrencyAsync(CurrencyModel currency)
    {
        var newCurrency = new Currency
        {
            abreviation = currency.Abreviation,
            value = currency.Value
        };

        await _currencyRepository.Create(newCurrency);
    }

    public async Task<List<CurrencyModel>> GetAllCurrenciesAsync()
    {
        var currencies = await _currencyRepository.GetAll();
        return currencies.Select(c => new CurrencyModel
        {
            Id = c.id,
            Abreviation = c.abreviation,
            Value = c.value
        }).ToList();
    }

    public async Task UpdateCurrencyAsync(CurrencyModel currency)
    {
        var currencyToUpdate = new Currency
        {
            id = currency.Id,
            abreviation = currency.Abreviation,
            value = currency.Value
        };

        await _currencyRepository.Update(currencyToUpdate);
    }

    public async Task UpdateAllCurrenciesFromBnrAsync()
    {
        var rates = await _currencyRateService.GetRatesAsync();
        var currencies = await _currencyRepository.GetAll();

        var existingAbbreviations = currencies.Select(c => c.abreviation).ToHashSet();

        foreach (var rate in rates)
        {
            if (existingAbbreviations.Contains(rate.Key))
            {
                var currency = currencies.First(c => c.abreviation == rate.Key);
                currency.value = rate.Value ;
                await _currencyRepository.Update(currency);
            }
            else
            {
                var newCurrency = new Currency
                {
                    abreviation = rate.Key,
                    value = rate.Value
                };
                await _currencyRepository.Create(newCurrency);
            }
        }
    }

    public async Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
    {
        var fromCurrencyData = await _currencyRepository.GetCurrencyByAbbreviationAsync(fromCurrency);
        var toCurrencyData = await _currencyRepository.GetCurrencyByAbbreviationAsync(toCurrency);

        if (fromCurrencyData == null)
        {
            throw new ArgumentException($"Currency '{fromCurrency}' not supported.");
        }

        if (toCurrencyData == null)
        {
            throw new ArgumentException($"Currency '{toCurrency}' not supported.");
        }

        decimal convertedValue = (amount * fromCurrencyData.value) / toCurrencyData.value;

        return convertedValue;
    }
}
