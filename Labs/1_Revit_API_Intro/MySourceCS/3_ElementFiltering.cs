#region Namespace
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace MyIntroCs
{
    [Transaction(TransactionMode.Manual)]
    internal class ElementFiltering : IExternalCommand
    {
        //Member variables.
        Application _app;
        Document _doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get the access to the top most objects.
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            _doc = uidoc.Document;

            //Filter the document looking for element of type: WallType.
            FilteredElementCollector wCollector = new FilteredElementCollector(_doc);
            wCollector.OfClass(typeof(WallType));

            //Create a list of elements with filtered elements, in this casse walls.
            IList<Element> walls = (IList<Element>)wCollector.ToElementIds();

            //Filter the document looking for elements of type: FamilySymbol and category: OST_Doors.
            FilteredElementCollector dCollector = new FilteredElementCollector(_doc);
            dCollector.OfClass(typeof(FamilySymbol));
            dCollector.OfCategory(BuiltInCategory.OST_Doors);

            //Create a list of elements with filtered elements, in this case doors.
            IList<Element> doors = (IList<Element>)dCollector.ToElementIds();

            TaskDialog.Show("Walls and Doors", $"{walls}\n {doors}");

            return Result.Succeeded;
        }
    }
}
