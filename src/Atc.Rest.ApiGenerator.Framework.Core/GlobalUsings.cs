global using System.Diagnostics.CodeAnalysis;
global using System.Reflection;
global using System.Text;

global using Atc.CodeDocumentation.CodeComment;
global using Atc.CodeGeneration.CSharp.Content;
global using Atc.CodeGeneration.CSharp.Content.Factories;
global using Atc.CodeGeneration.CSharp.Content.Generators;
global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.Framework.ContentGenerators;
global using Atc.Rest.ApiGenerator.Framework.Contracts;
global using Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;
global using Atc.Rest.ApiGenerator.Framework.Contracts.Extensions;
global using Atc.Rest.ApiGenerator.Framework.Contracts.Models;
global using Atc.Rest.ApiGenerator.Framework.Core.ContentGeneratorsParameters.Server;
global using Atc.Rest.ApiGenerator.Framework.Core.Factories.Server;
global using Atc.Rest.ApiGenerator.Framework.Core.Providers;
global using Atc.Rest.ApiGenerator.Framework.Helpers;
global using Atc.Rest.ApiGenerator.Framework.ProjectGenerator;
global using Atc.Rest.ApiGenerator.Framework.Writers;
global using Atc.Rest.ApiGenerator.Nuget.Clients;
global using Atc.Rest.ApiGenerator.OpenApi.Extensions;

global using Microsoft.Extensions.Logging;
global using Microsoft.OpenApi.Models;
global using Microsoft.OpenApi.Writers;