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

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
    {
        try
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");

                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            _logger.LogInformation($"Successfully created point of interest '{createdPointOfInterestToReturn.Name}' (id: {createdPointOfInterestToReturn.Id}) for city with id {cityId}");

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while creating point of interest for city with id {cityId}.", ex);

            return StatusCode(500, "A problem occurred while handling your request");
        }
    }

    [HttpPut("{pointofinterestid}")]
    public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
    {
        try
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");

                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                _logger.LogInformation($"Point of interest with id {pointOfInterestId} wasn't found for City id {cityId}");

                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated point of interest with id {pointOfInterestId} for City id {cityId}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while updating point of interest {pointOfInterestId} for city with id {cityId}.", ex);

            return StatusCode(500, "A problem occurred while handling your request");
        }
    }

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