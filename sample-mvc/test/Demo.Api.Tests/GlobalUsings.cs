global using System.CodeDom.Compiler;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;

global using Atc.Rest.Options;
global using Atc.Rest.Results;
global using Atc.XUnit;

global using Demo.Api.Generated;
global using Demo.Api.Generated.Contracts;
global using Demo.Api.Generated.Contracts.Accounts;
global using Demo.Api.Generated.Contracts.Addresses;
global using Demo.Api.Generated.Contracts.EventArgs;
global using Demo.Api.Generated.Contracts.Files;
global using Demo.Api.Generated.Contracts.Items;
global using Demo.Api.Generated.Contracts.Orders;
global using Demo.Api.Generated.Contracts.RouteWithDash;
global using Demo.Api.Generated.Contracts.Users;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;