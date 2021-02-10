using System.CodeDom.Compiler;
using Atc.Rest.Results;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestProject.AtcTest.Contracts.Test
{
    /// <summary>
    /// Results for operation request.
    /// Description: Get all users.
    /// Operation: GetUsers.
    /// Area: Test.
    /// </summary>
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class GetUsersResult : ResultBase
    {
        private GetUsersResult(ActionResult result) : base(result) { }

        /// <summary>
        /// 200 - Ok response.
        /// </summary>
        public static GetUsersResult Ok(string? message = null) => new GetUsersResult(new OkObjectResult(message));

        /// <summary>
        /// Performs an implicit conversion from GetUsersResult to ActionResult.
        /// </summary>
        public static implicit operator GetUsersResult(string x) => Ok(x);
    }
}