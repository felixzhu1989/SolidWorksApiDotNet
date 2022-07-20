using SolidWorks.Interop.sldworks;

namespace ConsoleAppDi
{
    public class SwService: ISwService
    {
        public ISldWorks SwApp => (Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as ISldWorks)!;
        public SwService()
        {
            SwApp.Visible=true;
            Console.WriteLine("SolidWorks准备就绪...");
        }
    }
}
