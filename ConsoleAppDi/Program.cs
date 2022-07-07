// See https://aka.ms/new-console-template for more information

using ConsoleAppDi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddSingleton<ISwService, SwService>()
            .AddTransient<Worker>())
    .Build();
DoMyWork(host.Services);
await host.RunAsync();

static void DoMyWork(IServiceProvider provider)
{
    //从IoC容器中获得打工人Worker
    Worker worker = provider.GetRequiredService<Worker>();
    //让打工人干活？
    worker.Drawing();
    Console.WriteLine("干得漂亮");
}