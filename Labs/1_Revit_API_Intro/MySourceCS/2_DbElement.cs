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
    /// <summary>
    /// DB Element - Retrieve element infos.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class DbElement : IExternalCommand
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
            
            //Pick an element on a screen.
            Reference r = uidoc.Selection.PickObject(ObjectType.Element, "Pick an element");

            //Get an element
            Element e = _doc.GetElement(r);

            //Call a method to show infos.
            ShowBasicElementInfo(e);

            //Call a method to identify element type.
            IdentifyElement(e);

            //Call a method to show parameters
            ShowParameters(e, "Element Parameters");

            ElementId eTypeId = e.GetTypeId();
            ElementType eType = (ElementType)_doc.GetElement(eTypeId);
            ShowParameters(eType, "Type Parameters");

            return Result.Succeeded;
        }

        /// <summary>
        /// Show basic information about the given element.
        /// </summary>
        public void ShowBasicElementInfo(Element e)
        {
            //Element infos
            string s = "You picked:\n";

            s += $"  Class name = {e.GetType().Name}\n";
            s += $"  Category = {e.Category.Name}\n";
            s += $"  Element id = {e.Id.ToString()}\n\n";

            //Type info
            ElementId elemTypeId = e.GetTypeId();
            ElementType elemType = (ElementType)_doc.GetElement(elemTypeId);

            s += $"It's ElementType:\n";
            s += $"  Class name = {elemType.GetType().Name}\n";
            s += $"  Category = {elemType.Category.Name}\n";
            s += $"  Element type id = {elemType.Id.ToString()}\n";

            TaskDialog.Show("Basic Element Info", s);
        }

        /// <summary>
        /// Identify the type of the element.
        /// </summary>
        public void IdentifyElement(Element e)
        {
            //An instance of a system family has a designated class
            //We can use this to identify the type of element
            //e.g., walls, floors, roofs
            string s = " ";
            if (e is Wall)
            {
                s = "Wall";
            }
            else if (e is Floor)
            {
                s = "Floor";
            }
            else if (e is RoofBase)
            {
                s = "Roof";
            }
            else if (e is FamilyInstance)
            {
                //An instance of a component family is all FamilyInstance
                //We'll need to further check it's category
                //e.g., doors, windows, furniture
                if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors)
                {
                    s = "Doors";
                }
                else if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Windows)
                {
                    s = "Windows";
                }
                else if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Furniture)
                {
                    s = "Furniture";
                }
                else
                {
                    //e.g., plant
                    s = "Component family instance";
                }
            }
            else if (e is HostObject)
            {
                //Check the base class
                //e.g, CeilingAndFloor
                s = "System family instance";
            }
            else
            {
                s = "Other";
            }

            s = $"You have picked: {s}";

            TaskDialog.Show("Identify Element", s);
        }

        /// <summary>
        /// Show all paramaters values of the element.
        /// </summary>
        public void ShowParameters(Element e, string header)
        {
            string s = string.Empty;

            foreach (Parameter p in e.GetOrderedParameters())
            {
                string name = p.Definition.Name;
                string val = ParameterToString(p);

                s += $"{name} = {val}\n";
            }

            TaskDialog.Show(header, s);
        }

        /// <summary>
        /// Return a string from of the given parameter.
        /// </summary>
        private string ParameterToString(Parameter p)
        {
            string val = "none";

            if (p == null)
            {
                return val;
            }

            switch (p.StorageType)
            {
                case StorageType.Double:
                    double dVal = p.AsDouble();
                    val = dVal.ToString();
                    break;
                case StorageType.Integer:
                    int iVal = p.AsInteger();
                    val = iVal.ToString();
                    break;
                case StorageType.String:
                    string sVal = p.AsString();
                    val = sVal.ToString();
                    break;
                case StorageType.ElementId:
                    ElementId eVal = p.AsElementId();
                    val = eVal.IntegerValue.ToString();
                    break;
                case StorageType.None:
                    break;
                default:
                    break;
            }

            return val;
        }
    }
}
