global using System.CodeDom.Compiler;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;

global using Atc.Rest.Options;
global using Atc.Rest.Results;
global using Atc.XUnit;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using TestUnit.Task.NsApi.Api.Generated;
global using TestUnit.Task.NsApi.Api.Generated.Contracts;
global using TestUnit.Task.NsApi.Api.Generated.Contracts.EventArgs;
global using TestUnit.Task.NsApi.Api.Generated.Contracts.Orders;
global using TestUnit.Task.NsApi.Api.Generated.Contracts.TestUnits;