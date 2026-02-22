using CampaignBaseAPI.Services.Interface;

namespace CampaignBaseAPI.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IConverterFileService, ConverterFileService>();
            services.AddScoped<IZipService, ZipService>();
            services.AddScoped<IPartitionSheetsService, PartitionSheetsService>();
            services.AddScoped<IBaseValidationService, BaseValidationService>();

            return services;
        }
        
    }
}