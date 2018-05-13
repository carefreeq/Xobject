using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;

namespace RevitToXObject
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class App : IExternalApplication
    {
        private static string name = "Revit to XObject";
        private static AddInId m_appId = new AddInId(new Guid("99B8B1D3-90CB-4146-9726-6DBBB2709824"));
        private static string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static UIControlledApplication UIControlledApplication { get; private set; }
        public Result OnStartup(UIControlledApplication application)
        {
            AddMenu(application);
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void AddMenu(UIControlledApplication app)
        {
            RibbonPanel rvtRibbonPanel = app.CreateRibbonPanel(name);
#if Pulldown
            PulldownButton btn = rvtRibbonPanel.AddItem(new PulldownButtonData("Options", "Revit to XObject")) as PulldownButton;

            btn.AddPushButton(new PushButtonData("HelloWorld", "Hello World...", location, "RevitToXObject.Command"));
#else
            PushButton btn = rvtRibbonPanel.AddItem(new PushButtonData("Revit To XObject", "Revit to XObject", location, "RevitToXObject.Command")) as PushButton;
#endif
            //btn.Image = GetEmbeddedImage("RevitToXObject.Resources.XObject_16.png");
            //btn.LargeImage = GetEmbeddedImage("RevitToXObject.Resources.XObject_32.png");
            UIControlledApplication = app;
        }
        static BitmapSource GetEmbeddedImage(string name)
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream(name);
                return BitmapFrame.Create(s);
            }
            catch
            {
                return null;
            }
        }
    }
}
