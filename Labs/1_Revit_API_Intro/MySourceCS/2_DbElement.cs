using System;
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

            //Call a method to show parameters.
            ShowParameters(e, "Element Parameters");

            //Check type parameters.
            ElementId eTypeId = e.GetTypeId();
            ElementType eType = (ElementType)_doc.GetElement(eTypeId);
            ShowParameters(eType, "Type Parameters: ");

            //Access to each parameter and type parameter
            RetrieveParameter(e, "Element parameter (by BuiltInParameter and Name)");
            RetrieveParameter(eType, "Type parameter (by BuiltInParameter and Name)");

            //Location
            ShowLocation(e);

            return Result.Succeeded;
        }

        #region ShowBasicElementInfo()
        /// <summary>
        /// Show basic information about the given element.
        /// <param name="e"></param>
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
        #endregion

        #region IdentifyElement()
        /// <summary>
        /// Identify the type of the element.
        /// <param name="e"></param>
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
        #endregion

        #region ShowParameters()
        /// <summary>
        /// Show all paramaters values of the element.
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// </summary>
        public void ShowParameters(Element e, string h)
        {
            string s = string.Empty;

            foreach (Parameter p in e.GetOrderedParameters())
            {
                string name = p.Definition.Name;
                string val = ParameterToString(p);

                s += $"{name} = {val}\n";
            }

            TaskDialog.Show(h, s);
        }
        #endregion

        #region ParameterToString()
        /// <summary>
        /// Return a string from of the given parameter.
        /// <param name="p"></param>
        /// </summary>
        public static string ParameterToString(Parameter p)
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
                    val = sVal;
                    break;
                case StorageType.ElementId:
                    ElementId idVal = p.AsElementId();
                    val = idVal.IntegerValue.ToString();
                    break;
                case StorageType.None:
                    break;
                default:
                    break;
            }

            return val;
        }
        #endregion
        /// <summary>
        /// Retrieve a specif parameter individually.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        #region RetrieveParameter()

        public void RetrieveParameter(Element e, string h)
        {
            string s = string.Empty;

            //Get a parameter by BuiltInParameter.
            Parameter param = e.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

            if (param != null)
            {
                s += $"Comments (by BuiltInParameter) = {ParameterToString(param)}\n";
            }

            //Get a parameter by Name.
            param = e.LookupParameter("Mark");

            if (param != null )
            {
                s += $"Mark (by Name) = {ParameterToString(param)}\n";
            }

            //Again get parameter by BuiltInParameter.
            param = e.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

            if (param != null)
            {
                s += $"Type Comments (by BuiltInParameter) = {ParameterToString(param)}\n";
            }

            //Again get parameter by Name.
            param = e.LookupParameter("Fire Rating");

            if (param != null)
            {
                s += $"Fire Rating (by Name) = {ParameterToString(param)}\n";
            }

            //Using the BuiltInParameter, we can sometimes access one that is not in the parameters set.
            //This works only for element type.
            param = e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM);

            if (param != null)
            {
                s += $"SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM (only by BuiltInParameter) = {ParameterToString(param)}\n";
            }

            param = e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM);

            if (param != null)
            {
                s += $"SYMBOL_FAMILY_NAME_PARAM (only by BuiltInParameter) = {ParameterToString(param)}\n";
            }

            //Show what we get.
            TaskDialog.Show(h, s);
        }
        #endregion
        /// <summary>
        /// Show the location of an element
        /// </summary>
        /// <param name="e"></param>
        #region ShowLocation()
        public void ShowLocation(Element e)
        {
            string s = "Location Information: \n\n";
            Location loc = e.Location;

            if (loc is LocationPoint)
            {
                //In case we've a location point

                LocationPoint locPoint = (LocationPoint)loc;
                XYZ pt = locPoint.Point;
                double r = locPoint.Rotation;

                s += $"LocationPont\n";
                s += $"Point = {PointToString(pt)}\n";
                s += $"Rotation = {r.ToString()}\n";
                s += "Rotation = " + r.ToString() + "\n";
            }
            else if (loc is LocationCurve)
            {
                //In case we've a location curve

                LocationCurve locCurve = (LocationCurve)loc;
                Curve crv = locCurve.Curve;

                s += $"LocationCurve: \n";
                s += $"EndPoint (0) / Start Point = {PointToString(crv.GetEndPoint(0))}\n";
                s += $"EndPoint (1) / End Point = {PointToString(crv.GetEndPoint(1))}\n";
                s += $"Lenght = {crv.Length.ToString()}\n";

                //Location Curve also has property JoinType at the end
                s += $"JoinType(0) = {locCurve.get_JoinType(0).ToString()}\n";
                s += $"JoinType(1) = {locCurve.get_JoinType(0).ToString()}\n";

                //Show it
                TaskDialog.Show("Show Location", s);
            }
        }

        public static string PointToString(XYZ pt)
        {
            if (pt == null)
            {
                return "";
            }

            //Return a formated XYZ with only 2 decimal places (F2)
            return string.Format($"({pt.X.ToString("F2")}, {pt.Y.ToString("F2")}, {pt.Z.ToString("F2")})");
        }
        #endregion
    }
}
