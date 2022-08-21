// See https://aka.ms/new-console-template for more information
using Common.Models;
using Microsoft.Extensions.Configuration;
using MinecraftServerManager.Bootstrap.Services;
using Topshelf;

// Build a config object, using env vars and JSON providers.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Get values from the config given their key and their target type.
ServerConfig settings = config.GetRequiredSection("ServerConfig").Get<ServerConfig>();

HostFactory.Run(hostConfig =>
{
    var config = hostConfig.Service<ShellService>(serviceConfig =>
    {
        serviceConfig.ConstructUsing(() => new ShellService(settings));
        serviceConfig.WhenStarted(async s => await s.Start());
        serviceConfig.WhenStopped(async s => await s.Stop());
    });

    config.SetDisplayName("MinecraftServerManager.Bootstrap");
    config.SetInstanceName("MinecraftServerManager.Bootstrap");
    config.SetServiceName("MinecraftServerManager.Bootstrap");
});
