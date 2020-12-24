using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts;
using Demo.Api.Generated.Contracts.Orders;

namespace Demo.Domain.Handlers.Orders
{
    public class GetOrderByIdHandler : IGetOrderByIdHandler
    {
        public async Task<GetOrderByIdResult> ExecuteAsync(GetOrderByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return parameters.Id.ToString() switch
            {
                "77a33260-0007-441f-ba60-b0a833803fab" => await Task.FromResult(GetOrderByIdResult.NotFound($"Could not find order with id={parameters.Id}")),
                "77a33260-0008-441f-ba60-b0a833803fab" => throw new Exception("Crash! Boom! Bang! # " + parameters.Id),
                "77a33260-0009-441f-ba60-b0a833803fab" => throw new InvalidOperationException("Crash! Boom! Bang! # " + parameters.Id),
                "77a33260-0010-441f-ba60-b0a833803fab" => throw new UnauthorizedAccessException("Crash! Boom! Bang! # " + parameters.Id),
                "77a33260-0011-441f-ba60-b0a833803fab" => throw new NotImplementedException("Crash! Boom! Bang! # " + parameters.Id),
                _ => await InvokeExecuteAsync(parameters, cancellationToken)
            };
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needed to show as an example.")]
        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Needed to show as an example.")]
        private async Task<GetOrderByIdResult> InvokeExecuteAsync(GetOrderByIdParameters parameters, CancellationToken cancellationToken)
        {
            var data = new Order
            {
                Id = parameters.Id,
                Description = "MyOrder",
                DeliveryAddress = new Address
                {
                    CityName = "Glostrup",
                    PostalCode = "2600",
                    MyCountry = new Country
                    {
                        Name = "Denmark",
                        Alpha2Code = "DK",
                        Alpha3Code = "DNK",
                    },
                },
            };

            return await Task.FromResult(data);
        }
    }
}