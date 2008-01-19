// MsdnDocumenter.cs - a MSDN-like documenter
// Copyright (C) 2006  Ken Kohler
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NDoc.Documenter.Msdn
{
    public partial class MsdnUrlValidator : Form
    {
        private bool validated = false;
        private bool validUrl = false;
        private string documentText;
        public MsdnUrlValidator()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        /// <summary>
        /// Use .NET WebBrowser control to verify URL
        /// </summary>
        /// <param name="url">url to verify</param>
        /// <returns>TRUE if valid URL</returns>
        public bool ValidateFile(string url)
        {
            documentText = ""; // Clear document content
            validated = false; 
            validUrl = false;
            
            // Navigate to the URL to verify it
            webBrowser1.Navigate(url);

            // Wait for navigation to complete
            while (!validated)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            return validUrl;
        }

        /// <summary>
        /// DocumentCompleted event is called when WebBrowser.Navigation is complete.  Now
        /// we can check if navigation succeeded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // If navigation failed, web page title will be "Cannot find server"
            if (webBrowser1.Document.Title != "Cannot find server")
            {
                validUrl = true;
                documentText = webBrowser1.DocumentText; // Preserve page content, in case needed for Member web pages
            }
            validated = true; // Break out of wait loop
        }

        /// <summary>
        /// Set/Retrieve content for web page.  Content is needed only for Members web pages to verify url of method/properties/etc.
        /// </summary>
        public string WebPage
        {
            get { return documentText; }
            set { documentText = value; }
        }
    }
}