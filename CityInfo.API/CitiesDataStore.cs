using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }

    public CitiesDataStore()
    {
        // Initialise dummy data
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "Stoke-on-Trent",
                Description = "Residing town.",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto() {
                        Id = 1,
                        Name = "The Potteries Museum & Art Gallery",
                        Description = "Displays locally made ceramics, decorative arts and a WWII Spitfire." },
                    new PointOfInterestDto() {
                        Id = 2,
                        Name = "Wedgwood Museum",
                        Description = "£10 million Wedgwood Museum visitor centre." }
                }
            },
            new CityDto()
            {
                Id = 2,
                Name = "Chakswari",
                Description = "Village home.",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto() {
                        Id = 3,
                        Name = "Mangla Dam",
                        Description = "The sixth-largest dam in the world." },
                    new PointOfInterestDto() {
                        Id = 4,
                        Name = "Chakswari Grand Central Jamia Mosque",
                        Description = "The main masjid in Chakswari." }
                }
            },
            new CityDto()
            {
                Id = 3,
                Name = "Islamabad",
                Description = "City home.",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto() {
                        Id = 5,
                        Name = "Faisal Mosque",
                        Description = "The fifth-largest mosque in the world and the largest within South Asia, located on the foothills of Margalla Hills in Islamabad. It is named after the late King Faisal of Saudi Arabia." },
                    new PointOfInterestDto() {
                        Id = 6,
                        Name = "Pakistan Monument",
                        Description = "A national monument and heritage museum located on the western Shakarparian Hills in Islamabad, Pakistan." },
                }
            },
        };
    }
}