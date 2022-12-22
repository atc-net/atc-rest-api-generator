﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
using Demo.Api.Generated.Contracts.Tasks;

namespace Demo.Api.Tests.Endpoints.Tasks.Generated
{
    [GeneratedCode("ApiGenerator", "2.0.259.29354")]
    public class GetTasksHandlerStub : IGetTasksHandler
    {
        public Task<GetTasksResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var data = new List<Demo.Api.Generated.Contracts.Tasks.Task>
            {
                new Demo.Api.Generated.Contracts.Tasks.Task
                {
                    Id = Guid.Parse("77a33260-0001-441f-ba60-b0a833803fab"),
                    Name = "Hallo11",
                },
                new Demo.Api.Generated.Contracts.Tasks.Task
                {
                    Id = Guid.Parse("77a33260-0002-441f-ba60-b0a833803fab"),
                    Name = "Hallo21",
                },
                new Demo.Api.Generated.Contracts.Tasks.Task
                {
                    Id = Guid.Parse("77a33260-0003-441f-ba60-b0a833803fab"),
                    Name = "Hallo31",
                },
            };

            return System.Threading.Tasks.Task.FromResult(GetTasksResult.Ok(data));
        }
    }
}