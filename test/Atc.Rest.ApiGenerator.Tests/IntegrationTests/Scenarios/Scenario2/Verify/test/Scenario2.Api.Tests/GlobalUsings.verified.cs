global using System;
global using System.CodeDom.Compiler;
global using System.Collections.Generic;
global using System.IO;
global using System.Net.Http;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;

global using Atc.Rest.Options;
global using Atc.Rest.Results;
global using Atc.XUnit;

global using AutoFixture;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Scenario2.Api.Generated;
global using Scenario2.Api.Generated.Contracts;
global using Scenario2.Api.Generated.Contracts.Accounts;
global using Scenario2.Api.Generated.Contracts.Addresses;
global using Scenario2.Api.Generated.Contracts.EventArgs;
global using Scenario2.Api.Generated.Contracts.Files;
global using Scenario2.Api.Generated.Contracts.Items;
global using Scenario2.Api.Generated.Contracts.List;
global using Scenario2.Api.Generated.Contracts.Orders;
global using Scenario2.Api.Generated.Contracts.Pagination;
global using Scenario2.Api.Generated.Contracts.RouteWithDash;
global using Scenario2.Api.Generated.Contracts.Users;

global using Xunit;