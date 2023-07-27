global using System.CodeDom.Compiler;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;

global using Atc.Rest.Options;
global using Atc.Rest.Results;
global using Atc.XUnit;

global using DemoSampleApi.Api.Generated;
global using DemoSampleApi.Api.Generated.Contracts;
global using DemoSampleApi.Api.Generated.Contracts.Accounts;
global using DemoSampleApi.Api.Generated.Contracts.Addresses;
global using DemoSampleApi.Api.Generated.Contracts.EventArgs;
global using DemoSampleApi.Api.Generated.Contracts.Files;
global using DemoSampleApi.Api.Generated.Contracts.Items;
global using DemoSampleApi.Api.Generated.Contracts.Orders;
global using DemoSampleApi.Api.Generated.Contracts.RouteWithDash;
global using DemoSampleApi.Api.Generated.Contracts.Users;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;