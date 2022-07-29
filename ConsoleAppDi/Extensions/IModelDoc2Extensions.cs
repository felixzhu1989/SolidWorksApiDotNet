using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace ConsoleAppDi.Extensions
{
    public static class IModelDoc2Extensions
    {
        /// <summary>
        /// //遍历特征，获取基准平面
        /// </summary>
        public static IEnumerable<IFeature> GetRefPlane(this IModelDoc2 swModel)
        {
            //获取第一个特征
            var swFeat=swModel.FirstFeature() as IFeature;
            while (swFeat!=null)
            {
                //判断如果是基准面，则使用迭代器yield return，流水线处理，挨个将其压入返回值
                if (swFeat.GetTypeName2() == "RefPlane") yield return swFeat;
                //下一个特征
                swFeat=swFeat.GetNextFeature() as IFeature;
            }
        }
    }
}
