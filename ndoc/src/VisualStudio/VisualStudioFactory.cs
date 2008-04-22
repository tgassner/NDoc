using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.XPath;

namespace NDoc.VisualStudio {
    public class VisualStudioFactory {
        static public ISolution CreateSolution(string slnPath) {

            StreamReader reader = null;
            using (reader = new StreamReader(slnPath)) {
                string line = reader.ReadLine();
                while (line != null && line.Length == 0) {
                    line = reader.ReadLine();
                }

                if (line == null || !line.StartsWith("Microsoft Visual Studio Solution File")) {
                    throw new ApplicationException("This is not a Microsoft Visual Studio Solution file.");
                }

                if (line.EndsWith("Format Version 10.00") || line.EndsWith("Format Version 9.00")) {
                    return new Solution0508(slnPath);

                }
                
                if (line.EndsWith("Format Version 8.00") || line.EndsWith("Format Version 7.00")) {
                    return new Solution0203(slnPath);
                }

                throw new ApplicationException("The version of Microsoft Visual Studio can not be detected.");
            }
        }

        static public IProject CreateProject(ISolution solution, string id, string name, ProjektType type) {
            switch (solution.Ide) {
                case IdeType.Studio2002:
                case IdeType.Studio2003:
                    return new Project0203(solution,id,name, type);
                    break;
                case IdeType.Studio2005:
                case IdeType.Studio2008:
                    return new Project0508(solution, id, name, type);
                    break;
                default:
                    return null;
            }
        }

        //static public IProjectConfig CreateProjectConfig(XPathNavigator navigator, IdeType ide) {
        //    switch (ide) {
        //        case IdeType.Studio2002:
        //        case IdeType.Studio2003:
        //            return new ProjectConfig0203(navigator);
        //            break;
        //        case IdeType.Studio2005:
        //        case IdeType.Studio2008:
        //            return new ProjectConfig0508(navigator);
        //            break;
        //        default:
        //            return null;
        //    }
        //}
    }
}
