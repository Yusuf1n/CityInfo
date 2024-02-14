namespace CityInfo.API.Models.Identity;

public class AuthenticationRequestBody
{
    public string? UserName { get; set; }

    public string? Password { get; set; }
}