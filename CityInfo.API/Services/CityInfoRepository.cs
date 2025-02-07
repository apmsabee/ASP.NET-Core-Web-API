﻿using CityInfo.API.DbContexts;
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
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }
        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery,
            int pageNumber, int pageSize)
        {
            var collection = _context.Cities as IQueryable<City>;

            if ((!string.IsNullOrEmpty(name)))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(s => s.Name.Contains(searchQuery)
                || (s.Description != null && s.Description.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();
            var metadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return (collectionToReturn, metadata);
        }
        public async Task<City?> GetCityAsync(int cityId, bool includePOIs)
        {
            if (includePOIs)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Cities
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
        }
        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }
        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int POIId)
        {
            return await _context.PointsOfInterest
                    .Where(c => c.CityId == cityId && c.Id == POIId)
                    .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId)
        {
            return await _context.PointsOfInterest
                    .Where(c => c.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if ((city != null))
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public async Task<bool> CityNameMatchesCityId(string cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
    }
}
