using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

// ReSharper disable UnusedMember.Local
// ReSharper disable InvertIf
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public class SyntaxGeneratorContractResult : ISyntaxOperationCodeGenerator
    {
        public SyntaxGeneratorContractResult(
            ApiProjectOptions apiProjectOptions,
            OperationType apiOperationType,
            OpenApiOperation apiOperation,
            string focusOnSegmentName)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.ApiOperationType = apiOperationType;
            this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        }

        public ApiProjectOptions ApiProjectOptions { get; }

        public OperationType ApiOperationType { get; }

        public OpenApiOperation ApiOperation { get; }

        public string FocusOnSegmentName { get; }

        public CompilationUnitSyntax? Code { get; private set; }

        public bool GenerateCode()
        {
            var resultTypeName = ApiOperation.GetOperationName() + NameConstants.ContractResult;

            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(
                ApiProjectOptions,
                NameConstants.Contracts,
                FocusOnSegmentName);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.CreateWithInheritClassTypeAndSuppressMessageAttributeByCheckId(
                resultTypeName,
                "ResultBase",
                1062,
                "Should not throw ArgumentNullExceptions from implicit operators.")
                .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
                .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResults(ApiOperation, FocusOnSegmentName));

            // Create members
            var memberDeclarations = CreateMembers(resultTypeName);

            // Add members to class
            classDeclaration = memberDeclarations.Aggregate(
                classDeclaration,
                (current, memberDeclaration) => current.AddMembers(memberDeclaration));

            // Add using statement to compilationUnit
            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectContractResultFactory.CreateUsingList(
                    ApiOperation.Responses,
                    ApiProjectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody));

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
            if (Code == null)
            {
                GenerateCode();
            }

            if (Code == null)
            {
                return $"Syntax generate problem for contract-result for apiOperation: {ApiOperation}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString()
                .FormatPublicPrivateLines()
                .FormatDoubleLines()
                .FormatBracketSpacing()
                .FormatConstructorWithInheritResult();
        }

        public LogKeyValueItem ToFile()
        {
            var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var resultName = ApiOperation.GetOperationName() + NameConstants.ContractResult;
            var file = Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractResults, resultName);
            return TextFileHelper.Save(file, ToCodeAsString());
        }

        public void ToFile(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            TextFileHelper.Save(file, ToCodeAsString());
        }

        public override string ToString()
        {
            return $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";
        }

        private List<MemberDeclarationSyntax> CreateMembers(string className)
        {
            var result = new List<MemberDeclarationSyntax>();

            // Add Field
            ////result.Add(CreateActionResultField());

            // Add Constructor
            result.AddRange(CreateConstructor(className, "result"));

            // Methods
            result.AddRange(CreateMethods(className));

            // Add Implicit Operator for ActionResult
            ////result.Add(
            ////    CreateImplicitOperatorForActionResult(className)
            ////        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResultsImplicitOperator(className)));

            // Add Implicit Operator
            var implicitOperator = CreateImplicitOperator(className, ApiOperation.Responses);
            if (implicitOperator != null)
            {
                result.Add(
                    implicitOperator
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResultsImplicitOperator(className)));
            }

            return result;
        }

        private static MemberDeclarationSyntax CreateActionResultField()
        {
            return SyntaxFactory.FieldDeclaration(
                    SyntaxVariableDeclarationFactory.Create(nameof(ActionResult), "result"))
                .WithModifiers(SyntaxTokenListFactory.PrivateReadonlyKeyword());
        }

        private static List<MemberDeclarationSyntax> CreateConstructor(
            string className,
            string parameterName)
        {
            return new MemberDeclarationSyntax[]
                {
                    SyntaxFactory.GlobalStatement(
                        SyntaxFactory.LocalFunctionStatement(
                            SyntaxFactory.IdentifierName(className),
                            SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxTokenFactory.PrivateKeyword()))
                        .WithParameterList(SyntaxParameterListFactory.CreateWithOneItem(nameof(ActionResult), parameterName))
                        .WithSemicolonToken(
                            SyntaxFactory.MissingToken(
                                SyntaxFactory.TriviaList(),
                                SyntaxKind.SemicolonToken,
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Trivia(
                                        SyntaxFactory.SkippedTokensTrivia()
                                        .WithTokens(
                                            SyntaxFactory.TokenList(
                                                SyntaxTokenFactory.Colon()))))))),
                    SyntaxFactory.GlobalStatement(
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.BaseExpression())
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameterName))))))
                        .WithSemicolonToken(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken))),
                    SyntaxFactory.GlobalStatement(SyntaxFactory.Block()),
                }.ToList();
        }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        private List<MemberDeclarationSyntax> CreateMethods(string className)
        {
            var result = new List<MemberDeclarationSyntax>();
            var httpStatusCodes = ApiOperation.Responses.GetHttpStatusCodes();
            foreach (var httpStatusCode in httpStatusCodes)
            {
                var isList = ApiOperation.Responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
                var modelName = ApiOperation.Responses.GetModelNameForStatusCode(httpStatusCode);
                var schema = ApiOperation.Responses.GetSchemaForStatusCode(httpStatusCode);
                var useProblemDetails = ApiOperation.Responses.IsSchemaTypeProblemDetailsForStatusCode(httpStatusCode);
                if (!useProblemDetails && ApiProjectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody)
                {
                    useProblemDetails = true;
                }

                MethodDeclarationSyntax? methodDeclaration;
                switch (httpStatusCode)
                {
                    case HttpStatusCode.OK:
                        var isPagination = ApiOperation.Responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);
                        if (useProblemDetails)
                        {
                            if (string.IsNullOrEmpty(modelName))
                            {
                                if (schema != null && schema.IsSimpleDataType())
                                {
                                    methodDeclaration = CreateTypeRequestObjectResult(className, httpStatusCode.ToNormalizedString(), schema.GetDataType(), "response", isList, isPagination);
                                }
                                else
                                {
                                    methodDeclaration = CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull("CreateContentResult", className, httpStatusCode);
                                }
                            }
                            else
                            {
                                methodDeclaration = CreateTypeRequestObjectResult(className, httpStatusCode.ToNormalizedString(), modelName, "response", isList, isPagination);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(modelName))
                            {
                                if (schema != null && schema.IsSimpleDataType())
                                {
                                    methodDeclaration = CreateTypeRequestObjectResult(className, httpStatusCode.ToNormalizedString(), schema.GetDataType(), "response", isList, isPagination);
                                }
                                else
                                {
                                    methodDeclaration = CreateTypeRequestWithMessageAllowNull(className, httpStatusCode, nameof(OkObjectResult));
                                }
                            }
                            else
                            {
                                methodDeclaration = CreateTypeRequestObjectResult(className, httpStatusCode.ToNormalizedString(), modelName, "response", isList, isPagination);
                            }
                        }

                        break;
                    case HttpStatusCode.Created:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResult", className, httpStatusCode)
                            : CreateStatusCodeResult(className, httpStatusCode);
                        break;
                    case HttpStatusCode.Accepted:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className, httpStatusCode)
                            : CreateTypeRequest(className, httpStatusCode, nameof(AcceptedResult));
                        break;
                    case HttpStatusCode.NoContent:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className, httpStatusCode)
                            : CreateTypeRequest(className, httpStatusCode, nameof(NoContentResult));
                        break;
                    case HttpStatusCode.NotModified:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className, httpStatusCode)
                            : CreateStatusCodeResult(className, httpStatusCode);
                        break;
                    case HttpStatusCode.BadRequest:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithProblemDetailsWithMessage(className, httpStatusCode)
                            : CreateTypeRequestWithMessage(className, httpStatusCode, nameof(BadRequestObjectResult));
                        break;
                    case HttpStatusCode.Unauthorized:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className, httpStatusCode)
                            : CreateTypeRequest(className, httpStatusCode, nameof(UnauthorizedResult));
                        break;
                    case HttpStatusCode.Forbidden:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className, httpStatusCode)
                            : CreateStatusCodeResult(className, httpStatusCode);
                        break;
                    case HttpStatusCode.NotFound:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull("CreateContentResultWithProblemDetails", className, httpStatusCode)
                            : CreateTypeRequestWithMessageAllowNull(className, httpStatusCode, nameof(NotFoundObjectResult));
                        break;
                    case HttpStatusCode.Conflict:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull("CreateContentResultWithProblemDetails", className, httpStatusCode, "error")
                            : CreateTypeRequestWithMessageAllowNull(className, httpStatusCode, nameof(ConflictObjectResult), "error");
                        break;
                    case HttpStatusCode.MethodNotAllowed:
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.NotImplemented:
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                        methodDeclaration = useProblemDetails
                            ? CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull("CreateContentResultWithProblemDetails", className, httpStatusCode)
                            : CreateTypeRequestWithObject(className, httpStatusCode, nameof(ContentResult));
                        break;
                    default:
                        throw new NotImplementedException("Method: " + nameof(httpStatusCode) + " " + httpStatusCode);
                }

                result.Add(
                    methodDeclaration
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResultsMethod(httpStatusCode)));
            }

            return result;
        }

        private static MethodDeclarationSyntax CreateStatusCodeResult(
            string className,
            HttpStatusCode httpStatusCode)
        {
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName(className), SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(className))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(nameof(StatusCodeResult)))
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName(nameof(StatusCodes)),
                                                                    SyntaxFactory.IdentifierName($"Status{(int)httpStatusCode}{httpStatusCode}"))))))))))))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static MethodDeclarationSyntax CreateTypeRequest(
            string className,
            HttpStatusCode httpStatusCode,
            string typeRequestName)
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxObjectCreationExpressionFactory.Create(className)
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.ObjectCreationExpression(
                                                    SyntaxFactory.IdentifierName(typeRequestName))
                                                .WithArgumentList(SyntaxFactory.ArgumentList())))))))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static MethodDeclarationSyntax CreateTypeRequestWithMessage(
            string className,
            HttpStatusCode httpStatusCode,
            string typeRequestName,
            string parameterName = "message")
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
                                .WithType(SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword())))))
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxObjectCreationExpressionFactory.Create(className)
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxObjectCreationExpressionFactory.Create(typeRequestName)
                                                .WithArgumentList(
                                                    SyntaxArgumentListFactory.CreateWithOneItem(parameterName))))))))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static MethodDeclarationSyntax CreateTypeRequestWithMessageAllowNull(
            string className,
            HttpStatusCode httpStatusCode,
            string typeRequestName,
            string parameterName = "message")
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
                                .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword())))
                                .WithDefault(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))))))
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxObjectCreationExpressionFactory.Create(className)
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxObjectCreationExpressionFactory.Create(typeRequestName)
                                                .WithArgumentList(
                                                    SyntaxArgumentListFactory.CreateWithOneItem(parameterName))))))))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static MethodDeclarationSyntax CreateTypeRequestWithObject(
            string className,
            HttpStatusCode httpStatusCode,
            string typeRequestName,
            string parameterName = "message")
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
                                .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword())))
                                .WithDefault(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))))))
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxObjectCreationExpressionFactory.Create(className)
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxObjectCreationExpressionFactory.Create(typeRequestName)
                                                .WithInitializer(
                                                    SyntaxFactory.InitializerExpression(
                                                        SyntaxKind.ObjectInitializerExpression,
                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    SyntaxFactory.IdentifierName("StatusCode"),
                                                                    SyntaxFactory.CastExpression(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName(nameof(HttpStatusCode)),
                                                                            SyntaxFactory.IdentifierName(httpStatusCode.ToNormalizedString())))),
                                                                SyntaxTokenFactory.Comma(),
                                                                SyntaxFactory.AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    SyntaxFactory.IdentifierName("Content"),
                                                                    SyntaxFactory.IdentifierName(parameterName)),
                                                            })))))))))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static MethodDeclarationSyntax CreateTypeRequestWithSpecifiedResultFactoryMethod(
            string resultFactoryMethodName,
            string className,
            HttpStatusCode httpStatusCode)
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.IdentifierName(className))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        SyntaxFactory.IdentifierName("ResultFactory"),
                                                        SyntaxFactory.IdentifierName(resultFactoryMethodName)))
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName(nameof(HttpStatusCode)),
                                                                        SyntaxFactory.IdentifierName(httpStatusCode.ToString()))),
                                                                SyntaxTokenFactory.Comma(),
                                                                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                                                            })))))))))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static MethodDeclarationSyntax CreateTypeRequestWithProblemDetailsWithMessage(
            string className,
            HttpStatusCode httpStatusCode,
            string parameterName = "message")
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
                                .WithType(SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword())))))
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.IdentifierName(className))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        SyntaxFactory.IdentifierName("ResultFactory"),
                                                        SyntaxFactory.IdentifierName("CreateContentResultWithProblemDetails")))
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName(nameof(HttpStatusCode)),
                                                                        SyntaxFactory.IdentifierName(httpStatusCode.ToString()))),
                                                                SyntaxTokenFactory.Comma(),
                                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameterName)),
                                                            })))))))))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static MethodDeclarationSyntax CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull(
            string resultFactoryMethodName,
            string className,
            HttpStatusCode httpStatusCode,
            string parameterName = "message")
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(httpStatusCode.ToNormalizedString()))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
                                .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword())))
                                .WithDefault(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))))))
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.IdentifierName(className))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        SyntaxFactory.IdentifierName("ResultFactory"),
                                                        SyntaxFactory.IdentifierName(resultFactoryMethodName)))
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName(nameof(HttpStatusCode)),
                                                                        SyntaxFactory.IdentifierName(httpStatusCode.ToString()))),
                                                                SyntaxTokenFactory.Comma(),
                                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameterName)),
                                                            })))))))))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static MethodDeclarationSyntax CreateTypeRequestObjectResult(
            string className,
            string methodName,
            string parameterTypeName,
            string parameterName = "response",
            bool asGenericList = false,
            bool asGenericPagination = false)
        {
            string? genericListTypeName = null;
            if (asGenericList)
            {
                genericListTypeName = Microsoft.OpenApi.Models.NameConstants.List;
            }
            else if (asGenericPagination)
            {
                genericListTypeName = Microsoft.OpenApi.Models.NameConstants.Pagination;
            }

            if (string.IsNullOrEmpty(parameterTypeName))
            {
                parameterTypeName = "string";
            }

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.IdentifierName(className),
                    SyntaxFactory.Identifier(methodName))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
                .WithParameterList(SyntaxParameterListFactory.CreateWithOneItem(parameterTypeName, parameterName, genericListTypeName))
                .WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxObjectCreationExpressionFactory.Create(className)
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxObjectCreationExpressionFactory.Create(methodName + nameof(ObjectResult))
                                                .WithArgumentList(SyntaxArgumentListFactory.CreateWithOneItem(parameterName))))))))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static ConversionOperatorDeclarationSyntax CreateImplicitOperatorForActionResult(string className)
        {
            return SyntaxFactory.ConversionOperatorDeclaration(
                    SyntaxTokenFactory.ImplicitKeyword(),
                    SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(nameof(ActionResult))))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword(true))
                .WithOperatorKeyword(SyntaxTokenFactory.OperatorKeyword())
                .AddParameterListParameters(SyntaxParameterFactory.Create(className, "x"))
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("x"),
                            SyntaxFactory.IdentifierName("result")))
                    .WithArrowToken(SyntaxTokenFactory.EqualsGreaterThan()))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static ConversionOperatorDeclarationSyntax? CreateImplicitOperator(string className, OpenApiResponses responses)
        {
            var httpStatusCodes = responses.GetHttpStatusCodes();
            if (!httpStatusCodes.Contains(HttpStatusCode.OK) &&
                !httpStatusCodes.Contains(HttpStatusCode.Created))
            {
                return null;
            }

            if (httpStatusCodes.Contains(HttpStatusCode.OK) &&
                httpStatusCodes.Contains(HttpStatusCode.Created))
            {
                return null;
            }

            var httpStatusCode = HttpStatusCode.Continue;
            if (httpStatusCodes.Contains(HttpStatusCode.OK))
            {
                httpStatusCode = HttpStatusCode.OK;
            }
            else if (httpStatusCodes.Contains(HttpStatusCode.Created))
            {
                httpStatusCode = HttpStatusCode.Created;
            }

            var modelName = responses.GetModelNameForStatusCode(httpStatusCode);
            if (string.IsNullOrEmpty(modelName) && httpStatusCode == HttpStatusCode.Created)
            {
                return null;
            }

            var isList = responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
            if (httpStatusCodes.Contains(HttpStatusCode.OK))
            {
                var isPagination = responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);

                if (string.IsNullOrEmpty(modelName))
                {
                    var schema = responses.GetSchemaForStatusCode(HttpStatusCode.OK);
                    if (schema != null && schema.IsSimpleDataType())
                    {
                        return CreateImplicitOperator(className, schema.GetDataType(), httpStatusCode, isList, isPagination);
                    }
                }

                return CreateImplicitOperator(className, modelName, httpStatusCode, isList, isPagination);
            }

            return CreateImplicitOperator(className, modelName, httpStatusCode, isList);
        }

        private static ConversionOperatorDeclarationSyntax CreateImplicitOperator(
            string className,
            string typeName,
            HttpStatusCode httpStatusCode,
            bool asGenericList = false,
            bool asGenericPagination = false)
        {
            string? genericListTypeName = null;
            if (asGenericList)
            {
                genericListTypeName = Microsoft.OpenApi.Models.NameConstants.List;
            }
            else if (asGenericPagination)
            {
                genericListTypeName = Microsoft.OpenApi.Models.NameConstants.Pagination;
            }

            var httpStatus = httpStatusCode.ToNormalizedString();

            if (string.IsNullOrEmpty(typeName))
            {
                typeName = "string";
            }

            return SyntaxFactory.ConversionOperatorDeclaration(
                    SyntaxTokenFactory.ImplicitKeyword(),
                    SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(className)))
                .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword(true))
                .WithOperatorKeyword(SyntaxTokenFactory.OperatorKeyword())
                .AddParameterListParameters(SyntaxParameterFactory.Create(typeName, "x", genericListTypeName))
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(httpStatus)))
                            .AddArgumentListArguments(SyntaxArgumentFactory.Create("x")))
                    .WithArrowToken(SyntaxTokenFactory.EqualsGreaterThan()))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }
    }
}