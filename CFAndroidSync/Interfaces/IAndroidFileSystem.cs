using CFAndroidSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFAndroidSync.Interfaces
{
    /// <summary>
    /// Interface for accessing Android file system
    /// </summary>
    public interface IAndroidFileSystem
    {
        List<FolderDetails> GetFolders(string folder);

        List<FileDetails> GetFiles(string folder);

        void CopyLocalFileTo(string localFile, string remoteFile);

        void CopyLocalFolderTo(string localFolder, string remoteFolder);
    }
}
