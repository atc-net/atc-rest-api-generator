namespace Atc.CodeGeneration.CSharp.Tests.Content.Generators;

public abstract class GenerateContentBaseTests
{
    protected const string HeaderContent = @"//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator X.X.X.X.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------";

    public ICodeDocumentationTagsGenerator CodeDocumentationTagsGenerator { get; } = new CodeDocumentationTagsGenerator();

    public IList<AttributeParameters> AttributesWithGeneratedCode { get; } = AttributesParametersFactory.Create("GeneratedCode", "\"ApiGenerator\", \"X.X.X.X\"");
}