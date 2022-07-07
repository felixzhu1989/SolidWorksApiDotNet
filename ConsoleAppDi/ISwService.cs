using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace ConsoleAppDi
{
    public interface ISwService
    {
        ISldWorks SwApp { get;}
    }
}
