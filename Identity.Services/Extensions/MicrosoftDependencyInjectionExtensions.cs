using Identity.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services.Extensions;

public static class MicrosoftDependencyInjectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        //mapper
        services.AddAutoMapper(
            expression => expression.AddMaps(typeof(MicrosoftDependencyInjectionExtensions).Assembly));

        return services
            .AddScoped<IUserStoreService, UserStoreService>();
    }
}