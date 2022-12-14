using MediatR;
using Paycat.Api;
using Paycat.Infrastructure.Extensions;
using Paycat.Infrastructure.RabbitMQ;
using Paycat.Infrastructure.RabbitMQ.Extensions;
using Paycat.Infrastructure.Redis;
using Paycat.Infrastructure.Redis.Extensions;
using System.Reflection;

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
        var options = builder.Configuration.GetSection(nameof(RabbitMqMessengerOptions))
            .Get<RabbitMqMessengerOptions>();
        options.HostName = Environment.GetEnvironmentVariable("HOSTNAME");
        rabbitMqBuilder.Options = options;
    }));
builder.Services.AddReceiver(receiverBuilder => receiverBuilder
    .AddRabbitMq(rabbitMqBuilder =>
    {
        var options = builder.Configuration.GetSection(nameof(RabbitMqReceiverOptions))
            .Get<RabbitMqReceiverOptions>();
        options.HostName = Environment.GetEnvironmentVariable("HOSTNAME");
        rabbitMqBuilder.Options = options;
    }));
builder.Services.AddHostedService<Worker>();
builder.Services.AddRepository(repositoryBuilder => repositoryBuilder
    .AddRedisRepository(redisRepositoryBuilder =>
    {
        var options = builder.Configuration.GetSection(nameof(RedisRepositoryOptions))
            .Get<RedisRepositoryOptions>();
        redisRepositoryBuilder.Options = options;
    }));

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
