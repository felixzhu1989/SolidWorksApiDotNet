using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace LearnSolidWorksApi;

public class EditFeature
{
    private readonly SldWorks _swApp;

    public EditFeature(SldWorks swApp)
    {
        _swApp = swApp;
    }

    /// <summary>
    /// 拉伸
    /// </summary>
    public void FeatureExtrusion()
    {
        //调用扩展方法新建零件
        var swModel = _swApp.CreatePart();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //选择前视基准平面，插入草图
        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null,
            (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);
        //草图中绘制中心矩形
        swSketchMgr.CreateCenterRectangle(0, 0, 0, 1, 1, 0);
        //退出草图
        swSketchMgr.InsertSketch(true);
        //适应屏幕，显示矩形
        swModel.ViewZoomtofit2();
        //绘制完草图后，草图默认处理选中状态，为了体现拉伸前必须选择草图，所以我们还是手动选择它
        //首先清除选择
        swModel.ClearSelection2(true);
        //以边线上某个点
        swModelDocExt.SelectByID2("", "SKETCH", 0, 1, 0, false, 0, null, 0);
        //创建拉伸特征,返回的是特征
        var swFeat = swFeatMgr.FeatureExtrusion3(
            true, false, false,//拉伸方向相关的参数，单向时给true
            (int)swEndConditions_e.swEndCondBlind, 0,//结束条件，可以根据枚举，也可以直接给int值
            2d, 0,//拉伸深度，由于单向拉伸，因此只给方向1，正方体
            false, false, false, false, 0, 0,//拔模相关,由于我们不拔模，所以就不演示了
            false, false, false, false,//反向等距和转化曲面（到指定面指定的距离）
            true, false, true,//多实体选项
            (int)swStartConditions_e.swStartSketchPlane, 0, false//起始条件相关参数
        );
        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Boss-Extrude1，特征类型：Extrusion
        }
    }

    /// <summary>
    /// 旋转
    /// </summary>
    public void FeatureRevolve()
    {
        var swModel = _swApp.CreatePart();
        var swModelExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        swModelExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0,null, 0);
        swSketchMgr.InsertSketch(true);

        //中心线
        swSketchMgr.CreateCenterLine(0, 0, 0, 0, 2, 0);
        //旋转的草图
        swSketchMgr.CreateCircleByRadius(-2, 1, 0, 0.5);
        swSketchMgr.InsertSketch(true);
        swModel.ViewZoomtofit2();

        //清除选择
        swModel.ClearSelection2(true);
        //中心标记为4
        swModelExt.SelectByID2("", "EXTSKETCHSEGMENT", 0, 1, 0, false, 4, null, 0);
        //草图圆标记为0
        swModelExt.SelectByID2("", "EXTSKETCHSEGMENT", -1.5, 1, 0, true, 0, null, 0);

        var swFeat= swFeatMgr.FeatureRevolve2(
            true,true,true,false,//旋转结果,片体如何呢,薄壁特征
            true,false,//旋转方向，改成顺时针看看
            (int)swEndConditions_e.swEndCondBlind,0,270*Math.PI/180d,0,//结束条件,旋转角度，270度看看，逆时针方向
            false,false,0,0,//偏移相关
            (int)swThinWallType_e.swThinWallOneDirection,10d/1000d,0,//薄壁特征相关
            true,false,true//多实体相关参数
        );

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Revolve1，特征类型：Revolution
            //(非实体，片体)特征名：Surface-Revolve1，特征类型：RevolvRefSurf
            //(薄壁特征)特征名：Revolve-Thin1，特征类型：RevolutionThin

        }
    }
    
    /// <summary>
    /// 扫描基体/凸台特征
    /// </summary>
    public void Sweep()
    {
        var swModel = _swApp.CreatePart();

        var swModelExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;
        //轮廓
        swModelExt.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateEllipse(0, 0, 0, 0.5, 0, 0, 0, 1, 0);
        swSketchMgr.InsertSketch(true);
        //路径
        swModel.ClearSelection2(true);
        swModelExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateArc(-2, 0, 0, 0, 0, 0, -2, 2, 0, 1);
        swSketchMgr.InsertSketch(true);

        swModel.ViewZoomtofit2();
        
        swModel.ClearSelection2(true);
        //选择轮廓标记为1
        swModelExt.SelectByID2("", "SKETCH", 0.5, 0, 0, false, 1, null, 0);
        //选择路径标记为4
        swModelExt.SelectByID2("", "SKETCH", -2, 2, 0, true, 4, null, 0);
        var swSweepData =(SweepFeatureData) swFeatMgr.CreateDefinition((int)swFeatureNameID_e.swFmSweep);
        //使用SweepFeatureData的默认值，不修改了
        var swFeat = swFeatMgr.CreateFeature(swSweepData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Sweep1，特征类型：Sweep
        }
    }

    /// <summary>
    /// 放样特征
    /// </summary>
    public void Loft()
    {
        var swModel = _swApp.CreatePart();

        var swModelExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //绘制第一个轮廓
        swModelExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateEllipse(0, 0, 0, 0.07, 0, 0, 0, 0.03, 0);//原点+长轴顶点+短轴顶点
        swSketchMgr.InsertSketch(true);

        //插入新的参考基准面，绘制第二个轮廓
        swModel.ClearSelection2(true);
        swModelExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        swFeatMgr.InsertRefPlane(8, 0.07, 0, 0, 0, 0);//Distance，按照距离创建参考基准面
        swModel.ClearSelection2(true);
        //z坐标，因为是相对前视基准平面偏移的，xy为0
        swModelExt.SelectByID2("", "PLANE", 0, 0, 0.07, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateEllipse(0, 0, 0, 0.05, 0, 0, 0, 0.01, 0);
        swSketchMgr.InsertSketch(true);

        //插入新的参考基准面，绘制第三个轮廓（与第二个一样）
        swModel.ClearSelection2(true);
        swModelExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        swFeatMgr.InsertRefPlane(8, 0.14, 0, 0, 0, 0);//Distance，按照距离创建参考基准面
        swModel.ClearSelection2(true);
        //z坐标，因为是相对前视基准平面偏移的，xy为0
        swModelExt.SelectByID2("", "PLANE", 0, 0, 0.14, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateEllipse(0, 0, 0, 0.06, 0, 0, 0, 0.02, 0);
        swSketchMgr.InsertSketch(true);

        //显示轴测图，方便观察
        swModel.ShowNamedView2("",(int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();

        //创建引导曲线
        swModel.ClearSelection2(true);
        //只用坐标选择，不用名字看看,Mark标记为1，选择第二个点起需要加选(椭圆的长轴顶点)
        swModelExt.SelectByID2("", "EXTSKETCHPOINT",0.07,0,0,false,1,null,0);
        swModelExt.SelectByID2("", "EXTSKETCHPOINT",0.05,0, 0.07, true,1,null,0);
        swModelExt.SelectByID2("", "EXTSKETCHPOINT",0.06,0, 0.14, true, 1,null,0);
        swModel.Insert3DSplineCurve(false);//根据前面选择的三个点创建3D样条曲线

        swModelExt.SelectByID2("", "EXTSKETCHPOINT", -0.07, 0, 0, false, 1, null, 0);
        swModelExt.SelectByID2("", "EXTSKETCHPOINT", -0.05, 0, 0.07, true, 1, null, 0);
        swModelExt.SelectByID2("", "EXTSKETCHPOINT", -0.06, 0, 0.14, true, 1, null, 0);
        swModel.Insert3DSplineCurve(false);//根据前面选择的三个点创建3D样条曲线

        //按照顺序选择轮廓(标记为1)和引导线(标记为2)，创建放样特征
        swModel.ClearSelection2(true);
        swModelExt.SelectByID2("", "SKETCH", 0.07, 0, 0, false, 1, null, 0);
        swModelExt.SelectByID2("", "SKETCH", 0.05, 0, 0.07, true, 1, null, 0);//加选
        swModelExt.SelectByID2("", "SKETCH", 0.06, 0, 0.14, true, 1, null, 0);

        swModelExt.SelectByID2("", "REFERENCECURVES", 0.05, 0, 0.07, true, 2, null, 0);
        swModelExt.SelectByID2("", "REFERENCECURVES", -0.05, 0, 0.07, true, 2, null, 0);

       var swFeat= swFeatMgr.InsertProtrusionBlend(false //闭合放样
            , true //保持相切
            , false //获得更光滑的表面
            , 1 //中间截面数量因子
            , 0 //起始相切类型
            , 0 //结束相切类型
            , 1 //起始切线长度
            , 1 //结束切线长度
            , true //起始沿着相切方向
            , true //结束沿着相切方向
            , false //薄壁特征false表示不是薄壁
            , 0 //壁厚1
            , 0 //壁厚2
            , 0 //壁厚类型
            , true //合并实体
            , true
            , true
        );//由于没有用InsertProtrusionBlend2方法，所以没有最后一个参数

       if (swFeat != null)
       {
           Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Loft1，特征类型：Blend
        }

    }

    /// <summary>
    /// 拉伸切除
    /// </summary>
    public void FeatureCut()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //选择现有正方体上的面
        swModel.ClearSelection2(true);
        //以超出正方体的一个点，射向正方体某个面,从点(0,0,3)沿着z轴负向(0,0,-1)射线(圆柱形射线半径为0.001)，于射线相交的第一个面swSelFACES
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);

        //创建键槽草图
        swSketchMgr.InsertSketch(true);
        //键槽类型，标注类型，宽度，圆心点1，圆心点2，圆端点,弧槽的绘制方向这里不适用，是否标注
        swSketchMgr.CreateSketchSlot((int)swSketchSlotCreationType_e.swSketchSlotCreationType_line,
            (int)swSketchSlotLengthType_e.swSketchSlotLengthType_FullLength,
            0.5,
            -0.25,0,0,
            0.25,0,0,
            0.25,0.25,0,
            1,true);
        //绘制完草图后，草图默认处于选中状态，可以直接拉伸切除
       var swFeat= swFeatMgr.FeatureCut4(
            true,false,false,//拉伸切除方向
            (int)swEndConditions_e.swEndCondBlind,0,//终止条件，给定深度
            1,0,//拉伸深度1米
            false,false,false,false,0,0,//拔模没有
            false,false,false,false,//偏移没有
            false,//钣金正交切除，这里不是钣金，如果以后遇到钣金再讨论，非钣金取false
            false,true,//多实体的特征范围
            false,true,false,//装配体特征范围
            (int)swStartConditions_e.swStartSketchPlane,0,false,//起始条件与偏移
            false//钣金优化几何图形
            );
       if (swFeat != null)
       {
           Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName()}");
            //特征名：Cut-Extrude1，特征类型：ICE（不知道是什么，这个时候请用GetTypeName()而不是GetTypeName2）
            //特征名：Cut-Extrude1，特征类型：Cut
        }

        //显示轴测图，方便观察
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }



}