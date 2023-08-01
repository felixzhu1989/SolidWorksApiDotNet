// See https://aka.ms/new-console-template for more information

using LearnSolidWorksApi;

var swApp = SwUtility.ConnectSw();
//swApp.SendMsgToUser("Hello, SolidWorks!");
Console.WriteLine("Hello, SolidWorks!");

#region 编辑草图
//EditSketch editSketch=new EditSketch(swApp);
//创建草图圆角
//editSketch.CreateFillet();
//创建草图倒角
//editSketch.CreateChamfer();
//剪裁草图实体
//editSketch.SketchTrim();
//延伸草图实体
//editSketch.SketchExtend();
//偏移草图实体
//editSketch.SketchOffset();
//镜像草图实体
//editSketch.SketchMirror();
//创建线性草图阵列
//editSketch.CreateLinearSketchPattern();
//编辑线性草图阵列
//editSketch.EditLinearSketchPattern();
//创建圆周草图阵列
//editSketch.CreateCircularSketchPattern();
//编辑圆周草图阵列
//editSketch.EditCircularSketchPattern();
//移动或复制草图实体
//editSketch.MoveOrCopy();
//旋转复制草图实体
//editSketch.RotateOrCopy();
//隐藏显示草图约束关系
//editSketch.ViewSketchRelations();
//添加草图约束关系
//editSketch.SketchAddConstraints();
//标注草图尺寸
//editSketch.AddDimension();
//转换为构造线
//editSketch.CreateConstructionGeometry();
//拆分草图实体
//editSketch.SplitSegment(); 
#endregion

#region 编辑特征

EditFeature feature = new EditFeature(swApp);
//拉伸
//feature.FeatureExtrusion();
//旋转
//feature.FeatureRevolve();
//扫描
//feature.Sweep();
//放样
//feature.Loft();
//拉伸切除
//feature.FeatureCut();
//旋转切除
//feature.RevolveCut();
//扫描切除
//feature.SweepCut();
//放样切除
feature.LoftCut();



#endregion