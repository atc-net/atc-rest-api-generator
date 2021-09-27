using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario1.Api.Generated.Contracts
{
    /// <summary>
    /// Address.
    /// </summary>
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class Address
    {
        [StringLength(255)]
        public string StreetName { get; set; }

        public string StreetNumber { get; set; }

        public string PostalCode { get; set; }

        public string CityName { get; set; }

        /// <summary>
        /// Country.
        /// </summary>
        public Country MyCountry { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(StreetNumber)}: {StreetNumber}, {nameof(PostalCode)}: {PostalCode}, {nameof(CityName)}: {CityName}, {nameof(MyCountry)}: ({MyCountry})";
        }
    }
}