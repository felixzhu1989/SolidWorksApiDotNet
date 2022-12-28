using SolidWorks.Interop.sldworks;

namespace LearnSolidWorksApi
{
    public class SolidWorksSingleton
    {
        private static SldWorks _swApp;

        public static SldWorks GetApplication()
        {
            if (_swApp == null)
            {
                _swApp=Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as SldWorks;
                if (_swApp != null)
                {
                    _swApp.Visible = true;
                    return _swApp;
                }
            }
            return _swApp;
        }
    }
}
