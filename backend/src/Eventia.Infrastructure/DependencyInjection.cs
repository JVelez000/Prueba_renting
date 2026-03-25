using Eventia.Application.Interfaces;
using Eventia.Domain.Interfaces;
using Eventia.Infrastructure.Persistence;
using Eventia.Infrastructure.Repositories;
using Eventia.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eventia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core — PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work (AppDbContext implements IUnitOfWork)
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        // Application services
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
