using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schimb_Valutar_API.Services.Interfaces
{
    public interface ICurrencyRateService
    {
        Task<Dictionary<string, decimal>> GetRatesAsync();
    }
}
