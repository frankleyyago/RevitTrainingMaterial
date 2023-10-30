#region Namespaces
using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
#endregion

namespace MyIntroCs
{
    [Transaction(TransactionMode.Manual)]
    internal class ModelCreation : IExternalCommand
    {
        //Member variables.
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

                //Call method to create four walls.
                CreateWalls();

                //Place a door in picked wall.
                AddDoor((Wall)ElementModification.PickedObj(_uidoc, _doc));

                //Place a window in picked wall.
                AddWindow((Wall)ElementModification.PickedObj(_uidoc, _doc));

                tx.Commit();
            }

            return Result.Succeeded;
        }

        #region  CreateWalls()
        /// <summary>
        /// Create four walls.
        /// </summary>
        /// <returns></returns>
        public List<Wall> CreateWalls()
        {
            //Dimensions of the house.
            double width = (10.0);
            double depth = (50.0);

            //Create a new level instance and assign to it the value of the filter result.
            Level level1 = (Level)ElementFiltering.FindElement(_doc, typeof(Level), "Level 1", null);

            //Make sure that exist a "Level 1" in the current doc.
            if (level1 == null)
            {
                TaskDialog.Show("Revit Intro Lab", "Cannot find (Level 1)");
                return null;
            }

            //Create a new level instance and assign to it the value of the filter result.
            Level level2 = (Level)ElementFiltering.FindElement(_doc, typeof(Level), "Level 2", null);

            //Make sure that exist a "Level 2" in the current doc.
            if (level2 == null)
            {
                TaskDialog.Show("Revit Intro Lab", "Cannot find (Level 2)");
                return null;
            }

            //Set four corner of walls.
            double dx = width * 0.5;
            double dy = depth * 0.5;

            List<XYZ> pts = new List<XYZ>(5);
            pts.Add(new XYZ(-dx, -dy, 0.0));
            pts.Add(new XYZ(dx, -dy, 0.0));
            pts.Add(new XYZ(dx, dy, 0.0));
            pts.Add(new XYZ(-dx, dy, 0.0));
            pts.Add(pts[0]);

            //Save walls created.
            List<Wall> walls = new List<Wall>(4);

            //Loop through a list of points and create four walls.
            for (int i = 0; i <= 3; i++)
            {
                //Create a line based in the current pts.
                Line baseCurve = Line.CreateBound(pts[i], pts[i + 1]);
                //Create a wall using the current baseCurve.
                Wall aWall = Wall.Create(_doc, baseCurve, level1.Id, false);
                //Set the top constrait to level 2.
                aWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
                //Save the wall.
                walls.Add(aWall);
            }

            _doc.Regenerate();
            _doc.AutoJoinElements();

            return walls;
        }
        #endregion

        #region AddDoor()
        /// <summary>
        /// Create a door in the middle of the wall.
        /// </summary>
        /// <param name="hostWall">Wall that will receive the door</param>
        public void AddDoor(Wall hostWall)
        {
            //Create strings with doors informations.
            string dFamilyName = "Single-Flush";
            string dTypeName = "30\" x 80\"";
            string dFamilyAndTypeName = $"{dFamilyName} : {dTypeName}";

            //Create a new door type.
            FamilySymbol dType = (FamilySymbol)ElementFiltering.FindFamilyType(_doc, typeof(FamilySymbol), dFamilyName, dTypeName, BuiltInCategory.OST_Doors);

            if (dType == null)
            {
                TaskDialog.Show("Revit Intro Lab", $"Cannot find {dFamilyAndTypeName}");
            }

            //Get the start and end points of the wall.
            LocationCurve locCurve = (LocationCurve)hostWall.Location;
            XYZ pt1 = locCurve.Curve.GetEndPoint(0);
            XYZ pt2 = locCurve.Curve.GetEndPoint(1);
            //Calculate the midpoint.
            XYZ pt = (pt1 + pt2) * 0.5;

            //Set the bottom of the wall and level as reference to the door.
            ElementId idLevel1 = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
            Level level1 = (Level)_doc.GetElement(idLevel1);

            //Create a door.
            FamilyInstance aDoor = _doc.Create.NewFamilyInstance(pt, dType, hostWall, level1, StructuralType.NonStructural);
        }
        #endregion

        #region AddWindow()
        /// <summary>
        /// Create a window in the middle of the wall.
        /// </summary>
        /// <param name="hostWall"></param>
        public void AddWindow(Wall hostWall)
        {
            //Create strings with windows informations.
            string wFamilyName = "Fixed";
            string wTypeName = "36\" x 48\"";
            string wFamilyAndTypeName = $"{wFamilyName} : {wTypeName}";
            double sillHeight = 3;

            //Create a new window type.
            FamilySymbol wType = (FamilySymbol)ElementFiltering.FindFamilyType(_doc, typeof(FamilySymbol), wFamilyName, wTypeName, BuiltInCategory.OST_Windows);

            if (wType == null)
            {
                TaskDialog.Show("Revit Intro Lab", $"Cannot find {wFamilyAndTypeName}");
            }

            //Get the start and end points of the wall.
            LocationCurve locCurve = (LocationCurve)hostWall.Location;
            XYZ pt1 = locCurve.Curve.GetEndPoint(0);
            XYZ pt2 = locCurve.Curve.GetEndPoint(1);
            //Calculate the midpoint.
            XYZ pt = (pt1 + pt2) * 0.5;

            //Set the bottom of the wall and level as reference to the window.
            ElementId idLevel1 = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
            Level level1 = (Level)_doc.GetElement(idLevel1);

            //Create a window.
            FamilyInstance aWindow = _doc.Create.NewFamilyInstance(pt, wType, hostWall, level1, StructuralType.NonStructural);

            //Set sill height.
            aWindow.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(sillHeight);
        }
        #endregion
    }
}
