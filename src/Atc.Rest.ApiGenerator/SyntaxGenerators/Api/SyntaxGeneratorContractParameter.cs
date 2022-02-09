using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.SyntaxFactories;

// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractParameter : ISyntaxOperationCodeGenerator
{
    public SyntaxGeneratorContractParameter(
        ApiProjectOptions apiProjectOptions,
        IList<OpenApiParameter> globalPathParameters,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName)
    {
        this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        this.GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
        this.ApiOperationType = apiOperationType;
        this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
        this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));

        this.IsForClient = false;
        this.UseOwnFolder = true;
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public IList<OpenApiParameter> GlobalPathParameters { get; }

    public OperationType ApiOperationType { get; }

    public OpenApiOperation ApiOperation { get; }

    public string FocusOnSegmentName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public bool IsForClient { get; set; }

    public bool UseOwnFolder { get; set; }

    [SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "Bug in CA1508.")]
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
                        ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                        ApiProjectOptions.IsForClient)
                    .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(parameter));
                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            }
        }

        if (ApiOperation.Parameters != null)
        {
            foreach (var parameter in ApiOperation.Parameters)
            {
                var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                        parameter,
                        ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                        ApiProjectOptions.IsForClient)
                    .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(parameter));
                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            }
        }

        var requestSchema = ApiOperation.RequestBody?.Content?.GetSchemaByFirstMediaType();

        if (ApiOperation.RequestBody != null && requestSchema != null)
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
                            ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
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
                        ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
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
        if (methodDeclaration != null)
        {
            methodDeclaration = methodDeclaration.WithLeadingTrivia(SyntaxDocumentationFactory.CreateForOverrideToString());
            classDeclaration = classDeclaration.AddMembers(methodDeclaration);
        }

        // Add using statement to compilationUnit
        compilationUnit = compilationUnit.AddUsingStatements(
            ProjectApiFactory.CreateUsingListForContractParameter(
                GlobalPathParameters,
                ApiOperation.Parameters,
                ApiOperation.RequestBody,
                ApiProjectOptions.IsForClient));

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

    public LogKeyValueItem ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var parameterName = ApiOperation.GetOperationName() + NameConstants.ContractParameters;

        var file = IsForClient
            ? Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ClientRequestParameters, parameterName)
            : UseOwnFolder
                ? Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractParameters, parameterName)
                : Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, parameterName);

        return TextFileHelper.Save(file, ToCodeAsString());
    }

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        TextFileHelper.Save(file, ToCodeAsString());
    }

    public override string ToString()
        => $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";
}