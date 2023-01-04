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
editSketch.SketchOffset();
