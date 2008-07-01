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
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace NDoc.VisualStudio
{
    /// <summary> Represents the different Projekttypes </summary>
    public enum ProjektType {
        /// <summary> C-Sharp Project </summary>
        CS,
        /// <summary> C++ Project </summary>
        CPP,
        /// <summary> Visual Basic Project </summary>
        VB,
        /// <summary> J# Project </summary>
        JS,
        /// <summary>  Setup Project </summary>
        Setup,
        /// <summary> Web Site </summary>
        WebSite,
        /// <summary> Solution </summary>
        Solution,
        /// <summary> The Project Type cannot be identified </summary>
        Unknown
    }

    public abstract class Project : IProject {

        /// <summary>Gets the name of the assembly this project generates.</summary>
        public abstract string AssemblyName {
            get;
        }

        /// <summary>Gets the configuration with the specified name.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        /// <returns>A ProjectConfig object.</returns>
        public abstract IProjectConfig GetConfiguration(string configName);

        /// <summary>Gets the relative path (from the solution directory) to the
        /// assembly this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public abstract string GetRelativeOutputPathForConfiguration(string configName);

        /// <summary>Gets the relative path (from the solution directory) to the
        /// XML documentation this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public abstract string GetRelativePathToDocumentationFile(string configName);

        protected string _ID;

        /// <summary>Gets the GUID that identifies the project.</summary>
        public string ID {
            get {
                return _ID;
            }
        }

        protected string _Name;

        /// <summary>Gets the name of the project.</summary>
        public string Name {
            get {
                return _Name;
            }
        }

        /// <summary>Gets the filename of the generated assembly.</summary>
        public abstract string OutputFile {
            get;
        }

        /// <summary>Gets the output type of the project.</summary>
        /// <value>"Library", "Exe", or "WinExe"</value>
        public abstract string OutputType {
            get;
        }

        protected ProjektType _Type;

        /// <summary> Gets the Type of the Project (CS, CPP, VB, Setup,..) </summary>
        public ProjektType Type {
            get {
                return _Type;
            }
        }

        /// <summary>Reads the project file from the specified path.</summary>
        /// <param name="path">The path to the project file.</param>
        public abstract void Read(string path);

        private string _RelativePath;

        /// <summary>Gets or sets the relative path (from the solution 
        /// directory) to the project directory.</summary>
        public string RelativePath {
            get {
                return _RelativePath;
            }
            set {
                _RelativePath = value;
            }
        }

        /// <summary>Gets the default namespace for the project.</summary>
        public abstract string RootNamespace {
            get;
        }

        protected ISolution _Solution;

        /// <summary>Gets the solution that contains this project.</summary>
        public ISolution Solution {
            get {
                return _Solution;
            }
        }

        /// <summary> Gets the project type with the guid in the solution file </summary>
        /// <param name="guid">String representation of the guid</param>
        /// <returns> The projecttype </returns>
        public static ProjektType GetProjectType(string guid) {
            return GetProjectType(new Guid(guid));
        }

        /// <summary> Gets the project type with the guid in the solution file </summary>
        /// <param name="guid">Guid Class representation of the guid</param>
        /// <returns> The projecttype </returns>
        public static ProjektType GetProjectType(Guid guid) {
            if (guid == new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"))
                return ProjektType.CS;
            if (guid == new Guid("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"))
                return ProjektType.VB;
            if (guid == new Guid("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}"))
                return ProjektType.CPP;
            if (guid == new Guid("{54435603-DBB4-11D2-8724-00A0C9A8B90C}"))
                return ProjektType.Setup;
            if (guid == new Guid("{E24C65DC-7377-472B-9ABA-BC803B73C61A}"))
                return ProjektType.WebSite;
            if (guid == new Guid("{66A26720-8FB5-11D2-AA7E-00C04F688DDE}"))
                return ProjektType.Solution;
            if (guid == new Guid("{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}"))
                return ProjektType.JS;

            return ProjektType.Unknown;

        }

        public override string ToString() {
            return this._Name;
        }
    }

	/// <summary>
	/// Represents a Visual Studio c# project file.
	/// </summary>
	public class Project0203 : Project
	{
        internal Project0203(ISolution solution, string id, string name, ProjektType projectType)
		{
			_Solution = solution;
			_ID = id;
			_Name = name;
            _Type = projectType;
		}

		private XPathDocument _ProjectDocument;
		private XPathNavigator _ProjectNavigator;


		/// <summary>Reads the project file from the specified path.</summary>
		/// <param name="path">The path to the project file.</param>
        public override void Read(string path)
		{
			_ProjectDocument = new XPathDocument(path);
			_ProjectNavigator = _ProjectDocument.CreateNavigator();
            //this.getProjectType();
		}

        ///// <summary>Gets a string that represents the type of project.</summary>
        ///// <value>"Visual C++" or "C# Local"</value>
        //private void getProjectType()
        //{
        //    get
        //    {
        //        string projectType = "";

        //        if ((bool)_ProjectNavigator.Evaluate("boolean(VisualStudioProject/@ProjectType='Visual C++')"))
        //        {
        //            projectType = "Visual C++";
        //        }
        //        else if ((bool)_ProjectNavigator.Evaluate("boolean(VisualStudioProject/CSHARP/@ProjectType='Local')"))
        //        {
        //            projectType = "C# Local";
        //        }
        //        else if ((bool)_ProjectNavigator.Evaluate("boolean(VisualStudioProject/CSHARP/@ProjectType='Web')"))
        //        {
        //            projectType = "C# Web";
        //        }
        //        return projectType;
        //    }
        //}

		/// <summary>Gets the name of the assembly this project generates.</summary>
        public override string AssemblyName
		{
			get
			{
				return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@AssemblyName)");
			}
		}

		/// <summary>Gets the output type of the project.</summary>
		/// <value>"Library", "Exe", or "WinExe"</value>
        public override string OutputType
		{
			get
			{
				return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@OutputType)");
			}
		}

		/// <summary>Gets the filename of the generated assembly.</summary>
        public override string OutputFile
		{
			get
			{
				string extension = "";

				switch (OutputType)
				{
					case "Library":
						extension = ".dll";
						break;
					case "Exe":
						extension = ".exe";
						break;
					case "WinExe":
						extension = ".exe";
						break;
				}

				return AssemblyName + extension;
			}
		}

		/// <summary>Gets the default namespace for the project.</summary>
        public override string RootNamespace
		{
			get
			{
				return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@RootNamespace)");
			}
		}

		/// <summary>Gets the configuration with the specified name.</summary>
		/// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
		/// <returns>A ProjectConfig object.</returns>
        public override IProjectConfig GetConfiguration(string configName)
		{
			XPathNavigator navigator = null;

			XPathNodeIterator nodes = 
				_ProjectNavigator.Select(
				String.Format(
				"/VisualStudioProject/CSHARP/Build/Settings/Config[@Name='{0}']", 
				configName));

			if (nodes.MoveNext())
			{
				navigator = nodes.Current;
			}

            return new ProjectConfig0203(navigator);
            //return VisualStudioFactory.CreateProjectConfig(navigator,this.Solution.Ide);
		}

		/// <summary>Gets the relative path (from the solution directory) to the
		/// assembly this project generates.</summary>
		/// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public override string GetRelativeOutputPathForConfiguration(string configName)
		{
			return Path.Combine(
				Path.Combine(RelativePath, GetConfiguration(configName).OutputPath), 
				OutputFile);
		}

		/// <summary>Gets the relative path (from the solution directory) to the
		/// XML documentation this project generates.</summary>
		/// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public override string GetRelativePathToDocumentationFile(string configName)
		{
			string path = null;

			string documentationFile = GetConfiguration(configName).DocumentationFile;

			if (documentationFile != null && documentationFile.Length > 0)
			{
				path = Path.Combine(RelativePath, documentationFile);
			}

			return path;
		}

	}


    /// <summary>
    /// Represents a Visual Studio c# project file.
    /// </summary>
    public class Project0508 : Project {

        internal Project0508(ISolution solution, string id, string name, ProjektType type) {
            _Solution = solution;
            _ID = id;
            _Name = name;
            _Type = type;
        }

        private XPathDocument _ProjectDocument;
        private XPathNavigator _ProjectNavigator;
        private IList<string> _ProjectFile = new List<string>();

        /// <summary>Reads the project file from the specified path.</summary>
        /// <param name="path">The path to the project file.</param>
        public override void Read(string path) {
            _ProjectDocument = new XPathDocument(path);
            _ProjectNavigator = _ProjectDocument.CreateNavigator();

            StreamReader reader = null;
            using (reader = new StreamReader(path)) {
                string line = reader.ReadLine();
                while (line != null) {
                    _ProjectFile.Add(line);
                    line = reader.ReadLine();
                }
            }
        }

        /// <summary>Gets the name of the assembly this project generates.</summary>
        public override string AssemblyName {
            get {
                //string str = (string)_ProjectNavigator.Evaluate("string(/Project/PropertyGroup/AssemblyName)");

                IEnumerator<string> projectFileEnum = _ProjectFile.GetEnumerator();

                while (projectFileEnum.MoveNext() && !projectFileEnum.Current.Contains("<PropertyGroup>")) {
                }

                while (projectFileEnum.MoveNext() && !projectFileEnum.Current.Contains("</PropertyGroup>")) {
                    if (projectFileEnum.Current.Contains("<AssemblyName>")) {
                        return projectFileEnum.Current.Trim().Replace("<AssemblyName>", "").Replace("</AssemblyName>", "");
                    }
                }
                return "";
            }
        }

        /// <summary>Gets the output type of the project.</summary>
        /// <value>"Library", "Exe", or "WinExe"</value>
        public override string OutputType {
            get {
                //return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@OutputType)");

                IEnumerator<string> projectFileEnum = _ProjectFile.GetEnumerator();

                while (projectFileEnum.MoveNext() && !projectFileEnum.Current.Contains("<PropertyGroup>")) {
                }

                while (projectFileEnum.MoveNext() && !projectFileEnum.Current.Contains("</PropertyGroup>")) {
                    if (projectFileEnum.Current.Contains("<OutputType>")) {
                        return projectFileEnum.Current.Trim().Replace("<OutputType>", "").Replace("</OutputType>", "");
                    }
                }
                return "";
            }
        }

        /// <summary>Gets the filename of the generated assembly.</summary>
        public override string OutputFile {
            get {
                string extension = "";

                switch (OutputType) {
                    case "Library":
                        extension = ".dll";
                        break;
                    case "Exe":
                        extension = ".exe";
                        break;
                    case "WinExe":
                        extension = ".exe";
                        break;
                }

                return AssemblyName + extension;
            }
        }

        /// <summary>Gets the default namespace for the project.</summary>
        public override string RootNamespace {
            get {
                return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@RootNamespace)");
            }
        }

        /// <summary>Gets the configuration with the specified name.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        /// <returns>A ProjectConfig object.</returns>
        public override IProjectConfig GetConfiguration(string configName) {
            XPathNavigator navigator = null;
            string name;
            string outputPath;
            string documentationFile;

            name = outputPath = documentationFile = string.Empty;

            IEnumerator<string> projectFileEnum = _ProjectFile.GetEnumerator();

            while (projectFileEnum.MoveNext() && !projectFileEnum.Current.Contains("<PropertyGroup Condition=") && !projectFileEnum.Current.Contains(configName)) {

            }

            name = configName;

            while (projectFileEnum.MoveNext() && !projectFileEnum.Current.Contains("</PropertyGroup>")) {
                if (projectFileEnum.Current.Contains("<OutputPath>")) {
                    outputPath = projectFileEnum.Current.Replace("<OutputPath>", "").Replace("</OutputPath>", "").Replace("\t", "").Trim();
                }

                if (projectFileEnum.Current.Contains("<DocumentationFile>")) {
                    documentationFile = projectFileEnum.Current.Replace("<DocumentationFile>", "").Replace("</DocumentationFile>", "").Replace("\t", "").Trim();
                }
            }


            return new ProjectConfig0508(name, outputPath, documentationFile);
            //return VisualStudioFactory.CreateProjectConfig(navigator, this.Solution.Ide);
        }

        /// <summary>Gets the relative path (from the solution directory) to the
        /// assembly this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public override string GetRelativeOutputPathForConfiguration(string configName) {
            return Path.Combine(
                Path.Combine(RelativePath, GetConfiguration(configName).OutputPath),
                OutputFile);
        }

        /// <summary>Gets the relative path (from the solution directory) to the
        /// XML documentation this project generates.</summary>
        /// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
        public override string GetRelativePathToDocumentationFile(string configName) {
            string path = null;

            string documentationFile = GetConfiguration(configName).DocumentationFile;

            if (documentationFile != null && documentationFile.Length > 0) {
                path = Path.Combine(RelativePath, documentationFile);
            }

            return path;
        }
    }
}
