using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniCrm.Infrastructure.Persistence;

namespace MiniCrm.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddMiniCrmInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MiniCrm")
            ?? throw new InvalidOperationException("Connection string 'MiniCrm' not found.");
        
        services.AddDbContext<MiniCrmDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            }));

        return services;
    }
}