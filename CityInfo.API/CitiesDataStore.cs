using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }

    public static  CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        // Initialise dummy data
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "Stoke-on-Trent",
                Description = "Residing town."
            },
            new CityDto()
            {
                Id = 2,
                Name = "Chakswari",
                Description = "Village home."
            },
            new CityDto()
            {
                Id = 3,
                Name = "Islamabad",
                Description = "City home."
            },
        };
    }
}