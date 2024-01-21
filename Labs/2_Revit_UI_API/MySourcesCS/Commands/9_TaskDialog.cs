﻿#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using System.Collections;
#endregion

namespace MyUiCs
{
    [Transaction(TransactionMode.Manual)]
    internal class UITaskDialog : IExternalCommand
    {
        //Member variables.
        UIApplication _uiapp;
        UIDocument _uidoc;
        Document _doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _uiapp = commandData.Application;
            _uidoc = _uiapp.ActiveUIDocument;
            _doc = _uidoc.Document;

            //(1) Shows a simple task dialog box.
            TaskDialog.Show("Task Dialog Static 1", "Main message");

            //(2) Adding [Yes] [No] [Cancel] buttons.
            TaskDialogResult tdr = default(TaskDialogResult);
            tdr = TaskDialog.Show("Task Dialog Static 2", "Main message", (TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No | TaskDialogCommonButtons.Cancel));

            return Result.Succeeded;
        }
    }
}