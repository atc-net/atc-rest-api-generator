﻿namespace Structure1.Domain.Addresses.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Get addresses by postal code.
/// Operation: GetAddressesByPostalCodes.
/// </summary>
public sealed class GetAddressesByPostalCodesHandler : IGetAddressesByPostalCodesHandler
{
    public Task<GetAddressesByPostalCodesResult> ExecuteAsync(
        GetAddressesByPostalCodesParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetAddressesByPostalCodesHandler");
    }
}