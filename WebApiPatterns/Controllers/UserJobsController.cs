using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApiPatterns.Application;
using WebApiPatterns.Jobs;
using WebApiPatterns.Jobs.Commands;

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
        public async Task<ActionResult> ExportDataTask([FromBody] ApplicationUserCommand userCommand)
        {

            var command = new ExportDataCommand(userCommand.UserName, userCommand.CommandDescription);

            await _jobMediator.ReceiveCommand(command);

            return Accepted();
        }

        [HttpPost("CancelExport")]
        public async Task<ActionResult> CancelExportData()
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
        public async Task<ActionResult> GenerateReport([FromBody] ApplicationUserCommand userCommand)
        {
            
            var command = new GenerateReportCommand(userCommand.CommandDescription, userCommand.UserName);

            await _jobMediator.ReceiveCommand(command);

            return Accepted();
        }

    }
}
