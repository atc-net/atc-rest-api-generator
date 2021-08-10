﻿using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Eventargs;

namespace Demo.Domain.Handlers.Eventargs
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get EventArgs List.
    /// Operation: GetEventArgs.
    /// Area: Eventargs.
    /// </summary>
    public class GetEventArgsHandler : IGetEventArgsHandler
    {
        public Task<GetEventArgsResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}