using CityInfo.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Services;

public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();

    Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

    Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

    Task<bool> CityExistsAsync(int cityId);

    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

    Task<(IEnumerable<PointOfInterest>, PaginationMetadata)> GetPointsOfInterestForCityAsync(int cityId, [FromQuery] string? name, string? searchQuery, int pageNumber, int pageSize);

    Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterest);

    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

    void DeletePointOfInterest(PointOfInterest pointOfInterest);

    Task<bool> CityNameMatchesCityId(string? cityName, int? cityId);

    Task<bool> SaveChangesAsync();
}