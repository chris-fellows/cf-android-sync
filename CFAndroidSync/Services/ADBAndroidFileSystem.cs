using CFAndroidSync.Exceptions;
using CFAndroidSync.Interfaces;
using CFAndroidSync.Models;
using System.Diagnostics;
using System.Text;

namespace CFAndroidSync.Services
{
    /// <summary>
    /// Android file system via adb.exe
    /// </summary>
    public class ADBAndroidFileSystem : IPhoneFileSystem
    {
        // TODO: Remove this
        private string _adbPathToEXE = "C:\\Program Files (x86)\\Android\\android-sdk\\platform-tools";

        private ProcessStartInfo CreateProcessStartInfo(string arguments)
        {
            return new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Path.Combine(_adbPathToEXE, "adb.exe"),
                // -L : Follows symolic links. E.g. sdcard is a symbolic link
                // -p : Adds a / at the end of folder names
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        public bool IsConnected
        {
            get
            {
                var isConnected = false;

                using (var process = new Process())
                {
                    process.StartInfo = CreateProcessStartInfo($"devices");
                    process.Start();
                    process.WaitForExit();

                    // Synchronously read the standard output of the spawned process.
                    StreamReader readerOutput = process.StandardOutput;                                                          

                    // Process output                    
                    while (!readerOutput.EndOfStream)
                    {
                        var line = readerOutput.ReadLine(); 
                        if (!line.Equals("List of devices attached") && line.Length > 0)
                        {
                            isConnected = true;
                        }                        
                    }
                }

                return isConnected;
            }
        }

        public List<FolderDetails> GetFolders(string folder)
        {
            var folders = new List<FolderDetails>();            

            using (var process = new Process())
            {
                process.StartInfo = CreateProcessStartInfo($"shell ls -L -p '{folder}'");              
                process.Start();
                process.WaitForExit();                

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                // Process errors
                var errorMessage = $"Error {process.ExitCode} getting folders";
                while (!readerError.EndOfStream)
                {
                    errorMessage = readerError.ReadLine();
                    //MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }
                
                if (process.ExitCode != 0)
                {
                    throw new PhoneException(errorMessage);                    
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
                }
            }

            return folders;
        }

        public List<FileDetails> GetFiles(string folder)
        {
            var files = new List<FileDetails>();
            
            using (var process = new Process())
            {
                process.StartInfo = CreateProcessStartInfo($"shell ls -L -p -a '{folder}'");

                /*
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
                */
                
                process.Start();
                process.WaitForExit();

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                // Process errors
                var errorMessage = $"Error {process.ExitCode} getting files";
                while (!readerError.EndOfStream)
                {
                    errorMessage = readerError.ReadLine();
                    //MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                if (process.ExitCode != 0)
                {
                    throw new PhoneException(errorMessage);
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
            using (var process = new Process())
            {
                process.StartInfo = CreateProcessStartInfo($"push '{localFile}' '{remoteFile}'");
                process.Start();
                process.WaitForExit();

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                StringBuilder output = new StringBuilder("");
                while (!readerOutput.EndOfStream)
                {
                    output.AppendLine(readerOutput.ReadLine());
                }

                // Process errors
                var errorMessage = $"Error {process.ExitCode} copying local file to";
                while (!readerError.EndOfStream)
                {
                    errorMessage = readerError.ReadLine();
                    //MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                if (process.ExitCode != 0)
                {
                    throw new PhoneException(errorMessage);
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

        public void CopyFileFrom(string remoteFile, string localFile)
        {
            var localFolder = Path.GetDirectoryName(localFile);
            Directory.CreateDirectory(localFolder);

            Char quotes = '"';
            using (var process = new Process())
            {
                process.StartInfo = CreateProcessStartInfo($"pull {quotes}{remoteFile}{quotes} {quotes}{localFolder}{quotes}");

                process.Start();
                process.WaitForExit();

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                // Process errors
                var errorMessage = $"Error {process.ExitCode} copying file from";
                while (!readerError.EndOfStream)
                {
                    errorMessage = readerError.ReadLine();
                    //MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                if (process.ExitCode != 0)
                {
                    throw new PhoneException(errorMessage);
                }

                // Process output
                while (!readerOutput.EndOfStream)
                {
                    var line = readerOutput.ReadLine();
                    int xxx = 1000;
                }
            }
        }

        public void CopyFolderFrom(string remoteFolder, string localFolder)
        {
            var folderName = remoteFolder.Split('/').Last();

            // Copy files
            var remoteFiles = GetFiles(remoteFolder);
            foreach(var currentRemoteFile in remoteFiles)
            {
                var localFile = Path.Combine(localFolder, folderName);
                CopyFileFrom(currentRemoteFile.Path, localFile);
            }

            // Copy sub-folders
            var remoteFolders = GetFolders(remoteFolder);
            foreach(var remoteSubFolder in remoteFolders)
            {
                var localSubFolder = Path.Combine(localFolder, remoteSubFolder.Name);
                CopyFolderFrom(remoteSubFolder.Path, localSubFolder);
            }
        }

        public void DeleteFile(string file)
        {            
            using (var process = new Process())
            {
                process.StartInfo = CreateProcessStartInfo($"shell rm -f '{file}'");
                
                process.Start();
                process.WaitForExit();

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                // Process errors
                var errorMessage = $"Error {process.ExitCode} deleting file";
                while (!readerError.EndOfStream)
                {
                    errorMessage = readerError.ReadLine();
                    //MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                if (process.ExitCode != 0)
                {
                    throw new PhoneException(errorMessage);
                }

                // Process output
                while (!readerOutput.EndOfStream)
                {
                    var line = readerOutput.ReadLine();
                    int xxx = 1000;
                }
            }
        }

        public void DeleteFolder(string folder)
        {
            using (var process = new Process())
            {
                // -rR : Remove recursively
                process.StartInfo = CreateProcessStartInfo($"shell rm -rR '{folder}'");  
                process.Start();
                process.WaitForExit();

                // Synchronously read the standard output of the spawned process.
                StreamReader readerOutput = process.StandardOutput;
                StreamReader readerError = process.StandardError;

                // Process errors
                var errorMessage = $"Error {process.ExitCode} deleting folder";
                while (!readerError.EndOfStream)
                {
                    errorMessage = readerError.ReadLine();
                    //MessageBox.Show($"Error={line}");
                    int xxx1 = 1000;
                }

                if (process.ExitCode != 0)
                {
                    throw new PhoneException(errorMessage);
                }

                // Process output
                while (!readerOutput.EndOfStream)
                {
                    var line = readerOutput.ReadLine();
                    int xxx = 1000;
                }
            }
        }
    }
}
