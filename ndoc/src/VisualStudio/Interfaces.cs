using System;
using System.Collections.Generic;
using System.Text;

namespace NDoc.VisualStudio {

    /// <summary>
    /// The Interface for Classes which represents a Visual Studio solution file.
    /// </summary>
    public interface ISolution {

        /// <summary>Gets the SolutionDirectory property.</summary>
        /// <remarks>This is the directory that contains the VS.NET
        /// solution file.</remarks>
        string Directory {
            get;
        }

        /// <summary>
        /// Get the solution's configurations Names.
        /// VS 2005 and 2008 Solutions return Config and Plattform kombinations
        /// </summary>
        /// <returns>A collection of configuration names.</returns>
        ICollection<string> GetConfigurationsNames();

        /// <summary>Gets the project with the specified name.</summary>
        /// <param name="name">The project name.</param>
        /// <returns>The project.</returns>
        IProject GetProject(string name);

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
        string GetProjectConfigName(string solutionConfig, string projectId);

        /// <summary>Allows you to enumerate (using foreach) over the 
        /// solution's projects.</summary>
        /// <returns>An enumerable list of projects.</returns>
        ICollection<IProject> GetProjects();

        /// <summary>Gets the SolutionName property.</summary>
        /// <remarks>This is the name of the VS.NET solution file
        /// without the .sln extension.</remarks>
        string Name {
            get;
        }

        /// <summary>Gets the Ide property.</summary>
        /// <remarks>This is the Version of Visual Studio or other supported IDEs
        ///  a Value of the enum NDoc.VisualStudio.IdeType
        /// </remarks>
        IdeType Ide {
            get;
        }

        /// <summary>Gets a count of the number of projects in the solution</summary>
        int ProjectCount {
            get;
        }
    }

    public interface IProject {

        /// <summary>Gets the name of the assembly this project generates.</summary>
        string AssemblyName {
            get;
        }

        /// <summary>Gets the configuration with the specified name.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        /// <returns>A ProjectConfig object.</returns>
        IProjectConfig GetConfiguration(string configName);

        /// <summary>Gets the relative path (from the solution directory) to the
        /// assembly this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        string GetRelativeOutputPathForConfiguration(string configName);

        /// <summary>Gets the relative path (from the solution directory) to the
        /// XML documentation this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        string GetRelativePathToDocumentationFile(string configName);

        /// <summary>Gets the GUID that identifies the project.</summary>
        string ID {
            get;
        }

        /// <summary>Gets the name of the project.</summary>
        string Name {
            get;
        }

        /// <summary>Gets the filename of the generated assembly.</summary>
        string OutputFile {
            get;
        }

        /// <summary>Gets the output type of the project.</summary>
        /// <value>"Library", "Exe", or "WinExe"</value>
        string OutputType {
            get;
        }

        /// <summary> Gets the Type of the Project (CS, CPP, VB, Setup,..) </summary>
        ProjektType Type {
            get;
        }

        /// <summary>Reads the project file from the specified path.</summary>
        /// <param name="path">The path to the project file.</param>
        void Read(string path);

        /// <summary>Gets or sets the relative path (from the solution 
        /// directory) to the project directory.</summary>
        string RelativePath {
            get;
            set;
        }

        /// <summary>Gets the default namespace for the project.</summary>
        string RootNamespace {
            get;
        }

        /// <summary>Gets the solution that contains this project.</summary>
        ISolution Solution {
            get;
        }
    }

    public interface IProjectConfig {

        /// <summary>Gets the name of the file (relative to the project 
        /// directory) into which documentation comments will be 
        /// processed.</summary>
        string DocumentationFile {
            get;
        }

        /// <summary>Gets the name of the configuration.</summary>
        /// <remarks>This is usually "Debug" or "Release".</remarks>
        string Name {
            get;
        }

        /// <summary>Gets the location of the output files (relative to the 
        /// project directory) for this project's configuration.</summary>
        string OutputPath {
            get;
        }
    }
}
