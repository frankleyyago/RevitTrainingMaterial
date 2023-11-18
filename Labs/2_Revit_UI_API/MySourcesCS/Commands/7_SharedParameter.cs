#region Namespaces
using System;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using System.Windows.Controls;
#endregion

namespace MyUiCs
{
    [Transaction(TransactionMode.Manual)]
    internal class SharedParameter : IExternalCommand
    {
        //Member variables.
        Application _app;
        Document _doc;
        UIDocument _uidoc;

        const string kSharedParamsGroupAPI = "API Parameters";
        const string kSharedParamsDefFireRating = "API FireRating";
        const string kSharedParamsPath = "C:\\temp\\SharedParams.txt";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            _uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            _doc = _uidoc.Document;

            //Get the current shared params definition file.
            DefinitionFile sharedParamsFile = GetSharedParamsFile(_app);

            if (sharedParamsFile == null)
            {
                message = "Error getting the shared params file.";
                return Result.Failed;
            }

            //Get or create the shared params group.
            DefinitionGroup sharedParamsGroup = GetOrCreateSharedParamsGroup(sharedParamsFile, kSharedParamsGroupAPI);

            if (sharedParamsGroup == null)
            {
                message = "Error getting the shared params file.";
                return Result.Failed;
            }

            Category cat = _doc.Settings.Categories.get_Item(BuiltInCategory.OST_Doors);
            bool visible = cat.AllowsBoundParameters;

            //Get or create the shared params definition.
            ForgeTypeId forgeTypeId = new ForgeTypeId(SpecTypeId.Number.ToString());
            Definition fireRatingParamDef = GetOrCreateSharedParamsDefinition(sharedParamsGroup, forgeTypeId, kSharedParamsDefFireRating, visible);

            if (fireRatingParamDef == null)
            {
                message = "Error in creating shared parameter.";
                return Result.Failed;
            }

            CategorySet catSet = _app.Create.NewCategorySet();
            try
            {
                catSet.Insert(cat);
            }
            catch (Exception)
            {
                message = string.Format(
                "Error adding '{0}' category to parameters binding set.",
                cat.Name);
                return Result.Failed;
            }
            // Bind the param
            try
            {
                Binding binding = _app.Create.NewInstanceBinding(catSet);
                // We could check if already bound, but looks like Insert will 
                // just ignore it in such case
                _doc.ParameterBindings.Insert(fireRatingParamDef, binding);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        #region GetSharedParamsFile()
        /// <summary>
        /// Create new shared params.
        /// </summary>
        /// <param name="app">Application</param>
        /// <returns></returns>
        public static DefinitionFile GetSharedParamsFile(Application app)
        {
            //Get current shared params file name.
            string sharedParamsFileName;

            try
            {
                sharedParamsFileName = app.SharedParametersFilename;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Get shared params file", $"No shared params file set: {ex.Message}");
                return null;
            }

            if (sharedParamsFileName.Length == 0 || !System.IO.File.Exists(sharedParamsFileName))
            {
                StreamWriter stream;
                stream = new StreamWriter(kSharedParamsPath);
                stream.Close();
                app.SharedParametersFilename = kSharedParamsPath;
                sharedParamsFileName = app.SharedParametersFilename;
            }

            //Get the current file object and return it.
            DefinitionFile sharedParametersFile;

            try
            {
                sharedParametersFile = app.OpenSharedParameterFile();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Get shared params file", $"Cannot open shared params file: {ex.Message}");
                sharedParametersFile = null;
            }

            return sharedParametersFile;
        }
        #endregion

        #region GetOrCreateSharedParamsGroup()
        /// <summary>
        /// Creates a specific parameter group.
        /// </summary>
        /// <param name="sharedParametersFile"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static DefinitionGroup GetOrCreateSharedParamsGroup(DefinitionFile sharedParametersFile, string groupName)
        {
            DefinitionGroup g = sharedParametersFile.Groups.get_Item(groupName);

            if (g == null)
            {
                try
                {
                    g = sharedParametersFile.Groups.Create(groupName);
                }
                catch (Exception)
                {
                    g = null;
                }
            }

            return g;
        }
        #endregion

        #region GetOrCreateSharedParamsDefinition()
        /// <summary>
        /// Checks if a given parameter definition already exists.
        /// </summary>
        /// <param name="defGroup"></param>
        /// <param name="forgeTypeId"></param>
        /// <param name="defName"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public static Definition GetOrCreateSharedParamsDefinition(DefinitionGroup defGroup, ForgeTypeId forgeTypeId, string defName, bool visible)
        {
            Definition definition = defGroup.Definitions.get_Item(defName);

            if (definition == null)
            {
                try
                {
                    ExternalDefinitionCreationOptions opt = new ExternalDefinitionCreationOptions(defName, forgeTypeId);
                    opt.Visible = true;
                    definition = defGroup.Definitions.Create(opt);
                }
                catch (Exception)
                {
                    definition = null;
                }
            }

            return definition;
        }
        #endregion
    }
}
