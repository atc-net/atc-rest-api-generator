// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractParameter : ISyntaxOperationCodeGenerator
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractParameter(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        IList<OpenApiParameter> globalPathParameters,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
        ApiOperationType = apiOperationType;
        ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));

        IsForClient = false;
        UseOwnFolder = true;
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public IList<OpenApiParameter> GlobalPathParameters { get; }

    public OperationType ApiOperationType { get; }

    public OpenApiOperation ApiOperation { get; }

    public string FocusOnSegmentName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public bool IsForClient { get; set; }

    public bool UseOwnFolder { get; set; }

    public bool GenerateCode()
    {
        var parameterTypeName = ApiOperation.GetOperationName() + NameConstants.ContractParameters;

        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(
            ApiProjectOptions,
            NameConstants.Contracts,
            FocusOnSegmentName);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create(parameterTypeName)
            .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameters(ApiOperation, FocusOnSegmentName));

        // Add properties to the class
        if (GlobalPathParameters.Any())
        {
            foreach (var parameter in GlobalPathParameters)
            {
                var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                        parameter,
                        ApiProjectOptions.UseNullableReferenceTypes,
                        ApiProjectOptions.IsForClient)
                    .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(parameter));
                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            }
        }

        if (ApiOperation.Parameters is not null)
        {
            foreach (var parameter in ApiOperation.Parameters)
            {
                var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                        parameter,
                        ApiProjectOptions.UseNullableReferenceTypes,
                        ApiProjectOptions.IsForClient)
                    .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(parameter));
                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            }
        }

        var requestSchema = ApiOperation.RequestBody?.Content?.GetSchemaByFirstMediaType();

        if (ApiOperation.RequestBody is not null &&
            requestSchema is not null)
        {
            var isFormatTypeOfBinary = requestSchema.IsFormatTypeBinary();
            var isItemsOfFormatTypeBinary = requestSchema.HasItemsWithFormatTypeBinary();

            var requestBodyType = "string?";
            if (requestSchema.Reference is not null)
            {
                requestBodyType = requestSchema.Reference.Id.EnsureFirstCharacterToUpper();
            }
            else if (isFormatTypeOfBinary)
            {
                requestBodyType = "IFormFile";
            }
            else if (isItemsOfFormatTypeBinary)
            {
                requestBodyType = "IFormFile";
            }
            else if (requestSchema.Items is not null)
            {
                requestBodyType = requestSchema.Items.Reference.Id.EnsureFirstCharacterToUpper();
            }

            PropertyDeclarationSyntax propertyDeclaration;
            if (requestSchema.IsTypeArray())
            {
                if (ApiProjectOptions.IsForClient)
                {
                    propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateListAuto(requestBodyType, NameConstants.Request)
                        .AddValidationAttribute(new RequiredAttribute())
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                }
                else
                {
                    propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateListAuto(requestBodyType, NameConstants.Request);

                    propertyDeclaration = requestSchema.HasItemsWithFormatTypeBinary()
                        ? propertyDeclaration.AddFromFormAttribute()
                        : propertyDeclaration.AddFromBodyAttribute();

                    propertyDeclaration = propertyDeclaration
                        .AddValidationAttribute(new RequiredAttribute())
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                }
            }
            else
            {
                if (ApiProjectOptions.IsForClient)
                {
                    propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                            parameterLocation: null,
                            isNullable: false,
                            isRequired: true,
                            requestBodyType,
                            NameConstants.Request,
                            ApiProjectOptions.UseNullableReferenceTypes,
                            initializer: null)
                        .AddValidationAttribute(new RequiredAttribute())
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                }
                else
                {
                    propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                        parameterLocation: null,
                        isNullable: false,
                        isRequired: true,
                        requestBodyType,
                        NameConstants.Request,
                        ApiProjectOptions.UseNullableReferenceTypes,
                        initializer: null);

                    propertyDeclaration = requestSchema.HasAnyPropertiesWithFormatTypeBinary() || requestSchema.HasAnyPropertiesAsArrayWithFormatTypeBinary()
                        ? propertyDeclaration.AddFromFormAttribute()
                        : propertyDeclaration.AddFromBodyAttribute();

                    propertyDeclaration = propertyDeclaration
                        .AddValidationAttribute(new RequiredAttribute())
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                }
            }

            classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
        }

        var methodDeclaration = SyntaxMethodDeclarationFactory.CreateToStringMethod(GlobalPathParameters, ApiOperation.Parameters, ApiOperation.RequestBody);
        if (methodDeclaration is not null)
        {
            methodDeclaration = methodDeclaration.WithLeadingTrivia(SyntaxDocumentationFactory.CreateForOverrideToString());
            classDeclaration = classDeclaration.AddMembers(methodDeclaration);
        }

        if (!ApiProjectOptions.ApiOptions.Generator.UseGlobalUsings)
        {
            // Add using statement to compilationUnit
            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectApiFactory.CreateUsingListForContractParameter(
                    GlobalPathParameters,
                    ApiOperation.Parameters,
                    ApiOperation.RequestBody,
                    ApiProjectOptions.IsForClient));
        }

        // Add the class to the namespace.
        @namespace = @namespace.AddMembers(classDeclaration);

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit.AddMembers(@namespace);

        // Set code property
        Code = compilationUnit;
        return true;
    }

    public string ToCodeAsString()
    {
        if (Code is null)
        {
            GenerateCode();
        }

        if (Code is null)
        {
            return $"Syntax generate problem for contract-parameter for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatAutoPropertiesOnOneLine()
            .FormatPublicPrivateLines()
            .FormatDoubleLines();
    }

    public void ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var parameterName = ApiOperation.GetOperationName() + NameConstants.ContractParameters;

        var file = IsForClient
            ? DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ClientRequestParameters, parameterName)
            : UseOwnFolder
                ? DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractParameters, parameterName)
                : DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, parameterName);

        ToFile(new FileInfo(file));
    }

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileDisplayLocation = file.FullName.Replace(ApiProjectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, file.FullName, fileDisplayLocation, ToCodeAsString());
    }

    public override string ToString()
        => $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";
}