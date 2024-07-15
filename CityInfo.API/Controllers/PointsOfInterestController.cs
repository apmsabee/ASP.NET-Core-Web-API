using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsOfInterest")]
    [Authorize(Policy = "MustBeFromAntwerp")]
    [ApiController]
    [ApiVersion(2)]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService localMailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> getPointsOfInterest(int cityId)
        {
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if(!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} was not found when accessing POIs");
                return NotFound();
            }

            var pointsOfInterest = await _cityInfoRepository.GetPointsOfInterestAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));
        }

        [HttpGet("{POIId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> getPointOfInterest(int cityId, int POIId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} was not found when accessing POIs");
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestAsync(cityId, POIId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationdto pointOfInterest)
        {

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} was not found when posting POIs");
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPOIToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    POIId = createdPOIToReturn.Id
                },
                createdPOIToReturn);
        }

        [HttpPut("{POIId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int POIId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} was not found when updating POIs");
                return NotFound();
            }

            var storePOI = await _cityInfoRepository
                .GetPointOfInterestAsync(cityId, POIId);
            if (storePOI == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, storePOI);
            await _cityInfoRepository.SaveChangesAsync();
            //HTTP standard says Put should fully update the resource
            //storePOI.Name = pointOfInterest.Name;
            //storePOI.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{POIId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int POIId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} was not found when updating POIs");
                return NotFound();
            }

            var storePOI = await _cityInfoRepository
                .GetPointOfInterestAsync(cityId, POIId);
            if (storePOI == null)
            {
                return NotFound();
            }

            var patchPOI = _mapper.Map<PointOfInterestForUpdateDto>(storePOI);

            patchDocument.ApplyTo(patchPOI, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchPOI))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(patchPOI, storePOI);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{POIId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int POIId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} was not found when updating POIs");
                return NotFound();
            }
            
            var storePOI = await _cityInfoRepository
                .GetPointOfInterestAsync(cityId, POIId);
            if (storePOI == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(storePOI);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {storePOI.Name} with id {storePOI.Id} was deleted.");

            return NoContent();
        }
    }
}
