global using System;
global using System.CodeDom.Compiler;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;

global using Atc.Rest.Client;
global using Atc.Rest.Client.Builder;

global using DemoSampleApi.ApiClient.Generated.Contracts;
global using DemoSampleApi.ApiClient.Generated.Contracts.Accounts;
global using DemoSampleApi.ApiClient.Generated.Contracts.Addresses;
global using DemoSampleApi.ApiClient.Generated.Contracts.EventArgs;
global using DemoSampleApi.ApiClient.Generated.Contracts.Files;
global using DemoSampleApi.ApiClient.Generated.Contracts.Items;
global using DemoSampleApi.ApiClient.Generated.Contracts.Orders;
global using DemoSampleApi.ApiClient.Generated.Contracts.Tasks;
global using DemoSampleApi.ApiClient.Generated.Contracts.Users;
global using DemoSampleApi.ApiClient.Generated.Endpoints.Accounts.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.Addresses.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.EventArgs.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.Files.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.Items.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.Orders.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.RouteWithDash.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.Tasks.Interfaces;
global using DemoSampleApi.ApiClient.Generated.Endpoints.Users.Interfaces;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;