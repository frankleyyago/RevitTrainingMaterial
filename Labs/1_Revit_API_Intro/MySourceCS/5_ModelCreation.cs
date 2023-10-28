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

                CreateWalls();

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
    }
}
