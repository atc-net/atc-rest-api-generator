using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Domain
{
    public class SyntaxGeneratorHandler
    {
        public SyntaxGeneratorHandler(
            DomainProjectOptions domainProjectOptions,
            OperationType apiOperationType,
            OpenApiOperation apiOperation,
            string focusOnSegmentName,
            bool hasParametersOrRequestBody)
        {
            this.DomainProjectOptions = domainProjectOptions ?? throw new ArgumentNullException(nameof(domainProjectOptions));
            this.ApiOperationType = apiOperationType;
            this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
            this.HasParametersOrRequestBody = hasParametersOrRequestBody;
        }

        public DomainProjectOptions DomainProjectOptions { get; }

        public OperationType ApiOperationType { get; }

        public OpenApiOperation ApiOperation { get; }

        public string FocusOnSegmentName { get; }

        public CompilationUnitSyntax? Code { get; private set; }

        public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.Handler;

        public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

        public string ResultTypeName => ApiOperation.GetOperationName() + NameConstants.ContractResult;

        public string HandlerTypeName => ApiOperation.GetOperationName() + NameConstants.Handler;

        public bool HasParametersOrRequestBody { get; }

        public bool GenerateCode()
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(
                DomainProjectOptions,
                NameConstants.Handlers,
                FocusOnSegmentName,
                false);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.CreateWithInterface(HandlerTypeName, InterfaceTypeName)
                .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForHandlers(ApiOperation, FocusOnSegmentName));

            // Create members
            var memberDeclarations = CreateMembers();

            // Add members to class
            classDeclaration = memberDeclarations.Aggregate(
                classDeclaration,
                (current, memberDeclaration) => current.AddMembers(memberDeclaration));

            // Add using statement to compilationUnit
            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectHandlerFactory.CreateUsingList(
                    DomainProjectOptions,
                    FocusOnSegmentName));

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
                return $"Syntax generate problem for handler for apiOperation: {ApiOperation}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString();
        }

        public LogKeyValueItem ToFile()
        {
            var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var file = Util.GetCsFileNameForHandler(DomainProjectOptions.PathForSrcHandlers!, area, HandlerTypeName);
            return TextFileHelper.Save(file, ToCodeAsString(), false);
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

        private List<MemberDeclarationSyntax> CreateMembers()
        {
            var result = new List<MemberDeclarationSyntax>
            {
                CreateExecuteAsyncMethod(ParameterTypeName, ResultTypeName, HasParametersOrRequestBody),
            };

            if (HasParametersOrRequestBody)
            {
                result.Add(CreateInvokeExecuteAsyncMethod(ParameterTypeName, ResultTypeName, HasParametersOrRequestBody));
            }

            return result;
        }

        private MemberDeclarationSyntax CreateExecuteAsyncMethod(string parameterTypeName, string resultTypeName, bool hasParameters)
        {
            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                };

            var codeBody = hasParameters
                ? SyntaxFactory.Block(
                    SyntaxIfStatementFactory.CreateParameterArgumentNullCheck("parameters"),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("InvokeExecuteAsync"))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                            SyntaxTokenFactory.Comma(),
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(CancellationToken).EnsureFirstCharacterToLower())),
                                        })))))
                : SyntaxFactory.Block(
                    SyntaxThrowStatementFactory.CreateNotImplementedException());

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName)),
                    SyntaxFactory.Identifier("ExecuteAsync"))
                .WithModifiers(SyntaxTokenListFactory.PublicKeyword())
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithBody(codeBody);
        }

        private MemberDeclarationSyntax CreateInvokeExecuteAsyncMethod(string parameterTypeName, string resultTypeName, bool hasParameters)
        {
            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                };

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName)),
                    SyntaxFactory.Identifier("InvokeExecuteAsync"))
                .WithModifiers(SyntaxTokenListFactory.PrivateAsyncKeyword())
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithBody(SyntaxFactory.Block(SyntaxThrowStatementFactory.CreateNotImplementedException()));
        }
    }
}