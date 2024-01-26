using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
        IMailService mailService,
        ICityInfoRepository cityInfoRepository,
        IMapper mapper)
    {
        _logger = logger 
                  ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService 
                  ?? throw new ArgumentNullException(nameof(mailService));
        _cityInfoRepository = cityInfoRepository 
                  ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper 
                  ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogError($"City with id {cityId} wasn't found when accessing points of interest.");

            return NotFound();
        }

        var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogError($"City with id {cityId} wasn't found when accessing points of interest.");

            return NotFound();
        }

        var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

        if (pointOfInterest == null)
        {
            _logger.LogInformation($"Point of interest with id {pointOfInterestId} wasn't found for City id {cityId}");

            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
    }

    //[HttpPost]
    //public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterestDto)
    //{
    //    try
    //    {
    //        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

    //        if (city == null)
    //        {
    //            _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");

    //            return NotFound();
    //        }

    //        var maxPointOfInterestId = _citiesDataStore.Cities
    //            .SelectMany(c => c.PointsOfInterest)
    //            .Max(p => p.Id);

    //        var finalPointOfInterest = new PointOfInterestDto()
    //        {
    //            Id = ++maxPointOfInterestId,
    //            Name = pointOfInterestDto.Name,
    //            Description = pointOfInterestDto.Description,
    //        };

    //        city.PointsOfInterest.Add(finalPointOfInterest);

    //        _logger.LogInformation($"Successfully created point of interest '{finalPointOfInterest.Name}' (id: {finalPointOfInterest.Id}) for city with id {cityId}");

    //        return CreatedAtRoute("GetPointOfInterest",
    //            new
    //            {
    //                cityId = cityId,
    //                pointOfInterestId = finalPointOfInterest.Id
    //            },
    //            finalPointOfInterest);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogCritical($"Exception while creating point of interest for city with id {cityId}.", ex);

    //        return StatusCode(500, "A problem occurred while handling your request");
    //    }
    //}

    //[HttpPut("{pointofinterestid}")]
    //public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
    //{
    //    try
    //    {
    //        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

    //        if (city == null)
    //        {
    //            _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");

    //            return NotFound();
    //        }

    //        // Find the point of interest
    //        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

    //        if (pointOfInterestFromStore == null)
    //        {
    //            _logger.LogInformation($"Point of interest with id {pointOfInterestId} wasn't found for City id {cityId}");

    //            return NotFound();
    //        }

    //        pointOfInterestFromStore.Name = pointOfInterest.Name;
    //        pointOfInterestFromStore.Description = pointOfInterest.Description;

    //        _logger.LogInformation($"Successfully updated point of interest with id {pointOfInterestId} for City id {cityId}");

    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogCritical($"Exception while updating point of interest {pointOfInterestId} for city with id {cityId}.", ex);

    //        return StatusCode(500, "A problem occurred while handling your request");
    //    }
    //}

    //[HttpPatch("{pointofinterestid}")]
    //public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    //{
    //    try
    //    {
    //        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

    //        if (city == null)
    //        {
    //            _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");

    //            return NotFound();
    //        }

    //        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

    //        if (pointOfInterestFromStore == null)
    //        {
    //            _logger.LogInformation($"Point of interest with id {pointOfInterestId} wasn't found for City id {cityId}");

    //            return NotFound();
    //        }

    //        var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
    //        {
    //            Name = pointOfInterestFromStore.Name,
    //            Description = pointOfInterestFromStore.Description,
    //        };

    //        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

    //        if (!ModelState.IsValid)
    //        {
    //            _logger.LogError("A problem occurred while handling your request");

    //            return BadRequest();
    //        }

    //        if (!TryValidateModel(pointOfInterestToPatch))
    //        {
    //            _logger.LogError("A problem occurred while handling your request");

    //            return BadRequest(ModelState);
    //        }

    //        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
    //        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

    //        _logger.LogInformation($"Successfully partially updated point of interest with id {pointOfInterestId} for City id {cityId}");

    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogCritical($"Exception while partially updating point of interest {pointOfInterestId} for city with id {cityId}.", ex);

    //        return StatusCode(500, "A problem occurred while handling your request");
    //    }
    //}

    //[HttpDelete("{pointOfInterestId}")]
    //public ActionResult Delete(int cityId, int pointOfInterestId)
    //{
    //    try
    //    {
    //        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

    //        if (city == null)
    //        {
    //            _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");

    //            return NotFound();
    //        }

    //        var pointOfInterestFromStore = city.PointsOfInterest
    //            .FirstOrDefault(c => c.Id == pointOfInterestId);

    //        if (pointOfInterestFromStore == null)
    //        {
    //            _logger.LogInformation($"Point of interest with id {pointOfInterestId} wasn't found for City id {cityId}");

    //            return NotFound();
    //        }

    //        city.PointsOfInterest.Remove(pointOfInterestFromStore);

    //        _mailService.Send(
    //            "Point of interest deleted.",
    //            $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted");
            
    //        _logger.LogInformation($"Successfully deleted point of interest with id {pointOfInterestId} for City id {cityId}");

    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogCritical($"Exception while deleting point of interest {pointOfInterestId} for city with id {cityId}.", ex);

    //        return StatusCode(500, "A problem occurred while handling your request");
    //    }
    //}
}