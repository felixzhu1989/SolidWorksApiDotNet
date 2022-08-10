using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using ConsoleAppDi.Extensions;

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
            
            Console.WriteLine($"{nameof(NewDoc)} - 新建文档，1参数，文档类型（prt，asm，drw）");
            Console.WriteLine($"{nameof(Save)} - 保存文档");
            Console.WriteLine($"{nameof(SaveAs)} - 另存为文档，1参数，文档另存为完整地址（后缀：零件.sldprt，装配体.sldasm,工程图.slddrw）");
            Console.WriteLine($"{nameof(Close)} - 关闭当前激活文档");
            Console.WriteLine($"{nameof(Open)} - 打开文档，1参数，完整地址");
            Console.WriteLine($"{nameof(Line)} - 草图-绘制直线，5参数，草图名称，x1,y1,x2,y2");
            Console.WriteLine($"{nameof(CenterLine)} - 草图-绘制中心直线，5参数，草图名称，x1,y1,x2,y2");
            Console.WriteLine($"{nameof(CornerRectangle)} - 草图-绘制边角矩形（对角点坐标），5参数，草图名称，x1,y1,x2,y2");
            Console.WriteLine($"{nameof(CenterRectangle)} - 草图-绘制中心矩形（中心点和角点），5参数，草图名称，x1,y1,x2,y2");
            Console.WriteLine($"{nameof(ThreePointCornerRectangle)} - 草图-绘制3点边角矩形（矩形的三个角点），7参数，草图名称，x1,y1,x2,y2,x3,y3");
            Console.WriteLine($"{nameof(ThreePointCenterRectangle)} - 草图-绘制3点中心矩形（中心点、边中点、边端点），7参数，草图名称，x1,y1,x2,y2,x3,y3");
            Console.WriteLine($"{nameof(Parallelogram)} - 草图-绘制平行四边形（平行四边形的三个角点），7参数，草图名称，x1,y1,x2,y2,x3,y3");

            //Console.WriteLine($"{nameof(CreatePolygon)} - 创建多边形");

        }

        /// <summary>
        /// 根据模板新建文档
        /// Creates a new document based on the specified template.
        /// </summary>
        /// <param name="type"></param>
        public void NewDoc(string type)
        {
            string template = string.Empty;
            int size = 0;
            //需要预先设置模板，否则获取的为空
            switch (type.ToLower())
            {
                case "prt":
                    template = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e
                        .swDefaultTemplatePart);
                    break;
                case "asm":
                    template = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e
                        .swDefaultTemplateAssembly);
                    break;
                case "drw":
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
            if (swModel == null)
            {
                Console.WriteLine("请打开或新建文档");
                return;
            }
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
            if (swModel == null)
            {
                Console.WriteLine("请打开或新建文档");
                return;
            }
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
        public void Open(string name)
        {
            if (_swApp==null) return;
            //判断文档是否存在
            if (!File.Exists(name)) return;
            int type = 0;
            switch (name.Substring(name.Length-3).ToLower())
            {
                case "prt":
                    type = (int)swDocumentTypes_e.swDocPART;
                    break;
                case "asm":
                    type = (int)swDocumentTypes_e.swDocASSEMBLY;
                    break;
                case "drw":
                    type = (int)swDocumentTypes_e.swDocDRAWING;
                    break;
            }
            //If this argument is empty or the specified configuration is not present in the model, the model is opened in the last-used configuration
            //如果配置给空值，则默认打开上一次使用的配置
            int error = 0;
            int warings = 0;
            //乱输入参数，居然让SolidWorks崩溃了
            var swModel = _swApp.OpenDoc6(name, type, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref error, ref warings);
            //由于接下来我们不操作什么，所以不接受返回值也行，这里接收一下，打印一下名字
            Console.WriteLine(swModel.GetPathName());
        }

        /// <summary>
        /// 绘制直线草图
        /// Creates a sketch line in the currently active 2D or 3D sketch. 
        /// </summary>
        /// <param name="sketchName">草图名称</param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void Line(string sketchName, string x1, string y1, string x2, string y2)
        {
            if (_swApp==null) return;
            var swModel = (ModelDoc2)_swApp.ActiveDoc;
            if (swModel == null)
            {
                Console.WriteLine("请打开或新建文档");
                return;
            }
            var swExtension = swModel.Extension;
            //选择草图,如果没选中，则选择前视基准面
            var boolStatus = swExtension.SelectByID2(sketchName, "SKETCH", 0, 0, 0, false, 0, null, 0);
            if (!boolStatus) swExtension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            //BV1ya4y1n7xQ
            //遍历特征，获取基准平面


            var swSketchManager = swModel.SketchManager;
            //进入草图编辑界面
            swSketchManager.InsertSketch(true);
            //关闭草图捕捉
            var myLine = swSketchManager.CreateLine(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0);
            if (myLine!=null) Console.WriteLine($"Name:{myLine.GetName()},Length:{myLine.GetLength()}");
            swModel.ViewZoomToSelection();
            //退出草图编辑
            swSketchManager.InsertSketch(true);
        }

        /// <summary>
        /// 绘制中心线
        /// Creates a center line between the specified points. 
        /// </summary>
        /// <param name="sketchName">草图名称</param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void CenterLine(string sketchName, string x1, string y1, string x2, string y2)
        {
            if (_swApp==null) return;
            //一不做二不休，把后面的全部搞成扩展方法，省的很多重复代码
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //Action需要传递一个参数swSketchManager
                var myLine = swSketchManager.CreateCenterLine(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0);
                if (myLine!=null) Console.WriteLine($"Name:{myLine.GetName()},Length:{myLine.GetLength()}");
            });

            //var swModel = (ModelDoc2)_swApp.ActiveDoc;
            //if (swModel == null)
            //{
            //    Console.WriteLine("请打开或新建文档");
            //    return;
            //}
            //var swExtension = swModel.Extension;
            ////选择草图,如果没选中，则选择前视基准面
            //var boolStatus = swExtension.SelectByID2(sketchName, "SKETCH", 0, 0, 0, false, 0, null, 0);
            //if (!boolStatus)
            //{
            //    //swExtension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            //    swModel.GetRefPlane().FirstOrDefault()?.Select2(false, 0);
            //}

            //var swSketchManager = swModel.SketchManager;
            ////进入草图编辑界面
            //swSketchManager.InsertSketch(true);
            ////关闭草图捕捉
            //_swApp.WithToggleState(swUserPreferenceToggle_e.swSketchInference, false, () =>
            //{
            //    var myLine = swSketchManager.CreateCenterLine(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0);
            //    if (myLine!=null) Console.WriteLine($"Name:{myLine.GetName()},Length:{myLine.GetLength()}");
            //});

            //swModel.ViewZoomToSelection();
            ////退出草图编辑
            //swSketchManager.InsertSketch(true);
        }

        /// <summary>
        /// 创建边角矩形（两个对角点坐标）
        /// Creates a corner rectangle. 
        /// </summary>
        /// <param name="sketchName"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void CornerRectangle(string sketchName, string x1, string y1, string x2, string y2)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //返回值是一个数组，Array of sketch segments that represent the edges created for this corner rectangle
                var vSkLines = swSketchManager.CreateCornerRectangle(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0) as object[];
                //循环该数组
                foreach (var skLine in vSkLines!)
                {
                    if (skLine is ISketchSegment skSegment) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()}");
                }
            });
        }

        /// <summary>
        /// 创建一个中心矩形
        /// Creates a center rectangle. 
        /// </summary>
        public void CenterRectangle(string sketchName, string x1, string y1, string x2, string y2)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //返回值是一个数组，Array of sketch segments that represent the edges created for this corner rectangle
                //前三个参数为中心点，后三个参数为任意一个角点
                var vSkLines = swSketchManager.CreateCenterRectangle(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0) as object[];
                //循环该数组
                foreach (var skLine in vSkLines!)
                {
                    if (skLine is ISketchSegment skSegment) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()}");
                }
            });
        }


        /// <summary>
        /// 按照任意角度创建3点边角矩形
        /// Creates a 3-point corner rectangle at any angle. 
        /// </summary>
        public void ThreePointCornerRectangle(string sketchName, string x1, string y1, string x2, string y2, string x3, string y3)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //返回值是一个数组，Array of sketch segments that represent the edges created for this corner rectangle
                //三组角点的坐标
                var vSkLines = swSketchManager.Create3PointCornerRectangle(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0, double.Parse(x3), double.Parse(y3), 0) as object[];
                //循环该数组
                foreach (var skLine in vSkLines!)
                {
                    if (skLine is ISketchSegment skSegment) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()}");
                }
            });
        }

        /// <summary>
        /// 按照任意角度创建3点中心矩形
        /// Creates a 3-point center rectangle at any angle. 
        /// </summary>
        public void ThreePointCenterRectangle(string sketchName, string x1, string y1, string x2, string y2, string x3, string y3)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //返回值是一个数组，Array of sketch segments that represent the edges created for this corner rectangle
                //前三个参数为中心点坐标，中间三个参数为任意一条边的中点，后三个参数为该边的端点坐标
                var vSkLines = swSketchManager.Create3PointCenterRectangle(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0, double.Parse(x3), double.Parse(y3), 0) as object[];
                //循环该数组
                foreach (var skLine in vSkLines!)
                {
                    if (skLine is ISketchSegment skSegment) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()}");
                }
            });
        }

        /// <summary>
        /// 创建一个平行四边形
        /// Creates a parallelogram. 
        /// </summary>
        public void Parallelogram(string sketchName, string x1, string y1, string x2, string y2, string x3, string y3)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //返回值是一个数组，Array of sketch segments that represent the edges created for this corner rectangle
                //平行四边形的三个角点
                var vSkLines = swSketchManager.CreateParallelogram(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0, double.Parse(x3), double.Parse(y3), 0) as object[];
                //循环该数组
                foreach (var skLine in vSkLines!)
                {
                    if (skLine is ISketchSegment skSegment) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()}");
                }
            });
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
