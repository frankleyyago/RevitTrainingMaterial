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
    internal class Selection : IExternalCommand
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

            ////Show a pre selected element list.
            //ICollection<ElementId> selectedElementIds = _uidoc.Selection.GetElementIds();
            //ShowElementList(selectedElementIds, "Pre-selection: ");

            ////Show info of selected element.
            //PickMethod_PickObject();

            ////Show info of several selected elements.
            //PickMethod_PickObjects();

            ////Show info of elements selected by rectangle.
            //PickMethod_PickElementByRectangle();

            ////Show coordinates of a picked point.
            //PickMethod_PickPoint();

            //Show coordinates of a picked point on a element.
            PickMethod_PointOnElement();

            return Result.Succeeded;
        }

        #region ShowElementList()
        /// <summary>
        /// Show a list of pre selected elements
        /// </summary>
        /// <param name="eIds">Elements Id</param>
        /// <param name="h">Header</param>
        public void ShowElementList(IEnumerable eIds, string h)
        {
            string s = "\n\n - Class - Category - Name (or Family: Type Name) - Id - \n\n";
            int count = 0;

            foreach (ElementId eId in eIds)
            {
                count++;
                Element e = _uidoc.Document.GetElement(eId);
                s += ElementToString(e);
            }

            s = $"{h} ({count}) {s}";
            TaskDialog.Show("Revit UI Lab", s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">Element</param>
        /// <returns></returns>
        private string ElementToString(Element e)
        {
            if (e == null)
            {
                return "none";
            }

            string name = "";

            if (e is ElementType)
            {
                Parameter p = e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM);
                if (p != null)
                {
                    name = p.AsString();
                }
            }
            else
            {
                name = e.Name;
            }

            return $"{e.GetType().Name}; {e.Category.Name}; {name}; {e.Id.IntegerValue}\r\n";
        }

        /// <summary>
        /// Show coordinates of a point.
        /// </summary>
        /// <param name="pt">Point</param>
        /// <returns></returns>
        public static string PointToString(XYZ pt)
        {
            if(pt == null )
            {
                return "";
            }

            return $"({pt.X.ToString("F2")}, {pt.Y.ToString("F2")}, {pt.Z.ToString("F2")})";
        }
        #endregion

        #region PickMethod_PickObject()
        /// <summary>
        /// Show basic selected element info.
        /// </summary>
        public void PickMethod_PickObject()
        {
            Reference r = _uidoc.Selection.PickObject(ObjectType.Element, "Select one element");

            Element e = _uidoc.Document.GetElement(r);

            DbElement.ShowBasicElementInfo(_doc, e);
        }
        #endregion

        #region PickMethod_PickObjects()
        /// <summary>
        /// Prompt the user to select multiples elements.
        /// </summary>
        public void PickMethod_PickObjects()
        {
            IList<Reference> refs = _uidoc.Selection.PickObjects(ObjectType.Element, "Select multiples elements");

            IList<ElementId> eIds = new List<ElementId>();
            foreach (Reference r in refs)
            {
                eIds.Add(r.ElementId);
            }

            ShowElementList(eIds, "Pick Objects");
        }
        #endregion

        #region PickeMethod_PickElementByRectangle()
        /// <summary>
        /// Prompt the user to select by drawing rectangle.
        /// </summary>
        public void PickMethod_PickElementByRectangle()
        {
            IList<Element> e = _uidoc.Selection.PickElementsByRectangle("Draw a rectangle");
            IList<ElementId> eIds = new List<ElementId>();
            foreach (Element el in e)
            {
                eIds.Add(el.Id);
            }

            ShowElementList(eIds, "Pick By Rectangle: ");
        }
        #endregion

        #region PickMethod_PickPoint()
        public void PickMethod_PickPoint()
        {
            XYZ pt = _uidoc.Selection.PickPoint("Pick a point");

            string msg = "Pick Point: ";
            msg += PointToString(pt);

            TaskDialog.Show("PickPoint", msg);
        }
        #endregion

        #region PickMethod_PointOnElement()
        /// <summary>
        /// Prompts the user to pick a point on a element.
        /// </summary>
        public void PickMethod_PointOnElement()
        {
            Reference r = _uidoc.Selection.PickObject(ObjectType.PointOnElement, "Select a point on element");
            Element e = _uidoc.Document.GetElement(r);
            XYZ pt = r.GlobalPoint;

            string msg = "";
            if (pt != null)
            {
                msg = $"You picked the point {PointToString(pt)} on an element {e.Id.ToString()} \r\n";
            }
            else
            {
                msg = "no Point picked \n";
            }

            TaskDialog.Show("PickPointOnElement", msg);
        }
        #endregion
    }
}
