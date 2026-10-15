using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Repository;
using MiniCrm.Domain.Services;
using MiniCrm.Worker.Notification;

namespace Microsoft.Extensions.DependencyInjection;

public static class MiniCrmServiceCollectionExtensions
{
    public static IServiceCollection AddMiniCrmDomain(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStore>(); // register InMemoryStore as a Singleton
        services.TryAddEnumerable(ServiceDescriptor.Scoped<ICustomerValidator, NameRequiredValidator>()); // register ICustomerValidator with its implementation
        services.TryAddEnumerable(ServiceDescriptor.Scoped<ICustomerValidator, EmailFormatValidator>()); // register ICustomerValidator with its implementation
        services.TryAddScoped<ICustomerRepository, InMemoryCustomerRepository>(); // register ICustomerRepository with its implementation
        services.TryAddScoped<ICustomerService, CustomerService>(); // register ICustomerService with its implementation

        return services;
    }

    // public static IServiceCollection AddNotificationServices(this IServiceCollection services, IConfiguration config)
    // {
    //     services.AddOptions<NotificationOptions>()
    //             .Bind(config.GetSection(NotificationOptions.SectionName))
    //             .ValidateOnStart(); // bind NotificationOptions from configuration and validate

    //     services.AddKeyedSingleton<INotificationSender, EmailNotificationSender>(NotificationChannels.Email); // register EmailNotificationSender as a Singleton
    //     services.AddKeyedSingleton<INotificationSender, SmsNotificationSender>(NotificationChannels.Sms); // register SmsNotificationSender as a Singleton

    //     services.AddSingleton(sp =>
    //     {
    //         var opts = sp.GetRequiredService<IOptions<NotificationOptions>>().Value;
    //         return sp.GetRequiredKeyedService<INotificationSender>(opts.Default);
    //     });

    //     return services;
    // }
}