// See https://aka.ms/new-console-template for more information

using ConsoleAppDi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddSingleton<ISwService, SwService>()
            .AddTransient<Worker>())
    .Build();
DoMyWork();
await host.RunAsync();

//Main函数的内部方法
void DoMyWork()
{
    //从IoC容器中获得打工人Worker
    Worker worker = host.Services.GetRequiredService<Worker>();
    //新需求，在控制台输入命令，让SolidWorks执行特定的操作
    //命令对应了Worker类中的方法，通过反射得到Worker中的方法，然后执行该方法
    //while死循环，独占控制台，让用户输入
    while (true)
    {
        //用户输入,提示，用Help提示用户有那些命令，并注意方法的大小写
        Console.WriteLine("\n请输入命令(注意大小写，Help查询)：\n");
        //如果方法带参数，我们使用空格分开
        var command= Console.ReadLine();
        //如果输入为空，则退出程序
        if (string.IsNullOrWhiteSpace(command)) Environment.Exit(0);
        var commands = command.Split(' ');
        //第一个是方法，第二个才是参数的第一个,参数比输入的少一个
        var parameters = new object?[commands.Length-1];
        //循环commands，将参数添加到parameters中
        for (int i = 0; i < commands.Length-1; i++)
        {
            parameters[i]=commands[i+1];
        }
        var method = typeof(Worker).GetMethod(commands[0])!;
        try
        {
            //类的实例，执行方法，如果commands长度为1则没有参数，传递null
            method.Invoke(worker, commands.Length==1? null:parameters);
        }
        catch (Exception e)
        {
            Console.WriteLine($"非法命令{command},详细：{e}");
            worker.Help();
        }
    }
}