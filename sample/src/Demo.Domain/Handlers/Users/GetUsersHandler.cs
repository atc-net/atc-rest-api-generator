﻿using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Users;

namespace Demo.Domain.Handlers.Users
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get all users.
    /// Operation: GetUsers.
    /// Area: Users.
    /// </summary>
    public class GetUsersHandler : IGetUsersHandler
    {
        public Task<GetUsersResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}