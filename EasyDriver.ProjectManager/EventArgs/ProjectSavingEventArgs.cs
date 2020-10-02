using EasyDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.ProjectManager
{
    public class ProjectSavingEventArgs : EventArgs
    {
        public IEasyScadaProject OldProject { get; private set; }
        public IEasyScadaProject NewProject { get; private set; }

        public ProjectSavingEventArgs(IEasyScadaProject oldPorject, IEasyScadaProject newProject)
        {
            OldProject = oldPorject;
            NewProject = newProject;
        }
    }
}
