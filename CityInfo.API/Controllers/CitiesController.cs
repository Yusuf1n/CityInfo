using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoCityInfoRepository;

    public CitiesController(ICityInfoRepository cityInfoCityInfoRepository)
    {
        _cityInfoCityInfoRepository = cityInfoCityInfoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
    {
        var cityEntities = await _cityInfoCityInfoRepository.GetCitiesAsync();

        var results = new List<CityWithoutPointsOfInterestDto>();

        foreach (var cityEntity in cityEntities)
        {
            results.Add(new CityWithoutPointsOfInterestDto
            {
                Id = cityEntity.Id,
                Name = cityEntity.Name,
                Description = cityEntity.Description
            });
        }

        return Ok(results);
    }

    //[HttpGet("{id}")]
    //public ActionResult<CityDto> GetCity(int id)
    //{
    //    // Find City
    //    var cityToReturn = _citiesDataStore.Cities
    //        .FirstOrDefault(c => c.Id == id);

    //    if (cityToReturn == null)
    //    {
    //        return NotFound();
    //    }

    //    return Ok(cityToReturn);
    //}
}