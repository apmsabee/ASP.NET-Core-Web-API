using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;
        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper) 
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name, string? searchQuery,
            int pageNumber = 1, int pageSize = 10)
        {
            if(pageSize > maxPageSize)
            {
                pageNumber = maxPageSize;
            }
            var (cityEntities, pagination) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(pagination));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePOIs = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePOIs);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
