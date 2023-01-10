// See https://aka.ms/new-console-template for more information

using LearnSolidWorksApi;
using SolidWorks.Interop.sldworks;

SldWorks swApp = SolidWorksSingleton.GetApplication();
//swApp.SendMsgToUser("Hello, SolidWorks!");
Console.WriteLine("Hello, SolidWorks!");

EditSketch editSketch=new EditSketch(swApp);
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
editSketch.RotateOrCopy();

