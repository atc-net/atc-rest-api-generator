global using System.CodeDom.Compiler;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;

global using Atc.Rest.Options;
global using Atc.XUnit;

global using AutoFixture;

global using ExNsWithTask.Api.Generated;
global using ExNsWithTask.Api.Generated.Contracts;
global using ExNsWithTask.Api.Generated.Contracts.EventArgs;
global using ExNsWithTask.Api.Generated.Contracts.Orders;
global using ExNsWithTask.Api.Generated.Contracts.TestUnits;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Xunit;