using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CityInfo.API.Models
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; } 
        public static CitiesDataStore Current { get; set; } = new CitiesDataStore();
        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name =  "Oshkosh",
                    Description = "My hometown! Not much going on here, but we do have the wonderful Lake Winnebago",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Lake Winnebago",
                            Description = "A Large (albeit pretty nasty) body of water with plenty of waterfront activites based on it"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "County Park",
                            Description = "A public park with plenty of walking trails, tennis/basketball courts, open fields, and a baseball diamond. Great for hosting events!"
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name =  "Madison",
                    Description = "My college town(and still current town)! A young, vibrant city with a lot to do",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "State Street",
                            Description = "A 6-block stretch from Bascom Hill up to the State Capitol. Great shopping area with some historic campus spots not far away."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "Leopold's",
                            Description = "A bookstore, bar, coffee shop, oyster bar, restaurant, deli, and live jazz venue in the summer. Everything you could ask for!"
                        }

                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name =  "Denver",
                    Description = "My hopeful next stop in life, but who's to say? My brother Adam lives there now.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 5,
                            Name = "The Rocky Mountains",
                            Description = "Gigantic Mountains. Need I say more?"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 6,
                            Name = "Mile High Stadium",
                            Description = "Home of the Denver Broncos, bless their hearts."
                        },
                    }
                }
            };
        }
    }
}
