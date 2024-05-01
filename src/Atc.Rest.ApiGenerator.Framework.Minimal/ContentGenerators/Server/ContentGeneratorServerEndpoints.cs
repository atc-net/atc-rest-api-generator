namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators.Server;

public sealed class ContentGeneratorServerEndpoints : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerEndpointParameters parameters;

    public ContentGeneratorServerEndpoints(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerEndpointParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();

        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTags));
        }

        sb.AppendLine(codeAttributeGenerator.Generate());

        sb.AppendLine($"public sealed class {parameters.ApiGroupName}EndpointDefinition : IEndpointDefinition");
        sb.AppendLine("{");

        sb.AppendLine(4, $"internal const string ApiRouteBase = \"{parameters.RouteBase}\";");
        sb.AppendLine();

        AppendDefineEndpoints(sb);

        AppendRouteHandlers(sb);

        // TODO: Define methods

        sb.Append('}');

        return sb.ToString();
    }

    private void AppendDefineEndpoints(
        StringBuilder sb)
    {
        var routeGroupBuilderName = parameters.ApiGroupName.EnsureFirstCharacterToLower();

        sb.AppendLine(4, "public void DefineEndpoints(");
        sb.AppendLine(8, "WebApplication app)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, $"var {routeGroupBuilderName} = app");
        sb.AppendLine(12, $".NewVersionedApi(\"{parameters.ApiGroupName}\")");
        sb.AppendLine(12, ".MapGroup(ApiRouteBase);");
        sb.AppendLine();

        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            AppendRoute(sb, routeGroupBuilderName, item);

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine(4, "}");
        sb.AppendLine();
    }

    private void AppendRoute(
        StringBuilder sb,
        string routeGroupBuilderName,
        ContentGeneratorServerEndpointMethodParameters item)
    {
        // TODO: item.RouteSuffix - if null "/"

        var description = item.DocumentationTags.Summary
            .Replace("Description: ", string.Empty, StringComparison.Ordinal)
            .Replace($"Operation: {item.Name}.", string.Empty, StringComparison.Ordinal)
            .Replace(Environment.NewLine, string.Empty, StringComparison.Ordinal)
            .Trim();

        var summary = item.Name;

        sb.AppendLine(8, routeGroupBuilderName);
        sb.AppendLine(12,
            item.RouteSuffix is null
                ? $".Map{item.OperationTypeRepresentation}(\"/\", {item.Name})"
                : $".Map{item.OperationTypeRepresentation}(\"{item.RouteSuffix}\", {item.Name})");
        sb.AppendLine(12, $".WithName(\"{item.Name}\")");
        sb.AppendLine(12, $".WithDescription(\"{description}\")");
        sb.AppendLine(12, $".WithSummary(\"{summary}\");");

        /*
         TODO:
                usersV1
                    .MapGet("/", GetAllUsers)
                    .WithName(Names.UserDefinitionNames.GetAllUsers)
                    .WithDescription("Retrieve all users.")
                    .WithSummary("Retrieve all users.");

                usersV1
                    .MapPost("/", CreateUser)
                    .WithName(Names.UserDefinitionNames.CreateUser)
                    .WithDescription("Create user.")
                    .WithSummary("Create user.")
                    .AddEndpointFilter<ValidationFilter<CreateUserParameters>>()
                    .ProducesValidationProblem();

                usersV1
                    .MapPut("/{userId}", UpdateUserById)
                    .WithName(Names.UserDefinitionNames.UpdateUserById)
                    .WithDescription("Update user.")
                    .WithSummary("Update user.")
                    .AddEndpointFilter<ValidationFilter<UpdateUserByIdParameters>>()
                    .ProducesValidationProblem();
         */

        ////.ProducesProblem(StatusCodes.Status409Conflict)
        ////.ProducesValidationProblem(); //Only for 400
    }

    private void AppendRouteHandlers(
        StringBuilder sb)
    {
        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            sb.Append(4, "internal Task<");

            switch (item.HttpResults.Count)
            {
                case > 1:
                {
                    sb.Append("Results<");

                    for (var x = 0; x < item.HttpResults.Count; x++)
                    {
                        var (httpStatusCode, returnType) = item.HttpResults[x];
                        if (string.IsNullOrEmpty(returnType))
                        {
                            sb.Append($"{httpStatusCode.ToNormalizedString()}");
                        }
                        else
                        {
                            sb.Append($"{httpStatusCode.ToNormalizedString()}<{returnType}>");
                        }

                        if (x != item.HttpResults.Count - 1)
                        {
                            sb.Append(", ");
                        }
                    }

                    sb.Append('>');
                    break;
                }

                case 1:
                {
                    var (httpStatusCode, returnType) = item.HttpResults[0];

                    sb.Append($"{httpStatusCode.ToNormalizedString()}<{returnType}>");
                    break;
                }
            }

            sb.AppendLine($"> {item.Name}(");

            sb.AppendLine(8, $"[FromServices] {item.InterfaceName} handler,");

            if (!string.IsNullOrEmpty(item.ParameterTypeName))
            {
                sb.AppendLine(8, $"[AsParameters] {item.ParameterTypeName} parameters,");
            }

            sb.AppendLine(8, "CancellationToken cancellationToken)");

            sb.AppendLine(
                8,
                !string.IsNullOrEmpty(item.ParameterTypeName)
                    ? "=> handler.ExecuteAsync(parameters, cancellationToken);"
                    : "=> handler.ExecuteAsync(cancellationToken);");

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }
    }
}