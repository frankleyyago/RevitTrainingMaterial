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

                CreateHouse();

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
            double width = (25.0);
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

            //Activate symbols that are not placed in the model.
            if (!dType.IsActive)
            {
                dType.Activate();
            }

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

            //Activate symbols that are not placed in the model.
            if (!wType.IsActive)
            {
                wType.Activate();
            }

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

        #region AddRoof()
        /// <summary>
        /// Create a roof.
        /// </summary>
        /// <param name="w">walls list</param>
        public void AddRoof(List<Wall> w)
        {
            //Create string with roof informations.
            string rFamilyName = "Basic Roof";
            string rTypeName = "Generic - 9\"";
            string rFamilyAndTypeName = $"{rFamilyName} : {rTypeName}";

            //Create a new roof type.
            RoofType rType = (RoofType)ElementFiltering.FindFamilyType(_doc, typeof(RoofType), rFamilyName, rTypeName, null);

            //In case roof information is missing.
            if (rType == null)
            {
                TaskDialog.Show("Revit Intro Lab", $"Cannot find {rFamilyAndTypeName}");
            }

            //Create roof footprint.
            double wThickness = w[0].Width;
            double dt = wThickness * 3;
            List<XYZ> dts = new List<XYZ>(5);
            dts.Add(new XYZ(-dt, -dt, 0.0));
            dts.Add(new XYZ(dt, -dt, 0.0));
            dts.Add(new XYZ(dt, dt, 0.0));
            dts.Add(new XYZ(-dt, dt, 0.0));
            dts.Add(dts[0]);

            //Set the profile from four walls.
            CurveArray footPrint = new CurveArray();
            for (int i = 0; i <= 3; i++)
            {
                LocationCurve locCurve = (LocationCurve)w[i].Location;
                XYZ pt1 = locCurve.Curve.GetEndPoint(0) + dts[i];
                XYZ pt2 = locCurve.Curve.GetEndPoint(1) + dts[i + 1];
                Line line = Line.CreateBound(pt1, pt2);
                footPrint.Append(line);
            }

            //Get the level.
            ElementId idLevel2 = w[0].get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
            Level level2 = (Level)_doc.GetElement(idLevel2);

            //Footprint to model curve mapping.
            ModelCurveArray mapping = new ModelCurveArray();

            FootPrintRoof aRoof = _doc.Create.NewFootPrintRoof(footPrint, level2, rType, out mapping);

            //Setting the slope.
            foreach (ModelCurve modelCurve in mapping)
            {
                aRoof.set_DefinesSlope(modelCurve, true);
                aRoof.set_SlopeAngle(modelCurve, 0.5);
            }
        }
        #endregion

        #region CreateHouse()
        /// <summary>
        /// Create a house.
        /// </summary>
        public void CreateHouse()
        {
            //Create four walls.
            List<Wall> w = CreateWalls();

            //Create a doors in first wall.
            AddDoor(w[0]);

            //Create a windows in all other walls.
            for (int i = 1; i <= 3; i++)
            {
                AddWindow(w[i]);
            }

            //Create roof.
            AddRoof(w);
        }
        #endregion

    }
}
