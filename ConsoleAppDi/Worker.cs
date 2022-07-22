using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Drawing;

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
            Console.WriteLine($"{nameof(Save)} - 保存文档");
            Console.WriteLine($"{nameof(SaveAs)} - 另存为文档，1参数，文档另存为完整地址（后缀：零件.sldprt，装配体.sldasm,工程图.slddrw）");
            Console.WriteLine($"{nameof(Close)} - 关闭当前激活文档");
            Console.WriteLine($"{nameof(Open)} - 打开文档，2参数，1完整地址，2文档类型（Part，Assembly，Drawing）");
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
                    size = (int)swDwgPaperSizes_e.swDwgPaperA4size;
                    break;
            }
            if (string.IsNullOrEmpty(template))
            {
                Console.WriteLine("模板是空的，无法新建文档，请到SolidWorks系统设置-默认模板，配置默认模板");
                return;
            }
            _swApp.INewDocument2(template, size, 0, 0);
        }

        /// <summary>
        /// 保存当前文档
        /// Saves the current document. 
        /// </summary>
        public void Save()
        {
            if (_swApp==null) return;
            var swModel = (ModelDoc2)_swApp.ActiveDoc;
            int error = 0;
            int warings = 0;
            bool boolStatus = swModel.Save3((int)swSaveAsOptions_e.swSaveAsOptions_Silent, ref error, ref warings);
            //如果是新建的文档，必须先保存再某个位置，建议使用SaveAs保存
            Console.WriteLine(boolStatus ? "保存成功！" : $"保存失败，错误代码{error}\n如果是新建文档，请使用SaveAs命令");
        }

        /// <summary>
        /// 将当前文档保存到指定目录
        /// Saves the active document to the specified name with advanced options. 
        /// </summary>
        /// <param name="name">完整路径Full pathname of the document to save</param>
        public void SaveAs(string name)
        {
            if (_swApp==null) return;
            var swModel = (ModelDoc2)_swApp.ActiveDoc;
            var swExtension = swModel.Extension;
            int error = 0;
            int warings = 0;
            //由于我们不是保存其他的版本，所有直接给null
            bool boolStatus = swExtension.SaveAs3(name, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, null, ref error, ref warings);
            Console.WriteLine(boolStatus ? $"{name}另存为成功！" : $"{name}另存为失败，错误代码{error}");
        }

        /// <summary>
        /// 关闭文档
        /// Closes the specified document. 
        /// </summary>
        public void Close()
        {
            if (_swApp==null) return;
            //an empty string  (""), then the active document is closed without saving
            //关闭当前激活的文档，不保存，测试一些不保存，最好是先调用一下保存
            Save();
            _swApp.CloseDoc("");
            Console.WriteLine("当前激活的文档已关闭");
        }

        /// <summary>
        /// 打开一个已存在的文档
        /// Opens an existing document and returns a pointer to the document object. 
        /// </summary>
        /// <param name="name">包括扩展名的完整路径Document name or full path if not in current directory, including extension</param>
        /// <param name="type">文件的类型，Document type as defined in swDocumentTypes_e </param>
        public void Open(string name, string type)
        {
            if (_swApp==null) return;
            //判断文档是否存在
            if (!File.Exists(name)) return;
            int tp = 0;
            switch (type)
            {
                case "Part":
                    tp = (int)swDocumentTypes_e.swDocPART;
                    break;
                case "Assembly":
                    tp = (int)swDocumentTypes_e.swDocASSEMBLY;
                    break;
                case "Drawing":
                    tp = (int)swDocumentTypes_e.swDocDRAWING;
                    break;
            }
            //If this argument is empty or the specified configuration is not present in the model, the model is opened in the last-used configuration
            //如果配置给空值，则默认打开上一次使用的配置
            int error = 0;
            int warings = 0;
            //乱数参数，居然让SolidWorks崩溃了
            var swModel = _swApp.OpenDoc6(name, tp, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref error, ref warings);
            //由于接下来我们不操作什么，所以不接受返回值也行，这里接收一下，打印一下名字
            Console.WriteLine(swModel.GetPathName());
        }




        //打工人要画图
        public void CreatePolygon()
        {
            if (_swApp==null) return;
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
