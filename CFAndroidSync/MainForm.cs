using CFAndroidSync.Controls;
using CFAndroidSync.Exceptions;
using CFAndroidSync.Interfaces;
using CFAndroidSync.Models;
using CFAndroidSync.Services;
using CFPlaylistManager.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace CFAndroidSync
{
    public partial class MainForm : Form
    {
        private enum MyNodeTypes
        {
            Unknown,
            FileDetails,
            FolderDetails,
        }

        private readonly IPhoneFileSystem _phoneFileSystem;        

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(IPhoneFileSystem phoneFileSystem)
        {
            InitializeComponent();

            DisplayStatus("Initialising");

            _phoneFileSystem = phoneFileSystem;
            
            DisplayStatus("Ready");         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tscCategory.Items.Clear();
            tscCategory.Items.Add("File system");
            tscCategory.SelectedIndex = 0;

            // Warn if phone not connected
            if (!_phoneFileSystem.IsConnected)
            {
                cmsFolder.Visible = false;  // Prevent folder context actions
                MessageBox.Show("Please ensure that the Android phone is properly connected", "Warning", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Refreshes file system list
        /// </summary>
        private void RefreshFileSystemTree()
        {
            DisplayStatus("Refreshing file system");
            
            tvwFileSystem.Nodes.Clear();
            
            // Display top level folders
            DisplayFileSystemFolderInTree("/", tvwFileSystem, false, null);

            // Expand top level folder nodes
            tvwFileSystem.Nodes[0].Expand();

            DisplayStatus("Ready");
        }

        /// <summary>
        /// Displays file system folder for the specific folder in the tree. Includes sub-folders if requested
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="treeView"></param>
        /// <param name="includeSubFolders"></param>
        /// <param name="parentFolderNode"></param>
        private void DisplayFileSystemFolderInTree(string folder, TreeView treeView, bool includeSubFolders,
                                           TreeNode parentFolderNode = null)
        {
            // If root then add parent node. Don't add context menu
            if (parentFolderNode == null)
            {                
                var rootFolder = new FolderDetails()
                {
                    Name = "/",
                    Path = "/"
                };
                var rootNode = tvwFileSystem.Nodes.Add(GetFileSystemNodeKey(rootFolder), rootFolder.Name);
                rootNode.Tag = rootFolder;
                parentFolderNode = rootNode;
            }

            // Get folders, handle errors (E.g. Permissions error
            List<FolderDetails> currentFolders = new List<FolderDetails>();
            try
            {

                currentFolders = _phoneFileSystem.GetFolders(folder);
            }
            catch(PhoneException phoneException)   // Permission denied?
            {                
                parentFolderNode.Text += $" [Error: {phoneException.Message}]";                
            }

            if (currentFolders != null)   // Got folders
            {
                // Add folder nodes to parent
                foreach (var currentFolder in currentFolders)
                {
                    // Add folder node
                    TreeNode nodeFolder = parentFolderNode.Nodes.Add(GetFileSystemNodeKey(currentFolder), currentFolder.Name);
                    nodeFolder.Tag = currentFolder;
                    nodeFolder.ContextMenuStrip = cmsFolder;
                    
                    // Process sub-folders
                    if (includeSubFolders)
                    {
                        DisplayFileSystemFolderInTree(currentFolder.Path, treeView, includeSubFolders, nodeFolder);
                    }
                    else
                    {
                        // Check if sub-folders and add a dummy node so that we can populate the sub-folders when the user
                        // expands the node. This is more efficient then displaying the whole Android file system.
                        try
                        {
                            var subFolders = _phoneFileSystem.GetFolders(currentFolder.Path);
                            if (subFolders.Any())
                            {
                                var nodeDummy = nodeFolder.Nodes.Add("Dummy");
                            }
                        }
                        catch (PhoneException phoneException)    // Permission denied?
                        {                          
                            nodeFolder.Text += $" [Error: {phoneException.Message}]";                           
                        }
                    }
                }
            }
        }

        private void tscCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tscCategory.SelectedIndex)
            {
                case 0:
                    RefreshFileSystemTree();
                    break;
            }
        }

        /// <summary>
        /// Copies selected folder to Android file system
        /// </summary>
        private void CopySelectedFolderToFileSystem(string localFolder, string remoteFolder)
        {
            var folderName = new DirectoryInfo(localFolder).Name;

            //string remoteFolder = $"/sdcard/Music/{folderName}";
            //string remoteFolder = $"/sdcard/Music/Podcasts";
            //string remoteFolder = $"/sdcard/Music/RadioStreams";

            // Set function to filter if file copied
            Func<string, bool> isCheckCopyFile = (file) =>
            {
                return true;    // Copy all files
            };

            string scriptFile = $"D:\\Data\\Dev\\C#\\cf-media-player-mob-local\\EmulatorScripts\\CopyFolderToEmulator-{folderName}.cmd";
            if (Directory.Exists(localFolder))
            {
                ADBUtilities.ScriptCopyFolderToDevice(localFolder, remoteFolder, isCheckCopyFile, scriptFile);
            }

            MessageBox.Show("Files copied");

            /*
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileExtensionsToIgnore = new[] { ".ini", ".db" };       // desktop.ini, thumbs.db

                // Set function to filter which files to copy
                Func<string, bool> isCheckCopyFile = (localFile) =>
                {
                    string fileExtension = Path.GetExtension(localFile).ToLower();
                    return !fileExtensionsToIgnore.Contains(fileExtension);
                };

                var localFolder = dialog.SelectedPath;
                var folderName = new DirectoryInfo(localFolder).Name;

                //string remoteFolder = $"/sdcard/Music/{folderName}";
                string remoteFolder = $"/sdcard/Music/Podcasts";
                //string remoteFolder = $"/sdcard/Music/RadioStreams";

                string scriptFile = $"D:\\Data\\Dev\\C#\\cf-media-player-mob-local\\EmulatorScripts\\CopyFolderToEmulator-{folderName}.cmd";
                if (Directory.Exists(localFolder))
                {
                    ADBUtilities.ScriptCopyFolderToDevice(localFolder, remoteFolder, isCheckCopyFile, scriptFile);
                }

                MessageBox.Show("Files copied");
            }
            */
        }

        private void CopySelectedFolderToFileSystem()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileExtensionsToIgnore = new[] { ".ini", ".db" };       // desktop.ini, thumbs.db

                // Set function to filter which files to copy
                Func<string, bool> isCheckCopyFile = (localFile) =>
                {
                    string fileExtension = Path.GetExtension(localFile).ToLower();
                    return !fileExtensionsToIgnore.Contains(fileExtension);
                };

                var localFolder = dialog.SelectedPath;
                var folderName = new DirectoryInfo(localFolder).Name;

                //string remoteFolder = $"/sdcard/Music/{folderName}";
                string remoteFolder = $"/sdcard/Music/Podcasts";
                //string remoteFolder = $"/sdcard/Music/RadioStreams";

                string scriptFile = $"D:\\Data\\Dev\\C#\\cf-media-player-mob-local\\EmulatorScripts\\CopyFolderToEmulator-{folderName}.cmd";
                if (Directory.Exists(localFolder))
                {
                    ADBUtilities.ScriptCopyFolderToDevice(localFolder, remoteFolder, isCheckCopyFile, scriptFile);
                }

                MessageBox.Show("Files copied");
            }
        }

        private void DisplayStatus(string status)
        {
            tssStatus.Text = $" {status}";
        }

        private void tvwFileSystem_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // If node contains dummy node then read replace with nodes for sub-folders
            if (e.Node != null &&
                e.Node.Nodes.Count > 0 &&
                e.Node.Nodes[0].Text.Equals("Dummy"))
            {
                // Clear dummy node                
                e.Node.Nodes.Clear();

                DisplayStatus("Reading folder");

                // Display folder
                FolderDetails folder = (FolderDetails)e.Node.Tag;
                DisplayFileSystemFolderInTree(folder.Path, tvwFileSystem, false, e.Node);

                DisplayStatus("Ready");
            }
        }

        private void tvwFileSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();

            if (e.Node.Tag is FolderDetails)
            {
                var folder = (FolderDetails)e.Node.Tag;

                    DisplayStatus("Displaying files");
                    
                    var folderControl = new RemoteFolderControl(_phoneFileSystem);
                    folderControl.Dock = DockStyle.Fill;
                    folderControl.ModelToView(folder);
                    splitContainer1.Panel2.Controls.Add(folderControl);

                    DisplayStatus("Ready");
            }
        }

        /// <summary>
        /// Copies local files to phone
        /// </summary>
        private void CopyLocalFilesTo()
        {
            FolderDetails folder = (FolderDetails)tvwFileSystem.SelectedNode.Tag;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Local Files",
                CheckPathExists = true,
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK &&
                openFileDialog.FileNames.Any())
            {
                if (MessageBox.Show($"Copy selected files to {folder.Path}?", "Copy Local Files To", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DisplayStatus("Copying selected files...");

                    foreach (var localFile in openFileDialog.FileNames)
                    {
                        var remoteFile = $"{folder.Path}/{Path.GetFileName(localFile)}";
                        _phoneFileSystem.CopyLocalFileTo(localFile, remoteFile);
                    }

                    DisplayStatus("Ready");

                    MessageBox.Show("Files copied", "Copy Local Files To");

                    RefreshFileSystemTree();
                }
            }
        }

        private void copyLocalFilesToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyLocalFilesTo();
        }

        //private MyNodeTypes GetNodeType(TreeNode node)
        //{
        //    if (node.Tag is FolderDetails) return MyNodeTypes.FolderDetails;
        //    if (node.Tag is FileDetails) return MyNodeTypes.FileDetails;
        //    return MyNodeTypes.Unknown;
        //}

        private void tvwFileSystem_MouseUp(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Copies local folder to phone
        /// </summary>
        /// <param name="folder"></param>
        public void CopyLocalFolderTo(FolderDetails folder)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                ShowPinnedPlaces = false,
                ShowNewFolderButton = false,
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var localFolder = folderBrowserDialog.SelectedPath;
                var folderName = new DirectoryInfo(localFolder).Name;
                var remoteFolder = $"{folder.Path}/{folderName}";

                if (MessageBox.Show($"Copy local folder {localFolder} to {remoteFolder}?", "Copy Local Folder To", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DisplayStatus("Copying local folder...");
                    _phoneFileSystem.CopyLocalFolderTo(localFolder, remoteFolder);
                    DisplayStatus("Ready");

                    MessageBox.Show("Folder copied", "Copy Local Folder To");

                    RefreshFileSystemTree();
                }
            }
        }

        private void copyLocalFolderToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderDetails folder = (FolderDetails)tvwFileSystem.SelectedNode.Tag;
            CopyLocalFolderTo(folder);
        }

        /// <summary>
        /// Copies phone folder to local folder
        /// </summary>
        /// <param name="folder"></param>
        public void CopyFolderToLocalFolder(FolderDetails folder)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                ShowPinnedPlaces = false,
                ShowNewFolderButton = true,
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var localFolder = folderBrowserDialog.SelectedPath;

                if (MessageBox.Show($"Copy remote folder {folder.Path} to {localFolder}?", "Copy Folder To Local Folder", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DisplayStatus("Copying remote folder...");
                    _phoneFileSystem.CopyFolderFrom(folder.Path, localFolder);
                    DisplayStatus("Ready");

                    MessageBox.Show("Folder copied", "Copy Folder To Local Folder");
                }
            }
        }

        private void copyFolderToLocalFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderDetails folder = (FolderDetails)tvwFileSystem.SelectedNode.Tag;
            CopyFolderToLocalFolder(folder);
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderDetails folder = (FolderDetails)tvwFileSystem.SelectedNode.Tag;            

            if (MessageBox.Show($"Delete remote folder {folder.Path}?", "Delete Remote Folder", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DisplayStatus("Deleting remote folder...");
                _phoneFileSystem.DeleteFolder(folder.Path);                
                DisplayStatus("Ready");

                MessageBox.Show("Folder deleted", "Delete Remote Folder");

                // Refresh the parent folder node
                TreeNode parentFolderNode = tvwFileSystem.SelectedNode.Parent;
                if (parentFolderNode == null)   // Deleted top level folder, no parent, refresh whole treew
                {
                    RefreshFileSystemTree();
                }
                else
                {
                    FolderDetails parentFolder = (FolderDetails)parentFolderNode.Tag;
                    RefreshFileSystemFolderInTree(parentFolder);
                }
            }
        }

        /// <summary>
        /// Refreshes the node for the particular folder in the tree. It's more efficient the refreshing the whole
        /// tree/
        /// </summary>
        /// <param name="folder"></param>
        private void RefreshFileSystemFolderInTree(FolderDetails folder)
        {
            var folderNode = GetFileSystemTreeNode(folder, null);
            folderNode.Nodes.Clear();

            DisplayFileSystemFolderInTree(folder.Path, tvwFileSystem, false, folderNode);
        }

        private static string GetFileSystemNodeKey(FolderDetails folder)
        {
            return $"Folder.{folder.Path}";
        }

        private TreeNode GetFileSystemTreeNode(FolderDetails folder, TreeNode currentNode)
        {
            var folderNodeKey = GetFileSystemNodeKey(folder);

            if (currentNode == null)
            {
                foreach(TreeNode node in tvwFileSystem.Nodes)
                {
                    var nodeFound = GetFileSystemTreeNode(folder, node);
                    if (nodeFound != null) return nodeFound;
                }
            }
            else
            {
                if (currentNode.Name == folderNodeKey) return currentNode;

                // Check child nodes
                foreach (TreeNode node in currentNode.Nodes)
                {
                    if (node.Name == folderNodeKey) return node;

                    var foundNode = GetFileSystemTreeNode(folder, node);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }

            return null;
        }
    }
}
