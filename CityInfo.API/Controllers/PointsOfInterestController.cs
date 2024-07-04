using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsOfInterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService localMailService,
            CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(_citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> getPointsOfInterest(int cityId)
        {
                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with ID {cityId} could not be found when accessing points of interest");
                    return NotFound();
                }
                return Ok(city.PointsOfInterest);

        }

        [HttpGet("{POIId}", Name = "GetPointOfInterest")]
        public ActionResult<IEnumerable<PointOfInterestDto>> getPointOfInterest(int cityId, int POIId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var POI = city.PointsOfInterest.FirstOrDefault(c => c.Id == POIId);

            if (POI == null)
            {
                return NotFound();
            }
            return Ok(POI);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationdto pointOfInterest)
        {

            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    POIId = finalPointOfInterest.Id
                },
                finalPointOfInterest);
        }

        [HttpPut("{POIId}")]
        public ActionResult UpdatePointOfInterest(int cityId, int POIId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var storePOI = city.PointsOfInterest.FirstOrDefault(c => c.Id == POIId);
            if (storePOI == null)
            {
                return NotFound();
            }

            //HTTP standard says Put should fully update the resource
            storePOI.Name = pointOfInterest.Name;
            storePOI.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{POIId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int POIId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var storePOI = city.PointsOfInterest.FirstOrDefault(c => c.Id == POIId);
            if (storePOI == null)
            {
                return NotFound();
            }

            //HTTP standard says Put should fully update the resource
            var patchPOI = new PointOfInterestForUpdateDto()
            {

                Name = storePOI.Name,
                Description = storePOI.Description
            };

            patchDocument.ApplyTo(patchPOI, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchPOI))
            {
                return BadRequest(ModelState);
            }

            storePOI.Name = patchPOI.Name;
            storePOI.Description = patchPOI.Description;

            return NoContent();
        }

        [HttpDelete("{POIId}")]
        public ActionResult DeletePointOfInterest(int cityId, int POIId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var storePOI = city.PointsOfInterest.FirstOrDefault(c => c.Id == POIId);
            if (storePOI == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(storePOI);

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {storePOI.Name} with id {storePOI.Id} was deleted.");

            return NoContent();
        }
    }
}
