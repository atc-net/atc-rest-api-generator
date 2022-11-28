global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Net;
global using System.Text;

global using Atc.Helpers;
global using Atc.Rest.ApiGenerator.Framework.Contracts;
global using Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters;
global using Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;
global using Atc.Rest.ApiGenerator.Framework.Contracts.Models;
global using Atc.Rest.ApiGenerator.Framework.Readers;
global using Atc.Rest.ApiGenerator.Framework.ToRefactor;
global using Atc.Rest.ApiGenerator.OpenApi.Extensions;
global using Atc.Rest.ApiGenerator.OpenApi.Models;
global using Atc.Rest.ApiGenerator.OpenApi.Readers;

global using Microsoft.Extensions.Logging;
global using Microsoft.OpenApi.Models;
global using Microsoft.OpenApi.Readers;