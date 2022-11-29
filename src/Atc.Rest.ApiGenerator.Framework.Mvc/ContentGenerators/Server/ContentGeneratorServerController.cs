namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public sealed class ContentGeneratorServerController : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerControllerParameters parameters;

    public ContentGeneratorServerController(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerControllerParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Endpoint definitions.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[Authorize]");
        sb.AppendLine("[ApiController]");
        sb.AppendLine($"[Route(\"{parameters.RouteBase}\")]");
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public class {parameters.ApiGroupName}Controller : ControllerBase");
        sb.AppendLine("{");

        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            AppendMethodContent(sb, item);

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void AppendMethodContent(
        StringBuilder sb,
        ContentGeneratorServerControllerMethodParameters item)
    {
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, $"/// Description: {item.Description}");
        sb.AppendLine(4, $"/// Operation: {item.Name}.");
        sb.AppendLine(4, "/// </summary>");

        AppendMethodContentAuthorizationIfNeeded(sb, item);

        sb.AppendLine(4, string.IsNullOrEmpty(item.RouteSuffix)
            ? $"[Http{item.OperationTypeRepresentation}]"
            : $"[Http{item.OperationTypeRepresentation}(\"{item.RouteSuffix}\")]");

        if (item.MultipartBodyLengthLimit.HasValue)
        {
            sb.AppendLine(4, item.MultipartBodyLengthLimit.Value == long.MaxValue
                ? "[RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]"
                : $"[RequestFormLimits(MultipartBodyLengthLimit = {item.MultipartBodyLengthLimit.Value})]");
        }

        foreach (var producesResponseTypeRepresentation in item.ProducesResponseTypeRepresentations)
        {
            sb.AppendLine(4, $"[{producesResponseTypeRepresentation}]");
        }

        sb.AppendLine(4, $"public async Task<ActionResult> {item.Name}(");

        if (!string.IsNullOrEmpty(item.ParameterTypeName))
        {
            sb.AppendLine(8, $"{item.ParameterTypeName} parameters,");
        }

        sb.AppendLine(8, $"[FromServices] {item.InterfaceName} handler,");
        sb.AppendLine(8, "CancellationToken cancellationToken)");

        sb.AppendLine(8, !string.IsNullOrEmpty(item.ParameterTypeName)
            ? "=> await handler.ExecuteAsync(parameters, cancellationToken);"
            : "=> await handler.ExecuteAsync(cancellationToken);");
    }

    private static void AppendMethodContentAuthorizationIfNeeded(
        StringBuilder sb,
        ContentGeneratorServerControllerMethodParameters item)
    {
        if (ShouldUseAuthorizeAttribute(item))
        {
            var authorizeLineBuilder = new StringBuilder();
            var authRoles = string.Join(',', item.ApiPathAuthorizationRoles.Concat(item.ApiOperationAuthorizationRoles).Distinct(StringComparer.Ordinal).OrderBy(x => x));
            var authSchemes = string.Join(',', item.ApiPathAuthenticationSchemes.Concat(item.ApiOperationAuthenticationSchemes).Distinct(StringComparer.Ordinal).OrderBy(x => x));

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
        else if (ShouldUseAllowAnonymousAttribute(item))
        {
            sb.AppendLine(4, "[AllowAnonymous]");
        }
    }

    private static bool ShouldUseAuthorizeAttribute(
        ContentGeneratorServerControllerMethodParameters item)
    {
        var apiPathUseAuthorization = item.ApiPathUseAuthorization.HasValue &&
                                      item.ApiPathUseAuthorization.Value;

        var apiPathAnyRolesOrAuthenticationSchemes = item.ApiPathAuthorizationRoles.Any() ||
                                                     item.ApiPathAuthenticationSchemes.Any();

        var apiOperationUseAuthorizationIsSet = item.ApiOperationUseAuthorization.HasValue;
        var apiOperationUseAuthorization = item.ApiOperationUseAuthorization.HasValue &&
                                           item.ApiOperationUseAuthorization.Value;

        var apiOperationAnyRolesOrAuthenticationSchemes = item.ApiOperationAuthorizationRoles.Any() ||
                                                          item.ApiOperationAuthenticationSchemes.Any();

        var result = apiPathUseAuthorization ||
                     apiOperationUseAuthorization ||
                     apiPathAnyRolesOrAuthenticationSchemes ||
                     apiOperationAnyRolesOrAuthenticationSchemes;

        if (result &&
            apiOperationUseAuthorizationIsSet &&
            !apiOperationUseAuthorization)
        {
            return false;
        }

        return result;
    }

    private static bool ShouldUseAllowAnonymousAttribute(
        ContentGeneratorServerControllerMethodParameters item)
    {
        var apiPathUseAuthorizationIsSet = item.ApiPathUseAuthorization.HasValue;
        var apiPathUseAuthorization = item.ApiPathUseAuthorization.HasValue &&
                                      item.ApiPathUseAuthorization.Value;

        var apiOperationUseAuthorizationIsSet = item.ApiOperationUseAuthorization.HasValue;
        var apiOperationUseAuthorization = item.ApiOperationUseAuthorization.HasValue &&
                                           item.ApiOperationUseAuthorization.Value;

        return (apiPathUseAuthorizationIsSet && !apiPathUseAuthorization) ||
               (apiOperationUseAuthorizationIsSet && !apiOperationUseAuthorization);
    }
}