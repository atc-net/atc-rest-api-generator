namespace Atc.CodeGeneration.CSharp.Content;

public enum AccessModifiers
{
    None,

    [Description("public")]
    Public,

    [Description("private")]
    Private,

    [Description("protected")]
    Protected,

    [Description("internal")]
    Internal,

    [Description("protected internal")]
    ProtectedInternal,

    [Description("private protected")]
    PrivateProtected,
}