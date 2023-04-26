using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await Host.CreateDefaultBuilder()
    .UseOrleans(builder =>
    {
        builder.UseLocalhostClustering();
        builder.AddMemoryGrainStorageAsDefault();
    })
    .ConfigureLogging(builder => builder.AddConsole())
    .RunConsoleAsync();