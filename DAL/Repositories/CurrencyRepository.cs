using Schimb_Valutar_API.DAL.Repositories.Interfaces;
using Schimb_Valutar_API.DAL.DBO;
using Microsoft.EntityFrameworkCore;

namespace Schimb_Valutar_API.DAL.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyDbContext _dbContext;

        public CurrencyRepository(CurrencyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Currency currency)
        {
            _dbContext.Currency.Add(currency);
            _dbContext.SaveChangesAsync();
        }

        public async Task<Currency> GetById (int id)
        {
            return await _dbContext.Currency.FindAsync(id);
        }

        public async Task<List<Currency>> GetAll()
        {
            var results = _dbContext.Currency.ToList();
            return results;
        }
        public async Task Update(Currency currency)
        {
            _dbContext.Currency.Entry(currency).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Currency> GetCurrencyByAbbreviationAsync(string abbreviation)
        {
            var lowerCaseAbbreviation = abbreviation.ToLower();
            return await _dbContext.Currency
                .AsNoTracking() 
                .FirstOrDefaultAsync(c => c.abreviation.ToLower() == lowerCaseAbbreviation);
        }
    }
}
