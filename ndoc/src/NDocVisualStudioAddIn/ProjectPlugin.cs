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

        /// <summary>Gets the name of the assembly this project generates.</summary>
        public string AssemblyName {
            get {
                EnvDTE.Project proj = this._applicationObject.Solution.Projects.Item(_uniqueName);
                return proj.Name;
            }
        }

        /// <summary>Gets the configuration with the specified name.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        /// <returns>A ProjectConfig object.</returns>
        public NDoc.VisualStudio.IProjectConfig GetConfiguration(string configName) {
            string[] configAndPlatform = configName.Split(new char[] { '|' }, StringSplitOptions.None);
            if (configAndPlatform.Length != 2) {
                return null;
            }
            return new ProjectConfigPlugin(this._applicationObject, this._uniqueName, configAndPlatform[0], configAndPlatform[1]);

        }

        /// <summary>Gets the relative path (from the solution directory) to the
        /// assembly this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public string GetRelativeOutputPathForConfiguration(string configName) {
            return System.IO.Path.Combine(
                            System.IO.Path.Combine(RelativePath, GetConfiguration(configName).OutputPath),
                            OutputFile);
        }

        /// <summary>Gets the relative path (from the solution directory) to the
        /// XML documentation this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public string GetRelativePathToDocumentationFile(string configName) {
            string path = string.Empty;

            string documentationFile = GetConfiguration(configName).DocumentationFile;

            if (documentationFile != null && documentationFile.Length > 0) {
                path = System.IO.Path.Combine(RelativePath, documentationFile);
            }
            return path;
        }

        /// <summary>Gets the GUID that identifies the project.</summary>
        public string ID {
            get {
                return this._uniqueName;
            }
        }

        /// <summary>Gets the name of the project.</summary>
        public string Name {
            get {
                EnvDTE.Project proj = this._applicationObject.Solution.Projects.Item(_uniqueName);
                return proj.Name;
            }
        }

        /// <summary>Gets the filename of the generated assembly.</summary>
        public string OutputFile {
            get {
                return this.getProjectProperty(_uniqueName, "OutputFileName");
            }
        }

        /// <summary>Gets the output type of the project.</summary>
        /// <value>"Library", "Exe", or "WinExe"</value>
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

        /// <summary> Gets the Type of the Project (CS, CPP, VB, Setup,..) </summary>
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

        /// <summary>Gets or sets the relative path (from the solution 
        /// directory) to the project directory.</summary>
        public string RelativePath {
            get {
                return this.getProjectProperty(_uniqueName, "FullPath");
            }
            set {
                throw new NotImplementedException("ProjectPlugin.RelativePath.set");
            }
        }

        /// <summary>Gets the default namespace for the project.</summary>
        public string RootNamespace {
            get {
                return this.getProjectProperty(_uniqueName, "RootNamespace");
            }
        }

        /// <summary>Gets the solution that contains this project.</summary>
        public NDoc.VisualStudio.ISolution Solution {
            get {
                return _solution;
            }
        }

        /// <summary>
        /// Helpmethod to extract the Properties of a Project
        /// </summary>
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
