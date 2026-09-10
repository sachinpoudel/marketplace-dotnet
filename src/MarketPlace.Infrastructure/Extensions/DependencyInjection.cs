using System.Xml.Serialization;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Infrastructure.Extensions;
using MarketPlace.Infrastructure.Identity.Services;
using MarketPlace.Infrastructure.Persistence.Repositories;
using MarketPlace.Infrastructure.Persistence.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MarketPlace.Infrastructure.Extensions;


public static  class InfrastructureExtensions
{
     public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
  

        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IHttpContextProvider, HttpContextProvider>();

        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        return services;
    }

   
}