using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace ConsoleAppDi
{
    public class SwService: ISwService
    {
        public ISldWorks SwApp => Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as ISldWorks;
    }
}
