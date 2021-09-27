using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario1.Api.Generated.Contracts.Users
{
    /// <summary>
    /// A single user.
    /// </summary>
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// Undefined description.
        /// </summary>
        /// <remarks>
        /// Email validation being enforced.
        /// </remarks>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Undefined description.
        /// </summary>
        /// <remarks>
        /// Url validation being enforced.
        /// </remarks>
        [Uri]
        public Uri Homepage { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        public Address HomeAddress { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        public Address CompanyAddress { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(Email)}: {Email}, {nameof(Homepage)}: {Homepage}, {nameof(HomeAddress)}: ({HomeAddress}), {nameof(CompanyAddress)}: ({CompanyAddress})";
        }
    }
}