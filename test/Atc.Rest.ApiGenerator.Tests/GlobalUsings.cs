global using System.Diagnostics.CodeAnalysis;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.RegularExpressions;

global using Atc.CodeDocumentation.Markdown;
global using Atc.DotNet;
global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.Framework;
global using Atc.Rest.ApiGenerator.Framework.Models;
global using Atc.Rest.ApiGenerator.Generators;
global using Atc.Rest.ApiGenerator.Helpers;
global using Atc.Rest.ApiGenerator.Helpers.XunitTest;
global using Atc.Rest.ApiGenerator.Models;
global using Atc.Rest.ApiGenerator.Models.Options;
global using Atc.Rest.ApiGenerator.SyntaxGenerators;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.Domain;
global using Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator;
global using Atc.XUnit;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.OpenApi.Models;
global using Microsoft.OpenApi.Readers;

global using Xunit.Abstractions;