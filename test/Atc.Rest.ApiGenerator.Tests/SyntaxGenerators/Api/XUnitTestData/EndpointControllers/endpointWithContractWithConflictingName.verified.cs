using System;
using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestProject.AtcTest.Endpoints
{
    using TestProject.AtcTest.Contracts.Tasks;

    /// <summary>
    /// Endpoint definitions.
    /// Area: Tasks.
    /// </summary>
    [ApiController]
    [Route("/api/v1/tasks")]
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class TasksController : ControllerBase
    {
        /// <summary>
        /// Description: Get.
        /// Operation: Get.
        /// Area: Tasks.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Task), StatusCodes.Status200OK)]
        public Task<ActionResult> GetAsync([FromServices] IGetHandler handler, CancellationToken cancellationToken)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return InvokeGetAsync(handler, cancellationToken);
        }

        private static async Task<ActionResult> InvokeGetAsync([FromServices] IGetHandler handler, CancellationToken cancellationToken)
        {
            return await handler.ExecuteAsync(cancellationToken);
        }
    }
}