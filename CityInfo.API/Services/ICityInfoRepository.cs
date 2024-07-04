using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync(); //could also be IQueryable, but for our DB/API complexity IEnumerable is fine
        Task<City?> GetCityAsync(int cityId, bool includePOIs);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int POIId);


    }
}
