using Schimb_Valutar_API.DAL.DBO;

namespace Schimb_Valutar_API.DAL.Repositories.Interfaces
{
    public interface ICurrencyRepository
    {
        Task Create(Currency currency);
        Task<Currency> GetById (int id);
        Task<List<Currency>> GetAll();
        Task Update(Currency currency);
        Task<Currency> GetCurrencyByAbbreviationAsync(string abbreviation);
    }
}
