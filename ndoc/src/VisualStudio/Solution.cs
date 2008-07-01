#region Copyright © 2002 Jean-Claude Manoli [jc@manoli.net]
/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *   1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 * 
 *   2. Altered source versions must be plainly marked as such, and must not
 *      be misrepresented as being the original software.
 * 
 *   3. This notice may not be removed or altered from any source distribution.
 */
#endregion

using System;
//using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.DirectoryServices;
using System.Collections.Generic; // to get IIS virtual directory fiel path.

namespace NDoc.VisualStudio {
    /// <summary> Represents the different supported IDEs </summary>
    public enum IdeType {
        /// <summary> Visual Studio Version 7 (2002) </summary>
        Studio2002,
        /// <summary> Visual Studio Version 7.1 (2003) </summary>
        Studio2003,
        /// <summary> Visual Studio Version 2005 (8) </summary>
        Studio2005,
        /// <summary> Visual Studio Version 2008 (9) </summary>
        Studio2008,
        /// <summary> No IDE is defines (initial Value) </summary>
        Unknown
    }

    /// <summary>
    /// Abstract representation of a Visual Studio solution file.
    /// </summary>
    public abstract class Solution : ISolution {

        protected IDictionary<string, IProject> _projects = new Dictionary<string,IProject>();

        protected string _directory;

        /// <summary>Gets the SolutionDirectory property.</summary>
        /// <remarks>This is the directory that contains the VS.NET
        /// solution file.</remarks>
        public string Directory {
            get {
                return _directory;
            }
        }

        /// <summary> List of Configurations </summary>
        protected Dictionary<string, Dictionary<string, string>> _configurations = new Dictionary<string, Dictionary<string, string>>();

        /// <summary> List of Plattforms </summary>
        protected Dictionary<string, Dictionary<string, string>> _platforms = new Dictionary<string, Dictionary<string, string>>();

        /// <summary> List of Configurations and Plattformcombinations</summary>
        protected Dictionary<string, Dictionary<string, string>> _configurationAndPlatforms = new Dictionary<string, Dictionary<string, string>>();


        /// <summary>
        /// Get the solution's configurations Names.
        /// VS 2005 and 2008 Solutions return Config and Plattform kombinations
        /// </summary>
        /// <returns>A collection of configuration names.</returns>
        public abstract ICollection<string> GetConfigurationsNames();

        /// <summary>Gets the project with the specified name.</summary>
        /// <param name="name">The project name.</param>
        /// <returns>The project.</returns>
        public IProject GetProject(string name) {
            foreach (IProject project in _projects.Values) {
                if (project.Name == name) {
                    return project;
                }
            }
            return null;
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
        public abstract string GetProjectConfigName(string solutionConfig, string projectId);

        protected string _name;

        /// <summary>Gets the SolutionName property.</summary>
        /// <remarks>This is the name of the VS.NET solution file
        /// without the .sln extension.</remarks>
        public string Name {
            get {
                return _name;
            }
        }

        protected IdeType _ide = IdeType.Unknown;

        /// <summary>Gets the Ide property.</summary>
        /// <remarks>This is the Version of Visual Studio or other supported IDEs
        ///  a Value of the enum NDoc.VisualStudio.IdeType
        /// </remarks>
        public IdeType Ide {
            get {
                return _ide;
            }
        }

        /// <summary>Allows you to enumerate (using foreach) over the 
        /// solution's projects.</summary>
        /// <returns>An enumerable list of projects.</returns>
        public ICollection<IProject> GetProjects() {
            return _projects.Values;
        }


        /// <summary>Gets a count of the number of projects in the solution</summary>
        public int ProjectCount {
            get {
                return _projects.Count;
            }
        }

        /// <summary>
        /// Overritten ToString() Method
        /// </summary>
        /// <returns>The name of the Solution</returns>
        public override string ToString() {
            return this._name;
        }
    }

    /// <summary>
    /// Represents a Visual Studio 2002 or 2003 solution file.
    /// </summary>
    /// <remarks>
    /// This class is used to read a Visual Studio solution file
    /// </remarks>
    public class Solution0203 : Solution {
        /// <summary>
        /// Initializes a new instance of the Solution of Studio 2002 or 2003 class.
        /// </summary>
        /// <param name="slnPath">The Visual Studio 2002 or 2003 solution file to parse.</param>
        internal Solution0203(string slnPath) {
            Read(slnPath);
        }

