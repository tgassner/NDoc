using System;
using System.Collections.Generic;
using System.Text;

namespace NDocVisualStudioAddIn
{
    public class ProjectPlugin : NDoc.VisualStudio.IProject{

        public string AssemblyName
        {
            get { throw new NotImplementedException(); }
        }

        public NDoc.VisualStudio.IProjectConfig GetConfiguration(string configName)
        {
            throw new NotImplementedException();
        }

        public string GetRelativeOutputPathForConfiguration(string configName)
        {
            throw new NotImplementedException();
        }

        public string GetRelativePathToDocumentationFile(string configName)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string OutputFile
        {
            get { throw new NotImplementedException(); }
        }

        public string OutputType
        {
            get { throw new NotImplementedException(); }
        }

        public NDoc.VisualStudio.ProjektType Type
        {
            get { throw new NotImplementedException(); }
        }

        public void Read(string path)
        {
            throw new NotImplementedException();
        }

        public string RelativePath
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string RootNamespace
        {
            get { throw new NotImplementedException(); }
        }

        public NDoc.VisualStudio.ISolution Solution
        {
            get { throw new NotImplementedException(); }
        }
    }
}
