global using System.Collections.Concurrent;
global using System.Collections.Immutable;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Net;
global using System.Net.Mime;
global using System.Reflection;
global using System.Text;

global using Atc.CodeDocumentation.CodeComment;
global using Atc.CodeGeneration.CSharp.Content;
global using Atc.CodeGeneration.CSharp.Content.Factories;
global using Atc.CodeGeneration.CSharp.Content.Generators;
global using Atc.CodeGeneration.CSharp.Extensions;
global using Atc.DotNet;
global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.Contracts;
global using Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters;
global using Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters.Client;
global using Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters.Server;
global using Atc.Rest.ApiGenerator.Contracts.Extensions;
global using Atc.Rest.ApiGenerator.Contracts.Models;
global using Atc.Rest.ApiGenerator.Framework.ContentGenerators;
global using Atc.Rest.ApiGenerator.Framework.ContentGeneratorsParameters.Server;
global using Atc.Rest.ApiGenerator.Framework.Extensions;
global using Atc.Rest.ApiGenerator.Framework.Factories.Parameters.ServerClient;
global using Atc.Rest.ApiGenerator.Framework.Factories.Server;
global using Atc.Rest.ApiGenerator.Framework.Helpers;
global using Atc.Rest.ApiGenerator.Framework.Providers;
global using Atc.Rest.ApiGenerator.Framework.Readers;
global using Atc.Rest.ApiGenerator.Framework.ToRefactor;
global using Atc.Rest.ApiGenerator.Framework.Writers;
global using Atc.Rest.ApiGenerator.Nuget.Clients;
global using Atc.Rest.ApiGenerator.OpenApi.Extensions;
global using Atc.Rest.ApiGenerator.OpenApi.Extractors;
global using Atc.Rest.ApiGenerator.OpenApi.Models;
global using Atc.Rest.ApiGenerator.OpenApi.Readers;

global using Microsoft.Extensions.Logging;
global using Microsoft.OpenApi.Any;
global using Microsoft.OpenApi.Models;
global using Microsoft.OpenApi.Readers;
global using Microsoft.OpenApi.Writers;