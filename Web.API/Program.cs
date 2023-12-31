using System.Threading.RateLimiting;
using Application;
using Application.Abstractions;
using Application.Orders.Create.Saga;
using Carter;
using Infrastructure;
using Persistence;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Serilog;
using Web.API.Extensions;
using Web.API.Middleware;
using Web.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddCarter();

builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    options.AddPolicy("fixed", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10)
            }));
});

builder.Services.AddRebus(rebus => rebus
        .Routing(r =>
            r.TypeBased().MapAssemblyOf<ApplicationAssemblyReference>("eshop-queue"))
        .Transport(t =>
            t.UseRabbitMq(
                builder.Configuration.GetConnectionString("MessageBroker"),
                "eshop-queue"))
        .Sagas(s =>
            s.StoreInPostgres(
                builder.Configuration.GetConnectionString("Database"),
                "sagas",
                "saga_indexes")),
    onCreated: async bus =>
    {
        await bus.Subscribe<OrderConfirmationEmailSentEvent>();
        await bus.Subscribe<OrderPaymentRequestCreatedEvent>();
        await bus.Subscribe<OrderPaymentRequestFailedEvent>();
        await bus.Subscribe<OrderStockReservedEvent>();
        await bus.Subscribe<OrderStockReleasedEvent>();
        await bus.Subscribe<OrderCancelledEvent>();
    });

builder.Services.AutoRegisterHandlersFromAssemblyOf<ApplicationAssemblyReference>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapCarter();

app.UseSerilogRequestLogging();

app.Run();
