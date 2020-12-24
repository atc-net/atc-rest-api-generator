using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Users;

namespace Demo.Domain.Handlers.Users
{
    public class GetUsersHandler : IGetUsersHandler
    {
        public async Task<GetUsersResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var data = new List<User>
            {
                new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "John.Doe@jd.com",
                },
                new User
                {
                    FirstName = "John",
                    LastName = "Doe2",
                    Email = "John.Doe2@jd.com",
                },
            };

            return await Task.FromResult(data);
        }
    }
}