using CertiBlock.Services.Users.Core.ValueObjects;

namespace CertiBlock.Services.Users.Core.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public User(Guid userId, string email, string password, Role role, bool isActive, DateTime createdAt)
    {
        UserId = userId;
        Email = email;
        Password = password;
        Role = role;
        IsActive = isActive;
        CreatedAt = createdAt;
    }
}