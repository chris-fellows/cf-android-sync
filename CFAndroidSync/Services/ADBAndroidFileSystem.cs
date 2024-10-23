using CFAndroidSync.Interfaces;
using CFAndroidSync.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace CFAndroidSync.Services
{
    /// <summary>
    /// Android file system via adb.exe
    /// </summary>
    public class ADBAndroidFileSystem : IAndroidFileSystem
    {
        // TODO: Remove this
        private string _adbPathToEXE = "C:\\Program Files (x86)\\Android\\android-sdk\\platform-tools";

        public List<FolderDetails> GetFolders(string folder)
        {
            var folders = new List<FolderDetails>();
            Char quotes = '"';

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,                    
                    FileName = Path.Combine(_adbPathToEXE, "adb.exe"),
                    // -L : Follows symolic links. E.g. sdcard is a symbolic link
                    // -p : Adds a / at the end of folder names

                    Arguments = $"shell ls -L -p {quotes}{folder}{quotes}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                               
                process.Start();
                process.WaitForExit();                

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;
                
                // Process errors
                while (!readerError.EndOfStream)
                {
                    var line = readerError.ReadLine();
                    //MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                // Process output
                while (!readerOutput.EndOfStream)
                {
                    var line = readerOutput.ReadLine();
                    //System.Diagnostics.Debug.WriteLine(line);
                    var itemName = line;

                    if (itemName.EndsWith('/') &&      // Folders end with /
                        Array.IndexOf(new[] { ".", ".." }, itemName) == -1)
                        
                    {
                        itemName = itemName.Substring(0, itemName.Length - 1);
                        var folderInfo = new FolderDetails()
                        {
                            Name = itemName,
                            Path =  folder == "/" ? $"{folder}{itemName}" : $"{folder}/{itemName}"
                        };
                        folders.Add(folderInfo);
                    }
                    int xxx2 = 1000;
                }
                
                int xxx = 1000;
            }

            return folders;
        }

        public List<FileDetails> GetFiles(string folder)
        {
            var files = new List<FileDetails>();
            Char quotes = '"';

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = Path.Combine(_adbPathToEXE, "adb.exe"),
                    // -L : Follows symolic links. E.g. sdcard is a symbolic link
                    // -p : Adds a / at the end of folder names
                    // -F : append / dir * exe @sym | FIFO
                    // -f : Files
                    // -a : All files including hidden
                    Arguments = $"shell ls -L -p -a {quotes}{folder}{quotes}",    
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                
                process.Start();
                process.WaitForExit();

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                // Process errors
                while (!readerError.EndOfStream)
                {
                    var line = readerError.ReadLine();
                    MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                // Process output
                while (!readerOutput.EndOfStream)
                {
                    var line = readerOutput.ReadLine();
                    //System.Diagnostics.Debug.WriteLine(line);
                    var itemName = line;

                    if (!itemName.EndsWith('/') &&      // Folders end with /
                        Array.IndexOf(new[] { ".", ".." }, itemName) == -1)

                    {                        
                        var fileDetails = new FileDetails()
                        {
                            Name = itemName,
                            Path = $"{folder}/{itemName}"
                        };
                        files.Add(fileDetails);
                    }                    
                }                
            }

            return files;
        }

        public void CopyLocalFileTo(string localFile, string remoteFile)
        {            
            Char quotes = '"';
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = Path.Combine(_adbPathToEXE, "adb.exe"),                    
                    Arguments = $"push {quotes}{localFile}{quotes} {quotes}{remoteFile}{quotes}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                process.Start();
                process.WaitForExit();

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                // Process errors
                while (!readerError.EndOfStream)
                {
                    var line = readerError.ReadLine();
                    MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                // Process output
                while (!readerOutput.EndOfStream)
                {
                    var line = readerOutput.ReadLine();
                    int xxx = 1000;
                }
            }            
        }

        public void CopyLocalFolderTo(string localFolder, string remoteFolder)
        {
            var localFiles = Directory.GetFiles(localFolder);
            foreach(var localFile in localFiles)
            {
                var remoteFile = $"{remoteFolder}/{Path.GetFileName(localFile)}";

                CopyLocalFileTo(localFile, remoteFile);
            }

            foreach(var localSubFolder in Directory.GetDirectories(localFolder))
            {
                var subFolderName = new DirectoryInfo(localSubFolder).Name;
                var remoteSubFolder = $"{remoteFolder}/{subFolderName}";

                CopyLocalFolderTo(localSubFolder, remoteSubFolder);
            }
        }
    }
}
