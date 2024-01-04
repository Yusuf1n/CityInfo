namespace CityInfo.API.Services;

public class CloudMailService : IMailService
{
    private string _mailTo = "admin@emailprovider.com";
    private string _mailFrom = "noreply@emailprovider.com";

    public void Send(string subject, string message)
    {
        // Send mail - output to console window
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(CloudMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}