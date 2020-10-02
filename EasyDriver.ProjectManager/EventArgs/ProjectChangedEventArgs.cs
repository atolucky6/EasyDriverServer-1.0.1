using EasyDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.ProjectManager
{
    public class ProjectChangedEventArgs : EventArgs
    {
        public IEasyScadaProject OldProject { get; private set; }
        public IEasyScadaProject NewProject { get; private set; }

        public ProjectChangedEventArgs(IEasyScadaProject oldProject, IEasyScadaProject newProject)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }
    }
}
