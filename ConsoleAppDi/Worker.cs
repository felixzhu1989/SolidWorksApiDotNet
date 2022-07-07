using SolidWorks.Interop.sldworks;

namespace ConsoleAppDi
{
    public class Worker
    {
        private readonly ISwService _service;
        private ISldWorks _swApp;

        public Worker(ISwService service)
        {
            _service = service;
            _swApp=_service.SwApp;
            _swApp.Visible=true;
            //_swApp.SendMsgToUser("Hello World!");
        }
        //打工人要画图
        public void Drawing()
        {
           if(_swApp==null)return;
           var swModel = (ModelDoc2)_swApp.NewPart();
           //var swModelDocExt = swModel.Extension;
           var swSketchMgr = swModel.SketchManager;
            swSketchMgr.InsertSketch(false);
           var polygon = (Object[])swSketchMgr.CreatePolygon(0,
                0, 0, 0.1, 0, 0, 5, true);
           //true表示外切，false表示内接
            swModel.ViewZoomtofit2();
            // Set the selection mode to default
            swModel.SetPickMode();
            swSketchMgr.InsertSketch(true);
        }
    }
}
