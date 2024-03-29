// ReSharper disable CheckNamespace
namespace Atc.Rest.ApiGenerator.Framework.Contracts;

[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "OK.")]
public enum CardinalityType
{
    None,
    Single,
    Multiple,
    Paged,
}