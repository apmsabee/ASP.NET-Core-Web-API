using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync(); //could also be IQueryable, but for our DB/API complexity IEnumerable is fine
        Task<City?> GetCityAsync(int cityId, bool includePOIs);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int POIId);
        Task<bool> CityExistsAsync(int cityId);
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
    }
}
