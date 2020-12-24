using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

// ReSharper disable UseDeconstructionOnParameter
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.ProjectSyntaxFactories
{
    internal static class SyntaxDocumentationFactory
    {
        private const string Prefix = "/// ";

        public static SyntaxTriviaList Create(OpenApiSchema apiSchema)
        {
            if (apiSchema == null)
            {
                throw new ArgumentNullException(nameof(apiSchema));
            }

            var comments = new List<SyntaxTrivia>();
            comments.AddRange(CreateSummary(apiSchema));
            comments.AddRange(CreateExample(apiSchema));
            comments.AddRange(CreateRemarks(apiSchema));

            return SyntaxFactory.TriviaList(comments);
        }

        public static SyntaxTriviaList CreateForInterface(OpenApiOperation apiOperation, string area)
        {
            if (apiOperation == null)
            {
                throw new ArgumentNullException(nameof(apiOperation));
            }

            return CreateSummary("Domain Interface for RequestHandler.", apiOperation, area);
        }

        public static SyntaxTriviaList CreateForInterfaceMethod(bool hasParameters)
        {
            if (hasParameters)
            {
                return SyntaxFactory.TriviaList(new List<SyntaxTrivia>
                {
                    CreateComment("<summary>"),
                    CreateComment("Execute method."),
                    CreateComment("</summary>"),
                    CreateComment("<param name=\"parameters\">The parameters.</param>"),
                    CreateComment("<param name=\"cancellationToken\">The cancellation token.</param>"),
                });
            }

            return SyntaxFactory.TriviaList(new List<SyntaxTrivia>
            {
                CreateComment("<summary>"),
                CreateComment("Execute method."),
                CreateComment("</summary>"),
                CreateComment("<param name=\"cancellationToken\">The cancellation token.</param>"),
            });
        }

        public static SyntaxTriviaList CreateForParameters(OpenApiOperation apiOperation, string area)
        {
            if (apiOperation == null)
            {
                throw new ArgumentNullException(nameof(apiOperation));
            }

            return CreateSummary("Parameters for operation request.", apiOperation, area);
        }

        public static SyntaxTriviaList CreateForParameter(OpenApiParameter apiParameter)
        {
            if (apiParameter == null)
            {
                throw new ArgumentNullException(nameof(apiParameter));
            }

            var comments = new List<SyntaxTrivia>();
            comments.AddRange(CreateSummaryForParameter(apiParameter));

            return SyntaxFactory.TriviaList(comments);
        }

        public static SyntaxTriviaList CreateForParameter(OpenApiRequestBody apiRequestBody)
        {
            if (apiRequestBody == null)
            {
                throw new ArgumentNullException(nameof(apiRequestBody));
            }

            var comments = new List<SyntaxTrivia>();
            comments.AddRange(CreateSummaryForParameter(apiRequestBody));

            return SyntaxFactory.TriviaList(comments);
        }

        public static SyntaxTriviaList CreateForResults(OpenApiOperation apiOperation, string area)
        {
            if (apiOperation == null)
            {
                throw new ArgumentNullException(nameof(apiOperation));
            }

            return CreateSummary("Results for operation request.", apiOperation, area);
        }

        public static SyntaxTriviaList CreateForResultsImplicitOperator(string className)
        {
            return CreateSummary($"Performs an implicit conversion from {className} to ActionResult.");
        }

        public static SyntaxTriviaList CreateForResultsMethod(HttpStatusCode httpStatusCode, string? validationTypeName = null)
        {
            if (string.IsNullOrEmpty(validationTypeName))
            {
                return SyntaxFactory.TriviaList(new List<SyntaxTrivia>
                {
                    CreateComment("<summary>"),
                    CreateComment($"{(int) httpStatusCode} - {httpStatusCode.ToNormalizedString()} response."),
                    CreateComment("</summary>"),
                });
            }

            return SyntaxFactory.TriviaList(new List<SyntaxTrivia>
            {
                CreateComment("<summary>"),
                CreateComment($"{(int)httpStatusCode} - {httpStatusCode.ToNormalizedString()} response ({validationTypeName})."),
                CreateComment("</summary>"),
            });
        }

        public static SyntaxTriviaList CreateForEndpoints(string area)
        {
            return SyntaxFactory.TriviaList(new List<SyntaxTrivia>
            {
                CreateComment("<summary>"),
                CreateComment("Endpoint definitions."),
                CreateComment($"Area: {area.EnsureFirstCharacterToUpper()}."),
                CreateComment("</summary>"),
            });
        }

        public static SyntaxTriviaList CreateForEndpointMethods(KeyValuePair<OperationType, OpenApiOperation> apiOperation, string area)
        {
            var comments = new List<SyntaxTrivia>();

            var operationName = apiOperation.Value.GetOperationName();
            var operationSummary = apiOperation.Value.Summary;

            if (string.IsNullOrEmpty(operationSummary))
            {
                operationSummary = apiOperation.Value.Description;
            }

            if (string.IsNullOrEmpty(operationSummary))
            {
                operationSummary = "Undefined description.";
            }

            comments.Add(CreateComment("<summary>"));
            comments.Add(CreateComment($"Description: {operationSummary}", true));
            comments.Add(CreateComment($"Operation: {operationName}", true));
            comments.Add(CreateComment($"Area: {area.EnsureFirstCharacterToUpper()}", true));
            comments.Add(CreateComment("</summary>"));

            return SyntaxFactory.TriviaList(comments);
        }

        public static SyntaxTriviaList CreateForHandlers(OpenApiOperation apiOperation, string area)
        {
            if (apiOperation == null)
            {
                throw new ArgumentNullException(nameof(apiOperation));
            }

            return CreateSummary("Handler for operation request.", apiOperation, area);
        }

        public static SyntaxTriviaList CreateForOverrideToString()
        {
            return SyntaxFactory.TriviaList(new List<SyntaxTrivia>
            {
                CreateComment("<summary>"),
                CreateComment("Converts to string."),
                CreateComment("</summary>"),
            });
        }

        private static SyntaxTrivia CreateComment(string value, bool ensureEndingDot = false)
        {
            if (ensureEndingDot && !value.EndsWith(".", StringComparison.Ordinal))
            {
                value += ".";
            }

            return SyntaxFactory.Comment(Prefix + value);
        }

        public static SyntaxTriviaList CreateSummary(string value, bool ensureEndingDot = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var comments = new List<SyntaxTrivia>
            {
                CreateComment("<summary>"),
                CreateComment(value, ensureEndingDot),
                CreateComment("</summary>"),
            };

            return SyntaxFactory.TriviaList(comments);
        }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        private static IEnumerable<SyntaxTrivia> CreateSummary(OpenApiSchema apiSchema)
        {
            if (string.IsNullOrEmpty(apiSchema.Title) &&
                string.IsNullOrEmpty(apiSchema.Description) &&
                !ShouldGenerateDefaultSummary(apiSchema) &&
                !apiSchema.OneOf.Any())
            {
                return SyntaxFactory.TriviaList();
            }

            var comments = new List<SyntaxTrivia>
            {
                CreateComment("<summary>"),
            };

            if (!string.IsNullOrEmpty(apiSchema.Description))
            {
                var lines = apiSchema.Description.Split('\n');
                for (var i = 0; i < lines.Length; i++)
                {
                    var s = lines[i].Trim();
                    comments.Add(i == lines.Length - 1
                        ? CreateComment(s, true)
                        : CreateComment(s));
                }
            }
            else if (!string.IsNullOrEmpty(apiSchema.Title))
            {
                comments.Add(CreateComment(apiSchema.Title, true));
            }
            else if (apiSchema.OneOf != null &&
                     apiSchema.OneOf.Count == 1 &&
                     apiSchema.OneOf.First().Reference != null)
            {
                var schema = apiSchema.OneOf.First();
                if (!string.IsNullOrEmpty(schema.Description))
                {
                    comments.Add(CreateComment(schema.Description, true));
                }
                else if (!string.IsNullOrEmpty(schema.Title))
                {
                    comments.Add(CreateComment(schema.Title, true));
                }
            }

            if (ShouldGenerateDefaultSummary(apiSchema) && comments.Count == 1)
            {
                comments.Add(CreateComment("Undefined description."));
            }

            comments.Add(CreateComment("</summary>"));

            return comments;
        }

        private static IEnumerable<SyntaxTrivia> CreateSummary(
            string title,
            string description,
            string operationId,
            string area)
        {
            return new List<SyntaxTrivia>
            {
                CreateComment("<summary>"),
                CreateComment(title),
                CreateComment($"Description: {description}", true),
                CreateComment($"Operation: {operationId}."),
                CreateComment($"Area: {area.EnsureFirstCharacterToUpper()}."),
                CreateComment("</summary>"),
            };
        }

        private static SyntaxTriviaList CreateSummary(string title, OpenApiOperation apiOperation, string area)
        {
            var operationSummary = apiOperation.Summary;
            if (string.IsNullOrEmpty(operationSummary))
            {
                operationSummary = apiOperation.Description;
            }

            var comments = new List<SyntaxTrivia>();
            comments.AddRange(CreateSummary(title, operationSummary, apiOperation.GetOperationName(), area));

            return SyntaxFactory.TriviaList(comments);
        }

        private static SyntaxTriviaList CreateSummaryForParameter(OpenApiParameter apiParameter)
        {
            var comments = new List<SyntaxTrivia>();

            var apiParameterDescription = apiParameter.Description;
            if (!string.IsNullOrEmpty(apiParameterDescription))
            {
                comments.Add(CreateComment("<summary>"));
                comments.Add(CreateComment(apiParameterDescription, true));
                comments.Add(CreateComment("</summary>"));
            }

            return SyntaxFactory.TriviaList(comments);
        }

        private static SyntaxTriviaList CreateSummaryForParameter(OpenApiRequestBody apiRequestBody, string contentType = MediaTypeNames.Application.Json)
        {
            var comments = new List<SyntaxTrivia>();

            var (key, value) = apiRequestBody.Content.FirstOrDefault(x => x.Key == contentType);
            var apiParameterDescription = key == null
                ? string.Empty
                : value.Schema.Description;

            if (!string.IsNullOrEmpty(apiParameterDescription))
            {
                comments.Add(CreateComment("<summary>"));
                comments.Add(CreateComment(apiParameterDescription, true));
                comments.Add(CreateComment("</summary>"));
            }

            return SyntaxFactory.TriviaList(comments);
        }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        private static IEnumerable<SyntaxTrivia> CreateExample(OpenApiSchema apiSchema)
        {
            if (apiSchema.Extensions == null || apiSchema.Extensions.All(x => x.Key != "x-examples"))
            {
                return SyntaxFactory.TriviaList();
            }

            var list = apiSchema.Extensions
                .Where(x => x.Key == "x-examples")
                .ToList();

            var comments = new List<SyntaxTrivia>
            {
                CreateComment("<example>"),
            };

            foreach (var pair in list)
            {
                if (!(pair.Value is OpenApiObject openApiObject))
                {
                    continue;
                }

                foreach (var item in openApiObject!.Values)
                {
                    if (!(item is Dictionary<string, IOpenApiAny> dictionaries))
                    {
                        continue;
                    }

                    if (!dictionaries.Any())
                    {
                        continue;
                    }

                    var lastItem = dictionaries.Last();
                    var isLastItem = false;

                    foreach (var dict in dictionaries)
                    {
                        if (dict.Equals(lastItem))
                        {
                            isLastItem = true;
                        }

                        switch (dict.Value)
                        {
                            case OpenApiString openApiString:
                                comments.Add(CreateComment($"{dict.Key}: {openApiString.Value}", isLastItem));
                                break;
                            case OpenApiInteger openApiInteger:
                                comments.Add(CreateComment($"{dict.Key}: {openApiInteger.Value}", isLastItem));
                                break;
                        }
                    }
                }
            }

            comments.Add(CreateComment("</example>"));
            return comments;
        }

        private static IEnumerable<SyntaxTrivia> CreateRemarks(OpenApiSchema apiSchema)
        {
            return apiSchema.Format switch
            {
                OpenApiFormatTypeConstants.Byte => new List<SyntaxTrivia>
                {
                    CreateComment("<remarks>"),
                    CreateComment("This string should be base64-encoded."),
                    CreateComment("</remarks>"),
                },
                OpenApiFormatTypeConstants.Email => new List<SyntaxTrivia>
                {
                    CreateComment("<remarks>"),
                    CreateComment("Email validation being enforced."),
                    CreateComment("</remarks>"),
                },
                OpenApiFormatTypeConstants.Uri => new List<SyntaxTrivia>
                {
                    CreateComment("<remarks>"),
                    CreateComment("Url validation being enforced."),
                    CreateComment("</remarks>"),
                },
                _ => SyntaxFactory.TriviaList()
            };
        }

        private static bool ShouldGenerateDefaultSummary(OpenApiSchema apiSchema)
        {
            return apiSchema.Format == OpenApiFormatTypeConstants.Byte ||
                   apiSchema.Format == OpenApiFormatTypeConstants.Email ||
                   apiSchema.Format == OpenApiFormatTypeConstants.Uri;
        }
    }
}