        /// <summary>
        /// Get the solution's configurations Names.
        /// </summary>
        /// <returns>A collection of configuration names.</returns>
        public override ICollection<string> GetConfigurationsNames() {
            return _configurations.Keys;
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
        public override string GetProjectConfigName(string solutionConfig, string projectId) {
            IDictionary<string, string> ce = _configurations[solutionConfig];
            if (ce == null) {
                return null;
            } else {
                return ce[projectId];
            }
        }

        /// <summary>Reads a .sln file.</summary>
        /// <param name="path">The path to the .sln file.</param>
        private void Read(string path) {
            path = Path.GetFullPath(path);
            _directory = Path.GetDirectoryName(path);
            _name = Path.GetFileNameWithoutExtension(path);

            StreamReader reader = null;
            using (reader = new StreamReader(path)) {
                string line = reader.ReadLine();
                while (line != null && line.Length == 0) {
                    line = reader.ReadLine();
                }

                if (line == null || !line.StartsWith("Microsoft Visual Studio Solution File")) {
                    throw new ApplicationException("This is not a Microsoft Visual Studio Solution file.");
                }

                if (line.EndsWith("Format Version 8.00")){
                    this._ide = IdeType.Studio2003;
                } else if (line.EndsWith("Format Version 7.00")) {
                    this._ide = IdeType.Studio2002;
                }

                while ((line = reader.ReadLine()) != null) {
                    if (line.StartsWith("Project")) {
                        AddProject(line);
                    } else if (line.StartsWith("\tGlobalSection(SolutionConfiguration)")) {
                        ReadSolutionConfig(reader);
                    } else if (line.StartsWith("\tGlobalSection(ProjectConfiguration)")) {
                        ReadProjectConfig(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the Konfig defined in the Solution
        /// </summary>
        /// <param name="reader"></param>
        private void ReadSolutionConfig(TextReader reader) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("\tEndGlobalSection"))
                    return;

                int eqpos = line.IndexOf('=');
                string config = line.Substring(eqpos + 2);

                _configurations.Add(config, new Dictionary<string,string>());
            }
        }

        private void ReadProjectConfig(TextReader reader) {
            const string pattern = @"^\t\t(?<projid>\S+)\.(?<solcfg>\S+)\.Build\.\d+ = (?<projcfg>\S+)\|.+";
            Regex regex = new Regex(pattern);
            string line;

            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("\tEndGlobalSection"))
                    return;

                Match match = regex.Match(line);
                if (match.Success) {
                    string projid = match.Groups["projid"].Value;
                    string solcfg = match.Groups["solcfg"].Value;
                    string projcfg = match.Groups["projcfg"].Value;
                    //projid = (new Guid(projid)).ToString();

                    _configurations[solcfg].Add(projid, projcfg);
                }
            }
        }

        private void AddProject(string projectLine) {
            //string pattern = @"^Project\(""(?<unknown>\S+)""\) = ""(?<name>\S+)"", ""(?<path>\S+)"", ""(?<id>\S+)""";
            // fix for bug 887476 
            string pattern = @"^Project\(""(?<projecttype>.*?)""\) = ""(?<name>.*?)"", ""(?<path>.*?)"", ""(?<id>.*?)""";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(projectLine);

            if (match.Success) {
                string projectTypeGUID = match.Groups["projecttype"].Value;
                string name = match.Groups["name"].Value;
                string path = match.Groups["path"].Value;
                string id = match.Groups["id"].Value;

                //we check the GUID as this tells us what type of VS project to process
                //this ensures that it a standard project type, and not an third-party one,
                //which might have a completely differant structure that we could not handle!
                if (Project.GetProjectType(projectTypeGUID) == ProjektType.CS) //C# project
				{
                    IProject project = VisualStudioFactory.CreateProject(this,id, name, Project.GetProjectType(projectTypeGUID));
                    string absoluteProjectPath = String.Empty;

                    if (path.StartsWith("http:")) {
                        Uri projectURL = new Uri(path);
                        if (projectURL.Authority == "localhost") {
                            //we will assume thet the virtual directory is on site 1 of localhost
                            DirectoryEntry root = new DirectoryEntry("IIS://localhost/w3svc/1/root");
                            string rootPath = root.Properties["Path"].Value as String;
                            //we will also assume that the user has been clever and changed to virtual directory local path...
                            absoluteProjectPath = rootPath + projectURL.AbsolutePath;
                        }
                    } else {
                        absoluteProjectPath = Path.Combine(_directory, path);
                    }


                    if (absoluteProjectPath.Length > 0) {
                        project.Read(absoluteProjectPath);

                        string relativeProjectPath = Path.GetDirectoryName(absoluteProjectPath);
                        project.RelativePath = relativeProjectPath;

                        if (project.Type == ProjektType.CS || project.Type == ProjektType.WebSite) {
                            _projects.Add(project.ID, project);
                        }

                        //if (project.ProjectType == "C# Local") {
                        //    _projects.Add(project.ID, project);
                        //}
                        //if (project.ProjectType == "C# Web") {
                        //    _projects.Add(project.ID, project);
                        //}
                    }
                }
                if (projectTypeGUID == "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}") // VB.NET project
				{
                }

                if (projectTypeGUID == "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}") // C++ project
				{
                }
            }
        }


        //		/// <summary>Gets the project with the specified GUID.</summary>
        //		/// <param name="id">The GUID used to identify the project in the .sln file.</param>
        //		/// <returns>The project.</returns>
        //		public Project GetProject(Guid id)
        //		{
        //			return (Project)_projects[id];
        //		}
    }

