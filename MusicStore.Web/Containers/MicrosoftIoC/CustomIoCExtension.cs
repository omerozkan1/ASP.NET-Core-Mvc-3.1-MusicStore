using Microsoft.Extensions.DependencyInjection;
using MusicStore.DataAccess.Interfaces;
using MusicStore.DataAccess.Repositories;

namespace MusicStore.Web.Containers.MicrosoftIoC
{
    public static class CustomIoCExtension
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
