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

        public ICollection<NDoc.VisualStudio.ConfigurationElements> GetConfigurations() {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
            //this._applicationObject.Solution.Projects.Item(0).
        }

        public string GetProjectConfigName(string solutionConfig, string projectId) {
            throw new NotImplementedException();
            //IDictionary<Guid, string> ce = _configurationAndPlatforms[solutionConfig];
            //if (ce == null) {
            //    return null;
            //} else {
            //    return ce[new Guid(projectId)];
            //}
            return null;
        }

        public ICollection<NDoc.VisualStudio.IProject> GetProjects() {
            return _projects.Values;
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
                return _projects.Count;
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
