// ReSharper disable CheckNamespace
namespace Atc.Rest.ApiGenerator.Contracts;

/// <summary>
/// Http Operation type.
/// </summary>
public enum HttpOperationType
{
    /// <summary>
    /// A definition of a GET operation on this path.
    /// </summary>
    Get,

    /// <summary>
    /// A definition of a PUT operation on this path.
    /// </summary>
    Put,

    /// <summary>
    /// A definition of a POST operation on this path.
    /// </summary>
    Post,

    /// <summary>
    /// A definition of a DELETE operation on this path.
    /// </summary>
    Delete,

    /// <summary>
    /// A definition of a OPTIONS operation on this path.
    /// </summary>
    Options,

    /// <summary>
    /// A definition of a HEAD operation on this path.
    /// </summary>
    Head,

    /// <summary>
    /// A definition of a PATCH operation on this path.
    /// </summary>
    Patch,

    /// <summary>
    /// A definition of a TRACE operation on this path.
    /// </summary>
    Trace,
}