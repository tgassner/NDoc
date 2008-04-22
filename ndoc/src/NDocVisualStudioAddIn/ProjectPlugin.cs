using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;

namespace NDocVisualStudioAddIn {
    public class ProjectPlugin : NDoc.VisualStudio.IProject {

        private DTE2 _applicationObject;
        private string _uniqueName;
        private NDoc.VisualStudio.ISolution _solution;

        public ProjectPlugin(NDoc.VisualStudio.ISolution solution, DTE2 _applicationObject, string uniqueName) {
            this._applicationObject = _applicationObject;
            this._uniqueName = uniqueName;
            this._solution = solution;
        }

        public string AssemblyName {
            get {
                EnvDTE.Project proj = this._applicationObject.Solution.Projects.Item(_uniqueName);
                return proj.Name;
            }
        }

        public NDoc.VisualStudio.IProjectConfig GetConfiguration(string configName) {
            string[] configAndPlatform = configName.Split(new char[] { '|' }, StringSplitOptions.None);
            if (configAndPlatform.Length != 2) {
                return null;
            }
            return new ProjectConfigPlugin(this._applicationObject, this._uniqueName, configAndPlatform[0], configAndPlatform[1]);

        }

        public string GetRelativeOutputPathForConfiguration(string configName) {
            return System.IO.Path.Combine(
                            System.IO.Path.Combine(RelativePath, GetConfiguration(configName).OutputPath),
                            OutputFile);
        }

        public string GetRelativePathToDocumentationFile(string configName) {
            string path = string.Empty;

            string documentationFile = GetConfiguration(configName).DocumentationFile;

            if (documentationFile != null && documentationFile.Length > 0) {
                path = System.IO.Path.Combine(RelativePath, documentationFile);
            }
            return path;
        }

        public string ID {
            get {
                return this._uniqueName;
            }
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
            get {
                //http://msdn2.microsoft.com/en-us/library/aa983979(VS.71).aspx
                try {
                    int outputtype = Int32.Parse(this.getProjectProperty(_uniqueName, "OutputType"));
                    switch (outputtype) {
                        case 0:
                            return "WinExe";
                        case 1:
                            return "Exe";
                        case 2:
                            return "Library";
                        default:
                            return string.Empty;
                    }

                } catch (Exception) {
                    return string.Empty;
                }
            }
        }

        public NDoc.VisualStudio.ProjektType Type {
            get {
                EnvDTE.Project proj = this._applicationObject.Solution.Projects.Item(_uniqueName);
                return NDoc.VisualStudio.Project.GetProjectType(proj.Kind);
            }
        }

        /// <summary>
        /// Readn doesn't need to be implemented, because all needed data 
        /// can be extracted using the EnvDTE80.DTE2 Object
        /// </summary>
        /// <param name="path"></param>
        public void Read(string path) {
            throw new NotImplementedException("public void Read(string path)");
        }

        public string RelativePath {
            get {
                return this.getProjectProperty(_uniqueName, "FullPath");
            }
            set {
                throw new NotImplementedException("ProjectPlugin.RelativePath.set");
            }
        }

        public string RootNamespace {
            get {
                return this.getProjectProperty(_uniqueName, "RootNamespace");
            }
        }

        public NDoc.VisualStudio.ISolution Solution {
            get {
                return _solution;
            }
        }

        private string getProjectProperty(string projectUniqueName, string key) {
            try {
                EnvDTE.Project prj = _applicationObject.Solution.Projects.Item(projectUniqueName);
                return prj.Properties.Item(key).Value.ToString();
            } catch (Exception) {
                return string.Empty;
            }
        }

        public override string ToString() {
            return String.Format("unique Name: {0}  Name:{1}", this._uniqueName, this.Name);
        }
    }
}
