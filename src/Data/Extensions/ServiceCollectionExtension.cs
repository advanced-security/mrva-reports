using Microsoft.Extensions.DependencyInjection;
using MRVA.Reports.Data.Services;

namespace MRVA.Reports.Data.Extensions;

public static class ServiceCollectionExtension
{

    public static IServiceCollection AddReportData(this IServiceCollection services)
    {
        services.AddSingleton<DataStore>();
        return services;
    }

    public static async Task InitializeReportDataAsync(this IServiceProvider services, byte[] dbBytes)
    {
        var dataStore = services.GetRequiredService<DataStore>();
        await dataStore.InitializeAsync(dbBytes);
    }
    
}