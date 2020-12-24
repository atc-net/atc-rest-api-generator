using System.IO;
using Atc.Data.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators
{
    public interface ISyntaxCodeGenerator
    {
        public string FocusOnSegmentName { get; }

        CompilationUnitSyntax? Code { get; }

        bool GenerateCode();

        string ToCodeAsString();

        LogKeyValueItem ToFile();

        void ToFile(FileInfo file);
    }
}