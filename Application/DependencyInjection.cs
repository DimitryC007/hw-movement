using Application.Data;
using Application.Data;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDataService, DataService>();
        services.AddValidatorsFromAssemblyContaining<DataValidator>();

        return services;
    }
}
