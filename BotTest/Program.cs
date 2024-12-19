using BotTest.Main;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        string botToken = context.Configuration["DiscordConnectionStrings:BotToken"] ?? string.Empty;
        services.AddSingleton(new Main(botToken));
    })
    .Build();

var main = host.Services.GetRequiredService<Main>();
await main.RunBotAsync();
