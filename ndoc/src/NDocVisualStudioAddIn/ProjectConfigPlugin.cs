using System;
using System.Collections.Generic;
using System.Text;
using NDoc.VisualStudio;
using EnvDTE80;
using EnvDTE;

namespace NDocVisualStudioAddIn {
    public class ProjectConfigPlugin : IProjectConfig{

        private string _uniqueName;
        private string _configuration;
        private string _platform;
        private DTE2 _applicationObject;

        public ProjectConfigPlugin(DTE2 applicationObject, string uniqueName, string configuration, string platform) {
            this._uniqueName = uniqueName;
            this._configuration = configuration;
            this._platform = platform;
            this._applicationObject = applicationObject;
        }
        
        private EnvDTE.Project getProject() {
            return this._applicationObject.Solution.Projects.Item(this._uniqueName); ;
        }

        private EnvDTE.Configuration getConfiguration() {
            EnvDTE.Project proj = getProject();
            return proj.ConfigurationManager.Item(_configuration,_platform);
        }

        private string getConfigurationProperty(string key) {
            try {
                EnvDTE.Configuration config = getConfiguration();
                return config.Properties.Item(key).Value.ToString();
            } catch (Exception) {
                return string.Empty;
            }
        }

        public string DocumentationFile {
            get {
                return getConfigurationProperty("DocumentationFile");
            }
        }

        public string Name {
            get { 
                return _configuration + "|" + _platform; 
            }
        }

        public string OutputPath {
            get {
                //string msg = string.Empty;
                //Properties props = getConfiguration().Properties;
                //foreach (Property prop in props) {
                //    msg += "\n" + prop.Name;
                //    try {
                //        msg += ": " + prop.Value;
                //    } catch (Exception) {}
                //}
                //System.Windows.Forms.MessageBox.Show(msg);
                return getConfigurationProperty("OutputPath");
            }
        }
    }
}
