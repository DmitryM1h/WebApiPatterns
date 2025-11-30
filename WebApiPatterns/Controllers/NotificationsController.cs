using Microsoft.AspNetCore.Mvc;
using WebApiPatterns.Application;
using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class NotificationsController(NotificationDispatcher dispatcher) : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> ReceiveNotification(NotificationRequest notification)
        {

            var response = await dispatcher.SendAsync(notification);

            return Accepted(response);
        }

    }

}


