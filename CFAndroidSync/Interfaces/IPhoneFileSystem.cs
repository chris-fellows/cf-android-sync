using CFAndroidSync.Models;

namespace CFAndroidSync.Interfaces
{
    /// <summary>
    /// Interface for accessing phone file system
    /// </summary>
    public interface IPhoneFileSystem
    {
        /// <summary>
        /// Gets all folders that are children of folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        List<FolderDetails> GetFolders(string folder);

        /// <summary>
        /// Gets files in folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        List<FileDetails> GetFiles(string folder);

        /// <summary>
        /// Copies local file to remote file
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="remoteFile"></param>
        void CopyLocalFileTo(string localFile, string remoteFile);

        /// <summary>
        /// Copies local folder to remote folder
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        void CopyLocalFolderTo(string localFolder, string remoteFolder);

        /// <summary>
        /// Copies remote file to local file
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="localFile"></param>
        void CopyFileFrom(string remoteFile, string localFile);

        /// <summary>
        /// Copies remote folder to local folder
        /// </summary>
        /// <param name="remoteFolder"></param>
        /// <param name="localFolder"></param>
        void CopyFolderFrom(string remoteFolder, string localFolder);

        /// <summary>
        /// Deletes remote file
        /// </summary>
        /// <param name="file"></param>
        void DeleteFile(string file);

        /// <summary>
        /// Deletes remote folder
        /// </summary>
        /// <param name="file"></param>
        void DeleteFolder(string folder);
    }
}
