using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace MyIntroCs
{
    /// <summary>
    /// Hello World #3 - minimum external application.
    /// </summary>
    internal class HelloWorldApp : IExternalApplication
    {
        //OnShutdown() method is called when Revit shuts down.
        public Autodesk.Revit.UI.Result OnShutdown (Autodesk.Revit.UI.UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        //OnStartup() method is called when Revit starts up.
        public Autodesk.Revit.UI.Result OnStartup (Autodesk.Revit.UI.UIControlledApplication  application)
        {
            TaskDialog.Show("My Dialog Title", "Hello World from App!");
            return Result.Succeeded;
        }
       
    }
}
