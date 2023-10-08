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
    /// Hello World #1 - simplified without full namespace.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class HelloWorldSimple : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("My Dialog Title", "Hello World Simple!");

            return Result.Succeeded;
        }
    }
}
