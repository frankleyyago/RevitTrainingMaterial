#region Namespaces
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

            //(0) Shows a simple task dialog box.
            TaskDialog.Show("Task Dialog Static 1", "Main message");

            //(1) Adding [Yes] [No] [Cancel] buttons.
            TaskDialogResult tdr = default(TaskDialogResult);
            tdr = TaskDialog.Show("Task Dialog Static 2", "Main message", (TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No | TaskDialogCommonButtons.Cancel));

            //(2) Sets a default button.
            TaskDialogResult tdr2 = default(TaskDialogResult);
            TaskDialogResult defaultButton = TaskDialogResult.No;
            tdr2 = TaskDialog.Show("Task Dialog Static 3", "Main message", (TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No|TaskDialogCommonButtons.Cancel), defaultButton);

            InstanceOfTaskDialog();

            return Result.Succeeded;
        }

        public void InstanceOfTaskDialog()
        {
            //(0) Create an instance of task dialog to set more options.
            TaskDialog myTaskDialog = new TaskDialog("Revit UI Labs - Task Dialog Options");

            //(1) Set icon to the top portin.
            myTaskDialog.MainIcon = TaskDialogIcon.TaskDialogIconWarning;

            //(2) Set main instruction.
            myTaskDialog.MainInstruction = "Main instruction: This is Revit UI Lab 3 Task Dialog";

            //(3) Set main content.
            myTaskDialog.MainContent = "Main content: You can add detailed description here.";

            //(4) Set the button area.
            myTaskDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No | TaskDialogCommonButtons.Cancel;
            myTaskDialog.DefaultButton = TaskDialogResult.Yes;

            //(5) Set expanded content.
            myTaskDialog.ExpandedContent = "Expanded content: The visibility of this portion is controled by Show/Hide button";

            //(6) Set verification message.
            myTaskDialog.VerificationText = "Verification: Do not show this message again.";

            //(7) Set footer text.
            myTaskDialog.FooterText = "Access: <a href=\"https://www.autodesk.com/developrevit\">Revit Developer Center</a>";

            //(8) Add commands links.
            myTaskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Command Link 1", "Description 1");
            myTaskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Command Link 2", "Description 2");
            myTaskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "Command Link 3", "Description 3");
            myTaskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, "Command Link 4", "Description 4");

            //(9) Assign link to the CommandLink4.
            TaskDialogResult res = myTaskDialog.Show();
            if (TaskDialogResult.CommandLink4 == res)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "https://google.com";
                process.Start();
            }
        }
    }
}
