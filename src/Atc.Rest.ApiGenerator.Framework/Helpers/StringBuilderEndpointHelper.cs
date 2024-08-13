namespace Atc.Rest.ApiGenerator.Framework.Helpers;

public static class StringBuilderEndpointHelper
{
    public static void AppendMethodContentAuthorizationIfNeeded(
        StringBuilder sb,
        ApiAuthorizeModel? authorizationForController,
        ApiAuthorizeModel? authorizationForEndpoint)
    {
        ArgumentNullException.ThrowIfNull(sb);

        if (authorizationForEndpoint is null)
        {
            return;
        }

        if (authorizationForEndpoint.UseAllowAnonymous)
        {
            if (authorizationForController is not null &&
                authorizationForController.UseAllowAnonymous)
            {
                return;
            }

            sb.AppendLine(4, "[AllowAnonymous]");
            return;
        }

        if (authorizationForController is not null &&
            !authorizationForController.UseAllowAnonymous &&
            (authorizationForEndpoint.Roles is null || authorizationForEndpoint.Roles.Count == 0) &&
            (authorizationForEndpoint.AuthenticationSchemes is null || authorizationForEndpoint.AuthenticationSchemes.Count == 0))
        {
            return;
        }

        var authorizeLineBuilder = new StringBuilder();
        var authRoles = authorizationForEndpoint.Roles is null
            ? null
            : string.Join(',', authorizationForEndpoint.Roles);
        var authSchemes = authorizationForEndpoint.AuthenticationSchemes is null
            ? null
            : string.Join(',', authorizationForEndpoint.AuthenticationSchemes);

        authorizeLineBuilder.Append(4, "[Authorize");

        if (!string.IsNullOrEmpty(authRoles))
        {
            authorizeLineBuilder.Append($"(Roles = \"{authRoles}\"");
        }

        if (!string.IsNullOrEmpty(authSchemes))
        {
            authorizeLineBuilder.Append(string.IsNullOrEmpty(authRoles)
                ? $"(AuthenticationSchemes = \"{authSchemes}\""
                : $", AuthenticationSchemes = \"{authSchemes}\"");
        }

        if (!string.IsNullOrEmpty(authRoles) || !string.IsNullOrEmpty(authSchemes))
        {
            authorizeLineBuilder.Append(')');
        }

        authorizeLineBuilder.Append(']');
        sb.AppendLine(authorizeLineBuilder.ToString());
    }
}