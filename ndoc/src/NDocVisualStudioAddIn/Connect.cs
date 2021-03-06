using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

namespace NDocVisualStudioAddIn {
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget {
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect() {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;
            if (connectMode == ext_ConnectMode.ext_cm_UISetup) {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                string toolsMenuName;

                try {
                    //If you would like to move the command to a different menu, change the word "Tools" to the 
                    //  English version of the menu. This code will take the culture, append on the name of the menu
                    //  then add the command to that menu. You can find a list of all the top-level menus in the file
                    //  CommandBar.resx.
                    string resourceName;
                    ResourceManager resourceManager = new ResourceManager("NDocVisualStudioAddIn.CommandBar", Assembly.GetExecutingAssembly());
                    CultureInfo cultureInfo = new CultureInfo(_applicationObject.LocaleID);

                    if (cultureInfo.TwoLetterISOLanguageName == "zh") {
                        System.Globalization.CultureInfo parentCultureInfo = cultureInfo.Parent;
                        resourceName = String.Concat(parentCultureInfo.Name, "Tools");
                    } else {
                        resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
                    }
                    toolsMenuName = resourceManager.GetString(resourceName);
                } catch {
                    //We tried to find a localized version of the word Tools, but one was not found.
                    //  Default to the en-US word, which may work for the current culture.
                    toolsMenuName = "Tools";
                }

                //Place the command on the tools menu.
                //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
                Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

                //Find the Tools command bar on the MenuBar command bar:
                CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
                CommandBar toolsCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)[toolsMenuName];
                CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;
                CommandBar nDocCommandbar = null;

                try {
                    try {
                        //while (true)
                        //{
                        nDocCommandbar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["NDoc"];
                        //nDocCommandbar.Delete();
                        //}
                    } catch (Exception) {
                        nDocCommandbar = (Microsoft.VisualStudio.CommandBars.CommandBar)commands.AddCommandBar("NDoc", vsCommandBarType.vsCommandBarTypeMenu, toolsCommandBar, 1);
                    }
                } catch (System.ArgumentException) {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try {
                    Command command = commands.AddNamedCommand2(_addInInstance, "NDocVisualStudioAddInFromCurrentSolution", "NDoc composition from current Solution", "Executes a composition from the current Solution", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                    //Add a control for the command to the tools menu:
                    if ((command != null) && (nDocCommandbar != null)) {
                        command.AddControl(nDocCommandbar, 1);
                    }
                } catch (System.ArgumentException) {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try {
                    Command command = commands.AddNamedCommand2(_addInInstance, "NDocVisualStudioAddInFromCurrentSolutionAndConfig", "NDoc composition from current Solution and current Config", "Executes a composition from the current Solution with the current Configuration", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                    //Add a control for the command to the tools menu:
                    if ((command != null) && (nDocCommandbar != null)) {
                        command.AddControl(nDocCommandbar, 1);
                    }
                } catch (System.ArgumentException) {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) {
            //try {
            //    CommandBar nDocCommandbar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["NDoc"];
            //    nDocCommandbar.Delete();
            //} catch (Exception) {
            //}
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom) {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom) {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom) {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText) {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone) {
                if (commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddInFromCurrentSolutionAndConfig") {
                    if (this._applicationObject.Solution.Projects.Count > 0) {
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    } else {
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusLatched;
                    }
                    return;
                }

                if (commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddInFromCurrentSolution") {
                    if (this._applicationObject.Solution.Projects.Count > 0) {
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    }
                    else {
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusLatched;
                    }
                    return;
                }
            }
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled) {
            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault) {
                if (commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddInFromCurrentSolutionAndConfig") {

                    NDoc.Gui.MainForm gui = new NDoc.Gui.MainForm(null);
                    gui.Show();
                    gui.FromExternSolution(new SolutionPlugin(this._applicationObject),getSolutionProperty("ActiveConfig"));

                    handled = true;
                    return;
                }


                if (commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddInFromCurrentSolution") {
                    NDoc.Gui.MainForm gui = new NDoc.Gui.MainForm(null);
                    gui.Show();
                    gui.FromExternSolution(new SolutionPlugin(this._applicationObject), null);

                    handled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Extracts all Configs and Platforms in the current Solution
        /// </summary>
        /// <returns>a List with the Configs and Platforms in the current solution
        /// like "Debug|Any CPU"</returns>
        private IList<string> getAllSolutionConfigsAndPlatforms() {
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
                    //msg = msg + "    Configuration Name: " + solContext.ConfigurationName + "\r\n        Platform Name: " + solContext.PlatformName + "/tProject Name: " + solContext.ProjectName + /*"\tShouldBuild: " + solContext.ShouldBuild + "\tShould Deploy: " + solContext.ShouldDeploy +*/ "\r\n";
                }
            }

            foreach (string configs in solCfgs) {
                foreach (string platforms in solPltfrms) {
                    solCfgsAndPltfrms.Add(configs + "|" + platforms);
                }
            }

            return solCfgsAndPltfrms;
        }

        /// <summary>
        /// Extracts the Configs of a Project
        /// </summary>
        /// <param name="projectUniqueName">Unique Project name of which the 
        /// Configs will be returned</param>
        /// <returns></returns>
        private IList<string> getConfigurations(string projectUniqueName) {
            IList<string> configs = new List<string>();
            Project prj = this._applicationObject.Solution.Projects.Item(projectUniqueName);

            foreach (string config in (Array)prj.ConfigurationManager.ConfigurationRowNames) {
                configs.Add(config);
            }
            return configs;
        }

        /// <summary>
        /// Extracts the supported Platforms by the Project.
        /// </summary>
        /// <param name="projectUniqueName">the unique Projectname</param>
        /// <returns>A list with all Platforms supported by the Project</returns>
        private IList<string> getPlatforms(string projectUniqueName) {
            IList<string> platforms = new List<string>();
            Project prj = this._applicationObject.Solution.Projects.Item(projectUniqueName);

            foreach (string config in (Array)prj.ConfigurationManager.PlatformNames) {
                platforms.Add(config);
            }
            return platforms;
        }

        /// <summary>
        /// Gets a Property of a Project.
        /// </summary>
        /// <param name="projectUniqueName">The unique name of the project of 
        /// which the property shoult be shown</param>
        /// <param name="key">Key of the Property</param>
        /// <returns>The Value of the property</returns>
        internal string getProjectProperty(string projectUniqueName, string key) {
            try {
                Project prj = _applicationObject.Solution.Projects.Item(projectUniqueName);
                return prj.Properties.Item(key).Value.ToString();
            } catch (Exception) {
                return string.Empty;
            }
        }

        /// <summary>
        /// Extracts a Property from the Solution
        /// </summary>
        /// <param name="key">key of the property</param>
        /// <returns>the value of the property</returns>
        internal string getSolutionProperty(string key) {
            try {
                Solution sln = this._applicationObject.Solution;
                return sln.Properties.Item(key).Value.ToString();
            } catch (Exception) {
                return string.Empty;
            }
        }

        #region helpOrDiagnoseMethods

        /// <summary>
        /// Shows all Properties in the Solution in a MessageBox
        /// </summary>
        private void ShowSolutionProperties() {
            try {   // Open a project before running this sample.
                Solution sln = this._applicationObject.Solution;
                string msg, msg2 = "Global Variables:";
                msg = "FileName: " + sln.FileName;
                msg += "\nFullName: " + sln.FullName;
                msg += "\nIsOpen " + sln.IsOpen;
                msg += "\nProperties: ";
                foreach (Property prop in sln.Properties) {
                    msg += "\n  " + prop.Name;
                    try {
                        if (prop.Name == "ExtenderNames") {
                            msg += ": Länge: " + ((Array)prop.Value).Length + " ";
                            foreach (string s in (Array)prop.Value) {
                                msg += ": " + s;
                            }
                        }
                        else {
                            if (prop.Value != null) {
                                msg += ": " + prop.Value;
                            }
                        }
                    }
                    catch (Exception) { }
                }
                foreach (String s in (Array)sln.Globals.VariableNames) {
                    msg2 += "\n  " + s;
                }

                System.Windows.Forms.MessageBox.Show(msg, "Solution Name: " + sln.FullName);
                System.Windows.Forms.MessageBox.Show(msg2);
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Shows the Configuration of the first project in a MessageBox
        /// </summary>
        private void ShowConfigurationProjectConfigurations() {
            try {   // Open a project before running this sample.
                string msg = "";
                bool b = false;
                Properties props;

                string unique = _applicationObject.Solution.Projects.Item(1).UniqueName;

                foreach (Project prj in _applicationObject.Solution.Projects) {
                    foreach (string platform in getPlatforms(unique)) {
                        foreach (string configg in getConfigurations(unique)) {
                            Configuration configx = prj.ConfigurationManager.Item(configg, platform);
                            props = configx.Properties;
                            msg = prj.Name + ": " + configg + "|" + platform;
                            foreach (Property prop in props) {
                                if (b)
                                    msg += "\n";
                                else
                                    msg += "\t";
                                msg += prop.Name;
                                try {
                                    msg += ": " + prop.Value;
                                }
                                catch (Exception) {
                                }
                                b = !b;
                            }
                            //System.Windows.Forms.MessageBox.Show(s);
                            System.Windows.Forms.MessageBox.Show(msg);
                        }
                    }
                }
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Shows the properties of the Project in a MessageBox
        /// </summary>
        /// <param name="projectNo">number of the project</param>
        private void ShowProjectProperties(int projectNo) {
            try {   // Open a project before running this sample.
                System.Windows.Forms.MessageBox.Show(this._applicationObject.Solution.Projects.Count.ToString());
                Project prj = this._applicationObject.Solution.Projects.Item(projectNo);
                //System.Windows.Forms.MessageBox.Show(prj.);
                Projects prjs;
                string msg, msg2 = "Global Variables:";
                msg = "FileName: " + prj.FileName;
                msg += "\nFullName: " + prj.FullName;
                //msg += "\nProject-level access to " + prj.CodeModel.CodeElements.Count.ToString() +
                //    " CodeElements through the CodeModel";
                prjs = prj.Collection;
                msg += "\nThere are " + prjs.Count.ToString() + " projects in the same collection.";
                msg += "\nApplication containing this project: " + prj.DTE.Name;
                if (prj.Saved)
                    msg += "\nThis project hasn't been modified since the last save.";
                else
                    msg += "\nThis project has been modified since the last save.";
                msg += "\nProperties: ";
                int i = 0;
                foreach (Property prop in prj.Properties) {
                    if ((i % 2) == 0)
                        msg += "\n  ";
                    else
                        msg += "\t  ";
                    msg += prop.Name;
                    try {
                        if (prop.Value != null)
                            msg += ": " + prop.Value;
                    }
                    catch (Exception) { }
                    i++;
                }
                foreach (String s in (Array)prj.Globals.VariableNames) {
                    msg2 += "\n  " + s;
                }

                System.Windows.Forms.MessageBox.Show(msg, "Project Name: " + prj.Name);
                //System.Windows.Forms.MessageBox.Show(msg2);
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Shows the extenders of the first Project in a MessageBox
        /// </summary>
        private void ShowExtender() {
            Project prj = this._applicationObject.Solution.Projects.Item(1);
            foreach (string exten in (Array)prj.ExtenderNames) {
                System.Windows.Forms.MessageBox.Show(exten);
            }
        }

        /// <summary>
        /// Shows the Items of the first Project in a MessageBox
        /// </summary>
        private void ShowItems() {
            Project prj = this._applicationObject.Solution.Projects.Item(1);
            foreach (ProjectItem item in prj.ProjectItems) {
                System.Windows.Forms.MessageBox.Show(item.Name);
            }
        }

        /// <summary>
        /// Shows the Outputgroups of a Project in a MessageBox
        /// </summary>
        /// <param name="proj"> the project of which the outputgroups will be shown</param>
        private void outputgroup(Project proj) {
            OutputGroups groups = proj.ConfigurationManager.ActiveConfiguration.OutputGroups;
            int c = 1;
            //int x = 0;
            string msg = "";
            // Find an outputgroup with at least one file.
            foreach (OutputGroup grp in groups) {
                //x++;
                //if (grp.FileCount > 0)
                //    c = x;

                msg += "\nOutputGroup Description: " + grp.Description;
                msg += "\n\tNumber of Associated Files: " + grp.FileCount.ToString();
                msg += "\n\tCanonicalName: " + grp.CanonicalName;
                msg += "\n\tDisplayName: " + grp.DisplayName;
                msg += "\n\tAssociated File Names: ";

                foreach (String str in (Array)grp.FileNames) {
                    msg += "\n\t\t" + str;
                }
                //msg += "\n\t\tFiles Built in OutputGroup: ";
                foreach (String fURL in (Array)grp.FileURLs) {
                    msg += "\n\t\t" + fURL;
                }

            }
            OutputGroup group = groups.Item(c);
            System.Windows.Forms.MessageBox.Show(msg);
        }

        #endregion


        private DTE2 _applicationObject;
        private AddIn _addInInstance;
    }
}