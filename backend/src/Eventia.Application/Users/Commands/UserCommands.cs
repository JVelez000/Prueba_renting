using Eventia.Application.DTOs;
using Eventia.Domain.Entities;
using Eventia.Domain.Interfaces;
using Eventia.Domain.ValueObjects;
using MediatR;

namespace Eventia.Application.Users.Commands;

// --- Create User ---
public record CreateUserCommand(string Name, string Email, string Password, string? Role) : IRequest<UserDto>;

public class CreateUserCommandHandler(IUserRepository userRepo, IUnitOfWork uow) : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var existing = await userRepo.GetByEmailAsync(request.Email, ct);
        if (existing != null) throw new InvalidOperationException("Email already registered.");

        // Default to Agent if no role is provided or the value is empty/invalid
        var role = UserRole.Agent;
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out role))
                role = UserRole.Agent;
        }

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.Name, request.Email, hash, role);
        await userRepo.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);
        return new UserDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt);
    }
}

// --- Disable User ---
public record DisableUserCommand(Guid UserId) : IRequest;

public class DisableUserCommandHandler(IUserRepository userRepo, IUnitOfWork uow) : IRequestHandler<DisableUserCommand>
{
    public async Task Handle(DisableUserCommand request, CancellationToken ct)
    {
        var user = await userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("User not found.");
        user.Disable();
        userRepo.Update(user);
        await uow.SaveChangesAsync(ct);
    }
}

// --- Enable User ---
public record EnableUserCommand(Guid UserId) : IRequest;

public class EnableUserCommandHandler(IUserRepository userRepo, IUnitOfWork uow) : IRequestHandler<EnableUserCommand>
{
    public async Task Handle(EnableUserCommand request, CancellationToken ct)
    {
        var user = await userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("User not found.");
        user.Enable();
        userRepo.Update(user);
        await uow.SaveChangesAsync(ct);
    }
}

// --- Update User Role ---
public record UpdateUserRoleCommand(Guid UserId, string Role) : IRequest<UserDto>;

public class UpdateUserRoleCommandHandler(IUserRepository userRepo, IUnitOfWork uow) : IRequestHandler<UpdateUserRoleCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserRoleCommand request, CancellationToken ct)
    {
        var user = await userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("User not found.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            throw new ArgumentException($"Invalid role: {request.Role}. Valid roles: Agent, Supervisor, Admin.");

        user.UpdateRole(role);
        userRepo.Update(user);
        await uow.SaveChangesAsync(ct);
        return new UserDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt);
    }
}
