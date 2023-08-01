using System.Diagnostics;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;

namespace LearnSolidWorksApi
{
    public class SwUtility
    {
        private static ISldWorks? _swApp;
        private const string ProgId = "SldWorks.Application";
        public static ISldWorks ConnectSw()
        {
            if (_swApp != null) return _swApp;
            try
            {
                //尝试连接已打开的Solidworks
                _swApp = (SldWorks)GetActiveObject(ProgId);
            }
            catch
            {
                //如果没有事先打开SW，那么打开你上一次使用的SW
                var swType = Type.GetTypeFromProgID(ProgId);
                _swApp=(ISldWorks)Activator.CreateInstance(swType!)!;
            }
            _swApp.Visible=true;
            var swRev = Convert.ToInt32(_swApp.RevisionNumber()[..2]) - 8;
            Debug.Print($"SolidWorks 20{swRev}");
            return _swApp;
        }

        [DllImport("oleaut32.dll", PreserveSig = false)]
        private static extern void GetActiveObject(
            ref Guid rclsid,
            IntPtr pvReserved,
            [MarshalAs(UnmanagedType.IUnknown)] out Object ppunk
        );

        [DllImport("ole32.dll")]
        private static extern int CLSIDFromProgID(
            [MarshalAs(UnmanagedType.LPWStr)] string lpszProgID,
            out Guid pclsid
        );

        private static object GetActiveObject(string progId)
        {
            Guid clsid;
            CLSIDFromProgID(progId, out clsid);

            object obj;
            GetActiveObject(ref clsid, IntPtr.Zero, out obj);

            return obj;
        }
    }
}
