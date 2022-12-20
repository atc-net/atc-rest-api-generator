// ReSharper disable CheckNamespace
namespace Atc.Rest.ApiGenerator.Framework.Contracts;

/// <summary>
/// The location of the parameter.
/// </summary>
public enum ParameterLocationType
{
    /// <summary>
    /// The none.
    /// </summary>
    None,

    /// <summary>
    /// Parameters that are appended to the URL.
    /// </summary>
    Query,

    /// <summary>
    /// Custom headers that are expected as part of the request.
    /// </summary>
    Header,

    /// <summary>
    /// Used together with Path/Route Templating,
    /// where the parameter value is actually part of the operation's URL
    /// </summary>
    Route,

    /// <summary>
    /// Used to pass a specific cookie value to the API.
    /// </summary>
    Cookie,

    /// <summary>
    /// Parameters that are appended to the body.
    /// </summary>
    Body,
}