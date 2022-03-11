using Demo.Api.Generated.Contracts.Orders;
using Demo.Api.Generated.Contracts.Users;
using Demo.Domain.Handlers.Orders;
using Demo.Domain.Handlers.Users;
using Demo.Domain.Validators.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Api.Full.Configuration
{
    public static class Configure
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            // Handlers
            services.AddTransient<IGetOrderByIdHandler, GetOrderByIdHandler>();
            services.AddTransient<IGetOrdersHandler, GetOrdersHandler>();
            services.AddTransient<IPatchOrdersIdHandler, PatchOrdersIdHandler>();
            services.AddTransient<IDeleteUserByIdHandler, DeleteUserByIdHandler>();
            services.AddTransient<IGetUserByEmailHandler, GetUserByEmailHandler>();
            services.AddTransient<IGetUserByIdHandler, GetUserByIdHandler>();
            services.AddTransient<IGetUsersHandler, GetUsersHandler>();
            services.AddTransient<IPostUserHandler, PostUserHandler>();
            services.AddTransient<IUpdateMyTestGenderHandler, UpdateMyTestGenderHandler>();
            services.AddTransient<IUpdateUserByIdHandler, UpdateUserByIdHandler>();

            // Validators
            services.AddTransient<IValidator<PostUserParameters>, PostUserParametersValidator>();
        }
    }
}