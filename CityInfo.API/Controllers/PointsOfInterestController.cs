using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers;

[Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
[Authorize(Policy = "MustBeFromStokeOnTrent")]
[ApiVersion("2.0")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    private const int maxPageSize = 20;

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

    /// <summary>
    /// Get points of interest for a city, includes the ability to search and filter,
    /// </summary>
    /// <param name="cityId">The id of the city related to the points of interest to get</param>
    /// <param name="name">Filtering the points of interest to get</param>
    /// <param name="searchQuery">Searching the points of interest to get</param>
    /// <returns>An ActionResult</returns>
    /// <returns> code="200">Returns the requested point of interest</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId, [FromQuery] string? name, [FromQuery] string? searchQuery, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize > maxPageSize)
        {
            pageSize = maxPageSize;
        }

        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogError($"City with id {cityId} wasn't found when accessing points of interest.");

            return NotFound();
        }

        var (pointsOfInterestForCityEntities, paginationMetadata) = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId, name, searchQuery, pageNumber, pageSize);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCityEntities));
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

    [HttpPatch("{pointofinterestid}")]
    public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
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

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                _logger.LogError("A problem occurred while handling your request");

                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                _logger.LogError("A problem occurred while handling your request");

                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            _logger.LogInformation($"Successfully partially updated point of interest with id {pointOfInterestId} for City id {cityId}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while partially updating point of interest {pointOfInterestId} for city with id {cityId}.", ex);

            return StatusCode(500, "A problem occurred while handling your request");
        }
    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> Delete(int cityId, int pointOfInterestId)
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

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted");

            _logger.LogInformation($"Successfully deleted point of interest with id {pointOfInterestId} for City id {cityId}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while deleting point of interest {pointOfInterestId} for city with id {cityId}.", ex);

            return StatusCode(500, "A problem occurred while handling your request");
        }
    }
}