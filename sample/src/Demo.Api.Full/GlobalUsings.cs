global using System.Reflection;

global using Atc;
global using Atc.Rest.Extended.Options;
global using Atc.Rest.Options;

global using Demo.Api.Full.Configuration;
global using Demo.Api.Generated;
global using Demo.Api.Generated.Contracts.Orders;
global using Demo.Api.Generated.Contracts.Users;
global using Demo.Domain;
global using Demo.Domain.Handlers.Orders;
global using Demo.Domain.Handlers.Users;
global using Demo.Domain.Validators.Users;

global using FluentValidation;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;
global using Microsoft.OpenApi.Models;

global using Swashbuckle.AspNetCore.SwaggerGen;