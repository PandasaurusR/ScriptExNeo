using System;
using System.IO;
using System.Reflection;

namespace ScriptExNeo.Assets {
    public static class Asset {
        // NOTE: All paths for embedded resources use '.' separators
        public static readonly string CrashText = GetFromResources("Assets.Raw.CrashText.txt");
        public static readonly string TitleSplashText = GetFromResources("Assets.Raw.TitleSplash.txt");

        /// <summary>
        /// Return embedded text file as string. 
        /// Source: <see href="https://www.codeproject.com/Questions/96089/C-How-to-read-embedded-resource-and-return-as-stri"/>
        /// </summary>
        private static string GetFromResources(string resourceName) {
            Assembly assem = Assembly.GetExecutingAssembly();

            using (Stream stream = assem.GetManifestResourceStream(assem.GetName().Name + '.' + resourceName)) {
                using (var reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
