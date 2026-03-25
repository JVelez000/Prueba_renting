using Eventia.Application.DTOs;
using Eventia.Domain.Interfaces;
using MediatR;

namespace Eventia.Application.Users.Queries;

// --- Get All Users ---
public record GetUsersQuery : IRequest<IEnumerable<UserDto>>;

public class GetUsersQueryHandler(IUserRepository userRepo) : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await userRepo.GetAllAsync(ct);
        return users.Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role.ToString(), u.IsActive, u.CreatedAt));
    }
}

// --- Get User By Id ---
public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;

public class GetUserByIdQueryHandler(IUserRepository userRepo) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("User not found.");
        return new UserDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt);
    }
}
