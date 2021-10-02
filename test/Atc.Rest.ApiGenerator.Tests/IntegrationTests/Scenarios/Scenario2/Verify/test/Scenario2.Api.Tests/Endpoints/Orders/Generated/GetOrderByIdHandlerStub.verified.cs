﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scenario2.Api.Generated.Contracts;
using Scenario2.Api.Generated.Contracts.Orders;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Tests.Endpoints.Orders.Generated
{
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class GetOrderByIdHandlerStub : IGetOrderByIdHandler
    {
        public Task<GetOrderByIdResult> ExecuteAsync(GetOrderByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            var data = new Order
            {
                Id = Guid.Parse("77a33260-0000-441f-ba60-b0a833803fab"),
                Description = "Hallo1",
                MyTime = DateTimeOffset.Parse("2020-10-12T21:22:23"),
                MyEmail = "john.doe@example.com",
                MyNullableDateTime = DateTimeOffset.Parse("2020-10-12T21:22:23"),
                MyDateTime = DateTimeOffset.Parse("2020-10-12T21:22:23"),
                MyNumber = 20.2,
                MyInteger = 42,
                MyBool = true,
                MyUri = new Uri("http://www.dr.dk"),
                MyByte = "Hallo10",
                MyStringList = null,
                MyLong = 42,
                DeliveryAddress = new Address
                {
                    StreetName = "Hallo",
                    StreetNumber = "Hallo1",
                    PostalCode = "Hallo2",
                    CityName = "Hallo3",
                    MyCountry = new Country
                    {
                        Name = "Hallo1",
                        Alpha2Code = "Ha",
                        Alpha3Code = "Hal",
                    },
                },
            };

            return Task.FromResult(GetOrderByIdResult.Ok(data));
        }
    }
}