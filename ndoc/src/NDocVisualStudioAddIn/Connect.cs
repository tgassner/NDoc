using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

namespace NDocVisualStudioAddIn
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName;

				try
				{
					//If you would like to move the command to a different menu, change the word "Tools" to the 
					//  English version of the menu. This code will take the culture, append on the name of the menu
					//  then add the command to that menu. You can find a list of all the top-level menus in the file
					//  CommandBar.resx.
					string resourceName;
					ResourceManager resourceManager = new ResourceManager("NDocVisualStudioAddIn.CommandBar", Assembly.GetExecutingAssembly());
					CultureInfo cultureInfo = new CultureInfo(_applicationObject.LocaleID);
					
					if(cultureInfo.TwoLetterISOLanguageName == "zh")
					{
						System.Globalization.CultureInfo parentCultureInfo = cultureInfo.Parent;
						resourceName = String.Concat(parentCultureInfo.Name, "Tools");
					}
					else
					{
						resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
					}
					toolsMenuName = resourceManager.GetString(resourceName);
				}
				catch
				{
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

                try{
                    try{
                        //while (true)
                        //{
                            nDocCommandbar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["NDoc"];
                            //nDocCommandbar.Delete();
                        //}
                    }catch(Exception){
                        nDocCommandbar = (Microsoft.VisualStudio.CommandBars.CommandBar)commands.AddCommandBar("NDoc", vsCommandBarType.vsCommandBarTypeMenu, toolsCommandBar, 1);
                    }
                }
                catch (System.ArgumentException){
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
                    Command command = commands.AddNamedCommand2(_addInInstance, "NDocVisualStudioAddIn", "NDoc Test", "Executes the command for NDocVisualStudioAddIn", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                    
					//Add a control for the command to the tools menu:
                    if ((command != null) && (nDocCommandbar != null))
                    {
                        command.AddControl(nDocCommandbar, 1);
                    }
				}
				catch(System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try
                {
                    Command command = commands.AddNamedCommand2(_addInInstance, "NDocVisualStudioAddInFromCurrentSolutionAndConfig", "NDoc composition from current Solution and Config", "Executes a composition from the current Solution with the current Configuration", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                    //Add a control for the command to the tools menu:
                    if ((command != null) && (nDocCommandbar != null))
                    {
                        command.AddControl(nDocCommandbar, 1);
                    }
                }
                catch (System.ArgumentException)
                {
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
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
                if (commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddInFromCurrentSolutionAndConfig")
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }

				if(commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddIn")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
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
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
                if (commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddInFromCurrentSolutionAndConfig")
                {
                    System.Windows.Forms.MessageBox.Show("Neuer Menüpunkt");

                    handled = true;
                    return;
                }
                

				if(commandName == "NDocVisualStudioAddIn.Connect.NDocVisualStudioAddIn")
				{
                    //System.Windows.Forms.MessageBox.Show("Trallala");
                    //string msg = "";
                    //for (int i = 1; i <= _applicationObject.ToolWindows.OutputWindow.OutputWindowPanes.Count; i++ ){
                    //    msg = msg + _applicationObject.ToolWindows.OutputWindow.OutputWindowPanes.Item(i).Name + "\r\n";
                    //    }
                    //System.Windows.Forms.MessageBox.Show(msg);
                    //if (_applicationObject.ToolWindows.OutputWindow.ActivePane != null){
                    //     _applicationObject.ToolWindows.OutputWindow.ActivePane.OutputString("Untermenüpunkt gedrückt!");       
                    //}
                    
                    //string msg = String.Empty;
                    //foreach (SolutionConfiguration solCfg in _applicationObject.Solution.SolutionBuild.SolutionConfigurations){
                    //    msg = msg + solCfg.Name + "\r\n";
                    //    foreach (SolutionContext solContext in solCfg.SolutionContexts){
                    //        msg = msg + "    Configuration Name: " + solContext.ConfigurationName + "\r\n        Platform Name: " + solContext.PlatformName + "/tProject Name: " + solContext.ProjectName + /*"\tShouldBuild: " + solContext.ShouldBuild + "\tShould Deploy: " + solContext.ShouldDeploy +*/ "\r\n";
                    //    }
                    //}
                    //System.Windows.Forms.MessageBox.Show(msg);

                    string msg = String.Empty;
                    foreach(string cAp in this.getAllSolutionConfigsAndPlatforms()){
                        msg = msg + cAp + "\r\n";
                    }

                    //System.Windows.Forms.MessageBox.Show(msg);

                    //System.Windows.Forms.MessageBox.Show(_applicationObject.Version);

                    foreach (Project project in _applicationObject.Solution.Projects){
                        //System.Windows.Forms.MessageBox.Show(project.Name + " " + project.IsDirty.ToString());
                        outputgroup(project);
                    }

					handled = true;
					return;
				}
			}
		}

        private void outputgroup(Project proj){
            OutputGroups groups = proj.ConfigurationManager.ActiveConfiguration.OutputGroups;
            int c = 1;
            int x = 0;
            string msg = "";
            // Find an outputgroup with at least one file.
            foreach (OutputGroup grp in groups)
            {
                //x++;
                //if (grp.FileCount > 0)
                //    c = x;

                msg += "\nOutputGroup Description: " + grp.Description;
                msg += "\n\tNumber of Associated Files: " + grp.FileCount.ToString();
                msg += "\n\tCanonicalName: " + grp.CanonicalName;
                msg += "\n\tDisplayName: " + grp.DisplayName;
                msg += "\n\tAssociated File Names: ";

                foreach (String str in (Array)grp.FileNames)
                {
                    msg += "\n\t\t" + str;
                }
                //msg += "\n\t\tFiles Built in OutputGroup: ";
                //foreach (String fURL in (Array)grp.FileURLs)
                //{
                //    msg += "\n\t\t" + fURL;
                //}

            }
            OutputGroup group = groups.Item(c);
            //msg += "OutputGroup Description: " + group.Description;
            //msg += "\nNumber of Associated Files: " + group.FileCount.ToString();
            //msg += "\nAssociated File Names: ";
            //foreach (String str in (Array)group.FileNames)
            //{
            //    msg += "\n" + str;
            //}
            //msg += "\nFiles Built in OutputGroup: ";
            //foreach (String fURL in (Array)group.FileURLs)
            //{
            //    msg += "\n" + fURL;
            //}
            System.Windows.Forms.MessageBox.Show(msg);


        }

        private IList<string> getAllSolutionConfigsAndPlatforms(){
            IList<string> solCfgs = new List<string>();
            IList<string> solPltfrms = new List<string>();
            IList<string> solCfgsAndPltfrms = new List<string>();

            foreach (SolutionConfiguration solCfg in _applicationObject.Solution.SolutionBuild.SolutionConfigurations){
                if (!solCfgs.Contains(solCfg.Name)){
                    solCfgs.Add(solCfg.Name);
                }
                foreach (SolutionContext solContext in solCfg.SolutionContexts){
                    if (!solPltfrms.Contains(solContext.PlatformName)){
                        solPltfrms.Add(solContext.PlatformName);
                    }
                    //msg = msg + "    Configuration Name: " + solContext.ConfigurationName + "\r\n        Platform Name: " + solContext.PlatformName + "/tProject Name: " + solContext.ProjectName + /*"\tShouldBuild: " + solContext.ShouldBuild + "\tShould Deploy: " + solContext.ShouldDeploy +*/ "\r\n";
                }
            }

            
            foreach (string configs in solCfgs){
                foreach (string platforms in solPltfrms){
                    solCfgsAndPltfrms.Add(configs + "|" + platforms);
                }
            }

            return solCfgsAndPltfrms;
        }

		private DTE2 _applicationObject;
		private AddIn _addInInstance;
	}
}