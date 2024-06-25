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
global using Atc.Rest.Results;

global using DemoSample.ApiClient.Generated.Contracts;
global using DemoSample.ApiClient.Generated.Contracts.Accounts;
global using DemoSample.ApiClient.Generated.Contracts.Addresses;
global using DemoSample.ApiClient.Generated.Contracts.EventArgs;
global using DemoSample.ApiClient.Generated.Contracts.Files;
global using DemoSample.ApiClient.Generated.Contracts.Items;
global using DemoSample.ApiClient.Generated.Contracts.Orders;
global using DemoSample.ApiClient.Generated.Contracts.Tasks;
global using DemoSample.ApiClient.Generated.Contracts.Users;
global using DemoSample.ApiClient.Generated.Endpoints.Accounts.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.Addresses.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.EventArgs.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.Files.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.Items.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.Orders.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.RouteWithDash.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.Tasks.Interfaces;
global using DemoSample.ApiClient.Generated.Endpoints.Users.Interfaces;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;