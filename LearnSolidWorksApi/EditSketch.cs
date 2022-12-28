using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace LearnSolidWorksApi
{
    public class EditSketch
    {
        public ModelDoc2 SwModel { get; set; }
        public ModelDocExtension SwModelDocExt { get; set; }
        public SketchManager SwSketchManager { get; set; }
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
            SwModel=(_swApp.NewDocument(defaultTemplate,0,0,0) as ModelDoc2)!;
            SwModelDocExt = SwModel.Extension;
            SwSketchManager = SwModel.SketchManager;
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
            SwSketchManager.InsertSketch(true);
            //绘制矩形
            SwSketchManager.CreateCornerRectangle(0, 1, 0, 1, 0, 0);
            SwModel.ClearSelection2(true);
            //选择角点
            SwModelDocExt.SelectByID2("Point1", "SKETCHPOINT", 0, 1, 0, false, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);
            //创建草图圆角
            SwSketchManager.CreateFillet(0.1, (int)swConstrainedCornerAction_e.swConstrainedCornerDeleteGeometry);
            //调整视图
            SwModel.ShowNamedView2("",(int)swStandardViews_e.swFrontView);
            SwModel.ViewZoomtofit2();
            //退出草图编辑
            SwSketchManager.InsertSketch(true);
        }
    }
}
