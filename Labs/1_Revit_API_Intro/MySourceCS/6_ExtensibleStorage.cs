#region Namespaces
using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI.Selection;
#endregion

namespace MyIntroCs
{
    [Transaction(TransactionMode.Manual)]
    internal class ExtensibleStorage : IExternalCommand
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

            Transaction tx = new Transaction(_doc, "Extensible Storage");
            tx.Start();

            //Pick a wall.
            Reference r = _uidoc.Selection.PickObject(ObjectType.Element, new WallSelectionFilter());
            Wall w = _doc.GetElement(r) as Wall;

            tx.Commit();
            return Result.Succeeded;
        }

        #region WallSelectionFilter()
        /// <summary>
        /// Restrict selection to just select walls.
        /// </summary>
        class WallSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                return e is Wall;
            }

            public bool AllowReference(Reference r, XYZ p)
            {
                return true;
            }
        }
        #endregion
    }
}
