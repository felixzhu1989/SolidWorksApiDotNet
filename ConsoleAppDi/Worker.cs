using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

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
            //_swApp.SendMsgToUser("Hello World!");
        }
        /// <summary>
        /// 帮助
        /// </summary>
        public void Help()
        {
            Console.WriteLine($"{nameof(CreatePolygon)} - 创建多边形");
            Console.WriteLine($"{nameof(NewDocument)} - 新建文档，1参数，文档类型（Part，Assembly，Drawing）");

        }

        /// <summary>
        /// 根据模板新建文档
        /// Creates a new document based on the specified template.
        /// </summary>
        /// <param name="type"></param>
        public void NewDocument(string type)
        {
            string template = string.Empty;
            int size = 0;
            //需要预先设置模板，否则获取的为空
            switch (type)
            {
                case "Part":
                    template = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e
                        .swDefaultTemplatePart);
                    break;
                case "Assembly":
                    template = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e
                        .swDefaultTemplateAssembly);
                    break;
                case "Drawing":
                    template = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e
                        .swDefaultTemplateDrawing);
                    size = (int) swDwgPaperSizes_e.swDwgPaperA4size;
                    break;
            }
            if (string.IsNullOrEmpty(template))
            {
                Console.WriteLine("模板是空的，无法新建文档，请到SolidWorks系统设置-默认模板，配置默认模板");
                return;
            }
            _swApp.INewDocument2(template, size, 0,0);
        }


        //打工人要画图
        public void CreatePolygon()
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
