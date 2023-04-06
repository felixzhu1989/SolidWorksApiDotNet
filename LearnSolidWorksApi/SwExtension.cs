using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace LearnSolidWorksApi
{
    //扩展方法必须放在静态类中
    public static class SwExtension
    {
        /// <summary>
        /// 使用模板新建零件扩展方法
        /// </summary>
        public static ModelDoc2 CreatePart(this SldWorks swApp)
        {
            //扩展方法必须有this参数,比如这里的swApp，说明是对SldWorks的扩展方法
            //从用户设置中获取零件模板设置
            var defaultTemplate =
                swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            //使用默认模板新建零件
           return (swApp.NewDocument(defaultTemplate, 0, 0, 0) as ModelDoc2)!;
        }
    }
}
