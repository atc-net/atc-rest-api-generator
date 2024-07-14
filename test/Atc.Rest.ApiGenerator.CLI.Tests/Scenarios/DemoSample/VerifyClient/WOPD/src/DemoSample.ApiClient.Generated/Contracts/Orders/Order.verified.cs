﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Contracts.Orders;

/// <summary>
/// A single order.
/// Hallo description with multiline and no ending dot.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class Order
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public DateTimeOffset MyTime { get; set; }

    /// <summary>
    /// Undefined description.
    /// </summary>
    /// <remarks>
    /// Email validation being enforced.
    /// </remarks>
    [EmailAddress]
    public string MyEmail { get; set; } = "a@a.com";

    public DateTimeOffset? MyNullableDateTime { get; set; }

    public DateTimeOffset MyDateTime { get; set; }

    [Range(1.1, 20.2)]
    public double MyNumber { get; set; }

    [Range(int.MinValue, 50)]
    public int MyInteger { get; set; } = 15;

    /// <summary>
    /// MyBool is great.
    /// </summary>
    public bool MyBool { get; set; }

    /// <summary>
    /// This is the good uri :-).
    /// </summary>
    /// <remarks>
    /// Url validation being enforced.
    /// </remarks>
    [Uri]
    public Uri MyUri { get; set; }

    /// <summary>
    /// Undefined description.
    /// </summary>
    /// <remarks>
    /// This string should be base64-encoded.
    /// </remarks>
    public string MyByte { get; set; }

    /// <summary>
    /// Hallo myStringList desc :-).
    /// </summary>
    public List<string> MyStringList { get; set; } = new List<string>();

    [Range(10, int.MaxValue)]
    public long MyLong { get; set; }

    /// <summary>
    /// Address.
    /// </summary>
    public Address DeliveryAddress { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Description)}: {Description}, {nameof(MyTime)}: ({MyTime}), {nameof(MyEmail)}: {MyEmail}, {nameof(MyNullableDateTime)}: ({MyNullableDateTime}), {nameof(MyDateTime)}: ({MyDateTime}), {nameof(MyNumber)}: {MyNumber}, {nameof(MyInteger)}: {MyInteger}, {nameof(MyBool)}: {MyBool}, {nameof(MyUri)}: ({MyUri}), {nameof(MyByte)}: {MyByte}, {nameof(MyStringList)}.Count: {MyStringList?.Count ?? 0}, {nameof(MyLong)}: {MyLong}, {nameof(DeliveryAddress)}: ({DeliveryAddress})";
}