    /// <summary>
    /// Represents a Visual Studio 2005 or 2008 solution file.
    /// </summary>
    /// <remarks>
    /// This class is used to read a Visual Studio solution file
    /// </remarks>
    public class Solution0508 : Solution {
        /// <summary>
        /// Initializes a new instance of the Solution class.
        /// </summary>
        /// <param name="slnPath">The Visual Studio solution file to parse.</param>
        internal Solution0508(string slnPath) {
            Read(slnPath);
        }

        /// <summary>
        /// Get the solution's configurationsand Plattform combination Names.
        /// </summary>
        /// <returns>A collection of configurationand plattform combination names.</returns>
        public override ICollection<string> GetConfigurationsNames() {
            return _configurationAndPlatforms.Keys;
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
        public override string GetProjectConfigName(string solutionConfig, string projectId) {
            IDictionary<string, string> ce = _configurationAndPlatforms[solutionConfig];
            if (ce == null) {
                return null;
            } else {
                if (ce.ContainsKey(projectId))
                    return ce[projectId];
                else
                    return null;
            }
        }

        /// <summary>Reads a .sln file.</summary>
        /// <param name="path">The path to the .sln file.</param>
        private void Read(string path) {
            path = Path.GetFullPath(path);
            _directory = Path.GetDirectoryName(path);
            _name = Path.GetFileNameWithoutExtension(path);

            StreamReader reader = null;
            using (reader = new StreamReader(path)) {
                string line = reader.ReadLine();
                while (line != null && line.Length == 0) {
                    line = reader.ReadLine();
                }

                if (line == null || !line.StartsWith("Microsoft Visual Studio Solution File")) {
                    throw new ApplicationException("This is not a Microsoft Visual Studio Solution file.");
                }

                if (line.EndsWith("Format Version 10.00")) {
                    this._ide = IdeType.Studio2008;
                } else if (line.EndsWith("Format Version 9.00")) {
                    this._ide = IdeType.Studio2005;
                }

                while ((line = reader.ReadLine()) != null) {
                    if (line.StartsWith("Project")) {
                        AddProject(line);
                    }
                    if (line.StartsWith("\tGlobalSection(SolutionConfigurationPlatforms)")) {
                        ReadSolutionConfig(reader);
                    }
                    if (line.StartsWith("\tGlobalSection(ProjectConfigurationPlatforms)")) {
                        ReadProjectConfig(reader);
                    }
                }
            }
        }

        private void ReadSolutionConfig(TextReader reader) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("\tEndGlobalSection"))
                    return;

                int eqpos = line.IndexOf('=');
                string configAndPlatform = line.Substring(eqpos + 2);

