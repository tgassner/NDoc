using System;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Text;
using NDoc.VisualStudio;

namespace NDocVisualStudioAddIn {
    public class SolutionPlugin : NDoc.VisualStudio.ISolution {

        private DTE2 _applicationObject;
        protected IDictionary<string, IProject> _projects = new Dictionary<string, IProject>();


        public SolutionPlugin(DTE2 _applicationObject) {
            this._applicationObject = _applicationObject;
        }

        /// <summary>Gets the SolutionDirectory property.</summary>
        /// <remarks>This is the directory that contains the VS.NET
        /// solution file.</remarks>
        public string Directory {
            get {
                return System.IO.Path.GetDirectoryName(this._applicationObject.Solution.FullName);
            }
        }


        /// <summary>
        /// Get the solution's configurationsand Plattform combination Names.
        /// </summary>
        /// <returns>A collection of configurationand plattform combination names.</returns>
        public ICollection<string> GetConfigurationsNames() {
            IList<string> solCfgs = new List<string>();
            IList<string> solPltfrms = new List<string>();
            IList<string> solCfgsAndPltfrms = new List<string>();

            foreach (SolutionConfiguration solCfg in _applicationObject.Solution.SolutionBuild.SolutionConfigurations) {
                if (!solCfgs.Contains(solCfg.Name)) {
                    solCfgs.Add(solCfg.Name);
                }
                foreach (SolutionContext solContext in solCfg.SolutionContexts) {
                    if (!solPltfrms.Contains(solContext.PlatformName)) {
                        solPltfrms.Add(solContext.PlatformName);
                    }
                }
            }


            foreach (string configs in solCfgs) {
                foreach (string platforms in solPltfrms) {
                    solCfgsAndPltfrms.Add(configs + "|" + platforms);
                }
            }

            return solCfgsAndPltfrms;
        }

        /// <summary>This Mthod is not needet in this Implementation.
        /// If it's called anyway a NotImplementedException will be thrown.</summary>
        public NDoc.VisualStudio.IProject GetProject(string name) {
            throw new NotImplementedException("public NDoc.VisualStudio.IProject GetProject(string name)");
        }

        /// <summary>
        /// Returns the specified project's configuration name based for 
        /// a specific solution configuration.
        /// </summary>
        /// <param name="solutionConfig">A valid configuration name for the solution.</param>
        /// <param name="projectId">A valid project guid.</param>
        /// <returns>The project configuration name or null.</returns>
        /// <remarks>The null value is returned when the parameters are invalid,
        /// or if the project is not marked to be built under the specified
        /// solution configuration.</remarks>
        public string GetProjectConfigName(string solutionConfig, string projectId) {
            try {
                string[] configAndPlatform = solutionConfig.Split(new char[] { '|' }, StringSplitOptions.None);
                if (configAndPlatform.Length != 2) {
                    return null;
                }
                EnvDTE.Project prj = _applicationObject.Solution.Projects.Item(1);
                Configuration config = prj.ConfigurationManager.Item(configAndPlatform[0],configAndPlatform[1]);
                return config.ConfigurationName + "|" + config.PlatformName;
            } catch (Exception) {
                return null;
            }
        }

        /// <summary>Allows you to enumerate (using foreach) over the 
        /// solution's projects.</summary>
        /// <returns>An enumerable list of projects.</returns>
        public ICollection<NDoc.VisualStudio.IProject> GetProjects() {
            ICollection<IProject> projects = new List<IProject>();
            foreach (EnvDTE.Project prj in this._applicationObject.Solution.Projects) {
                if (NDoc.VisualStudio.Project.GetProjectType(prj.Kind) == ProjektType.CS){ 
                    //|| NDoc.VisualStudio.Project.GetProjectType(prj.Kind) == ProjektType.WebSite) {
                    projects.Add(new ProjectPlugin(this, this._applicationObject, prj.UniqueName));
                }
            }
            return projects;
        }

        /// <summary>Gets the SolutionName property.</summary>
        /// <remarks>This is the name of the VS.NET solution file
        /// without the .sln extension.</remarks>
        public string Name {
            get {
                return getSolutionProperty("Name");
            }
        }

        /// <summary>Gets the Ide property.</summary>
        /// <remarks>This is the Version of Visual Studio or other supported IDEs
        ///  a Value of the enum NDoc.VisualStudio.IdeType
        /// </remarks>
        public NDoc.VisualStudio.IdeType Ide {
            get {
                switch (this._applicationObject.Version) {
                    case "9.0": return NDoc.VisualStudio.IdeType.Studio2008;
                    case "8.0": return NDoc.VisualStudio.IdeType.Studio2005;
                    default: return NDoc.VisualStudio.IdeType.Unknown;
                }
            }
        }

        /// <summary>Gets a count of the number of projects in the solution</summary>
        public int ProjectCount {
            get {
                int count = 0;
                foreach (EnvDTE.Project prj in this._applicationObject.Solution.Projects) {
                    if (NDoc.VisualStudio.Project.GetProjectType(prj.Kind) == ProjektType.CS){
                        //|| NDoc.VisualStudio.Project.GetProjectType(prj.Kind) == ProjektType.WebSite) {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Help Method to find out Solsution Properties out of the Meta Informations.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string getSolutionProperty(string key) {
            try {
                EnvDTE.Solution sln = this._applicationObject.Solution;
                return sln.Properties.Item(key).Value.ToString();
            } catch (Exception) {
                return string.Empty;
            }
        }
    }
}
