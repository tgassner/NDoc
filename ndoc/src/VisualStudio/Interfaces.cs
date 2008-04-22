using System;
using System.Collections.Generic;
using System.Text;

namespace NDoc.VisualStudio {

    /// <summary>
    /// The Interface for Classes which represents a Visual Studio solution file.
    /// </summary>
    public interface ISolution {
        string Directory {
            get;
        }
        ICollection<string> GetConfigurationsNames();
        IProject GetProject(string name);
        string GetProjectConfigName(string solutionConfig, string projectId);
        ICollection<IProject> GetProjects();
        string Name {
            get;
        }
        IdeType Ide {
            get;
        }
        int ProjectCount {
            get;
        }
    }

    public interface IProject {
        string AssemblyName {
            get;
        }
        IProjectConfig GetConfiguration(string configName);
        string GetRelativeOutputPathForConfiguration(string configName);
        string GetRelativePathToDocumentationFile(string configName);
        string ID {
            get;
        }
        string Name {
            get;
        }
        string OutputFile {
            get;
        }
        string OutputType {
            get;
        }
        ProjektType Type {
            get;
        }
        void Read(string path);

        string RelativePath {
            get;
            set;
        }
        string RootNamespace {
            get;
        }
        ISolution Solution {
            get;
        }
    }

    public interface IProjectConfig {
        string DocumentationFile {
            get;
        }
        string Name {
            get;
        }
        string OutputPath {
            get;
        }
    }
}
