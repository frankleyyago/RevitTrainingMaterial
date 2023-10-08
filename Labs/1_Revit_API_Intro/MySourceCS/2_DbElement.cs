using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace MyIntroCs
{
    #region DbElement
    /// <summary>
    /// DB Element - Retrieve element infos
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class DbElement : IExternalCommand
    {
        //Member variables
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get the access to the top most objects
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;

            //Pick an element on a screen
            Reference pickedObj = uidoc.Selection.PickObject(ObjectType.Element, "Pick an element");

            //Get an element
            Element elem = doc.GetElement(pickedObj);

            //Call a method to show infos
            ShowBasicElementInfo(elem);

            return Result.Succeeded;
        }

        //Method to retrieve some elements info
        public void ShowBasicElementInfo(Element elem)
        {
            //Element infos
            string s = "You picked:\n";

            s += $"  Class name = {elem.GetType().Name}\n";
            s += $"  Category = {elem.Category.Name}\n";
            s += $"  Element id = {elem.Id.ToString()}\n\n";

            //Type info
            ElementId elemTypeId = elem.GetTypeId();
            ElementType elemType = (ElementType)doc.GetElement(elemTypeId);

            s += $"It's ElementType:\n";
            s += $"  Class name = {elemType.GetType().Name}\n";
            s += $"  Category = {elemType.Category.Name}\n";
            s += $"  Element type id = {elemType.Id.ToString()}\n";

            TaskDialog.Show("Basic Element Info", s);
        }        
    }
    #endregion
}
