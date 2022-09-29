// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable UseDeconstruction
using Atc.Rest.ApiGenerator.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractModel : ISyntaxSchemaCodeGenerator
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractModel(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        string apiSchemaKey,
        OpenApiSchema apiSchema,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        ApiSchemaKey = apiSchemaKey ?? throw new ArgumentNullException(nameof(apiSchemaKey));
        ApiSchema = apiSchema ?? throw new ArgumentNullException(nameof(apiSchema));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        if (FocusOnSegmentName == "#")
        {
            IsSharedContract = true;
        }

        IsForClient = false;
        UseOwnFolder = true;
    }

    private ApiProjectOptions ApiProjectOptions { get; }

    private bool IsSharedContract { get; }

    public string ApiSchemaKey { get; }

    public OpenApiSchema ApiSchema { get; }

    public string FocusOnSegmentName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public bool IsEnum { get; private set; }

    public bool IsForClient { get; set; }

    public bool UseOwnFolder { get; set; }

    public bool GenerateCode()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        FileScopedNamespaceDeclarationSyntax @namespace;
        if (ApiSchema.IsSchemaEnumOrPropertyEnum())
        {
            @namespace = GenerateCodeForEnum(ref compilationUnit);
        }
        else
        {
            @namespace = GenerateCodeForOtherThanEnum(ref compilationUnit);
        }

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit!.AddMembers(@namespace);

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
            return $"Syntax generate problem for contract-model for schema: {ApiSchemaKey}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatAutoPropertiesOnOneLine()
            .FormatPublicPrivateLines()
            .FormatDoubleLines()
            .EnsureFileScopedNamespace();
    }

    public void ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var modelName = ApiSchemaKey.EnsureFirstCharacterToUpper();

        if (IsForClient &&
            modelName.EndsWith(NameConstants.Request, StringComparison.Ordinal))
        {
            var clientFile = DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ClientRequestParameters, modelName);
            ToFile(new FileInfo(clientFile));
            return;
        }

        var file = IsEnum
            ? DirectoryInfoHelper.GetCsFileNameForContractEnumTypes(ApiProjectOptions.PathForContracts, modelName)
            : IsSharedContract
                ? DirectoryInfoHelper.GetCsFileNameForContractShared(ApiProjectOptions.PathForContractsShared, modelName)
                : UseOwnFolder
                    ? DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractModels, modelName)
                    : DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, modelName);

        ToFile(new FileInfo(file));
    }

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            ApiProjectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            ToCodeAsString());
    }

    public override string ToString()
        => $"{nameof(ApiSchemaKey)}: {ApiSchemaKey}, SegmentName: {FocusOnSegmentName}, IsShared: {IsSharedContract}, {nameof(IsEnum)}: {IsEnum}";

    private FileScopedNamespaceDeclarationSyntax GenerateCodeForEnum(
        ref CompilationUnitSyntax compilationUnit)
    {
        IsEnum = true;

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(
            ApiProjectOptions,
            NameConstants.Contracts);

        var apiEnumSchema = ApiSchema.GetEnumSchema();

        // Create an enum
        var enumDeclaration = SyntaxEnumFactory.Create(apiEnumSchema.Item1.EnsureFirstCharacterToUpper(), apiEnumSchema.Item2);

        // Add the enum to the namespace.
        @namespace = @namespace.AddMembers(enumDeclaration);
        return @namespace;
    }

    private FileScopedNamespaceDeclarationSyntax GenerateCodeForOtherThanEnum(
        ref CompilationUnitSyntax? compilationUnit)
    {
        // Create a namespace
        var @namespace = IsSharedContract
            ? SyntaxProjectFactory.CreateNamespace(ApiProjectOptions, NameConstants.Contracts)
            : SyntaxProjectFactory.CreateNamespace(ApiProjectOptions, NameConstants.Contracts, FocusOnSegmentName);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create(ApiSchemaKey.EnsureFirstCharacterToUpper())
            .AddGeneratedCodeAttribute(ApiProjectOptions.ApiGeneratorName, ApiProjectOptions.ApiGeneratorVersion.ToString())
            .WithLeadingTrivia(SyntaxDocumentationFactory.Create(ApiSchema));

        var hasAnyPropertiesAsArrayWithFormatTypeBinary = ApiSchema.HasAnyPropertiesAsArrayWithFormatTypeBinary();

        // Create class-properties and add to class
        if (ApiSchema.Properties is not null)
        {
            if (ApiSchema.IsTypeArray() ||
                hasAnyPropertiesAsArrayWithFormatTypeBinary)
            {
                var (key, _) = ApiProjectOptions.Document.Components.Schemas.FirstOrDefault(x =>
                    x.Key.Equals(ApiSchema.Title, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(ApiSchema.Title))
                {
                    ApiSchema.Title = ApiSchemaKey;
                    key = ApiSchemaKey;
                }

                if (ApiSchema.Items is not null &&
                    string.IsNullOrEmpty(ApiSchema.Items.Title))
                {
                    ApiSchema.Items.Title = ApiSchemaKey;
                }

                var title = key is not null
                    ? $"{ApiSchema.Title.EnsureFirstCharacterToUpperAndSingular()}List"
                    : ApiSchema.Title.EnsureFirstCharacterToUpper();

                var propertyDeclaration = hasAnyPropertiesAsArrayWithFormatTypeBinary
                    ? SyntaxPropertyDeclarationFactory.CreateListAuto("IFormFile", ApiSchema.ExtractPropertyNameWhenHasAnyPropertiesOfArrayWithFormatTypeBinary())
                        .WithLeadingTrivia(
                            SyntaxDocumentationFactory.CreateSummary("A list of File(s)."))
                    : SyntaxPropertyDeclarationFactory.CreateListAuto(ApiSchema.Items!.Title, title)
                        .WithLeadingTrivia(
                            SyntaxDocumentationFactory.CreateSummary($"A list of {ApiSchema.Items.Title}."));

                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            }
            else
            {
                foreach (var property in ApiSchema.Properties)
                {
                    var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                            property,
                            ApiSchema.Required,
                            ApiProjectOptions.UseNullableReferenceTypes)
                        .WithLeadingTrivia(SyntaxDocumentationFactory.Create(property.Value));
                    classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
                }
            }

            var methodDeclaration = SyntaxMethodDeclarationFactory.CreateToStringMethod(ApiSchema.Properties);
            if (methodDeclaration is not null)
            {
                methodDeclaration = methodDeclaration.WithLeadingTrivia(SyntaxDocumentationFactory.CreateForOverrideToString());
                classDeclaration = classDeclaration.AddMembers(methodDeclaration);
            }
        }

        // Add the class to the namespace.
        @namespace = @namespace.AddMembers(classDeclaration);
        return @namespace;
    }
}