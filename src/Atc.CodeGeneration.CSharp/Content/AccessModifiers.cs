namespace Atc.CodeGeneration.CSharp.Content;

[Flags]
public enum AccessModifiers
{
    None = 0x00,
    Public = 0x01,
    Private = 0x02,
    Protected = 0x04,
    Internal = 0x08,
}