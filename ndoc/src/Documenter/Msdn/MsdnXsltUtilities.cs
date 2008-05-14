using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.Msdn
{
	/// <summary>
	/// Provides an extension object for the xslt transformations.
	/// </summary>
	public class MsdnXsltUtilities
	{
		private const string sdkDoc10BaseNamespace = "MS.NETFrameworkSDK";
        private const string sdkDoc11BaseNamespace = "MS.NETFrameworkSDKv1.1";
        private const string sdkDoc20BaseNamespace = "MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en";
        private const string sdkDoc30BaseNamespace = "MS.VSCC.v90/MS.MSDNQTR.v90.en";
        private const string sdkDoc35BaseNamespace = "";
		private const string helpURL = "ms-help://";
        private string sdkRoot = "/cpref/html/frlrf";
        private string sdk9Root = "/dv_csref/html";
        private const string sdkDocPageExt = ".htm";
		//private const string msdnOnlineSdkBaseUrl = "http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrf";
        private const string msdnOnlineSdkBaseUrl = "http://msdn.microsoft.com/en-us/";
		//private const string msdnOnlineSdkPageExt = ".asp";
        private const string msdnOnlineSdkPageExt = ".aspx";
		private const string systemPrefix = "System.";
		private string frameworkVersion="";
		private string sdkDocBaseUrl; 
		private string sdkDocExt; 
		private StringDictionary fileNames;
		private StringDictionary elemNames;
		private StringCollection descriptions;
		private string encodingString;
        private Dictionary<string, string> map = new Dictionary<string, string>();
        private SdkVersion sdkVersion;
        private bool sdkLinksOnWeb;

		/// <summary>
		/// Initializes a new instance of class MsdnXsltUtilities
		/// </summary>
		/// <param name="fileNames">A StringDictionary holding id to file name mappings.</param>
		/// <param name="elemNames">A StringDictionary holding id to element name mappings</param>
		/// <param name="linkToSdkDocVersion">Specifies the version of the SDK documentation.</param>
		/// <param name="linkToSdkDocLangauge">Specifies the version of the SDK documentation.</param>
		/// <param name="SdkLinksOnWeb">Specifies if links should be to ms online documentation.</param>
		/// <param name="fileEncoding">Specifies if links should be to ms online documentation.</param>
		public MsdnXsltUtilities(
			StringDictionary fileNames, 
			StringDictionary elemNames, 
			SdkVersion  linkToSdkDocVersion,
			string linkToSdkDocLangauge,
			bool SdkLinksOnWeb,
			System.Text.Encoding fileEncoding)
		{
			Reset();

			this.fileNames = fileNames;
			this.elemNames = elemNames;
            this.sdkLinksOnWeb = SdkLinksOnWeb;

            switch (linkToSdkDocVersion) {
                case SdkVersion.SDK_v1_0:
                    frameworkVersion = "1.0";
                    sdkVersion = SdkVersion.SDK_v1_0;
                    break;
                case SdkVersion.SDK_v1_1:
                    frameworkVersion = "1.1";
                    sdkVersion = SdkVersion.SDK_v1_1;
                    break;
                case SdkVersion.SDK_v2_0:
                    frameworkVersion = "2.0";
                    sdkVersion = SdkVersion.SDK_v2_0;
                    break;
                case SdkVersion.SDK_v3_0:
                    frameworkVersion = "3.0";
                    sdkVersion = SdkVersion.SDK_v3_0;
                    break;
                case SdkVersion.SDK_v3_5:
                    frameworkVersion = "3.5";
                    sdkVersion = SdkVersion.SDK_v3_5;
                    break;
                default:
                    Debug.Assert(false);		// remind ourselves to update this list when new framework versions are supported
                    break;
            }

            if (SdkLinksOnWeb)
            {
                sdkDocBaseUrl = msdnOnlineSdkBaseUrl;
                sdkDocExt = msdnOnlineSdkPageExt;
            }
            else
            {
				switch (linkToSdkDocVersion)
				{
					case SdkVersion.SDK_v1_0:
						sdkDocBaseUrl = GetLocalizedFrameworkURL(sdkDoc10BaseNamespace,linkToSdkDocLangauge);
						sdkDocExt = sdkDocPageExt;
						break;
					case SdkVersion.SDK_v1_1:
						sdkDocBaseUrl = GetLocalizedFrameworkURL(sdkDoc11BaseNamespace,linkToSdkDocLangauge);
						sdkDocExt = sdkDocPageExt;
                        break;
                    case SdkVersion.SDK_v2_0:
                        sdkRoot = "/cpref2/html/";
                        sdkDocBaseUrl = GetLocalizedFrameworkURL(sdkDoc20BaseNamespace, linkToSdkDocLangauge);
                        sdkDocExt = sdkDocPageExt;
                        break;
                    case SdkVersion.SDK_v3_0:
                        break;
                    case SdkVersion.SDK_v3_5:
                        break;
					default:
						Debug.Assert( false );		// remind ourselves to update this list when new framework versions are supported
                        break;
				}
            }
			encodingString = "text/html; charset=" + fileEncoding.WebName;

            /// <summary>
            /// System reflection incorrectly reports Enumerator, KeyCollection and ValueCollection base types, so fix it here!
            /// </summary>
            map = new Dictionary<string, string>();
            map.Add("System.Collections.Generic.Enumerator`1", "System.Collections.Generic.IEnumerator`1");
            map.Add("System.Collections.Generic.Enumerator`2", "System.Collections.Generic.IEnumerator`1");
            map.Add("System.Collections.Generic.KeyCollection`2", "System.Collections.Generic.Dictionary`2_KeyCollection");
            map.Add("System.Collections.Generic.ValueCollection`2", "System.Collections.Generic.Dictionary`2_ValueCollection");
		}

		/// <summary>
		/// Reset Overload method checking state.
		/// </summary>
		public void Reset()
		{
			descriptions = new StringCollection();
		}

		/// <summary>
		/// Gets the base Url for system types links.
		/// </summary>
		public string SdkDocBaseUrl
		{
			get { return sdkDocBaseUrl; }
		}

		/// <summary>
		/// Gets the page file extension for system types links.
		/// </summary>
		public string SdkDocExt
		{
			get { return sdkDocExt; }
		}

		/// <summary>
		/// Gets the friendly version number for the framework.
		/// </summary>
		public string FrameworkVersion
		{
			get {return frameworkVersion;}
		}

		/// <summary>
		/// Returns an HRef for a CRef.
		/// </summary>
		/// <param name="cref">CRef for which the HRef will be looked up.</param>
		public string GetHRef(string cref)
		{
			if ((cref.Length < 2) || (cref[1] != ':'))
				return string.Empty;

			if ((cref.Length < 9)
				|| (cref.Substring(2, 7) != systemPrefix))
			{
				string fileName = fileNames[cref];
				if ((fileName == null) && cref.StartsWith("F:"))
					fileName = fileNames["E:" + cref.Substring(2)];

				if (fileName == null)
                {
					return "";
                }
				else
                {
					return fileName;
                }
			}
			else
			{
				switch (cref.Substring(0, 2))
				{
					case "N:":	// Namespace
                        return sdkDocBaseUrl + cref.Substring(2).Replace(".", "") + sdkDocExt;
					case "T:":	// Type: class, interface, struct, enum, delegate
						// pointer types link to the type being pointed to
                        if (frameworkVersion == "2.0" || frameworkVersion == "3.0" || frameworkVersion == "3.5")
                        {
                            string ret = sdkDocBaseUrl + "T_" + cref.Substring(2).Replace(".", "_").Replace("*", "") + sdkDocExt;
                            //string ret = sdkDocBaseUrl + cref.Substring(2).Replace("*", "") + sdkDocExt;
                            return ret;
                        }
						else 
                        {
                            return sdkDocBaseUrl + cref.Substring(2).Replace(".", "").Replace( "*", "" ) + "ClassTopic" + sdkDocExt;
                        }
                    case "F:":	// Field
                    case "P:":	// Property
                    case "M:":	// Method
                    case "E:":	// Event
                            return GetFilenameForSystemMember(cref);
					default:
						return string.Empty;
				}
			}
		}

		/// <summary>
		/// Returns a name for a CRef.
		/// </summary>
		/// <param name="cref">CRef for which the name will be looked up.</param>
		public string GetName(string cref)
		{
			if (cref.Length < 2)
				return cref;

			if (cref[1] == ':')
			{
				if ((cref.Length < 9)
					|| (cref.Substring(2, 7) != systemPrefix))
				{
					string name = elemNames[cref];
					if (name != null)
						return name;
				}

				int index;
				if ((index = cref.IndexOf(".#c")) >= 0)
					cref = cref.Substring(2, index - 2);
				else if ((index = cref.IndexOf("(")) >= 0)
					cref = cref.Substring(2, index - 2);
				else
					cref = cref.Substring(2);
			}

			return cref.Substring(cref.LastIndexOf(".") + 1);
		}

		private string GetFilenameForSystemMember(string id)
		{
			string crefName;
			int index;
			if ((index = id.IndexOf(".#c")) >= 0)
				crefName = id.Substring(2, index - 2) + ".ctor";
			else if ((index = id.IndexOf("(")) >= 0)
				crefName = id.Substring(2, index - 2);
			else
				crefName = id.Substring(2);
			index = crefName.LastIndexOf(".");
			string crefType = crefName.Substring(0, index);
			string crefMember = crefName.Substring(index + 1);
			return sdkDocBaseUrl + crefType.Replace(".", "") + "Class" + crefMember + "Topic" + sdkDocExt;
		}

		/// <summary>
		/// Looks up, whether a member has similar overloads, that have already been documented.
		/// </summary>
		/// <param name="description">A string describing this overload.</param>
		/// <returns>true, if there has been a member with the same description.</returns>
		/// <remarks>
		/// <para>On the members pages overloads are cumulated. Instead of adding all overloads
		/// to the members page, a link is added to the members page, that points
		/// to an overloads page.</para>
		/// <para>If for example one overload is public, while another one is protected,
		/// we want both to appear on the members page. This is to make the search
		/// for suitable members easier.</para>
		/// <para>This leads us to the similarity of overloads. Two overloads are considered
		/// similar, if they have the same name, declaring type, access (public, protected, ...)
		/// and contract (static, non static). The description contains these four attributes
		/// of the member. This means, that two members are similar, when they have the same
		/// description.</para>
		/// <para>Asking for the first time, if a member has similar overloads, will return false.
		/// After that, if asking with the same description again, it will return true, so
		/// the overload does not need to be added to the members page.</para>
		/// </remarks>
		public bool HasSimilarOverloads(string description)
		{
			if (descriptions.Contains(description))
				return true;
			descriptions.Add(description);
			return false;
		}

		/// <summary>
		/// Exposes <see cref="String.Replace(string, string)"/> to XSLT
		/// </summary>
		/// <param name="str">The string to search</param>
		/// <param name="oldValue">The string to search for</param>
		/// <param name="newValue">The string to replace</param>
		/// <returns>A new string</returns>
		public string Replace( string str, string oldValue, string newValue )
		{
			return str.Replace( oldValue, newValue );
		}	

		/// <summary>
		/// returns a localized sdk url if one exists for the <see cref="CultureInfo.CurrentCulture"/>.
		/// </summary>
		/// <param name="searchNamespace">base namespace to search for</param>
		/// <param name="langCode">the localization language code</param>
		/// <returns>ms-help url for sdk</returns>
		private string GetLocalizedFrameworkURL(string searchNamespace, string langCode)
		{
			if (langCode!="en")
			{
				return helpURL + searchNamespace + "." + langCode.ToUpper() + sdkRoot;
			}
			else
			{
				//default to non-localized namespace
				return helpURL + searchNamespace + sdkRoot;
			}
		}

		/// <summary>
		/// Gets HTML ContentType for the system's current ANSI code page. 
		/// </summary>
		/// <returns>ContentType attribute string</returns>
		public string GetContentType()
		{
			return encodingString;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public string GetClassTopic(string fullName)
        {
            string url;

            if (sdkLinksOnWeb) {
                url = sdkDocBaseUrl;
            } else {
                url = sdkDocBaseUrl + "T_";
            }

            if (sdkVersion != SdkVersion.SDK_v2_0 && sdkVersion != SdkVersion.SDK_v3_0 && sdkVersion != SdkVersion.SDK_v3_5)
            {
                fullName = fullName.Replace(".", "");
                fullName = fullName.Replace("[","");
                fullName = fullName.Replace("]","");
                fullName = fullName.Replace(",","");
                fullName = fullName.Replace("*","");
                // <xsl:value-of select="concat($ndoc-sdk-doc-base-url, translate($type-name, '.[,]*', ''), 'ClassTopic', $ndoc-sdk-doc-file-ext)" />
                url = sdkDocBaseUrl + fullName + "ClassTopic" + sdkDocPageExt;
                return url;
            }

            if (map.ContainsKey(fullName))
            {
                url = map[fullName];
                return url;
            }
            string name = fullName;

            // If we have a generic collection, or enumerator, just map it to the correct URL
            if (name.IndexOf("{") != -1)
            {
                string genericClassName = name.Substring(0, name.IndexOf("{"));
                if (map.ContainsKey(genericClassName))
                {
                    name = map[genericClassName];
                }
                else
                {
                    string newValue = name;
                    int genericStart = name.IndexOf("{");
                    if (genericStart != -1)
                    {
                        int openBrackets = 0;
                        int parameters = 1;
                        for (int i = genericStart; i < name.Length; i++)
                        {
                            if (name[i] == '{')
                                openBrackets++;
                            else if (name[i] == '}')
                            {
                                openBrackets--;
                            }
                            else if (name[i] == ',')
                                if (openBrackets == 1)
                                    parameters++;
                        }
                        newValue = name.Substring(0, genericStart);
                        newValue += "`" + parameters.ToString();
                    }
                    if (name != newValue)
                    {
                        // Fix Enumerator, KeyCollection and ValueCollection due to incorrect System Reflection results
                        if (map.ContainsKey(newValue))
                        {
                            newValue = map[newValue];
                        }
                        name = newValue;
                    }
                    url += name.Replace(".", "_");
                    url += sdkDocPageExt;
                }
            }
            else // Fix Class URL
            {
                StringBuilder newLink = new StringBuilder();
                if (name.Contains("`")) {
                    //msdnID = msdnResolver
                    //url += msdnID;
                    map[fullName] = url;
                } else {
                    for (int i = name.Length - 2; i > 0; i--) {
                        if (name[i] == '.') {
                            string ns = name.Substring(0, i);
                            if (MsdnHtmlUtilitiesV20.namespaces.ContainsKey(ns)) {
                                newLink.Append(MsdnHtmlUtilitiesV20.namespaces[ns]);
                                name = name.Substring(i + 1);
                                if (name.IndexOf('[') != -1)
                                    name = name.Substring(0, name.IndexOf('['));
                                newLink.Append(name);
                                if (sdkLinksOnWeb) {
                                    newLink.Replace("_", ".");
                                    newLink.Append(msdnOnlineSdkPageExt);
                                } else {
                                    newLink.Append(sdkDocPageExt);
                                }

                                break;
                            }
                        }
                    }
                    url += newLink.ToString();
                    url = url.Replace("@", "");
                    url = url.Replace("*", "");
                }
            }
            map[fullName] = url;
            return url;
        }
	}
}
