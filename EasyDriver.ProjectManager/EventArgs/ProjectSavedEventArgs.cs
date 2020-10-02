using EasyDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.ProjectManager
{
    public class ProjectSavedEventArgs : EventArgs
    {
        public IEasyScadaProject OldProject { get; private set; }
        public IEasyScadaProject NewProject { get; private set; }

        public ProjectSavedEventArgs(IEasyScadaProject oldPorject, IEasyScadaProject newProject)
        {
            OldProject = oldPorject;
            NewProject = newProject;
        }
    }
}
