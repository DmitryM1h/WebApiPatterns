using Microsoft.AspNetCore.Mvc;
using WebApiPatterns.Application;
using WebApiPatterns.Jobs;
using WebApiPatterns.Jobs.Commands;

using Math = Computator.Math;
using Say = Computator.Say;

namespace WebApiPatterns.Controllers
{
    public record ApplicationUserCommand(string UserName, string CommandDescription);


    [ApiController]
    [Route("[controller]")]
    public class UserJobsController(JobMediator _jobMediator, ILogger<UserJobsController> _logger) : ControllerBase
    {

        [HttpPost("ExportData")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize]
        public ActionResult ExportDataTask([FromBody] ApplicationUserCommand userCommand)
        {

            var command = new ExportDataCommand(userCommand.UserName, userCommand.CommandDescription);

           _jobMediator.ReceiveCommand(command);

            return Accepted();
        }

        [HttpPost("CancelExport")]
        public ActionResult CancelExportData()
        {
            string initiator = "TestUser";

            ExportDataToExternalSystem.CancelTask(initiator);

            return Ok();

        }

        [HttpPost("GenerateReport")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize]
        public ActionResult GenerateReport([FromBody] ApplicationUserCommand userCommand)
        {
            
            var command = new GenerateReportCommand(userCommand.CommandDescription, userCommand.UserName);

            _jobMediator.ReceiveCommand(command);

            return Accepted();
        }


        [HttpPost("VeryHeavyComputations")]
        public ActionResult Compute([FromQuery] int a, [FromQuery] int b)
        {
            var userName = HttpContext?.User?.Identity?.Name ?? "Dmitry";

            Say.hello(userName);

            var result = Math.multiply(a, b);
            return Ok(result);
        }
    }
}
