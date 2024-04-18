namespace Atc.CodeGeneration.CSharp.Content;

public enum AccessModifiers
{
    None,

    [Description("public")]
    Public,

    [Description("public async")]
    PublicAsync,

    [Description("public static")]
    PublicStatic,

    [Description("public static implicit operator")]
    PublicStaticImplicitOperator,

    [Description("public class")]
    PublicClass,

    [Description("public static class")]
    PublicStaticClass,

    [Description("public record")]
    PublicRecord,

    [Description("public record struct")]
    PublicRecordStruct,

    [Description("private")]
    Private,

    [Description("private async")]
    PrivateAsync,

    [Description("protected")]
    Protected,

    [Description("internal")]
    Internal,
}