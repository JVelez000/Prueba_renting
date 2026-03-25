using Eventia.Domain.Common;
using Eventia.Domain.ValueObjects;

namespace Eventia.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    // EF Core
    protected User() { }

    public static User Create(string name, string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password is required.");

        return new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Role = role
        };
    }

    public void Disable() { IsActive = false; Touch(); }
    public void Enable() { IsActive = true; Touch(); }

    public void UpdateRole(UserRole newRole) { Role = newRole; Touch(); }

    public void UpdateProfile(string name)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        Touch();
    }
}
