//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithNsWithTask.ApiClient.Generated.Contracts.EventArgs;

/// <summary>
/// EventArgs.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class EventArgs
{
    public Guid Id { get; set; }

    public string EventName { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(EventName)}: {EventName}";
}
