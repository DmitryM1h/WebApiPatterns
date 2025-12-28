using CriticalEvents.Application;
using CriticalEvents.Domain.Services.Requests;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CriticalEvents.EndPoints;

public static class CriticalEventsAPI
{
    public static void AddCriticalEventsEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("CriticalEvent", async ([FromBody] CriticalEventRequest criticalEvent, [FromServices] CriticalEventHandler eventsHandler) =>
        {
            eventsHandler.Handle(criticalEvent);

            return Results.Accepted();
        })
        .Produces(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Critical Events")
        .WithName("CreateCriticalEvent"); ;
    }
}
