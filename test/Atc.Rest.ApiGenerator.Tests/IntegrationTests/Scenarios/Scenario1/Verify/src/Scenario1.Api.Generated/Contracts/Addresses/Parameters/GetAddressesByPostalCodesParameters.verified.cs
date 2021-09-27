using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario1.Api.Generated.Contracts.Addresses
{
    /// <summary>
    /// Parameters for operation request.
    /// Description: Get addresses by postal code.
    /// Operation: GetAddressesByPostalCodes.
    /// Area: Addresses.
    /// </summary>
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class GetAddressesByPostalCodesParameters
    {
        /// <summary>
        /// The postalCode to limit addresses on.
        /// </summary>
        [FromRoute(Name = "postalCode")]
        [Required]
        public string PostalCode { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(PostalCode)}: {PostalCode}";
        }
    }
}