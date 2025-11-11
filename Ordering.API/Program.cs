using Microsoft.Extensions.Configuration;
using Ordering.Aplication.Messaging;
using Ordering.Application;
using Ordering.Infraestructure.EventMessage;
using Ordering.Infrastructure;
using Ordering.Infrastructure.EventMessage;
using RabbitMQ.Client;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Application & Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfraestructureServices(builder.Configuration);

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ordering.API by Daniel CT",
        Version = "v1"
    });
});
builder.Services.AddHostedService<RabbitMQOrderConsumer>();
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new RabbitMQ.Client.ConnectionFactory()
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});
builder.Services.AddSingleton<IChannel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return connection.CreateChannelAsync().GetAwaiter().GetResult();

});
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Enable CORS
app.UseCors("AllowAll");

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API V1 by Daniel CT");
        c.RoutePrefix = string.Empty;
   });


// HTTPS Redirection: opcional si quieres probar HTTP localmente
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
