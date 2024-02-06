#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
#endregion

namespace MyFamilyCs
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ColumnRectangle : IExternalCommand
    {
        //Member variables.
        Application _app;
        Document _doc;
        UIDocument _uidoc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _app = commandData.Application.Application;
            _doc = commandData.Application.ActiveUIDocument.Document;

            if (!isRightTemplate(BuiltInCategory.OST_Columns))
            {
                Util.ErrorMsg("Please open Metric Column.rft");
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        #region isRightTemplate()
        public bool isRightTemplate(BuiltInCategory targetCategory)
        {
            if (!_doc.IsFamilyDocument)
            {
                Util.ErrorMsg("This command works only in the family editor.");
                return false;
            }

            Category cat = _doc.Settings.Categories.get_Item(targetCategory);
            if (_doc.OwnerFamily == null)
            {
                Util.ErrorMsg("This command only works in the family context.");
                return false;
            }
            if (!cat.Id.Equals(_doc.OwnerFamily.FamilyCategory.Id))
            {
                Util.ErrorMsg("Category of this family document does not match the context required by this command.");
                return false;
            }

            return true;
        }
        #endregion

        #region createProfileRectangle()
        /// <summary>
        /// Creates column's rectangular profile.
        /// </summary>
        /// <returns></returns>
        public CurveArrArray createProfileRectangle()
        {
            //Size of column's section.
            double w = mmToFeet(600.0);
            double d = mmToFeet(600.0);

            //Number of vertices.
            const int nVerts = 4;

            XYZ[] pts = new XYZ[]
            {
                new XYZ(-w / 2.0, -d / 2.0, 0.0),
                new XYZ(w / 2.0, -d / 2.0, 0.0),
                new XYZ(w / 2.0, d / 2.0, 0.0),
                new XYZ(-w / 2.0, d / 2.0, 0.0),
                new XYZ(-w / 2.0, -d / 2.0, 0.0)
            };

            //Define a loop.
            CurveArray pLoop = _app.Create.NewCurveArray();
            for (int i = 0; i < nVerts; i++)
            {
                Line line = Line.CreateBound(pts[i], pts[i + 1]);
                pLoop.Append(line);
            }

            //Put the loop  in the curveArrArray ass a profile.
            CurveArrArray pProfile = _app.Create.NewCurveArrArray();

            return pProfile;
        }
        #endregion

        #region createSolid()
        public Extrusion createSolid()
        {
            CurveArrArray pProfile = createProfileRectangle();
            
            ReferencePlane pRefPlane = findElement(typeof(ReferencePlane), "Reference Plane") as ReferencePlane;
            SketchPlane pSketchPlane = SketchPlane.Create(_doc, pRefPlane.Plane);
        }
        #region

        #region helperFunctions

        #region mmToFeet()
        /// <summary>
        /// Converts milimeter to feet.
        /// </summary>
        /// <param name="mmVal"></param>
        /// <returns></returns>
        double mmToFeet(double mmVal)
        {
            return mmVal / 304.8;
        }
        #endregion

        #region findElement()
        /// <summary>
        /// Find an element of the figen type and name.
        /// </summary>
        /// <param name="targetType">Type of target to be find.</param>
        /// <param name="targetName">Name of target to be find.</param>
        /// <returns></returns>
        public Element findElement(Type targetType, string targetName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(_doc);
            collector.WherePasses(new ElementClassFilter(targetType));

            var targetElems = from element in collector where element.Name.Equals(targetName) select element;
            List<Element> elems = targetElems.ToList<Element>();

            if (elems.Count > 0 )
            {
                return elems[0];
            }

            return null;
        }
        #endregion

        #endregion
    }
}
