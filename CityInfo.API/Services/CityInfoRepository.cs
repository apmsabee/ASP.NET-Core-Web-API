using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;
        public CityInfoRepository(CityInfoContext context) 
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c=> c.Id).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePOIs)
        {
            return await _context.Cities;
        }

        public Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int POIId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId)
        {
            throw new NotImplementedException();
        }
    }
}
