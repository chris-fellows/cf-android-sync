using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CFPlaylistManager.Utilities
{
    internal class ADBUtilities
    {
        public static string PathToADBEXE = "C:\\Program Files (x86)\\Android\\android-sdk\\platform-tools";

        public static void ScriptCopyFolderToDevice(string localFolder, string remoteFolder, 
                        Func<string, bool> isCheckCopyFile, string scriptFile)
        {   
            // Delete existing script
            //if (File.Exists(scriptFile))
            //{
            //    File.Delete(scriptFile);
            //}

            using (var streamWriter = new StreamWriter(scriptFile, true, Encoding.UTF8))
            {
                var lines = GetCopyFolderScriptLines(localFolder, remoteFolder, true, isCheckCopyFile);
                lines.ForEach(line => streamWriter.WriteLine(line));
            }
        }

        public static void ScriptDeleteRemoteFolder(string remoteFolder,string scriptFile)
        {
            Char quotes = '"';
            var line = $"{quotes}{PathToADBEXE}\\adb.exe{quotes} shell rm - r {remoteFolder}";

            using (var streamWriter = new StreamWriter(scriptFile, true, Encoding.UTF8))
            {
                streamWriter.WriteLine(line);                    
            }
        }

        /// <summary>
        /// Gets script lines to copy local folder to remote
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        /// <returns></returns>
        private static List<string> GetCopyFolderScriptLines(string localFolder, string remoteFolder, bool includeSubFolders,
                            Func<string, bool> isCheckCopyFile)
        {
            List<string> lines = new List<string>();
            Char quotes = '"';

            // adb shell rm -r /sdcard/Music/Aerosmith

            // Copy files
            var localFiles = Directory.GetFiles(localFolder);
            foreach (var localFile in localFiles)
            {
                var isCopy = isCheckCopyFile(localFile);
                if (isCopy)
                {
                    string remoteFile = $"{remoteFolder}/{Path.GetFileName(localFile)}";
                    lines.Add($"{quotes}{PathToADBEXE}\\adb.exe{quotes} push {quotes}{localFile}{quotes} {quotes}{remoteFile}{quotes}");
                }
            }

            // Copy sub-folders
            if (includeSubFolders)
            {
                foreach (var localSubFolder in Directory.GetDirectories(localFolder))
                {
                    var folderName = new DirectoryInfo(localSubFolder).Name;
                    string remoteSubFolder = $"{remoteFolder}/{folderName}";

                    lines.AddRange(GetCopyFolderScriptLines(localSubFolder, remoteSubFolder, true, isCheckCopyFile));
                }
            }

            return lines;
        }

    }
}
