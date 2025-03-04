namespace Atc.CodeGeneration.CSharp.Content;

public enum DeclarationModifiers
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

    [Description("public partial class")]
    PublicPartialClass,

    [Description("public static class")]
    PublicStaticClass,

    [Description("public sealed class")]
    PublicSealedClass,

    [Description("public interface")]
    PublicInterface,

    [Description("public partial interface")]
    PublicPartialInterface,

    [Description("public static interface")]
    PublicStaticInterface,

    [Description("public record")]
    PublicRecord,

    [Description("public record struct")]
    PublicRecordStruct,

    [Description("public partial record")]
    PublicPartialRecord,

    [Description("public partial record struct")]
    PublicPartialRecordStruct,

    [Description("private")]
    Private,

    [Description("private async")]
    PrivateAsync,

    [Description("private static async")]
    PrivateStaticAsync,

    [Description("protected")]
    Protected,

    [Description("internal")]
    Internal,
}