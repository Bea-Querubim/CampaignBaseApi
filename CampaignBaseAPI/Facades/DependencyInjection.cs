using CampaignBaseAPI.Facades.Interface;

namespace CampaignBaseAPI.Facades
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFacades(this IServiceCollection services)
        {
            services.AddScoped<IProcessSheetsFacade, ProcessSheetsFacade>();

            return services;
        } 
        
    }
}