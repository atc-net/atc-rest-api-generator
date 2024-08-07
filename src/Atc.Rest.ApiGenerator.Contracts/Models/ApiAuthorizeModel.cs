namespace Atc.Rest.ApiGenerator.Contracts.Models;

public record ApiAuthorizeModel(
    IList<string>? Roles,
    IList<string>? AuthenticationSchemes,
    bool UseAllowAnonymous);