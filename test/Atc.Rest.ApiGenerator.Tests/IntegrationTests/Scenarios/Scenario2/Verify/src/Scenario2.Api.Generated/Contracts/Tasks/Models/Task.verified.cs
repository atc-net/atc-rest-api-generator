//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Contracts.Tasks;

/// <summary>
/// Describes a single task.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class Task
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
}