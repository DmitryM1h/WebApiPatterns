using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApiPatterns.Application;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserJobsController(JobMediator _jobMediator, ILogger<UserJobsController> _logger) : ControllerBase
    {

        [HttpPost("ExportData")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize]
        public async Task<ActionResult> ExportDataToExternalSystem([FromBody] string description)
        {
            long start = Stopwatch.GetTimestamp();

            var command = new ExportDataCommand(description);

            await _jobMediator.ReceiveCommand(command);

            long end = Stopwatch.GetTimestamp();

            var elapsed = (end - start) * 1000.0 / Stopwatch.Frequency;

            _logger.LogInformation("Handler was found in {Elapsed:F3}ms", elapsed);

            return Accepted();
        }


        [HttpPost("GenerateReport")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize]
        public async Task<ActionResult> GenerateReport([FromBody] string description)
        {
            long start = Stopwatch.GetTimestamp();
            
            var command = new GenerateReportCommand();

            await _jobMediator.ReceiveCommand(command);

            long end = Stopwatch.GetTimestamp();

            var elapsed = (end - start) * 1000.0 / Stopwatch.Frequency;

            _logger.LogInformation("Handler was found in {Elapsed:F3}ms", elapsed);
            return Accepted();
        }

    }
}
