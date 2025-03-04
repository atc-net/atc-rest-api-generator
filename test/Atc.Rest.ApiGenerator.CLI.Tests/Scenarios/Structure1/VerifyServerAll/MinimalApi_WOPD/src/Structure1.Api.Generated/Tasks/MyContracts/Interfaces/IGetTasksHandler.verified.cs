﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.Tasks.MyContracts;

/// <summary>
/// Domain Interface for RequestHandler.
/// Description: Returns tasks.
/// Operation: GetTasks.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetTasksHandler
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetTasksResult> ExecuteAsync(
        CancellationToken cancellationToken = default);
}