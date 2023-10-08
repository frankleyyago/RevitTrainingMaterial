using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIntroCs
{
    /// <summary>
    /// Command Argument and Revit Object Model.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class CommandData : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Print out a few information that you can get from commandData
            string versionName = app.VersionName;
            string docTitle = doc.Title;
            TaskDialog.Show("Revit Informations", $"Version name: {versionName}\nDocument Title: {docTitle}");

            //Print out a list of wall types available in the current Revit project
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Autodesk.Revit.DB.WallType));

            string s = "";
            foreach (WallType wallType in collector)
            {
                s += wallType.Name + "\r\n";
            }

            //Show the result
            TaskDialog.Show("Revit walls", $"Wall Types: {s}\n");

            return Result.Succeeded;
        }
    }
}
