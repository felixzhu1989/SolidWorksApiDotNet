using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace ConsoleAppDi.Extensions
{
    public static class ISldWorksExtensions
    {
        public static void WithToggleState(this ISldWorks swApp, swUserPreferenceToggle_e swUserPreference, bool actState, SketchManager swSketchManager,Action<SketchManager> action)
        {
            //先获取用户的设置状态
            var userState = swApp.GetUserPreferenceToggle((int)swUserPreference);
            //设置需要的状态
            swApp.SetUserPreferenceToggle((int)swUserPreference, actState);
            //执行操作
            action.Invoke(swSketchManager);
            //恢复用户设置状态
            swApp.SetUserPreferenceToggle((int)swUserPreference, userState);
        }

        public static void EditSketch(this ISldWorks swApp,string sketchName,Action<SketchManager> action)
        {
            var swModel = (ModelDoc2)swApp.ActiveDoc;
            if (swModel == null)
            {
                Console.WriteLine("请打开或新建文档");
                return;
            }
            var swExtension = swModel.Extension;
            //选择草图,如果没选中，则选择前视基准面
            var boolStatus = swExtension.SelectByID2(sketchName, "SKETCH", 0, 0, 0, false, 0, null, 0);
            if (!boolStatus)
            {
                //swExtension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
                swModel.GetRefPlane().FirstOrDefault()?.Select2(false, 0);
            }

            var swSketchManager = swModel.SketchManager;
            //进入草图编辑界面
            swSketchManager.InsertSketch(true);
            //关闭草图捕捉，Action带一个参数
            swApp.WithToggleState(swUserPreferenceToggle_e.swSketchInference, false, swSketchManager, action);

            swModel.ViewZoomToSelection();
            //退出草图编辑
            swSketchManager.InsertSketch(true);
        }

    }
}
