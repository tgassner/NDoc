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

            ///TODO: Projects Extrahieren und in Dictionary speichern!
        }


        public string Directory {
            get {
                return System.IO.Path.GetDirectoryName(this._applicationObject.Solution.FullName);
            }
        }

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

        public NDoc.VisualStudio.IProject GetProject(string name) {
            throw new NotImplementedException("public NDoc.VisualStudio.IProject GetProject(string name)");
            //this._applicationObject.Solution.Projects.Item(0).
        }

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

        public string Name {
            get {
                return getSolutionProperty("Name");
                //return System.IO.Path.GetFileNameWithoutExtension(this._applicationObject.Solution.FullName);
            }
        }

        public NDoc.VisualStudio.IdeType Ide {
            get {
                switch (this._applicationObject.Version) {
                    case "9.0": return NDoc.VisualStudio.IdeType.Studio2008;
                    case "8.0": return NDoc.VisualStudio.IdeType.Studio2005;
                    default: return NDoc.VisualStudio.IdeType.Unknown;
                }
            }
        }


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
