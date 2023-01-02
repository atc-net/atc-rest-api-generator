global using System.Collections.Concurrent;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Net;
global using System.Net.Mime;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.RegularExpressions;

global using Atc.CodeAnalysis.CSharp;
global using Atc.CodeAnalysis.CSharp.SyntaxFactories;
global using Atc.CodeDocumentation.CodeComment;
global using Atc.Console.Spectre;
global using Atc.DotNet;
global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.Extensions;
global using Atc.Rest.ApiGenerator.Framework.ContentGenerators;
global using Atc.Rest.ApiGenerator.Framework.Contracts;
global using Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters;
global using Atc.Rest.ApiGenerator.Framework.Contracts.Extensions;
global using Atc.Rest.ApiGenerator.Framework.Contracts.Models;
global using Atc.Rest.ApiGenerator.Framework.Contracts.Options;
global using Atc.Rest.ApiGenerator.Framework.Contracts.Resolver;
global using Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;
global using Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;
global using Atc.Rest.ApiGenerator.Framework.Factories.Parameters.ServerClient;
global using Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Client;
global using Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;
global using Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.ServerClient;
global using Atc.Rest.ApiGenerator.Framework.ToRefactor;
global using Atc.Rest.ApiGenerator.Framework.Writers;
global using Atc.Rest.ApiGenerator.Generators;
global using Atc.Rest.ApiGenerator.Helpers;
global using Atc.Rest.ApiGenerator.Helpers.XunitTest;
global using Atc.Rest.ApiGenerator.Models;
global using Atc.Rest.ApiGenerator.OpenApi;
global using Atc.Rest.ApiGenerator.OpenApi.Extensions;
global using Atc.Rest.ApiGenerator.OpenApi.Extractors;
global using Atc.Rest.ApiGenerator.OpenApi.Models;
global using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
global using Atc.Rest.ApiGenerator.SyntaxGenerators;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient.Interfaces;
global using Atc.Rest.Client;
global using Atc.Rest.Client.Builder;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.OpenApi.Any;
global using Microsoft.OpenApi.Models;
global using Microsoft.OpenApi.Writers;