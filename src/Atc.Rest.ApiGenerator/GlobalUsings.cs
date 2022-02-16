global using System.Collections.Concurrent;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Net;
global using System.Net.Mime;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Xml.Linq;

global using Atc.CodeAnalysis.CSharp;
global using Atc.CodeAnalysis.CSharp.SyntaxFactories;
global using Atc.Data;
global using Atc.Data.Models;
global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.Extensions;
global using Atc.Rest.ApiGenerator.Factories;
global using Atc.Rest.ApiGenerator.Generators;
global using Atc.Rest.ApiGenerator.Helpers;
global using Atc.Rest.ApiGenerator.Helpers.XunitTest;
global using Atc.Rest.ApiGenerator.Models;
global using Atc.Rest.ApiGenerator.Models.Options;
global using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
global using Atc.Rest.ApiGenerator.SyntaxFactories;
global using Atc.Rest.ApiGenerator.SyntaxGenerators;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient.Interfaces;
global using Atc.Rest.ApiGenerator.SyntaxGenerators.Domain;
global using Atc.Rest.Client;
global using Atc.Rest.Client.Builder;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.OpenApi;
global using Microsoft.OpenApi.Any;
global using Microsoft.OpenApi.Models;
global using Microsoft.OpenApi.Readers;
global using Microsoft.OpenApi.Writers;