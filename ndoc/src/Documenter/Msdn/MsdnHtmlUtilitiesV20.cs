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

using NDoc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace NDoc.Documenter.Msdn
{
    /// <summary>
    /// Utility class to migrate HREFs from V1.x format to V2.0 format
    /// </summary>
    class MsdnHtmlUtilitiesV20
    {
        /// <summary>
        /// Namespaces for all referenced assemblies.  Used to create a portion of the help URLs
        /// </summary>
        public static Dictionary<string, string> namespaces = new Dictionary<string, string>();

        /// <summary>
        /// Corrections made to namespace/class/method derived URLs to account for actual help file containing web page
        /// </summary>
        static Dictionary<string, string> baseUrlCorrections = new Dictionary<string, string>();

        /// <summary>
        /// Content for Class Members help pages.  Content from these pages is searched to find actual (relative) links for 
        /// all members, properties, overloads, etc.
        /// </summary>
        static Dictionary<string, string> msHelpContent = new Dictionary<string, string>(); // _Members.htm pages only

        /// <summary>
        /// For all ms-help URLs, verify that URL is correct.  If not, search for correct cpref? number to 
        /// be part of correct URL.  For Class Members pages, cache page content to be used for fixing method/property/etc. URLs
        /// </summary>
        /// <param name="line">ms-help URL to verify</param>
        /// <param name="documentor">MSDN IDocumentor class containing MsdnUrlValidator form</param>
        /// <returns>corrected url or original url, if no corrections made</returns>
        private static string VerifyUrl(string line, MsdnDocumenter documentor)
        {
            // If URL has already been verified, just replace initial URL with previous correction.
            // For Class Members pages, retrieve page content from cache.
            if (baseUrlCorrections.ContainsKey(line))
            {
                string correction = baseUrlCorrections[line];
                if (correction.IndexOf("_Members.htm") > 0)
                {
                    documentor.urlValidator.WebPage = msHelpContent[correction];
                }
                return correction;
            }

            string newLine = line;
            string initialUrl = line.Substring(6, line.Length - 7); // Remove href=" + trailing "
            string uri = initialUrl;
            bool valid = documentor.urlValidator.ValidateFile(uri);
            if (!valid)
            {
                // We know cpref2 (file) is not correct, check cpref3 thru cpref19
                // Cache page content for Class Members pages
                for (int i = 3; i <= 19; i++)
                {
                    string cpref = "cpref" + i.ToString();
                    uri = initialUrl.Replace("cpref2", cpref);
                    valid = documentor.urlValidator.ValidateFile(uri);
                    if (valid)
                    {
                        newLine = line.Replace(initialUrl, uri);
                        baseUrlCorrections[line] = newLine;
                        if (line.IndexOf("_Members.htm") > 0)
                        {
                            msHelpContent[newLine] = documentor.urlValidator.WebPage;
                        }
                        break;
                    }
                }
            }
            else // We don't need to validate this line again, preserve valid url and page content for Class Member pages
            {
                baseUrlCorrections[line] = newLine;
                if (line.IndexOf("_Members.htm") > 0)
                {
                    msHelpContent[newLine] = documentor.urlValidator.WebPage;
                }
            }
            return newLine;
        }

        /// <summary>
        /// Verify URL is in correct file (w/ VerifyURL), then use content from Class Members help page to find 
        /// actual URL for method/property/etc.
        /// </summary>
        /// <param name="line">ms-help URL to verify</param>
        /// <param name="documentor">MSDN IDocumentor class containing MsdnUrlValidator form</param>
        /// <param name="methodName">method/property/etc. for URL</param>
        /// <returns>corrected url or original url, if no corrections made</returns>
        private static string VerifyMethodUrl(string line, MsdnDocumenter documentor, string methodName)
        {
            // Find correct Class member page and get page content
            string url = VerifyUrl(line, documentor);

            // Retrieve page content, search for method/property name.  After name is found,
            // use URL from <a> tag immediately preceeding display text to update URL with 
            // correct value.
            string contents = documentor.urlValidator.WebPage;
            int index = contents.IndexOf(">" + methodName + "<");
            if (index > 2)
            {
                // We need the URL between ""
                index--; // Should be "
                int start = index - 1;
                while (start > 0)
                {
                    if (contents[start] == '"')
                        break;
                    start--;
                }
                // If we found what we're looking for, update URL with link from page content
                if (start > 0)
                {
                    string partial = contents.Substring(start + 1, index - start - 1);
                    index = url.LastIndexOf("/");
                    url = url.Substring(0, index + 1);
                    url += partial;
                    url += '"';
                }
            }
            return url;
        }

        private static string FixLocalUrl(string line, string href, string filePath, int index)
        {
            // Class URLs
            string url = href.Substring(6); // Remove HREF="
            int pos = url.IndexOf("\"");
            url = url.Substring(0, pos); // Remove trailing "
            if (url.IndexOf(".html") != -1)
            {
                if (!File.Exists(filePath + url))
                {
                    int start = index;
                    while (start > 0 && line[start] != '<')
                        start--;
                    int end = line.IndexOf("</a>", index);
                    string tempHRef = line.Substring(start, end - start + 4);
                    string noHRef = tempHRef.Substring(tempHRef.IndexOf('>') + 1);
                    noHRef = noHRef.Substring(0, noHRef.IndexOf('<'));
                    line = line.Replace(tempHRef, noHRef);
                }
            }
            return line;
        }

        /// <summary>
        /// Correct .NET 2.0 HREFs (i.e. translate from V1.x HREFs to V2.0 HREFs.  This is needed because the
        /// help file HREF format has changed for .NET 2.0
        /// </summary>
        /// <param name="line">HREF url to be updated</param>
        /// <param name="documentor"></param>
        /// <param name="project"></param>
        /// <param name="valid"></param>
        /// <returns>Updated url</returns>
        private static string FixV20HRef(string line, MsdnDocumenter documentor, string filePath, ref bool validLocalUrl)
        {
            validLocalUrl = true; // Only for local URLs, ignore otherwise

            // Class URLs
            string url = line.Substring(6); // Remove HREF="
            int index = url.IndexOf("\"");
            url = url.Substring(0, index); // Remove trailing "
            if (url.IndexOf("ms-help://") == -1)
            {
                validLocalUrl = File.Exists(filePath + url);
                return line; // not a MS-Help URL
            }

            if (url.IndexOf("Class") != -1 && url.IndexOf("Topic") != -1)
            {
                url = url.Substring(url.IndexOf("html/") + 5); // Remove html/
                string partialUrl = url.Replace("Topic.htm", ""); // Remove Topic.htm
                string methodName = partialUrl.Substring(partialUrl.IndexOf("Class") + 5); // Retrieve name of method to look for duplicates

                partialUrl = partialUrl.Substring(0, partialUrl.IndexOf("Class"));

                StringBuilder newLink = new StringBuilder();
                string ns = "";
                for (int i = partialUrl.Length - 1; i > 0; i--)
                {
                    if (Char.IsUpper(partialUrl[i]))
                    {
                        ns = partialUrl.Substring(0, i);
                        if (namespaces.ContainsKey(ns))
                        {
                            string className = partialUrl.Replace(ns, "");
                            string fullObjectName = namespaces[ns] + className;

                            newLink.Append("T_");
                            newLink.Append(namespaces[ns] + className);
                            newLink.Append("_Members.htm");
                            break;
                        }
                    }
                }
                line = line.Replace(url, newLink.ToString());
                line = VerifyMethodUrl(line, documentor, methodName);
            }
            else if (line.IndexOf("_") == -1) // Namespace URL
            {
                url = url.Substring(url.IndexOf("html/") + 5); // Remove html/
                StringBuilder newLink = new StringBuilder();
                newLink.Append("N_");
                for(int i = 0; i < url.Length; i++)
                {
                    if (i > 0)
                        if (!char.IsUpper(url[i - 1]) && char.IsUpper(url[i]))
                            newLink.Append("_");
                    newLink.Append(url[i]);
                }
                line = line.Replace(url, newLink.ToString());
                line = VerifyUrl(line, documentor);
            }
            else // otherwise, just verify URL is in correct file
            {
                line = VerifyUrl(line, documentor);
            }
            return line;
        }

        /// <summary>
        /// Initialize namespace dictionary
        /// </summary>
        public static void InitializeNamespaces(Project project)
        {
            // If we don't have namespaces list yet, go through each referenced assembly,
            // load the assembly, get the types, then cache the namespaces for all public types.
            if (namespaces.Count == 0)
            {
                foreach (AssemblySlashDoc doc in project.AssemblySlashDocs)
                {
                    Assembly theAssembly = Assembly.LoadFrom(doc.Assembly);
                    AssemblyName[] assemblies = theAssembly.GetReferencedAssemblies();
                    foreach (AssemblyName an in assemblies)
                    {
                        Assembly assembly = Assembly.LoadWithPartialName(an.Name);
                        // Fix from David Smith, March 30, 2006 @ 4:33 pm
                        if (assembly != null)
                        {
                            Type[] assemblyTypes = assembly.GetTypes();
                            foreach (Type type in assemblyTypes)
                            {
                                if (type.IsPublic)
                                {
                                    namespaces[type.Namespace] = type.Namespace.Replace('.', '_') + '_';
                                    namespaces[type.Namespace.Replace(".", "")] = type.Namespace.Replace('.', '_') + '_';
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read HTML file one line at a time.  If HREF is found, call Fix20HRef to update for V2.0 formatting
        /// </summary>
        /// <param name="project">project object containing properties for help file</param>
        /// <param name="fullPath">Fully qualified HTML file path</param>
        /// <param name="encoding">Current encoding for the HTML file</param>
        /// <param name="documentor">MSDN IDocumentor needed by helper methods</param>
        public static void UpdateHtmlHrefs(Project project, string fullPath, Encoding encoding, MsdnDocumenter documentor)
        {
            InitializeNamespaces(project);

            // For generated .html file, create a temporary (.tmp) file with corrected ms-help URLs.  When
            // finished correcting URLs, swap temporary file for generated one, then delete original file.
            string filePath = fullPath.Substring(0,fullPath.LastIndexOf("\\") + 1);
            string tempFile = fullPath + ".tmp";
            using (StreamReader streamReader = new StreamReader(File.Open(fullPath, FileMode.Open), encoding))
            using (StreamWriter streamWriter = new StreamWriter(File.Open(tempFile, FileMode.Create), encoding))
            {
                // Search generated .html file for href links.  For each link, call FixV20HRef to verify/correct link.
                string line;
                do
                {
                    line = streamReader.ReadLine();
                    if (line != null)
                    {
                        int index = 0;
                        while (line.IndexOf("href=\"",index) > 0)
                        {
                            index = line.IndexOf("href=\"", index);
                            int indexEnd = line.IndexOf("\"", index + 6);
                            string href = line.Substring(index, (indexEnd - index + 1));
                            bool validLocalUrl = false;
                            if (href.IndexOf("ms-help://") == -1)
                            {
                                line = FixLocalUrl(line,href,filePath,index);
                            }
                            else
                            {
                                string newHRef = FixV20HRef(href, documentor, filePath, ref validLocalUrl);
                                line = line.Replace(href, newHRef);
                            }
                            index++; // look for another HREF
                        }
                        streamWriter.WriteLine(line);
                    }
                }
                while (line != null);
            }
            // Swap updated HTML file with original HTML file, then delete original file.
            System.IO.File.Replace(tempFile, fullPath, fullPath + ".bak");
            System.IO.File.Delete(fullPath + ".bak");
        }
    }
}
