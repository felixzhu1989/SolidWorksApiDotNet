using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace LearnSolidWorksApi
{
    public class EditFeature
    {
        private readonly SldWorks _swApp;

        public EditFeature(SldWorks swApp)
        {
            _swApp = swApp;
        }

        public void FeatureExtrusion()
        {
            //调用扩展方法新建零件
            var swModel= _swApp.CreatePart();
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
                true,false,false,//拉伸方向相关的参数，单向时给true
                (int)swEndConditions_e.swEndCondBlind, 0,//结束条件，可以根据枚举，也可以直接给int值
                2d,0,//拉伸深度，由于单向拉伸，因此只给方向1，正方体
                false,false,false,false,0,0,//拔模相关,由于我们不拔模，所以就不演示了
                false,false,false,false,//反向等距和转化曲面（到指定面指定的距离）
                true,false,true,//多实体选项
                (int)swStartConditions_e.swStartSketchPlane,0,false//起始条件相关参数
                );
            if (swFeat != null)
            {
                Console.WriteLine($"特征名：{swFeat.Name}，特征类型：{swFeat.GetTypeName2()}");
                //特征名：Boss-Extrude1，特征类型：Extrusion
            }
        }
    }
}
