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
            Console.WriteLine($"{nameof(Circle)} - 草图-绘制圆（圆心和圆周上一点），5参数，草图名称，x1,y1,x2,y2");
            Console.WriteLine($"{nameof(CircleByRadius)} - 草图-绘制半径圆（圆心和半径），4参数，草图名称，x1,y1,r");
            Console.WriteLine($"{nameof(PerimeterCircle)} - 草图-绘制周边圆（周长上任意三点），7参数，草图名称，x1,y1,x2,y2,x3,y3");
            Console.WriteLine($"{nameof(Arc)} - 草图-绘制圆弧（中心点、起点、终点、方向），8参数，草图名称，x1,y1,x2,y2,x3,y3,d(1逆时针，-1顺时针)");
            Console.WriteLine($"{nameof(TangentArc)} - 草图-绘制切线弧（起点、终点、切线弧类型），6参数，草图名称，x1,y1,x2,y2,type(1前，3后，2左，4右)");
            Console.WriteLine($"{nameof(ThreePointArc)} - 草图-绘制3点圆弧（起点、终点、圆弧上任意一点），7参数，草图名称，x1,y1,x2,y2,x3,y3");
            Console.WriteLine($"{nameof(Polygon)} - 草图-绘制多边形（中心点、顶点、显示构造圆类型），7参数，草图名称，x1,y1,x2,y2,sides,inscribed(true内切构造圆，false外接构造圆)");
            Console.WriteLine($"{nameof(Slot)} - 草图-绘制键槽（槽类型,尺寸标志类型，宽度，三个点坐标，中心弧方向，是否标注尺寸），\n12参数，草图名称，slotType(0直槽，1中心点直槽，2中心点弧槽，3为3点弧槽)，lengthType(0为弧中心点距离，1全长)，width,\nx1,y1,x2,y2,x3,y3,direction(1逆时针，-1顺时针),addDim(true标注，false不标注)");
            Console.WriteLine($"{nameof(Point)} - 草图-绘制点（三个点坐标）3参数，草图名称，x,y");
            Console.WriteLine($"{nameof(Spline)} - 草图-绘制样条曲线（点的个数）2参数，草图名称，n");
            //EquationSpline Sketch1 x^2 -5 5
            Console.WriteLine($"{nameof(EquationSpline)} - 草图-绘制方程式驱动曲线，4参数，草图名称，y方程式，起始区间，终止区间");
            Console.WriteLine($"{nameof(Ellipse)} - 草图-绘制完整椭圆，7参数，草图名称，中心点x，中心点y，长轴点x，长轴点y，短轴点x，短轴点y");
            Console.WriteLine($"{nameof(EllipticalArc)} - 草图-绘制部分椭圆，12参数，草图名称，中心点x，中心点y，长轴点x，长轴点y，短轴点x，短轴点y，起点x，起点y，终点x，终点y，方向（1逆时针，-1顺时针）");
            Console.WriteLine($"{nameof(Parabola)} - 草图-绘制抛物线，7参数，草图名称，焦点x，焦点y，顶点x，顶y，起点x，终点x");
            Console.WriteLine($"{nameof(Conic)} - 草图-绘制抛物线，7参数，草图名称，焦点x，焦点y，顶点x，顶y，起点x，终点x");


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

        /// <summary>
        /// 根据圆心和圆周上一点绘制圆
        /// Creates a circle based on a center point and a point on the circle. 
        /// </summary>
        public void Circle(string sketchName, string x1, string y1, string x2, string y2)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //前三个参数为圆心点坐标，后三个参数为圆周上任意一点的坐标
                var skSegment = swSketchManager.CreateCircle(double.Parse(x1), double.Parse(y1),0, double.Parse(x2), double.Parse(y2),0);

                if (skSegment!=null) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()},Type:{typeof(swSketchSegments_e).GetEnumName(skSegment.GetType())}");
            });
        }

        /// <summary>
        /// 根据圆心和半径绘制圆
        /// Creates a circle based on a center point and a specified radius.  
        /// </summary>
        public void CircleByRadius(string sketchName, string x1, string y1, string radius)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //前三个参数为圆心点坐标，最后一个为半径值
                var skSegment = swSketchManager.CreateCircleByRadius(double.Parse(x1), double.Parse(y1), 0, double.Parse(radius));

                if (skSegment!=null) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()},Type:{typeof(swSketchSegments_e).GetEnumName(skSegment.GetType())}");
            });
        }

        /// <summary>
        /// 根据圆周上任意3点坐标绘制周边圆
        /// Draws a 3-point perimeter arc.  
        /// </summary>
        public void PerimeterCircle(string sketchName, string x1, string y1, string x2, string y2, string x3, string y3)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //圆周上任意3点坐标
                var skArc = swSketchManager.PerimeterCircle(double.Parse(x1), double.Parse(y1), double.Parse(x2), double.Parse(y2), double.Parse(x3), double.Parse(y3)) as ISketchArc;
                if (skArc!=null) Console.WriteLine($"Radius:{skArc.GetRadius()}");
            });
        }


        /// <summary>
        /// 根据中心点、起点、终点、方向创建圆弧
        /// Creates an arc based on a center point, a start point, an end point, and a direction. 
        /// </summary>
        public void Arc(string sketchName, string x1, string y1, string x2, string y2, string x3, string y3,string d)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //中心点、起点、终点、方向（1逆时针，-1顺时针）
                var skArc = swSketchManager.CreateArc(double.Parse(x1), double.Parse(y1), 0,double.Parse(x2), double.Parse(y2),0, double.Parse(x3), double.Parse(y3),0,short.Parse(d)) as ISketchArc;
                if (skArc!=null) Console.WriteLine($"Radius:{skArc.GetRadius()}");
            });
        }

        /// <summary>
        /// 创建切线弧
        /// Creates a tangent arc.  
        /// </summary>
        public void TangentArc(string sketchName, string x1, string y1, string x2, string y2,  string arcType)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //起点、终点、切线弧类型（1前，3后，2左，4右）
                var skArc = swSketchManager.CreateTangentArc(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0,  int.Parse(arcType)) as ISketchArc;
                if (skArc!=null) Console.WriteLine($"Radius:{skArc.GetRadius()}");
            });
        }


        /// <summary>
        /// 起点、终点、圆弧上任意一点绘制3点圆弧
        /// Creates a 3-point arc. 
        /// </summary>
        public void ThreePointArc(string sketchName, string x1, string y1, string x2, string y2, string x3, string y3)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //起点、终点、圆弧上任意一点
                var skArc = swSketchManager.Create3PointArc(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0, double.Parse(x3), double.Parse(y3), 0) as ISketchArc;
                if (skArc!=null) Console.WriteLine($"Radius:{skArc.GetRadius()}");
            });
        }


        /// <summary>
        /// 创建一个多边形
        /// Creates a polygon in the active sketch. 
        /// </summary>
        public void Polygon(string sketchName, string x1, string y1, string x2, string y2, string sides, string inscribed)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //返回值是一个数组，Array of sketch segments that represent the edges created for this corner rectangle
                //中心点，顶点，边数，显示构造圆类型（true内切构造圆，false外接构造圆）
                var vSkLines = swSketchManager.CreatePolygon(double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0, int.Parse(sides), bool.Parse(inscribed)) as object[];
                //循环该数组
                foreach (var skLine in vSkLines!)
                {
                    if (skLine is ISketchSegment skSegment) Console.WriteLine($"Name:{skSegment.GetName()},Length:{skSegment.GetLength()}");
                }
            });
        }

        /// <summary>
        /// 创建键槽草图
        /// Creates a sketch slot. 
        /// </summary>
        public void Slot(string sketchName,string slotType,string lengthType, string width,string x1, string y1, string x2, string y2, string x3, string y3,string d,string addDim)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //第一个参数为槽类型，0直槽，1中心点直槽，2中心点弧槽，3为3点弧槽
                //第二个参数为标注长度类型，0为弧中心点距离，1全长
                //第三个参数为宽度
                //三组点的坐标
                //中心弧线的方向
                //是否添加尺寸标注，true标注，false不标注
                var skSlot = swSketchManager.CreateSketchSlot(int.Parse(slotType),int.Parse(lengthType),double.Parse(width),double.Parse(x1), double.Parse(y1), 0, double.Parse(x2), double.Parse(y2), 0, double.Parse(x3), double.Parse(y3), 0,int.Parse(d),bool.Parse(addDim));
                if (skSlot != null)
                {
                    var centerPoint = skSlot.GetCenterPointHandle();
                    Console.WriteLine($"CenterPoint:({centerPoint.X},{centerPoint.Y},{centerPoint.Z})");
                }
            });
        }


        /// <summary>
        /// 创建点
        /// Creates a sketch point in the active 2D or 3D sketch. 
        /// </summary>
        public void Point(string sketchName,  string x, string y)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                var skPoint = swSketchManager.CreatePoint( double.Parse(x), double.Parse(y), 0 );
                if (skPoint != null)
                {
                    Console.WriteLine($"CenterPoint:({skPoint.X},{skPoint.Y},{skPoint.Z})");
                }
            });
        }

        /// <summary>
        /// 创建样条曲线,n为需要多少个点
        /// Creates either a 2D spline or a spline constrained to a surface. 
        /// </summary>
        public void Spline(string sketchName, string n)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //PointData ，第一个参数[start_pt_x, start_pt_y, start_pt_z, end_pt_x, end_pt_y, end_pt_z]
                double[] points = new double[int.Parse(n) * 3];//因为一个点有3个值xyz
                //循环，使用随机数构造一个点
                for (int i = 0; i < int.Parse(n); i++)
                {
                    var x = i;
                    var y =3* Random.Shared.NextDouble();//NextDouble结果为0.0-1.0，我们让他振幅大一点，看结果可调整
                    points[i*3] = x;
                    points[i*3+1] = y;
                    points[i*3+2] = 0;//Z取0，因为我们画2D草图
                }
                //和曲面相关的两个参数给null
                //True to simulate natural ends, false to not; valid only for 2D splines ,末端曲率相关
                //最后一个参数Status， empty for 2D splines ，2D时是空的，因此用下划线忽略他
                //返回值需要显式转换为ISketchSegment类型
                var skSpline = swSketchManager.CreateSpline3(points,null,null,true,out _) as ISketchSegment;
                if (skSpline != null)
                {
                    //获取它的类型swSketchSegments_e
                    Console.WriteLine($"SegmentType:{typeof(swSketchSegments_e).GetEnumName(skSpline.GetType())}");
                }
            });
        }


        /// <summary>
        /// 创建方程式驱动曲线
        /// Creates an equation-driven 2D explicit or parametric curve or a 3D parametric curve. 
        /// </summary>
        public void EquationSpline(string sketchName, string yEquation,string start,string end)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //2D explicit curve:
                //A parabola    y = x^2
                //CreateEquationSpline2("", "x^2", "", "-5", "5", False, 0, 0, 0, True, True)
                //输入命令：EquationSpline Sketch1 x^2 -5 5
                var skSpline =  swSketchManager.CreateEquationSpline2("", yEquation, "", start, end, false, 0, 0, 0, true, true);
                if (skSpline != null)
                {
                    skSpline.GetEquationParameters2(out _, out string yExpression, out _, out double rangeStart, out double rangeEnd, out _, out _, out _, out _, out _, out _);
                    Console.WriteLine($"Expression:{yExpression}\nStart:{rangeStart}\nEnd:{rangeEnd}");
                }
            });
        }
        
        /// <summary>
        /// 使用指定的中心点、长轴点和短轴点创建椭圆。
        /// Creates an ellipse using the specified center, major-axis, and minor-axis points. 
        /// </summary>
        public void Ellipse(string sketchName, string xCenter, string yCenter, string xMajor, string yMajor, string xMinor, string yMinor)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //中心点，长轴点，短轴点
                //swModel.SketchManager.CreateEllipse(CtrPt.X, CtrPt.Y, 0, MajPt.X, MajPt.Y, 0, CtrPt.X + pointvar[0], CtrPt.Y + pointvar[1], 0);
               var swSegment= swSketchManager.CreateEllipse(double.Parse(xCenter), double.Parse(yCenter), 0, double.Parse(xMajor), double.Parse(yMajor), 0, double.Parse(xMinor), double.Parse(yMinor), 0);
               if (swSegment != null)
               {
                   Console.WriteLine($"Type:{typeof(swSketchSegmentType_e).GetEnumName(swSegment.GetType())}");
               }
            });
        }

        /// <summary>
        /// 在给定一个中心点的情况下创建一个偏椭圆，两个点指定长轴和短轴，两个点定义椭圆起点和终点。
        /// Creates a partial ellipse given a center point, two points that specify the major and minor axis, and two points that define the elliptical start and end points.  
        /// </summary>
        public void EllipticalArc(string sketchName, string xCenter, string yCenter, string xMajor, string yMajor, string xMinor, string yMinor,string xStart,string yStart,string xEnd,string yEnd,string direction)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //中心点，长轴点，短轴点，起点坐标，终点坐标，方向（1逆时针，-1顺时针）
                var swSegment = swSketchManager.CreateEllipticalArc(double.Parse(xCenter), double.Parse(yCenter), 0, double.Parse(xMajor), double.Parse(yMajor), 0, double.Parse(xMinor), double.Parse(yMinor), 0,double.Parse(xStart),double.Parse(yStart),0,double.Parse(xEnd),double.Parse(yEnd),0,short.Parse(direction));
                if (swSegment != null)
                {
                    Console.WriteLine($"Type:{typeof(swSketchSegmentType_e).GetEnumName(swSegment.GetType())}");
                }
            });
        }

        /// <summary>
        /// 创建抛物线
        /// Creates a parabola in the active sketch.  
        /// </summary>
        public void Parabola(string sketchName, string xFocus, string yFocus, string xApex, string yApex, string xStart, string xEnd)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //焦点坐标，顶点坐标，起点坐标，终点坐标
                //x^2=2py,y=x^2/(2*p)
                //焦点坐标(0,p/2),(xFocus,yFocus) p=2*yFocus
                double p = 2 * double.Parse(yFocus);
                //计算得出起点和终点的坐标
                double yStart = Math.Pow(double.Parse(xStart), 2) / (2 * p);
                double yEnd = Math.Pow(double.Parse(xEnd), 2) / (2 * p);

                var swSegment = swSketchManager.CreateParabola(double.Parse(xFocus), double.Parse(yFocus), 0, double.Parse(xApex), double.Parse(yApex), 0, double.Parse(xStart), yStart, 0, double.Parse(xEnd), yEnd, 0);
                if (swSegment != null)
                {
                    Console.WriteLine($"Type:{typeof(swSketchSegmentType_e).GetEnumName(swSegment.GetType())}");
                }
            });
        }


        /// <summary>
        /// 创建圆锥曲线
        /// Creates a conic curve in the active sketch. 
        /// </summary>
        public void Conic(string sketchName, string xFocus, string yFocus, string xApex, string yApex, string xStart, string xEnd)
        {
            if (_swApp==null) return;
            _swApp.EditSketch(sketchName, (swSketchManager) =>
            {
                //抛物线是圆锥曲线之一，因此使用抛物线方程的参数也可以
                //焦点坐标，顶点坐标，起点坐标，终点坐标
                //x^2=2py,y=x^2/(2*p)
                //焦点坐标(0,p/2),(xFocus,yFocus) p=2*yFocus
                double p = 2 * double.Parse(yFocus);
                //计算得出起点和终点的坐标
                double yStart = Math.Pow(double.Parse(xStart), 2) / (2 * p);
                double yEnd = Math.Pow(double.Parse(xEnd), 2) / (2 * p);

                var swSegment = swSketchManager.CreateConic(double.Parse(xFocus), double.Parse(yFocus), 0, double.Parse(xApex), double.Parse(yApex), 0, double.Parse(xStart), yStart, 0, double.Parse(xEnd), yEnd, 0);
                if (swSegment != null)
                {
                    Console.WriteLine($"Type:{typeof(swSketchSegmentType_e).GetEnumName(swSegment.GetType())}");
                }
            });
        }
    }
}
