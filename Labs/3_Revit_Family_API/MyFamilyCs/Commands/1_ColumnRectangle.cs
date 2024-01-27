#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
#endregion

namespace MyFamilyCs
{
    public class ColumnRectangle : IExternalCommand
    {
        //Member variables.
        Application _apps;
        Document _doc;
        UIDocument _uidoc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Hello", "Hello");

            return Result.Succeeded;
        }
    }
}
