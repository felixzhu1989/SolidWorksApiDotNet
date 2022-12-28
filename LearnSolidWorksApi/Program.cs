// See https://aka.ms/new-console-template for more information

using LearnSolidWorksApi;
using SolidWorks.Interop.sldworks;

SldWorks swApp = SolidWorksSingleton.GetApplication();
//swApp.SendMsgToUser("Hello, SolidWorks!");
Console.WriteLine("Hello, SolidWorks!");

EditSketch editSketch=new EditSketch(swApp);
//创建圆角
editSketch.CreateFillet();
