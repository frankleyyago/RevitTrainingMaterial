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
    }
}
