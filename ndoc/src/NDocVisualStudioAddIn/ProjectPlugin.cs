using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;

namespace NDocVisualStudioAddIn {
    public class ProjectPlugin : NDoc.VisualStudio.IProject {

        private DTE2 _applicationObject;
        private string _uniqueName;

        public ProjectPlugin(DTE2 _applicationObject, string uniqueName) {
            this._applicationObject = _applicationObject;
            this._uniqueName = uniqueName;
        }

        public string AssemblyName {
            get {
                EnvDTE.Project proj = this._applicationObject.Solution.Projects.Item(_uniqueName);
                return proj.Name;
            }
        }

        public NDoc.VisualStudio.IProjectConfig GetConfiguration(string configName) {
            throw new NotImplementedException();
        }

        public string GetRelativeOutputPathForConfiguration(string configName) {
            //return Path.Combine(
            //                Path.Combine(FullPath, GetConfiguration(configName).OutputPath),
            //                OutputFileName);
            return string.Empty;
        }

        public string GetRelativePathToDocumentationFile(string configName) {
            throw new NotImplementedException();
        }

        public Guid ID {
            get { throw new NotImplementedException(); }
        }

        public string Name {
            get {
                EnvDTE.Project proj = this._applicationObject.Solution.Projects.Item(_uniqueName);
                return proj.Name;
            }
        }

        public string OutputFile {
            get {
                return this.getProjectProperty(_uniqueName, "OutputFileName");
            }
        }

        public string OutputType {
            get { throw new NotImplementedException(); }
        }

        public NDoc.VisualStudio.ProjektType Type {
            get {
                EnvDTE.Project proj = this._applicationObject.Solution.Projects.Item(_uniqueName);
                return NDoc.VisualStudio.Project.GetProjectType(proj.Kind);
            }
        }

        public void Read(string path) {
            throw new NotImplementedException();
        }

        public string RelativePath {
            get {
                return this.getProjectProperty(_uniqueName, "FullPath");
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string RootNamespace {
            get {
                return this.getProjectProperty(_uniqueName, "RootNamespace");
            }
        }

        public NDoc.VisualStudio.ISolution Solution {
            get { throw new NotImplementedException(); }
        }

        private string getProjectProperty(string projectUniqueName, string key) {
            try {
                EnvDTE.Project prj = _applicationObject.Solution.Projects.Item(projectUniqueName);
                return prj.Properties.Item(key).Value.ToString();
            } catch (Exception) {
                return string.Empty;
            }
        }
    }
}
