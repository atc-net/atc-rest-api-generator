using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Files;

namespace Demo.Domain.Handlers.Files
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get File By Id.
    /// Operation: GetFileById.
    /// Area: Files.
    /// </summary>
    public class GetFileByIdHandler : IGetFileByIdHandler
    {
        public Task<GetFileByIdResult> ExecuteAsync(GetFileByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

        private async Task<GetFileByIdResult> InvokeExecuteAsync(GetFileByIdParameters parameters, CancellationToken cancellationToken)
        {
            var bytes = await File.ReadAllBytesAsync(@"c:\temp\stoplight-studio-win.exe", cancellationToken);
            return GetFileByIdResult.Ok(bytes, "stoplight-studio-win.exe");

            //await Task.Delay(10, cancellationToken);
            //var bytes = Encoding.UTF8.GetBytes(parameters.Id);
            //return GetFileByIdResult.Ok(bytes, "dummy.txt");
        }
    }
}