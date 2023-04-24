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
}