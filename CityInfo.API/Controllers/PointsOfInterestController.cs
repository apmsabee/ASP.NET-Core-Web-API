using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsOfInterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> getPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{POIId}", Name = "GetPointOfInterest")]
        public ActionResult<IEnumerable<PointOfInterestDto>> getPointOfInterest(int cityId, int POIId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

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

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
            storePOI.Description= patchPOI.Description;

            return NoContent();
        }

        [HttpDelete("{POIId}")]
        public ActionResult DeletePointOfInterest(int cityId, int POIId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
            return NoContent();
        }
    }
}
