using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Diagnostics;

namespace LearnSolidWorksApi;

public class EditFeature
{
    private readonly ISldWorks _swApp;

    public EditFeature(ISldWorks swApp)
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

    /// <summary>
    /// 旋转切除
    /// </summary>
    public void RevolveCut()
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

        //创建草图，绘制中心线和圆
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCenterLine(0, 0, 0, 0, 1, 0);
        swSketchMgr.CreateCircleByRadius(0.5, 1, 0, 0.2);
        //绘制完草图后直接旋转切除，旋转切除的代码和旋转凸台差不多，只是切除参数改成了true
        var swFeat = swFeatMgr.FeatureRevolve2(
            true, true, false, //旋转结果,片体如何呢,薄壁先不要，再来看看薄壁切除特征
            true,//旋转切除参数赋值为true
            false, false,//旋转方向，改成顺时针看看,不能反转方向，否则就到外面去了，拉伸切除就失败了
            //如果360度就无所谓了
            (int)swEndConditions_e.swEndCondBlind, 0,//结束条件,
            360*Math.PI/180d, 0,//旋转角度，270度看看，逆时针方向
            false, false, 0, 0,//偏移相关
            (int)swThinWallType_e.swThinWallOneDirection, 10d/1000d, 0,//薄壁特征相关
            true, false, true//多实体相关参数
        );

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //凸台时：特征名：Revolve1，特征类型：Revolution
            //(非实体，片体)特征名：Surface-Revolve1，特征类型：RevolvRefSurf
            //(薄壁特征)特征名：Revolve-Thin1，特征类型：RevolutionThin
            //旋转切除时：特征名：Cut-Revolve1，特征类型：RevCut
            //(薄壁切除特征)特征名：Cut-Revolve-Thin1，特征类型：RevCutThin
            //(非实体，片体,不管是拉伸凸台还是切除，都不参与实体的布尔操作，保持独立片体)特征名：Surface-Revolve1，特征类型：RevolvRefSurf

        }
        //显示轴测图，方便观察
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 扫描切除
    /// </summary>
    public void SweepCut()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //选择正面,使用SelectByRay
        swModel.ClearSelection2(true);
        //以超出正方体的一个点，射向正方体某个面,从点(0,0,3)沿着z轴负向(0,0,-1)射线(圆柱形射线半径为0.001)，于射线相交的第一个面swSelFACES
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);
        //创建草图，绘制草图圆
        swSketchMgr.InsertSketch(true);
        //注意编辑草图时，草图原点默认落在零件模型的原点，要在靠近顶面的位置绘制圆，因此圆心坐标的y值为1
        swSketchMgr.CreateCircleByRadius(0, 1, 0, 0.2);
        swSketchMgr.InsertSketch(true);

        //选择顶面，绘制样条曲线
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByRay(0, 2, 1, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);
        //绘制样条曲线之前需要构建一个数组（点坐标x，y，z）
        var points = new double[12];//取4个点即可
        points[0] = 0;//x
        points[1] = 0;//y
        points[2] = 0;//z
        points[3] = 0.2;
        points[4] = -0.5;
        points[5] = 0;
        points[6] = -0.2;
        points[7] = -1.5;
        points[8] = 0;
        points[9] = 0;
        points[10] = -2;
        points[11] = 0;
        //创建样条曲线
        swSketchMgr.CreateSpline3(points, null, null, true, out _);
        swSketchMgr.InsertSketch(true);
        swModel.ClearSelection2(true);

        //显示轴测图，方便观察
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();

        //选择轮廓标记为1,草图圆
        swModelDocExt.SelectByID2("", "SKETCH", 0.2, 1, 2, false, 1, null, 0);
        //选择路径标记为4，样条曲线草图
        swModelDocExt.SelectByID2("", "SKETCH", 0, 1, 0, true, 4, null, 0);
        var swSweepData = (SweepFeatureData)swFeatMgr.CreateDefinition((int)swFeatureNameID_e.swFmSweepCut);
        //使用SweepFeatureData的默认值，不修改了
        var swFeat = swFeatMgr.CreateFeature(swSweepData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //扫描凸台swFmSweep：特征名：Sweep1，特征类型：Sweep
            //扫描切除swFmSweepCut：特征名：Cut-Sweep1，特征类型：SweepCut
        }

        ////显示轴测图，方便观察
        //swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        //swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 放样切除
    /// </summary>
    public void LoftCut()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //选择正面，绘制矩形
        //选择正面,使用SelectByRay
        swModel.ClearSelection2(true);
        //以超出正方体的一个点，射向正方体某个面,从点(0,0,3)沿着z轴负向(0,0,-1)射线(圆柱形射线半径为0.001)，于射线相交的第一个面swSelFACES
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);
        //创建草图，绘制草图
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCenterRectangle(0, 0, 0, 0.7, 0.7, 0);
        swSketchMgr.InsertSketch(true);
        swModel.ClearSelection2(true);

        //选择背面，绘制草图圆
        swModelDocExt.SelectByRay(0, 0, -3, 0, 0, 1, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);
        //创建草图，绘制草图
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(0, 0, 0, 0.5);
        swSketchMgr.InsertSketch(true);
        swModel.ClearSelection2(true);

        //选择两个轮廓草图，标记为1
        swModelDocExt.SelectByID2("", "SKETCH", 0.7, 0.7, 2, false, 1, null, 0);
        swModelDocExt.SelectByID2("", "SKETCH", 0.5, 0, 0, true, 1, null, 0);

        //创建放样切除特征
        //可以对比放样凸台的参数看看
        //var swFeat = swFeatMgr.InsertProtrusionBlend(false //闭合放样
        //    , true //保持相切
        //    , false //获得更光滑的表面
        //    , 1 //中间截面数量因子
        //    , 0 //起始相切类型
        //    , 0 //结束相切类型
        //    , 1 //起始切线长度
        //    , 1 //结束切线长度
        //    , true //起始沿着相切方向
        //    , true //结束沿着相切方向
        //    , false //薄壁特征false表示不是薄壁
        //    , 0 //壁厚1
        //    , 0 //壁厚2
        //    , 0 //壁厚类型
        //    , true //合并实体
        //    , true
        //    , true
        //);//由于没有用InsertProtrusionBlend2方法，所以没有最后一个参数
        var swFeat = swFeatMgr.InsertCutBlend(false//闭合放样
            , true//保持相切
            , true//获得更光滑的表面
            , 1//中间截面数量因子
            , 0//起始相切类型
            , 0//结束相切类型
            , false//薄壁特征false表示不是薄壁
            , 0 //壁厚1
            , 0 //壁厚2
            , 0//壁厚类型
            , true //受特征影响的范围
            , true);//自动选择受特征影响的实体
        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //放样切除：特征名：Cut-Loft1，特征类型：BlendCut
            //放样凸台：特征名：Loft1，特征类型：Blend
        }

        //显示轴测图，方便观察
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 异型孔向导
    /// </summary>
    public void HoleWizard()
    {
        var swModel = _swApp.CreatePart();
        var swModelExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        #region 创建一个长方体
        swModelExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCenterRectangle(0, 0, 0, 10d / 1000d, 5d / 1000d, 0);

        swFeatMgr.FeatureExtrusion2(true, false, false, 0, 0, 50 / 1000d, 0, false, false, false, false, 0, 0, false,
            false, false, false, true, true, true, 0, 0, false);
        #endregion

        //选择顶面
        swModelExt.SelectByRay(0, 10d/1000d, 30d/1000d, 0,
            -1, 0, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);//2，代表面
        //异型孔向导
        var swHoleFeature = swFeatMgr.HoleWizard5(
            0,//孔类型，这里代表柱形沉头孔
            8,//标准ISO
            139,//紧固件类型
            "M6",//孔规格
            1,//终止条件，完全贯穿
            6.6d/1000d,//孔直径
            -1,//孔深度，完全贯穿时不起作用
            -1,//槽长度，不适用于孔
            11d/1000d,//沉头直径
            6.4d/1000d,//沉头深度
            0,//头部间隙
            1,//螺钉套合，1正常
            0,//底部钻孔，贯穿时不适用
            0.012,//近端锥孔直径
            Math.PI/2d,//近端锥孔角度，90都的弧度
            7d/1000d,//螺钉下锥孔直径
            Math.PI/2d,//螺钉下锥孔角度
            7d/1000d,//远端锥孔直径
            Math.PI/2d,//远端锥孔角度
            0,//偏移，不适用
            "",//螺纹线等级
            false,//不反转方向
            true,//特征影响选定实体
            true,
            true,//装配体特征
            true,
            false);
        //选择第一个子特征，也就是位置草图特征
        var swSketchFeature = (Feature)swHoleFeature.GetFirstSubFeature();//特别注意对返回的object对象进行强制转换，因为C#是强类型语言
        swSketchFeature.Select2(false, 0);
        swModel.EditSketch();//编辑选中的草图
        var swSelectionManager = (SelectionMgr)swModel.SelectionManager;//选择管理器对象
        var swSketch = (Sketch)swSketchFeature.GetSpecificFeature2();//根据特征获取具体的对象类型，这里是草图对象
       var swSketchPointArray = swSketch.GetSketchPoints2() as object[];//实际上是一个草图点的数组
       //如果需要遍历的话需要转换成数组，删除位置草图中所有的点
       for (int i = 0; i < swSketchPointArray.Length; i++)
       {
           //遍历时，CSharp的语法比较简洁
           var boolstatus = swSelectionManager.AddSelectionListObject(swSketchPointArray[i], null);//将草图点添加到选择集中
           Debug.Print(boolstatus.ToString());
           swModel.EditDelete();//删除选中对象
       }
       //这里需要特别注意，编辑草图时草图的坐标
       //本案例中，图形处于y轴负方向
       //在实际应用时，可以通过参数来控制点的坐标
       swSketchMgr.CreatePoint(0, -10d/1000d, 0);
        //也可以创建多个点
        swSketchMgr.CreatePoint(0, -25d/1000d, 0);
        swSketchMgr.CreatePoint(0, -40d/1000d, 0);


        swSketchMgr.InsertSketch(true);


        if (swHoleFeature != null)
        {
            Console.WriteLine($"特征名：{swHoleFeature.Name}，特征类型：{swHoleFeature.GetTypeName2()}");
            //特征名：M6 六角凹头螺钉的柱形沉头孔1，特征类型：HoleWzd
        }

        //显示轴测图，方便观察
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 面圆角
    /// </summary>
    public void FaceFillet()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //Create the fillet feature data object
        var swFilletData = (SimpleFilletFeatureData2)swFeatMgr.CreateDefinition((int)swFeatureNameID_e.swFmFillet);//注意强制转换类型

        //Initialize the fillet feature data object with a simple fillet type
        swFilletData.Initialize((int)swSimpleFilletType_e.swFaceFillet);//初始化为面圆角


        swModel.ClearSelection2(true);
        // Select adjacent faces using Marks 2 and 4,选择相邻的两个面，并分别标记为2和4
        //正面
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, 2, false, 2, 0);
        swModelDocExt.SelectByRay(0, 3, 0, 0, -1, 0, 0.001, 2, true, 4, 0);
        swFilletData.AsymmetricFillet = true;//不对称
        swFilletData.DefaultRadius = 0.1d;//第一圆角大小，因为我们这里正方体比较大，所以就画得大
        swFilletData.DefaultDistance = 0.2d;//第二圆角大小


        //Narrow the fillet type by specifying the feature fillet profile type
        swFilletData.ConicTypeForCrossSectionProfile = (int)swFeatureFilletProfileType_e.swFeatureFilletCircular;


        //Create the fillet feature
        var swFeat = swFeatMgr.CreateFeature(swFilletData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Fillet1，特征类型：Fillet
        }
    }

    /// <summary>
    /// 固定大小圆角
    /// </summary>
    public void ConstRadiusFillet()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //Create the fillet feature data object
        var swFilletData = (SimpleFilletFeatureData2)swFeatMgr.CreateDefinition((int)swFeatureNameID_e.swFmFillet);//注意强制转换类型

        //Initialize the fillet feature data object with a simple fillet type
        swFilletData.Initialize((int)swSimpleFilletType_e.swConstRadiusFillet);//初始化为固定大小圆角


        swModel.ClearSelection2(true);
        //Call IModelDocExtension::SelectByID2 with Mark = 1 to select the edges, faces, features, or loops to fillet. 
        //edges,选择边的时候
        swModelDocExt.SelectByRay(0, 3, 2, 0, -1, 0, 0.001, (int)swSelectType_e.swSelEDGES, false, 1, 0);
        //faces，选择面的时候
        //swModelDocExt.SelectByRay(0, 3, 1, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, false, 1, 0);
        //features,选择特征时候，选择特征，我们最好是从特征树上选择
        //FeatureByPositionReverse逆序获取特征，0代表最后一个特征
        //var extrudeFeat = (Feature)swModel.FeatureByPositionReverse(0);
        //然后使用Select2方法选择它并标记为1
        //extrudeFeat.Select2(false, 1);

        //loops,比较复杂，有兴趣的朋友可以研究一下



        /*swFilletData.AsymmetricFillet = true;//不对称
        swFilletData.DefaultRadius = 0.1d;//第一圆角大小，因为我们这里正方体比较大，所以就画得大
        swFilletData.DefaultDistance = 0.2d;//第二圆角大小*/
        //对称
        swFilletData.AsymmetricFillet = false;
        swFilletData.DefaultRadius = 0.2d;


        //Narrow the fillet type by specifying the feature fillet profile type
        swFilletData.ConicTypeForCrossSectionProfile = (int)swFeatureFilletProfileType_e.swFeatureFilletCircular;


        //Create the fillet feature
        var swFeat = swFeatMgr.CreateFeature(swFilletData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Fillet1，特征类型：Fillet
        }
    }

    /// <summary>
    /// 完整圆角
    /// </summary>
    public void FullRoundFillet()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //Create the fillet feature data object
        var swFilletData = (SimpleFilletFeatureData2)swFeatMgr.CreateDefinition((int)swFeatureNameID_e.swFmFillet);//注意强制转换类型

        //Initialize the fillet feature data object with a simple fillet type
        swFilletData.Initialize((int)swSimpleFilletType_e.swFullRoundFillet);//初始化为完整圆角类型


        swModel.ClearSelection2(true);
        //Call IModelDocExtension::SelectByID2 with:
        //-Mark = 2 to select Face Set 1.左边面
        swModelDocExt.SelectByRay(-3, 0, 1, 1, 0, 0, 0.001, 2, false, 2, 0);

        //- Mark = 512 to select Center Face Set.正面作为中面
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, 2, true, 512, 0);

        //- Mark = 4 to select Face Set 2.右边面
        swModelDocExt.SelectByRay(3, 0, 1, -1, 0, 0, 0.001, 2, true, 4, 0);
        
        swFilletData.PropagateToTangentFaces=true;//可选项


        //Narrow the fillet type by specifying the feature fillet profile type
        swFilletData.ConicTypeForCrossSectionProfile = (int)swFeatureFilletProfileType_e.swFeatureFilletCircular;


        //Create the fillet feature
        var swFeat = swFeatMgr.CreateFeature(swFilletData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Fillet1，特征类型：Fillet
        }
    }

    /// <summary>
    /// 倒角
    /// </summary>
    public void Chamfer()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //选择边
        swModel.ClearSelection2(true);
        //swModelDocExt.SelectByRay(0, 3, 2, 0, -1, 0, 0.001, (int)swSelectType_e.swSelEDGES, false, 0, 0);

        //AngleDistance
        //var swFeat = swFeatMgr.InsertFeatureChamfer(
        //    (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,//切线延伸
        //    (int)swChamferType_e.swChamferAngleDistance,//角度距离
        //    0.1,Math.PI/4d,0, //距离，角度45度(Math.PI/2d是90度)
        //    0,0,0
        //    );

        //DistanceDistance
        //var swFeat = swFeatMgr.InsertFeatureChamfer(
        //    (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,//切线延伸
        //    (int)swChamferType_e.swChamferDistanceDistance,//距离距离
        //    0.1, 0, 0.2, //距离，0,距离
        //    0, 0, 0
        //);

        //EqualDistance，失败了，这个有问题，最好不要用
        //var swFeat = swFeatMgr.InsertFeatureChamfer(
        //    (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,//切线延伸
        //    (int)swChamferType_e.swChamferEqualDistance,//等距离
        //    0, 0, 0.2, //0，0,距离
        //    0, 0, 0
        //);

        //Vertex，顶点，因此需要选择顶点而不是边
        swModelDocExt.SelectByRay(1, 3, 2, 0, -1, 0, 0.001, (int)swSelectType_e.swSelVERTICES, false, 0, 0);

        var swFeat = swFeatMgr.InsertFeatureChamfer(
            (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,//切线延伸
            (int)swChamferType_e.swChamferVertex,//顶点
            0, 0, 0, //0，0,距离
            0.1, 0.2, 0.3 //顶点的距离
        );

        if (swFeat != null)
        {
            Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
            //特征名：Chamfer1，特征类型：Chamfer
        }

    }

    /// <summary>
    /// 镜像特征
    /// </summary>
    public void Mirror()
    {
        //创建一个正方体
        FeatureExtrusion();
        //首先准备一些模型对象
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatMgr = swModel.FeatureManager;

        //选择顶面再绘制一个拉伸特征
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByRay(0, 3, 0, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(-0.5, -1.5, 0, 0.4);
        //要镜像的拉伸特征
        var extrudeFeat = swFeatMgr.FeatureExtrusion3(true, false, false,
            (int)swEndConditions_e.swEndCondBlind, 0, 0.5d, 0,
            false, false, false, false, 0, 0,
            false, false, false, false,
            true, false, true, (int)swStartConditions_e.swStartSketchPlane, 0, false);
        swModel.ClearSelection2(true);
        //选择需要镜像的特征，标记为1，选择镜像平面标记为2
        extrudeFeat.Select2(false, 1);
        swModelDocExt.SelectByID2("Right Plane", "PLANE", 0, 0, 0, true, 2, null, 0);
        //插入镜像特征
      var swFeat=  swFeatMgr.InsertMirrorFeature2(
            false, //false表示镜像特征或面，true表示镜像体
            false, //false表示不是镜像几何体
            false, //合并所有镜像实体
            false, //Knit表面
            (int)swFeatureScope_e.swFeatureScope_AllBodies);//受镜像特征影响的实体


      if (swFeat != null)
      {
          Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
          //特征名：Mirror1，特征类型：MirrorPattern
        }
        //显示轴测图，方便观察
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
      swModel.ViewZoomtofit2();
    }






}