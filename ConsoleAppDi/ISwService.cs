using SolidWorks.Interop.sldworks;

namespace ConsoleAppDi
{
    public interface ISwService
    {
        ISldWorks SwApp { get;}
    }
}
