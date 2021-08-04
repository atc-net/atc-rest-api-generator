﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.219.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Users.Generated
{
    using Demo.Api.Generated.Contracts;
    using Demo.Api.Generated.Contracts.Users;

    [GeneratedCode("ApiGenerator", "1.1.219.0")]
    public class GetUsersHandlerStub : IGetUsersHandler
    {
        public Task<GetUsersResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var data = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("77a33260-0001-441f-ba60-b0a833803fab"),
                    Gender = GenderType.Female,
                    FirstName = "Hallo11",
                    LastName = "Hallo12",
                    Email = "john1.doe@example.com",
                    Homepage = new Uri("http://www.dr.dk"),
                    Color = ColorType.Red,
                    HomeAddress = new Address
                    {
                        StreetName = "Hallo1",
                        StreetNumber = "Hallo11",
                        PostalCode = "Hallo12",
                        CityName = "Hallo13",
                        MyCountry = new Country
                        {
                            Name = "Hallo11",
                            Alpha2Code = "Ha",
                            Alpha3Code = "Hal",
                        },
                    },
                    CompanyAddress = new Address
                    {
                        StreetName = "Hallo1",
                        StreetNumber = "Hallo11",
                        PostalCode = "Hallo12",
                        CityName = "Hallo13",
                        MyCountry = new Country
                        {
                            Name = "Hallo11",
                            Alpha2Code = "Ha",
                            Alpha3Code = "Hal",
                        },
                    },
                },
                new User
                {
                    Id = Guid.Parse("77a33260-0002-441f-ba60-b0a833803fab"),
                    Gender = GenderType.Female,
                    FirstName = "Hallo21",
                    LastName = "Hallo22",
                    Email = "john2.doe@example.com",
                    Homepage = new Uri("http://www.dr.dk"),
                    Color = ColorType.Red,
                    HomeAddress = new Address
                    {
                        StreetName = "Hallo2",
                        StreetNumber = "Hallo21",
                        PostalCode = "Hallo22",
                        CityName = "Hallo23",
                        MyCountry = new Country
                        {
                            Name = "Hallo21",
                            Alpha2Code = "Ha",
                            Alpha3Code = "Hal",
                        },
                    },
                    CompanyAddress = new Address
                    {
                        StreetName = "Hallo2",
                        StreetNumber = "Hallo21",
                        PostalCode = "Hallo22",
                        CityName = "Hallo23",
                        MyCountry = new Country
                        {
                            Name = "Hallo21",
                            Alpha2Code = "Ha",
                            Alpha3Code = "Hal",
                        },
                    },
                },
                new User
                {
                    Id = Guid.Parse("77a33260-0003-441f-ba60-b0a833803fab"),
                    Gender = GenderType.Female,
                    FirstName = "Hallo31",
                    LastName = "Hallo32",
                    Email = "john3.doe@example.com",
                    Homepage = new Uri("http://www.dr.dk"),
                    Color = ColorType.Red,
                    HomeAddress = new Address
                    {
                        StreetName = "Hallo3",
                        StreetNumber = "Hallo31",
                        PostalCode = "Hallo32",
                        CityName = "Hallo33",
                        MyCountry = new Country
                        {
                            Name = "Hallo31",
                            Alpha2Code = "Ha",
                            Alpha3Code = "Hal",
                        },
                    },
                    CompanyAddress = new Address
                    {
                        StreetName = "Hallo3",
                        StreetNumber = "Hallo31",
                        PostalCode = "Hallo32",
                        CityName = "Hallo33",
                        MyCountry = new Country
                        {
                            Name = "Hallo31",
                            Alpha2Code = "Ha",
                            Alpha3Code = "Hal",
                        },
                    },
                },
            };

            return System.Threading.Tasks.Task.FromResult(GetUsersResult.Ok(data));
        }
    }
}