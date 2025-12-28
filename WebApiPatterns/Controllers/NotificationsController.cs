//using Microsoft.AspNetCore.Mvc;
//using WebApiPatterns.Application;
//using WebApiPatterns.Application.Dtos;

//namespace WebApiPatterns.Controllers
//{

//    [ApiController]
//    [Route("[controller]")]
//    public class NotificationsController(NotificationDispatcher dispatcher, CriticalEventHandler handler) : ControllerBase
//    {

//        [HttpPost("Notification")]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status202Accepted)]
//        public async Task<ActionResult> ReceiveNotification(NotificationRequest notification)
//        {

//            var response = await dispatcher.SendAsync(notification);

//            return Accepted(response);
//        }

//        [HttpPost("CriticalEvent")]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status202Accepted)]
//        public async Task<ActionResult> ReceiveCriticalEvent(CriticalEventDto criticalEvent)
//        {
//            var criticalEventWithId = criticalEvent with { id = Guid.NewGuid()};

//            if (!Enum.IsDefined(typeof(CriticalEventType), criticalEvent.Type))
//            {
//                return BadRequest(new
//                {
//                    Error = $"Некорректный тип события: {criticalEvent.Type}"
//                });
//            }

//            handler.ProcessCriticalEvent(criticalEventWithId);
//            return Accepted(criticalEventWithId);
//        }

//    }

//}


