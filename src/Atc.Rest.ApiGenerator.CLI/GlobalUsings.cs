global using System;
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Net;

global using Atc.Console.Spectre;
global using Atc.Console.Spectre.CommandSettings;
global using Atc.Console.Spectre.Factories;
global using Atc.Console.Spectre.Helpers;
global using Atc.Console.Spectre.Logging;
global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.CLI.Commands;
global using Atc.Rest.ApiGenerator.CLI.Commands.Settings;
global using Atc.Rest.ApiGenerator.CLI.Extensions;
global using Atc.Rest.ApiGenerator.Framework.Readers;
global using Atc.Rest.ApiGenerator.Helpers;
global using Atc.Rest.ApiGenerator.Models.Options;
global using Atc.Rest.ApiGenerator.OpenApi.Extractors;
global using Atc.Rest.ApiGenerator.OpenApi.Interfaces;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Spectre.Console;
global using Spectre.Console.Cli;