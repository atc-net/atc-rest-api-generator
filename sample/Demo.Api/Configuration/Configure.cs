using Demo.Api.Generated.Contracts.Orders;
using Demo.Api.Generated.Contracts.Users;
using Demo.Domain.Handlers.Orders;
using Demo.Domain.Handlers.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Api.Configuration
{
    public static class Configure
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IGetOrderByIdHandler, GetOrderByIdHandler>();
            services.AddTransient<IGetUserByIdHandler, GetUserByIdHandler>();
            services.AddTransient<IGetUserByEmailHandler, GetUserByEmailHandler>();
        }
    }
}