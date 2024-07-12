﻿namespace CityInfo.API.Models;

/// <summary>
/// A DTO for a cities points of interest
/// </summary>
public class PointOfInterestDto
{
    /// <summary>
    /// The id of the city
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the city
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the city
    /// </summary>
    public string? Description { get; set; }
}