namespace Atc.Rest.ApiGenerator.Models.ApiOptions
{
    public class ApiOptions
    {
        public ApiOptionsGenerator Generator { get; set; } = new ApiOptionsGenerator();

        public ApiOptionsValidation Validation { get; set; } = new ApiOptionsValidation();
    }
}