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

        /// <summary>Gets the name of the file (relative to the project 
        /// directory) into which documentation comments will be 
        /// processed.</summary>
        public string DocumentationFile {
            get {
                return getConfigurationProperty("DocumentationFile");
            }
        }

        /// <summary>Gets the name of the configuration.</summary>
        /// <remarks>This is usually "Debug" or "Release".</remarks>
        public string Name {
            get { 
                return _configuration + "|" + _platform; 
            }
        }

        /// <summary>Gets the location of the output files (relative to the 
        /// project directory) for this project's configuration.</summary>
        public string OutputPath {
            get {
                return getConfigurationProperty("OutputPath");
            }
        }
    }
}
