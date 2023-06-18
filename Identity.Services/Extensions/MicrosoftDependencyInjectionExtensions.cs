using Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Services.Extensions;

public static class MicrosoftDependencyInjectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        //mapper
        services.AddAutoMapper(
            expression => expression.AddMaps(typeof(MicrosoftDependencyInjectionExtensions).Assembly));

        return services
            .AddScoped<IUserStoreService, UserStoreService>()
            .AddScoped<IInvalidateUserTokenService, InvalidateUserTokenService>()
            .AddScoped(typeof(IEmailSender), environment.IsDevelopment() ? typeof(FakeEmailSender) : typeof(EmailSender))
            .AddScoped<IEmailMessageBuilderByEmailType, EmailMessageBuilderByEmailType>()
            .AddScoped<INotificationClient, NotificationClient>()
            .AddScoped<IPasswordHashGenerator, PasswordHashGenerator>();
    }
}