﻿#region Namespace
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace MyUiCs
{
    [Transaction(TransactionMode.Manual)]
    internal class ElementFiltering : IExternalCommand
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

            ////Call the method.
            //ListFamilyTypes();

            ////Call the method with this given parameters.
            //FindFamilyType_Wall_v1("Basic Wall", "Generic - 8\"");
            //FindFamilyType_Wall_v2("Basic Wall", "Generic - 8\"");
            //FindFamilyType_Door_v1("Single-Flush", "36\" x 84\"");

            ////Create a ElementType of type WallType and assign to it the value of the return of the method.
            //ElementType wType = (ElementType)FindFamilyType(_doc, typeof(WallType), "Basic Wall", "Generic - 8\"", null);
            ////Create a TaskDialog to show the value of the element.
            //TaskDialog.Show("Wall", $"Name: {wType.Name}\nCategory: {wType.Category.Name}\nFamily: {wType.FamilyName}\nType Id: {wType.Id.IntegerValue.ToString()}");

            ////Create a ElementType of type FamilySymbol and assign to it the value of the return of the method.
            //ElementType dType = (ElementType)FindFamilyType(_doc, typeof(FamilySymbol), "Single-Flush", "36\" x 84\"", null);
            ////Create a TaskDialog to show the value of the element.
            //TaskDialog.Show("Door", $"Name: {dType.Name}\nCategory: {dType.Category.Name}\nFamily: {dType.FamilyName}\nType Id: {dType.Id.IntegerValue.ToString()}");

            //ElementId idWallType = wType.Id;
            //IList<Element> walls = FindInstancesOfType(typeof(Wall), idWallType, null);
            //ShowElementList(walls, "Wall");

            //ElementId idDoorType = dType.Id;
            //IList<Element> doors = FindInstancesOfType(typeof(FamilyInstance), idDoorType, BuiltInCategory.OST_Doors);
            //ShowElementList(doors, "Door");

            //Create a new level and assign to it a level with the given properties.
            Level level1 = (Level)FindElement(_doc, typeof(Level), "Level 1", null);
            //Access and show it's proprieties.
            ElementInfos(_doc, typeof(Level), "Level 1", null, level1);

            return Result.Succeeded;
        }

        #region ListFamilyType()
        /// <summary>
        /// Filter and list elements (walls and doors).
        /// </summary>
        public void ListFamilyTypes()
        {

            //Filter the document looking for element of type: WallType.
            FilteredElementCollector wCollector = new FilteredElementCollector(_doc)
                .WherePasses(new ElementClassFilter(typeof(WallType)));

            //Create a list of elements with filtered elements, in this casse walls.
            IList<Element> wallTypes = wCollector.ToElements();

            //Filter the document looking for elements of type: FamilySymbol and category: OST_Doors.
            FilteredElementCollector dCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors);

            //Create a list of elements with filtered elements, in this case doors.
            IList<Element> doorTypes = dCollector.ToElements();

            //Filter the document looking for elements of type: OST_Window.
            FilteredElementCollector wiCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows);

            //Create a list of elements with filtered elements, in this case windows.
            IList<Element> windowTypes = wiCollector.ToElements();

            //Filter the document looking for elements that are places in the model of type: OST_Doors.
            FilteredElementCollector doorCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_Doors);

            //Create a list of elements with filtered elements, in this case doors.
            IList<Element> doorInstances = doorCollector.ToElements();

            ShowElementList(wallTypes, "Walls types (by filter): ");
            ShowElementList(doorTypes, "Doors types (by filter): ");
            ShowElementList(windowTypes, "Windows types (by filter): ");
            ShowElementList(doorInstances, "Door Instances (by filter): ");            
        }

        /// <summary>
        /// Helper function 1: Show a list of elements (walls and doors).
        /// </summary>
        /// <param name="e">List of elements to be shown</param>
        /// <param name="h">Header of task dialog</param>
        public void ShowElementList(IList<Element> e, string h)
        {
            string s = " - Class - Category - Name (or Family: Type Name) - Id - \r\n";
            int num = 1;
            foreach (Element el in e)
            {
                s += $"{num} - {ElementToString(el)}";
                num++;
            }

            TaskDialog.Show($"{h} ({e.Count.ToString()}):", s);
        }

        /// <summary>
        /// Helper function 2: Summarize elements informatin as a line of text.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string ElementToString(Element e)
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

            return $"{e.GetType().Name}; {e.Category.Name}; {name}; {e.Id.IntegerValue.ToString()}\r\n";
        }
        #endregion

        #region FindFamilyType_Wall_v1()
        /// <summary>
        /// Find a specific family type for a wall with a given family and type name. Use LINQ.
        /// </summary>
        /// <param name="wFamilyName">Family name of the desired wall</param>
        /// <param name="wTypeName">Type name of the desired wall</param>
        /// <returns></returns>
        public Element FindFamilyType_Wall_v1(string wFamilyName, string wTypeName)
        {
            //Narrow down a collector by class.
            FilteredElementCollector wCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(WallType));

            //Use LINQ query.
            var wTypeElems =
                from e in wCollector
                where e.Name.Equals(wTypeName) &&
                e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM)
                .AsString().Equals(wFamilyName)
                select e;

            //Get the result.
            Element wType = null;

            //Create a list of elements to get the result of the query.
            IList<Element> wTypeList = wTypeElems.ToList();
            if (wTypeList.Count > 0)
            {
                wType = wTypeList[0];
                TaskDialog.Show("Wall", "Yeah, I found it!");
            }
            else
            {
                TaskDialog.Show("Wall", "Sorry, I don't found it!");
            }

            return wType;
        }
        #endregion

        #region FindFamilyType_Wall_v2()
        /// <summary>
        /// Find a specific family type for a wall with a given family and type name. Use iteration.
        /// </summary>
        /// <param name="wFamilyName">Family name of the desired wall</param>
        /// <param name="wTypeName">Type name of the desired wall</param>
        /// <returns></returns>
        public Element FindFamilyType_Wall_v2(string wFamilyName, string wTypeName)
        {
            //Narrow down the collector by class.
            FilteredElementCollector wCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(WallType));

            //Use iterator.
            FilteredElementIterator wTypeItr = wCollector.GetElementIterator();
            wTypeItr.Reset();

            Element wType = null;

            while (wTypeItr.MoveNext())
            {
                WallType wallType = (WallType)wTypeItr.Current;
                //Check two names: type name and family name.
                if ((wallType.Name == wTypeName) && (wallType.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(wFamilyName)))
                { 
                    wType = wallType;
                    TaskDialog.Show("Wall", "Yeah, I found it!");
                    break;
                }
                else
                {
                    TaskDialog.Show("Wall", "Sorry, I don't found it!");
                    break;
                }
            }

            return wType;
        }
        #endregion

        #region FindFamilyType_Door_v1()
        public Element FindFamilyType_Door_v1(string dFamilyName, string dTypeName)
        {
            //Narrow down the collection with class and category.
            FilteredElementCollector dCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors);

            //Use LINQ query.
            var dTypeElems =
                from e in dCollector
                where e.Name.Equals(dTypeName) &&
                e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM)
                .AsString().Equals(dFamilyName)
                select e;

            //Get the result.
            Element dType = null;

            //Create a list of elements to get the result of the query.
            IList<Element> dTypeList = dTypeElems.ToList();
            if (dTypeList.Count > 0)
            {
                dType = dTypeList[0];
                TaskDialog.Show("Door", "Yeah, I found it!");
            }
            else
            {
                TaskDialog.Show("Door", "Sorry, I don't found it!");
            }

            return dType;
        }
        #endregion

        #region FindFamilyType_Door_v2()
        public Element FindFamilyType_Door_v2(string dFamilyName, string dTypeName)
        {
            //(1) Find the family with the given name.

            //Narrow down the collector by class.
            FilteredElementCollector dCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(Family));

            //Use iterator.
            Family dFamily = null;
            FilteredElementIterator familyItr = dCollector.GetElementIterator();

            while ((familyItr.MoveNext()))
            {
                Family fam = (Family) familyItr.Current;
                //Check name and category.
                if ((fam.Name == dFamilyName) & (fam.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_Doors))
                {
                    dFamily = fam;
                    break;
                }
            }

            //(2) Find the type with the given name.

            Element dType = null;
            if (dFamily != null)
            {
                ISet<ElementId> familySymbolIds = dFamily.GetFamilySymbolIds();

                if (familySymbolIds.Count > 0)
                {
                    //Get family symbols which is contained in this family.
                    foreach (ElementId id in familySymbolIds)
                    {
                        FamilySymbol doorType = dFamily.Document.GetElement(id)
                            as FamilySymbol;
                        if ((doorType.Name == dTypeName))
                        {
                            dType = doorType;
                            break;
                        }
                    }
                }
            }

            return dType;
        }
        #endregion

        #region FindFamilyType()
        /// <summary>
        /// Find an element (target) of the given type, name and category (optional).
        /// </summary>
        /// <param name="doc">Current document</param>
        /// <param name="tType">Target type</param>
        /// <param name="tFamilyName">Target family name</param>
        /// <param name="tTypeName">Target type name</param>
        /// <param name="tCategory">Target type name</param>
        /// <returns></returns>
        public static Element FindFamilyType(Document doc, Type tType, string tFamilyName, string tTypeName, Nullable<BuiltInCategory> tCategory)
        {
            //Narrow down the collector.
            FilteredElementCollector tColletor = new FilteredElementCollector(doc)
                .OfClass(tType);

            //If you pass value to tCategory, then it'll narrow down even more the collector.
            if (tCategory.HasValue)
            {
                tColletor.OfCategory(tCategory.Value);
            }

            //Use LINQ query.
            var tElem =
                from e in tColletor
                where e.Name.Equals(tTypeName) &&
                e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM)
                .AsString().Equals(tFamilyName)
                select e;

            //Create a list of elements to get the result of the query.
            IList<Element> elems = tElem.ToList();

            //Return the result.
            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }
        #endregion

        #region FindInstancesOfType()
        /// <summary>
        /// Find an instance element (target) of the given type, name and category (optional).
        /// </summary>
        /// <param name="tType">Target type</param>
        /// <param name="idType">Target id type</param>
        /// <param name="tCategory">Target type name</param>
        /// <returns></returns>
        public IList<Element> FindInstancesOfType(Type tType, ElementId idType, Nullable<BuiltInCategory> tCategory = null)
        {
            //Narrow down the collector.
            FilteredElementCollector tCollector = new FilteredElementCollector(_doc)
                .OfClass(tType);

            //If you pass value to tCategory, then it'll narrow down even more the collector.
            if (tCategory.HasValue)
            {
                tCollector.OfCategory(tCategory.Value);
            }

            //Use LINQ query.
            var tElems =
                from e in tCollector
                where e.get_Parameter(BuiltInParameter.SYMBOL_ID_PARAM)
                .AsElementId().Equals(idType)
                select e;

            IList<Element> elems = tElems.ToList();

            return elems;
        }
        #endregion

        #region FindElements()
        /// <summary>
        /// Find a list of elements with the given class, name and category (optional).
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="tType">Target type</param>
        /// <param name="tName">Target name</param>
        /// <param name="tCategory">Target category (optional)</param>
        /// <returns></returns>
        public static IList<Element> FindElements(Document doc, Type tType, string tName, Nullable<BuiltInCategory> tCategory)
        {
            //Narrow down the collector.
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(tType);
            if (tCategory.HasValue)
            {
                collector.OfCategory(tCategory.Value);
            }

            //Use LINQ query.
            var tElems =
                from e in collector
                where e.Name.Equals(tName)
                select e;

            return tElems.ToList();
        }

        /// <summary>
        /// Helper function: searches elements with given class, name and category (optional)
        /// and return the first element found.
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="tType">Target type</param>
        /// <param name="tName">Target name</param>
        /// <param name="tCategory">Target category (optional)</param>
        /// <returns></returns>
        public static Element FindElement(Document doc, Type tType, string tName, Nullable<BuiltInCategory> tCategory)
        {
            //Find a list  of elements using the overloaded method.
            IList<Element> e = FindElements(doc, tType, tName, tCategory);

            //Return the first one from the result.
            if (e.Count > 0)
            {
                return e[0];
            }

            return null;
        }

        /// <summary>
        /// Show elements info: Name, VersionGUID, Id and Design Option
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="tType">Target type</param>
        /// <param name="tName">Target name</param>
        /// <param name="tCategory">Target category (optional)</param>
        /// <param name="tLevel">Target level</param>
        public static void ElementInfos(Document doc, Type tType, string tName, Nullable<BuiltInCategory> tCategory, Level tLevel)
        {
            tLevel = (Level)FindElement(doc, typeof(Level), tName, null);

            TaskDialog.Show("Title", $"Name: {tLevel.Name}\nGUID: {tLevel.VersionGuid}\nId: {tLevel.Id}\nSomething: {tLevel.DesignOption}");
        }
        #endregion
    }
}
