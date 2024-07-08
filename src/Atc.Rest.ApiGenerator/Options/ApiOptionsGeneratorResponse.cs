namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptionsGeneratorResponse
{
    public bool UseProblemDetailsAsDefaultBody { get; set; }

    public CustomErrorResponseModel? CustomErrorResponseModel { get; set; }

    public override string ToString()
        => $"{nameof(UseProblemDetailsAsDefaultBody)}: {UseProblemDetailsAsDefaultBody}, {nameof(CustomErrorResponseModel)}: {CustomErrorResponseModel}";
}