using System;
using System.Collections.Generic;
using System.Text;

namespace NDocVisualStudioAddIn
{
    public class SolutionPlugin : NDoc.VisualStudio.ISolution
    {
        public string Directory
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<NDoc.VisualStudio.ConfigurationElements> GetConfigurations()
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetConfigurationsNames()
        {
            throw new NotImplementedException();
        }

        public NDoc.VisualStudio.IProject GetProject(string name)
        {
            throw new NotImplementedException();
        }

        public string GetProjectConfigName(string solutionConfig, string projectId)
        {
            throw new NotImplementedException();
        }

        public ICollection<NDoc.VisualStudio.IProject> GetProjects()
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public NDoc.VisualStudio.IdeType Ide
        {
            get { throw new NotImplementedException(); }
        }

        public int ProjectCount
        {
            get { throw new NotImplementedException(); }
        }
    }
}
