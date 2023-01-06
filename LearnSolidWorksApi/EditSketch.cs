using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace LearnSolidWorksApi
{
    public class EditSketch
    {
        public ModelDoc2 SwModel { get; set; }
        public ModelDocExtension SwModelDocExt { get; set; }
        public SketchManager SwSketchMgr { get; set; }
        private readonly SldWorks _swApp;
        public EditSketch(SldWorks swApp)
        {
            _swApp = swApp;
        }
        /// <summary>
        /// 新建零件
        /// </summary>
        public void NewDocument()
        {
            //从用户设置中获取零件模板设置
            var defaultTemplate =
                _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            //使用默认模板新建零件
            SwModel=(_swApp.NewDocument(defaultTemplate, 0, 0, 0) as ModelDoc2)!;
            SwModelDocExt = SwModel.Extension;
            SwSketchMgr = SwModel.SketchManager;
        }

        /// <summary>
        /// 创建草图圆角
        /// </summary>
        public void CreateFillet()
        {
            //新建零件
            NewDocument();
            //选择前视基准平面，插入草图
            SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.InsertSketch(true);
            //绘制矩形
            SwSketchMgr.CreateCornerRectangle(0, 1, 0, 1, 0, 0);
            SwModel.ClearSelection2(true);
            //选择角点
            SwModelDocExt.SelectByID2("Point1", "SKETCHPOINT", 0, 1, 0, false, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);
            //创建草图圆角
            SwSketchMgr.CreateFillet(0.1, (int)swConstrainedCornerAction_e.swConstrainedCornerDeleteGeometry);
            //调整视图
            SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
            SwModel.ViewZoomtofit2();
            //退出草图编辑
            SwSketchMgr.InsertSketch(true);
        }

        /// <summary>
        /// 创建草图倒角
        /// </summary>
        public void CreateChamfer()
        {
            //新建零件
            NewDocument();
            //选择前视基准平面，插入草图
            SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.InsertSketch(true);
            //绘制矩形
            SwSketchMgr.CreateCornerRectangle(0, 1, 0, 1, 0, 0);
            SwModel.ClearSelection2(true);
            //选择两条边
            //第一条边
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", -0.9, 1, 0, false, 1, null, (int)swSelectOption_e.swSelectOptionDefault);
            //第二条边，加选
            SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", -1, 0.9, 0, true, 1, null, (int)swSelectOption_e.swSelectOptionDefault);

            //创建草图倒角
            //距离角度,角度换算成弧度,注意D1标注在后选的边上
            //SwSketchManager.CreateChamfer((int)swSketchChamferType_e.swSketchChamfer_DistanceAngle,0.1,30*Math.PI/180);

            //距离距离,注意D1标注在后选的边上,D2标在先选的边上
            //SwSketchManager.CreateChamfer((int)swSketchChamferType_e.swSketchChamfer_DistanceDistance, 0.1, 0.2);

            //等距
            SwSketchMgr.CreateChamfer((int)swSketchChamferType_e.swSketchChamfer_DistanceEqual, 0.1, 0.1);

            //调整视图
            SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
            SwModel.ViewZoomtofit2();
            //退出草图编辑
            SwSketchMgr.InsertSketch(true);
        }

        public void SketchOnFrontPlane()
        {
            //选择前视基准平面，插入草图
            SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.InsertSketch(true);
        }

        public void ViewZoomAndExitSketch()
        {
            //调整视图
            SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
            SwModel.ViewZoomtofit2();
            //退出草图编辑
            SwSketchMgr.InsertSketch(true);
        }
        /// <summary>
        /// 剪裁草图实体
        /// </summary>
        public void SketchTrim()
        {
            //新建零件
            NewDocument();
            SketchOnFrontPlane();
            //绘制两条直线
            SwSketchMgr.CreateLine(-2, 0, 0, 2, 0, 0);
            SwSketchMgr.CreateLine(0, -2, 0, 0, 2, 0);
            SwModel.ClearSelection2(true);

            //选择两条直线
            //第一条直线
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 1, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
            //第二条直线，加选
            SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", 0, 1, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
            //剪裁草图实体，以边角的形式
            var boolStatus = SwSketchMgr.SketchTrim((int)swSketchTrimChoice_e.swSketchTrimCorner, 0, 0, 0);
            Console.WriteLine(boolStatus);

            ViewZoomAndExitSketch();
        }
        /// <summary>
        /// 延伸草图实体
        /// </summary>
        public void SketchExtend()
        {
            NewDocument();
            SketchOnFrontPlane();
            //绘制两条直线，将要相交，但没产生交点
            SwSketchMgr.CreateLine(-0.5, 1, 0, 0, 0, 0);
            SwSketchMgr.CreateLine(-1, -0.5, 0, 1, -0.5, 0);
            SwModel.ClearSelection2(true);

            //选择需要延伸的草图线
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);

            //延伸草图实体
            var boolStatus = SwSketchMgr.SketchExtend(0, 0, 0);
            Console.WriteLine(boolStatus);

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 偏移草图实体
        /// </summary>
        public void SketchOffset()
        {
            NewDocument();
            SketchOnFrontPlane();
            
            //原始草图
            SwSketchMgr.CreateLine(-0.5, 0.75, 0, -0.25, -0.5, 0);
            SwSketchMgr.CreateLine(-0.75, -1.25, 0, 0.5, -1.25, 0);
            //与第一条线构成链，但是不选择它
            SwSketchMgr.CreateLine(-0.5, 0.75, 0, -0.5, 1.5, 0);
            SwModel.ClearSelection2(true);

            //选择需要偏移的草图实体
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //加选
            SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);

            //偏移草图实体，默认向左和下偏移
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, false, false, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, false);

            //反向偏移草图实体，标注偏移距离，反向，向右和上偏移
            //var boolStatus = SwSketchMgr.SketchOffset2(-0.5, false, false, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            //双侧偏移
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            //链选项
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, true, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            //圆弧封端
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            //直线封端
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetLineCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            //将原始草图设置为构造线
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetMakeOrigConstruction, true);

            //将偏移后的草图设置为构造线
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetMakeOffsConstruction, true);

            //将两者都设置为构造线
            var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetMakeBothConstruction, true);

            Console.WriteLine(boolStatus);
            ViewZoomAndExitSketch();
        }
        /// <summary>
        /// 镜像草图实体
        /// </summary>
        public void SketchMirror()
        {
            NewDocument();
            SketchOnFrontPlane();

            //中心线
            SwSketchMgr.CreateCenterLine(0, 0, 0, 0, 1, 0);
            //圆，作为被镜像的草图实体
            SwSketchMgr.CreateCircleByRadius(-0.5, 0.5, 0, 0.4);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            //选择中心线，标记为2
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 2, null, 0);
            //选择被镜像的草图，标记为1
            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, true, 1, null, 0);

            //镜像草图
            SwModel.SketchMirror();

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 创建草图阵列
        /// </summary>
        public void CreateLinearSketchPattern()
        {
            NewDocument();
            SketchOnFrontPlane();

            //绘制被阵列的草图，也叫种子
            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();
            //选择草图图元
            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //创建线性草图阵列
           var boolStatus= SwSketchMgr.CreateLinearSketchStepAndRepeat(3, 1, 1, 0, 0, 0, "", true, false, false, true, false);

           Console.WriteLine($"创建阵列：{boolStatus}");
            ViewZoomAndExitSketch();
        }
    }
}
