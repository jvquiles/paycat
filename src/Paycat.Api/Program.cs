using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Paycat.Api;
using Paycat.Infrastructure.Extensions;
using Paycat.Infrastructure.RabbitMQ;
using Paycat.Infrastructure.RabbitMQ.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddMessenger(messengerBuilder => messengerBuilder
    .AddRabbitMq(rabbitMqBuilder =>
    {
        var options = builder.Configuration.GetSection(nameof(RabbitMqOptions))
            .Get<RabbitMqOptions>();
        rabbitMqBuilder.Options = options;
    }));
builder.Services.AddReceiver(receiverBuilder => receiverBuilder
    .AddRabbitMq(rabbitMqBuilder =>
    {
        var options = builder.Configuration.GetSection(nameof(RabbitMqOptions))
            .Get<RabbitMqOptions>();
        rabbitMqBuilder.Options = options;
    }));
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
