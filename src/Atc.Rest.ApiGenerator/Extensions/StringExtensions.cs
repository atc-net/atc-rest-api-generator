using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace System
{
    internal static class StringExtensions
    {
        private const string AutoPropGetSetResultPattern = " { get; set; }";
        private const string AutoPropGetResultPattern = " { get; }";
        private static readonly Regex AutoPropGetSetRegex = new Regex(@"\s*\{\s*get;\s*set;\s*}\s");
        private static readonly Regex AutoPropGetRegex = new Regex(@"\s*\{\s*get;\s*}\s");
        private static readonly Regex AutoPropInitializerGetSetRegex = new Regex(@"\s*\{ get; set; }\s*= \s*");
        private static readonly Regex AutoPropInitializerGetRegex = new Regex(@"\s*\{ get; }\s*= \s*");
        private static readonly Regex AutoPublicLinesRegex = new Regex(@"\s*;\s*public \s*");
        private static readonly Regex AutoPrivateLinesRegex = new Regex(@"\s*;\s*private \s*");
        private static readonly Regex AutoCommentLinesRegex = new Regex(@"\s*;\s*/// \s*");
        private static readonly Regex AutoBracketSpacingStartRegex = new Regex(@"(\S)({)(\S)");
        private static readonly Regex AutoBracketSpacingEndRegex = new Regex(@"(\S)(})(\S)");
        private static readonly Regex ConstructorWithInheritResultRegex = new Regex(@":\s*base\(result\)\s*\{\s*\}");

        public static string FormatAutoPropertiesOnOneLine(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = AutoPropGetSetRegex.Replace(value, AutoPropGetSetResultPattern);
            value = AutoPropGetRegex.Replace(value, AutoPropGetSetResultPattern);
            value = AutoPropInitializerGetSetRegex.Replace(value, $"{AutoPropGetSetResultPattern} = ");
            value = AutoPropInitializerGetRegex.Replace(value, $"{AutoPropGetResultPattern} = ");
            value = value.Replace(">();", $">();{Environment.NewLine}", StringComparison.Ordinal);
            return value;
        }

        public static string FormatDoubleLines(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Replace($"{Environment.NewLine}{Environment.NewLine}    }}", $"{Environment.NewLine}    }}", StringComparison.Ordinal);
        }

        public static string FormatPublicPrivateLines(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = AutoPublicLinesRegex.Replace(value, $";{Environment.NewLine}{Environment.NewLine}        public ");
            value = AutoPrivateLinesRegex.Replace(value, $";{Environment.NewLine}{Environment.NewLine}        private ");
            value = AutoCommentLinesRegex.Replace(value, $";{Environment.NewLine}{Environment.NewLine}        /// ");
            return value;
        }

        public static string FormatBracketSpacing(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = AutoBracketSpacingStartRegex.Replace(value, "$1 { $3");
            value = AutoBracketSpacingEndRegex.Replace(value, "$1 } $3");
            return value;
        }

        public static string FormatConstructorWithInheritResult(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = ConstructorWithInheritResultRegex.Replace(value, ": base(result) { }");
            return value;
        }

        public static string FormatRemoveEmptyBracketsInitialize(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Replace("{};", "();", StringComparison.Ordinal);
        }

        public static string FormatCs1998(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            const string pattern = "CS1998 // Async method lacks 'await' operators and will run synchronously";
            return value.Replace(pattern + Environment.NewLine, pattern, StringComparison.Ordinal);
        }

        public static string FormatClientEndpointNewLineOnFluentMethod(this string value)
        {
            var methodList = new List<string>
            {
                // TODO: "FromTemplate",
                "FromResponse",
                "AddSuccessResponse",
                "AddErrorResponse",
                "BuildResponseAsync",
            };

            foreach (var method in methodList)
            {
                var s1 = $".{method}(";
                var s2 = $".{method}<";

                if (value.Contains(s1, StringComparison.Ordinal))
                {
                    value = value.Replace(
                        s1,
                        $"{Environment.NewLine}                {s1}",
                        StringComparison.Ordinal);
                }

                if (value.Contains(s2, StringComparison.Ordinal))
                {
                    value = value.Replace(
                        s2,
                        $"{Environment.NewLine}                {s2}",
                        StringComparison.Ordinal);
                }
            }

            return value;
        }

        public static string FormatClientEndpointNewLineSpace(this string value)
        {
            var list = new List<string>
            {
                "var requestBuilder = httpMessageFactory",
                "using var requestMessage",
                "return await httpMessageFactory",
            };

            return list.Aggregate(
                value,
                (current, item) => current.Replace(
                    $"            {item}",
                    $"{Environment.NewLine}            {item}",
                    StringComparison.Ordinal));
        }

        public static string EnsureNewlineAfterMethod(this string value, string methodName)
        {
            return value.Replace(methodName, methodName + Environment.NewLine, StringComparison.Ordinal);
        }
    }
}