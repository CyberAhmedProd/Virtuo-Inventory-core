using VirtuoInventory.Application.Interfaces;
using VirtuoInventory.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace VirtuoInventory.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}