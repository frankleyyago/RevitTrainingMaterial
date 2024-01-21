#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
#endregion

namespace MyUiCs
{
    internal class UIEvent : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }

        public Result OnStartup(UIControlledApplication application)
        {
            WindowDoorUpdater winDoorUpdater = new WindowDoorUpdater(application.ActiveAddInId);

            UpdaterRegistry.RegisterUpdater(winDoorUpdater);

            ElementClassFilter wallFilter = new ElementClassFilter(typeof(Wall));
            UpdaterRegistry.AddTrigger(winDoorUpdater.GetUpdaterId(), wallFilter, Element.GetChangeTypeGeometry());

            return Result.Succeeded;
        }
    }

    public class WindowDoorUpdater : IUpdater
    {
        UpdaterId m_updaterId = null;

        public static bool m_updateActive = false;

        public WindowDoorUpdater(AddInId id)
        {
            m_updaterId = new UpdaterId(id, new Guid("B9E701A7-D690-42AE-A647-5FE8906BEC42"));
        }

        public void Execute(UpdaterData data)
        {
            if (!m_updateActive) return;
        }

        public string GetAdditionalInformation()
        {
            return "Door/Window updater: keeps doors and windows at the center of walls.";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.DoorsOpeningsWindows;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Window/Door Updater";
        }
    }
}
