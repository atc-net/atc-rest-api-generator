global using System;
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Net;
global using System.Reflection;
global using System.Runtime.Serialization;
global using System.Text.Json;

global using Atc.Console.Spectre.Factories;
global using Atc.Console.Spectre.Logging;
global using Atc.Data.Models;
global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.CLI.Commands;
global using Atc.Rest.ApiGenerator.CLI.Commands.Settings;
global using Atc.Rest.ApiGenerator.CLI.Extensions;
global using Atc.Rest.ApiGenerator.Helpers;
global using Atc.Rest.ApiGenerator.Models.ApiOptions;
global using Atc.Serialization;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;

global using Spectre.Console.Cli;