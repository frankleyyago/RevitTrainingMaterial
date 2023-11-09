#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
#endregion

namespace MyIntroCs
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalApplication
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class App : IExternalApplication
    {
        /// <summary>
        /// Implements the on Shutdown event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Implements the OnStartup event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = RibbonPanel(application);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            if (panel.AddItem(new PushButtonData("FirstPlugin1", "FirstPlugin2", thisAssemblyPath, "MyIntroCs.1_HelloWorld"))
                is PushButton button1)
            {
                button1.ToolTip = "My First Plugin";

                Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "Icon.ico"));
                BitmapImage bitmapImage = new BitmapImage(uri);
                button1.LargeImage = bitmapImage;

            }

            panel.AddSeparator();

            if (panel.AddItem(new PushButtonData("FirstPlugin2", "FirstPlugin3", thisAssemblyPath, "MyIntroCs.2_DbElement"))
                is PushButton button2)
            {
                button2.ToolTip = "My First Plugin";

                Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "Icon.ico"));
                BitmapImage bitmapImage = new BitmapImage(uri);
                button2.LargeImage = bitmapImage;

            }

            if (panel.AddItem(new PushButtonData("FirstPlugin3", "FirstPlugin4", thisAssemblyPath, "MyIntroCs.Command"))
                is PushButton button3)
            {
                button3.ToolTip = "My First Plugin";

                Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "Icon.ico"));
                BitmapImage bitmapImage = new BitmapImage(uri);
                button3.LargeImage = bitmapImage;

            }

            return Result.Succeeded;

        }

        #region RibbonPanel
        /// <summary>
        /// Function that creates RibbonPanel
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "LabTab";

            RibbonPanel ribbonPanel = null;

            try
            {
                a.CreateRibbonTab(tab);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "LabTab");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == "LabTab"))
            {
                ribbonPanel = p;
            }

            return ribbonPanel;


        }
        #endregion
    }

}
