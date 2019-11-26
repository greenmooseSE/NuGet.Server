using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace NuGet.Server.Modules
{
    public static class IgnorePathsProvider
    {
        private static string IgnorePathsXml { get; } = "IgnorePaths.xml";

        public static bool ShouldAuthenticate(HttpContext context)
        {
            var ignorePaths = context.Cache["IgnorePathsList"] as List<string> ?? PopulateIgnorePaths(context);
            var currentPath = context.Request.Path;
            foreach (var ignorePathRegex in ignorePaths)
                if (Regex.IsMatch(currentPath, ignorePathRegex, RegexOptions.IgnoreCase))
                    return false;

            return true;
        }

        private static List<string> PopulateIgnorePaths(HttpContext context)
        {
            List<string> stringList;
            using (var fileStream = new FileStream(context.Server.MapPath($"~/App_Data/{IgnorePathsXml}"),
                FileMode.Open, FileAccess.Read))
            {
                using (var textReader =
                    XmlDictionaryReader.CreateTextReader((Stream) fileStream, new XmlDictionaryReaderQuotas()))
                {
                    stringList = (List<string>) new DataContractSerializer(typeof(List<string>)).ReadObject(textReader);
                }
            }

            context.Cache["IgnorePathsList"] = (object) stringList;
            return stringList;
        }
    }
}