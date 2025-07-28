namespace CertiBlock.Services.Users.Core.DTO;

public class AccountDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}