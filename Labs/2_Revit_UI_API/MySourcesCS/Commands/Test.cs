#region Namespace
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System.IO;
#endregion

namespace MyIntroCs
{
    #region HelloWorldFull
    /// <summary>
    /// Hello World #1 - A minimum Revit external command.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Test : Autodesk.Revit.UI.IExternalCommand
    {
        const string _introLabName = "MyIntroCs";
        const string _uiLabName = "MyUiCs";
        const string _dllExtension = ".dll";
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Autodesk.Revit.UI.TaskDialog.Show("My Dialog Title", $"{dir}\\{_introLabName}\\{_introLabName}{_dllExtension}");


            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
    #endregion
}
