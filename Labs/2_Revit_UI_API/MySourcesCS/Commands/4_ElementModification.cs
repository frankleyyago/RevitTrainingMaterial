#region Namespaces
using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace MyUiCs
{
    [Transaction(TransactionMode.Manual)]
    internal class ElementModification : IExternalCommand
    {
        //Create member variables.
        Application _app;
        Document _doc;
        UIDocument _uidoc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            _uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            _doc = _uidoc.Document;

            //Create a new transaction to commit changes to the model.
            using (Transaction tx = new Transaction(_doc, "Changing element"))
            {
                tx.Start();

                //Create a element and assign to it the picked element.
                Element e = PickedObj(_uidoc, _doc);

                //Call a method to modify wall type.
                ModifyElementPropertiesWall(e);

                //Call a method to change the location of a wall.
                ChangeLocationCurve(e);

                ModifyElementByTransformUtilsMethods(e);

                tx.Commit();
            }                

            return Result.Succeeded;
        }

        #region PickedObj()
        /// <summary>
        /// Allow the user to pick an element.
        /// </summary>
        /// <returns></returns>
        public static Element PickedObj(UIDocument uidoc, Document doc)
        {
            Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Pick a wall");
            Element e = doc.GetElement(pickedObj);

            return e;
        }
        #endregion

        #region ModifyElementPropertiesWall()
        /// <summary>
        /// Modify a picked wall to a specif wall with wTypeName and wTypeFamilyName.
        /// </summary>
        /// <param name="e">Element to be modified</param>
        public void ModifyElementPropertiesWall(Element e)
        {
            //Create some strings.
            string wFamilyName = "Basic Wall";
            string wTypeName = "Exterior - Brick on CMU";
            string wFamilyAndTypeName = $"{wFamilyName} : {wTypeName}";

            //In case the argument is not a wall, show a task dialog.
            if (!(e is Wall))
            {
                TaskDialog.Show("Revit Intro Lab", "Sorry, I only know how to modify a wall. Please select a wall.");
                return;
            }

            //Create a new Wall instance and assign to it the casted value of e.
            Wall aWall = (Wall)e;

            //Create a new Element instance and assign to it the method with the value of strings crated above.
            Element newWallType = ElementFiltering.FindFamilyType(_doc, typeof(WallType), wFamilyName, wTypeName, null);

            if (newWallType != null)
            {
                aWall.WallType = (WallType)newWallType;
                TaskDialog.Show("Wall Changed", $"Wall Type to: {wFamilyAndTypeName}");
            }
        }
        #endregion

        #region ChangeLocationCurve()
        public void ChangeLocationCurve(Element e)
        {
            LocationCurve wLocation = (LocationCurve)e.Location;

            if (e.Location is LocationCurve)
            {
                //Get the XYZ of start and end of e.
                XYZ pt1 = wLocation.Curve.GetEndPoint(0);
                XYZ pt2 = wLocation.Curve.GetEndPoint(1);

                //Create new point.
                XYZ newPt1 = new XYZ(10.0, 0.0, 0.0);
                XYZ newPt2 = new XYZ(20.0, 0.0, 0.0);

                //Create a new line bound.
                Line newWallLine = Line.CreateBound(newPt1, newPt2);

                //Change the curve.
                wLocation.Curve = newWallLine;
                TaskDialog.Show("Wall moved", $"Old position:\n{pt1}\n{pt2}\n\n\nNew position:\n{newPt1}\n{newPt2}");
            }
            else
            {
                TaskDialog.Show("Error", $"Selected wall have not a valid location");
            }
            
        }
        #endregion

        #region ModifyElementByTransformUtilsMethods()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">Element to be transformed</param>
        public void ModifyElementByTransformUtilsMethods(Element e)
        {
            //Create a new coordinate to move the e.
            XYZ newPosition = new XYZ(20.0, 20.0, 0.0);

            //Move e, in this case a picked wall.
            ElementTransformUtils.MoveElement(_doc, e.Id, newPosition);

            //Represent a point at the origin (0,0,0).
            XYZ pt1 = XYZ.Zero;
            //Represent the basis of the Z axis is the vector (0,0,1).
            XYZ pt2 = XYZ.BasisZ;
            //Create a new line from pt1 to pt2, in this case is a vertical line in z-axis.
            Line axis = Line.CreateBound(pt1, pt2);

            //Rotate e, in this case a picked wall.
            ElementTransformUtils.RotateElement(_doc, e.Id, axis, Math.PI/12.0);

            //Show the result.
            TaskDialog.Show("Modify element by utils methods", "Moved: (20, 20, 0)\nRotated: 15 degrees");
        }
        #endregion
    }
}
