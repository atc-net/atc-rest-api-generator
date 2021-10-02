﻿using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Contracts.Addresses
{
    /// <summary>
    /// Domain Interface for RequestHandler.
    /// Description: Get addresses by postal code.
    /// Operation: GetAddressesByPostalCodes.
    /// Area: Addresses.
    /// </summary>
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public interface IGetAddressesByPostalCodesHandler
    {
        /// <summary>
        /// Execute method.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<GetAddressesByPostalCodesResult> ExecuteAsync(GetAddressesByPostalCodesParameters parameters, CancellationToken cancellationToken = default);
    }
}