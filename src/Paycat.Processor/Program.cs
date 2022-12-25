using Paycat.Infrastructure.Extensions;
using Paycat.Infrastructure.RabbitMQ;
using Paycat.Infrastructure.RabbitMQ.Extensions;
using Paycat.Processor;
using System.Reflection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddListener(listenerBuilder => listenerBuilder
            .AddRabbitMq(rabbitMqBuilder =>
            {
                var options = context.Configuration.GetSection(nameof(RabbitMqOptions))
                    .Get<RabbitMqOptions>();
                rabbitMqBuilder.Options = options;
            }, Assembly.GetExecutingAssembly()));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();