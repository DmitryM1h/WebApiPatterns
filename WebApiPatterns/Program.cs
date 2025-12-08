using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Channels;
using WebApiPatterns.Application;
using WebApiPatterns.Application.Dtos;
using WebApiPatterns.Exceptions;
using WebApiPatterns.Interfaces;
using WebApiPatterns.Notificators;
using WebApiPatterns.Validators;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHealthChecks();

builder.Services.AddSignalR();


builder.Services.AddScoped<INotificator, TelegramSender>();
builder.Services.AddScoped<INotificator, SmsSenderFirst>();
builder.Services.AddScoped<INotificator, SmsSenderSecond>();
builder.Services.AddScoped<INotificator, EmailSender>();
builder.Services.AddScoped<INotificator, WebApplicationSender>();
builder.Services.AddScoped<NotificationDispatcher>();
builder.Services.AddScoped<NotificationValidator>();
builder.Services.AddSingleton<CriticalEventHandler>();

builder.Services.AddSingleton(factory =>
{
    var channel = Channel.CreateUnbounded<Accident>();
    return channel;
});

builder.Services.AddScoped<JobMediator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapHub<NotificationHub>("/notifications");

app.MapControllers();


app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        int status = ex switch
        {
            FailedToValidateNotification => StatusCodes.Status400BadRequest,

            HandlerNotFoundException => StatusCodes.Status500InternalServerError,

            MultipleHandlersException => StatusCodes.Status500InternalServerError,

            _ => StatusCodes.Status500InternalServerError

        };

        ctx.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = "An error occurred",
            Type = ex.GetType().Name,
            Detail = ex.Message
        };

        await ctx.Response.WriteAsJsonAsync(problemDetails);
    }
});

app.Use(async (ctx, next) =>
{
    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();

    long start = Stopwatch.GetTimestamp();

    await next();

    long end = Stopwatch.GetTimestamp();

    var elapsed = (end - start) * 1000.0 / Stopwatch.Frequency;

    logger.LogInformation("Handler was found in {Elapsed:F3}ms", elapsed);
});

app.Run();
