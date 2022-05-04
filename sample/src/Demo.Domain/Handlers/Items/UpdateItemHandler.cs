﻿using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Items;

namespace Demo.Domain.Handlers.Items
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Updates an item.
    /// Operation: UpdateItem.
    /// Area: Items.
    /// </summary>
    public class UpdateItemHandler : IUpdateItemHandler
    {
        public Task<UpdateItemResult> ExecuteAsync(UpdateItemParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<UpdateItemResult> InvokeExecuteAsync(UpdateItemParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}