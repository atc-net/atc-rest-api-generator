namespace Atc.CodeGeneration.CSharp.Tests.Content.Generators;

public class GenerateContentForRecordsTests : GenerateContentBaseTests
{
    [Fact]
    public void Scenario_EnergyConsumption_CreateLocationParameters()
    {
        const string expectedCode =
            @"namespace Energy.Consumption.Api.Contracts.Contracts.Petrol.Parameters;

public record CreateLocationParameters(
    CreateLocationRequest? Request);

public record GetLocationByIdParameters(
    [property: FromRoute, Required] Guid LocationId);

public record GetLocationsByCountryCodeA3Parameters(
    [property: FromQuery] string CountryCodeA3);";

        var recordsParameters = new RecordsParameters(
            HeaderContent: null,
            "Energy.Consumption.Api.Contracts.Contracts.Petrol.Parameters",
            DocumentationTags: null,
            Attributes: null,
            new List<RecordParameters>
            {
                new(
                    DocumentationTags: null,
                    AccessModifiers.PublicRecord,
                    Name: "CreateLocationParameters",
                    new List<ParameterBaseParameters>
                    {
                        new(
                            Attributes: null,
                            GenericTypeName: null,
                            IsGenericListType: false,
                            TypeName: "CreateLocationRequest?",
                            IsReferenceType: true,
                            Name: "Request",
                            DefaultValue: null),
                    }),
                new(
                    DocumentationTags: null,
                    AccessModifiers.PublicRecord,
                    Name: "GetLocationByIdParameters",
                    new List<ParameterBaseParameters>
                    {
                        new(
                            Attributes: new List<AttributeParameters> { new("FromRoute, Required", Content: null) },
                            GenericTypeName: null,
                            IsGenericListType: false,
                            TypeName: "Guid",
                            IsReferenceType: false,
                            Name: "LocationId",
                            DefaultValue: null),
                    }),
                new(
                    DocumentationTags: null,
                    AccessModifiers.PublicRecord,
                    Name: "GetLocationsByCountryCodeA3Parameters",
                    new List<ParameterBaseParameters>
                    {
                        new(
                            Attributes: new List<AttributeParameters> { new("FromQuery", Content: null) },
                            GenericTypeName: null,
                            IsGenericListType: false,
                            TypeName: "string",
                            IsReferenceType: false,
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