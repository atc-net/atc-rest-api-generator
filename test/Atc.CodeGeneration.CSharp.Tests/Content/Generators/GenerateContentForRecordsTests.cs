namespace Atc.CodeGeneration.CSharp.Tests.Content.Generators;

public class GenerateContentForRecordsTests : GenerateContentBaseTests
{
    [Fact]
    public void Scenario_EnergyConsumption_CreateLocationParameters()
    {
        const string expectedCode =
            @"namespace Energy.Consumption.Api.Contracts.Contracts.Petrol.Parameters;

public record struct CreateLocationParameters(
    CreateLocationRequest? Request);

public record struct GetLocationByIdParameters(
    [FromRoute, Required] Guid LocationId);

public record struct GetLocationsByCountryCodeA3Parameters(
    [FromQuery] string CountryCodeA3);";

        var recordsParameters = new RecordsParameters(
            HeaderContent: null,
            "Energy.Consumption.Api.Contracts.Contracts.Petrol.Parameters",
            DocumentationTags: null,
            Attributes: null,
            new List<RecordParameters>
            {
                new(
                    DocumentationTags: null,
                    AccessModifiers.PublicRecordStruct,
                    Name: "CreateLocationParameters",
                    new List<ParameterBaseParameters>
                    {
                        new(
                            Attributes: null,
                            GenericTypeName: null,
                            TypeName: "CreateLocationRequest?",
                            Name: "Request",
                            DefaultValue: null),
                    }),
                new(
                    DocumentationTags: null,
                    AccessModifiers.PublicRecordStruct,
                    Name: "GetLocationByIdParameters",
                    new List<ParameterBaseParameters>
                    {
                        new(
                            Attributes: new List<AttributeParameters> { new("FromRoute, Required", Content: null) },
                            GenericTypeName: null,
                            TypeName: "Guid",
                            Name: "LocationId",
                            DefaultValue: null),
                    }),
                new(
                    DocumentationTags: null,
                    AccessModifiers.PublicRecordStruct,
                    Name: "GetLocationsByCountryCodeA3Parameters",
                    new List<ParameterBaseParameters>
                    {
                        new(
                            Attributes: new List<AttributeParameters> { new("FromQuery", Content: null) },
                            GenericTypeName: null,
                            TypeName: "string",
                            Name: "CountryCodeA3",
                            DefaultValue: null),
                    }),
            });

        var sut = new GenerateContentForRecords(
            CodeDocumentationTagsGenerator,
            recordsParameters);

        var generatedCode = sut.Generate();

        Assert.NotNull(generatedCode);
        Assert.Equal(expectedCode, generatedCode);
    }
}