                string config = configAndPlatform.Substring(0, configAndPlatform.IndexOf('|'));
                string platform = configAndPlatform.Substring(configAndPlatform.IndexOf('|') + 1);

                if (!_configurationAndPlatforms.ContainsKey(configAndPlatform)) {
                    _configurationAndPlatforms.Add(configAndPlatform, new Dictionary<string, string>());
                }

                if (!_configurations.ContainsKey(config)) {
                    _configurations.Add(config, new Dictionary<string, string>());
                }

                if (!_platforms.ContainsKey(platform)) {
                    _platforms.Add(platform, new Dictionary<string, string>());
                }
            }
        }

        private void ReadProjectConfig(TextReader reader) {
            string pattern;
            string line;

            while ((line = reader.ReadLine()) != null && !line.StartsWith("\tEndGlobalSection")) {
                if (line.Contains("Build")) {

                    string projid = line.Substring(0, line.IndexOf('.')).Trim();

                    string solcfg = line.Substring(line.IndexOf('.') + 1, line.IndexOf("|") - line.IndexOf('.') - 1);
                    string solConfigPltfm = line.Substring(line.IndexOf('.') + 1);
                    solConfigPltfm = solConfigPltfm.Substring(0, solConfigPltfm.IndexOf('.'));
                    string solPltFm = solConfigPltfm.Substring(solConfigPltfm.IndexOf('|') + 1);

                    string projcfg = line.Substring(line.IndexOf('=') + 2, line.LastIndexOf('|') - line.IndexOf('=') - 2);
                    string projpltfm = line.Substring(line.LastIndexOf('|') + 1).Trim();
                    string projConfigPltFm = line.Substring(line.IndexOf('=') + 2);

                    if (!_configurations[solcfg].ContainsKey(projid)) {
                        _configurations[solcfg].Add(projid, projcfg);
                    }

                    if (!_platforms[solPltFm].ContainsKey(projid)) {
                        _platforms[solPltFm].Add(projid, projpltfm);
                    }

                    if (!_configurationAndPlatforms[solConfigPltfm].ContainsKey(projid)) {
                        _configurationAndPlatforms[solConfigPltfm].Add(projid, projConfigPltFm);
                    }
                }
            }
        }

        private void AddProject(string projectLine) {
            string pattern = @"^Project\(""(?<projecttype>.*?)""\) = ""(?<name>.*?)"", ""(?<path>.*?)"", ""(?<id>.*?)""";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(projectLine);

            if (match.Success) {
                string projectTypeGUID = match.Groups["projecttype"].Value;
                string name = match.Groups["name"].Value;
                string path = match.Groups["path"].Value;
                string id = match.Groups["id"].Value;

                //we check the GUID as this tells us what type of VS project to process
                //this ensures that it a standard project type, and not an third-party one,
                //which might have a completely differant structure that we could not handle!
                switch (Project.GetProjectType(projectTypeGUID)) {
                    case ProjektType.CS:
                    //case ProjektType.WebSite:

                        IProject project = VisualStudioFactory.CreateProject(this, id, name, Project.GetProjectType(projectTypeGUID));

                        string absoluteProjectPath = String.Empty;

                        if (path.StartsWith("http:")) {
                            Uri projectURL = new Uri(path);
                            if (projectURL.Authority == "localhost") {
                                //we will assume thet the virtual directory is on site 1 of localhost
                                DirectoryEntry root = new DirectoryEntry("IIS://localhost/w3svc/1/root");
                                string rootPath = root.Properties["Path"].Value as String;
                                //we will also assume that the user has been clever and changed to virtual directory local path...
                                absoluteProjectPath = rootPath + projectURL.AbsolutePath;
                            }
                        } else {
                            absoluteProjectPath = Path.Combine(_directory, path);
                        }


                        if (absoluteProjectPath.Length > 0) {
                            project.Read(absoluteProjectPath);

                            string relativeProjectPath = Path.GetDirectoryName(absoluteProjectPath);
                            project.RelativePath = relativeProjectPath;

                            _projects.Add(project.ID, project);

                        }
                        break;
                    case ProjektType.VB:
                        break;
                    case ProjektType.CPP:
                        break;
                    case ProjektType.Setup:
                    default:
                        break;
                }
            }
        }
    }
}
