namespace Atc.Rest.ApiGenerator.Framework.Contracts.Models;

public record ApiAuthorizeModel(
    IList<string>? Roles,
    IList<string>? AuthenticationSchemes,
    bool UseAllowAnonymous);