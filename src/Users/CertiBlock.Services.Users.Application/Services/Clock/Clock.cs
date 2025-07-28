namespace CertiBlock.Services.Users.Application.Services.Clock;

public class Clock : IClock
{
    public DateTime CurrentDate() => DateTime.UtcNow;
}