#region Namespace
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
#endregion

namespace MyIntroCs
{
    #region HelloWorldFull
    /// <summary>
    /// Hello World #1 - A minimum Revit external command.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class HelloWorldFull : Autodesk.Revit.UI.IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Autodesk.Revit.UI.TaskDialog.Show("My Dialog Title", "Hello World Full!");

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
    #endregion

    #region HelloWorldSimple
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
    #endregion

    #region HelloWordApp
    /// <summary>
    /// Hello World #3 - minimum external application.
    /// </summary>
    internal class HelloWorldApp : IExternalApplication
    {
        //OnShutdown() method is called when Revit shuts down.
        public Autodesk.Revit.UI.Result OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        //OnStartup() method is called when Revit starts up.
        public Autodesk.Revit.UI.Result OnStartup(Autodesk.Revit.UI.UIControlledApplication application)
        {
            TaskDialog.Show("My Dialog Title", "Hello World from App!");
            return Result.Succeeded;
        }

    }
    #endregion

    #region CommandData
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
    #endregion
}
