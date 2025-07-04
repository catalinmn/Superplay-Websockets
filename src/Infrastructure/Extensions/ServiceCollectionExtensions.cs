using Core.Abstractions;
using Core.Repositories;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Default")));

        // Add repositories
        services.AddScoped<IPlayerRepository, PlayerRepository>();

        // Add services
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<IGiftService, GiftService>();

        // Add initializer
        services.AddTransient<DbInitializer>();

        return services;
    }
}