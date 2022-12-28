// ReSharper disable CheckNamespace
namespace Atc.Rest.ApiGenerator.Framework.Contracts;

public enum SchemaType
{
    Unknown,
    None,
    ComplexType,
    ComplexTypeList,
    ComplexTypePagedList,
    SimpleType,
    SimpleTypeList,
    SimpleTypePagedList,
}