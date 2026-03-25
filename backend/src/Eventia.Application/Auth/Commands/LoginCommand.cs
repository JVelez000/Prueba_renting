using Eventia.Application.DTOs;
using Eventia.Domain.Interfaces;
using MediatR;

namespace Eventia.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public class LoginCommandHandler(IUserRepository userRepo, Interfaces.IJwtService jwtService) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepo.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is disabled.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
        return new LoginResponse(token, user.Name, user.Email, user.Role.ToString(), user.Id);
    }
}
