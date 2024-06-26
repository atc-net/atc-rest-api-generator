namespace ExampleWithNsWithTask.Domain.Handlers.Tasks;

/// <summary>
/// Handler for operation request.
/// Description: Returns tasks.
/// Operation: GetTasks.
/// </summary>
public class GetTasksHandler : IGetTasksHandler
{
    public Task<GetTasksResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetTasksHandler");
    }
}