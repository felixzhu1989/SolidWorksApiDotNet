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

        /// <summary>
        /// 在前视基准平面上绘制草图
        /// </summary>
        public void SketchOnFrontPlane()
        {
            //选择前视基准平面，插入草图
            SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.InsertSketch(true);
        }

        /// <summary>
        /// 调整视图并退出草图编辑
        /// </summary>
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
        /// 创建线性草图阵列
        /// </summary>
        public void CreateLinearSketchPattern()
        {
            NewDocument();
            SketchOnFrontPlane();

            //绘制被阵列的草图，也叫种子
            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            SwSketchMgr.CreateLine(-0.2, 0, 0, 0.2, 0, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();
            //选择草图图元
            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            //创建线性草图阵列
            var boolStatus = SwSketchMgr.CreateLinearSketchStepAndRepeat(3, 1, 1, 0, 0, 0, "", true, false, false, true, false);

            Console.WriteLine($"创建线性阵列：{boolStatus}");
            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 编辑线性草图阵列
        /// </summary>
        public void EditLinearSketchPattern()
        {
            CreateLinearSketchPattern();
            //选择草图中种子
            SwModelDocExt.SelectByID2("Arc1@Sketch1", "EXTSKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //编辑草图
            SwModel.EditSketch();
            //编辑线性草图阵列,种子图元名阵列必须带下划线
            //var boolStatus= SwSketchMgr.EditLinearSketchStepAndRepeat(5, 1, 1, 0, 0, 0, "", true, false, false, true, false, "Arc1_");

            //增加y轴阵列，同时显示距离标注和数量标注
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 0, 0, "", true, true, false, true, true, "Arc1_");
            //？,显示两个阵列方向的夹角，
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 0, 60*Math.PI/180, "", true, true, true, true, true, "Arc1_");
            //x方向夹角，两个阵列方向都是相对与x轴的夹角
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 10*Math.PI/180, 90*Math.PI/180, "", true, true, true, true, true, "Arc1_");

            //删除实例,从左往右第3列，从下网上第2行
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 10*Math.PI/180, 90*Math.PI/180, "(3,2)", true, true, true, true, true, "Arc1_");

            //删除多个实例
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 10*Math.PI/180, 90*Math.PI/180, "(3,2)(5,4)", true, true, true, true, true, "Arc1_");

            //多个种子图元,每个图元后都要带下划线
            var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 10*Math.PI/180, 90*Math.PI/180, "(3,2)(5,4)", true, true, true, true, true, "Arc1_Line1_");

            Console.WriteLine($"编辑线性草图阵列：{boolStatus}");
            //退出草图编辑
            SwSketchMgr.InsertSketch(true);
        }

        /// <summary>
        /// 创建圆周草图阵列
        /// </summary>
        public void CreateCircularSketchPattern()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateCircleByRadius(-1, 0, 0, 0.1);
            SwSketchMgr.CreateLine(-1, -0.1, 0, -1, 0.1, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);

            var boolStatus = SwSketchMgr.CreateCircularSketchStepAndRepeat(0.5, 0, 3, 30 * Math.PI / 180, false, "", true, true, true);

            Console.WriteLine($"创建圆周草图阵列：{boolStatus}");
            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 编辑圆周草图阵列
        /// </summary>
        public void EditCircularSketchPattern()
        {
            CreateCircularSketchPattern();

            SwModelDocExt.SelectByID2("Arc1@Sketch1", "EXTSKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModel.EditSketch();

            //更改圆周阵列的半径
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 0, 3, 30 * Math.PI / 180, false, "", true, true, true,"Arc1_Line1_");

            //更改起始阵列的旋转角度
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 10 * Math.PI / 180, 3, 30 * Math.PI / 180, false, "", true, true, true, "Arc1_Line1_");

            //更改阵列数量为5
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 10 * Math.PI / 180, 5, 30 * Math.PI / 180, false, "", true, true, true, "Arc1_Line1_");

            //更改阵列间距为45度
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 10 * Math.PI / 180, 5, 45 * Math.PI / 180, false, "", true, true, true, "Arc1_Line1_");

            //更改旋转草图，并没看见什么变化
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 10 * Math.PI / 180, 5, 45 * Math.PI / 180, true, "", true, true, true, "Arc1_Line1_");

            //删除第三个阵列实例
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 10 * Math.PI / 180, 5, 45 * Math.PI / 180, true, "(3)", true, true, true, "Arc1_Line1_");

            //不显示半径，角度和数量标注
            var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 10 * Math.PI / 180, 5, 45 * Math.PI / 180, true, "(3)", false, false, false, "Arc1_Line1_");

            Console.WriteLine($"编辑圆周草图阵列：{boolStatus}");
            SwSketchMgr.InsertSketch(true);
        }
        
        /// <summary>
        /// 移动或复制草图实体
        /// </summary>
        public void MoveOrCopy()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            SwSketchMgr.CreateLine(0, -0.2, 0, 0, 0.2, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            //移动草图实体
            //SwModelDocExt.MoveOrCopy(false,1,false,0,0,0,1,1,0);
            //保持约束关系,如果移动草图时，保留草图关系，可能会移不动草图，谨慎使用
            //SwModelDocExt.MoveOrCopy(false, 1, true, 0, 0, 0, 1, 1, 0);
            //移动两次,应该到2，2位置，但是并没有
            //SwModelDocExt.MoveOrCopy(false, 2, false, 0, 0, 0, 1, 1, 0);

            //复制草图
            //SwModelDocExt.MoveOrCopy(true, 1, false, 0, 0, 0, 1, 1, 0);
            //复制2个草图
            //SwModelDocExt.MoveOrCopy(true, 2, false, 0, 0, 0, 1, 1, 0);
            //保留约束，不明显，我们多画一个草图
            //SwModelDocExt.MoveOrCopy(true, 2, true, 0, 0, 0, 1, 1, 0);
            //没有草图约束
            SwModelDocExt.MoveOrCopy(true, 2, false, 0, 0, 0, 1, 1, 0);

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 旋转或复制草图实体
        /// </summary>
        public void RotateOrCopy()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateCornerRectangle(-0.2, -0.2, 0, 0.2, 0.2, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Line1","SKETCHSEGMENT",0,0,0,false,0,null,0);
            SwModelDocExt.SelectByID2("Line2","SKETCHSEGMENT",0,0,0,true,0,null,0);
            SwModelDocExt.SelectByID2("Line3","SKETCHSEGMENT",0,0,0,true,0,null,0);
            SwModelDocExt.SelectByID2("Line4","SKETCHSEGMENT",0,0,0,true,0,null,0);


            //基点0，0，0
            //SwModelDocExt.RotateOrCopy(false,1,false,0,0,0,0,0,1,45*Math.PI/180);

            //基点0.2, 0.2, 0
            //SwModelDocExt.RotateOrCopy(false, 1, false, 0.2, 0.2, 0, 0, 0, 1, 45*Math.PI/180);

            //改变旋转角度，草图实体是逆时针旋转
            //SwModelDocExt.RotateOrCopy(false, 1, false, 0.2, 0.2, 0, 0, 0, 1, 30*Math.PI/180);

            //复制
            //SwModelDocExt.RotateOrCopy(true, 1, false, 0.2, 0.2, 0, 0, 0, 1, 30*Math.PI/180);

            //复制,2个
            //SwModelDocExt.RotateOrCopy(true, 2, false, 0.2, 0.2, 0, 0, 0, 1, 30*Math.PI/180);

            //保留约束关系
            SwModelDocExt.RotateOrCopy(true, 2, true, 0.2, 0.2, 0, 0, 0, 1, 30*Math.PI/180);

            ViewZoomAndExitSketch();
        }



    }
}
