using CFAndroidSync.Controls;
using CFAndroidSync.Interfaces;
using CFAndroidSync.Models;
using CFAndroidSync.Services;
using CFPlaylistManager.Utilities;
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

        private readonly IAndroidFileSystem _androidFileSystem;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(IAndroidFileSystem androidFileSystem)
        {
            InitializeComponent();

            DisplayStatus("Initialising");

            _androidFileSystem = new ADBAndroidFileSystem();

            DisplayStatus("Ready");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tscCategory.Items.Clear();
            tscCategory.Items.Add("File system");
            tscCategory.SelectedIndex = 0;
        }

        /// <summary>
        /// Refreshes file system list
        /// </summary>
        private void RefreshFileSystem()
        {
            DisplayStatus("Refreshing file system");

            tvwFileSystem.Nodes.Clear();

            // Display top level folders
            DisplayFileSystemFolder("/", tvwFileSystem, false);

            DisplayStatus("Ready");
        }

        private void DisplayFileSystemFolder(string folder, TreeView treeView, bool includeSubFolders,
                                           TreeNode parentFolderNode = null)
        {
            var currentFolders = _androidFileSystem.GetFolders(folder);

            foreach (var currentFolder in currentFolders)
            {
                TreeNode nodeFolder = null;
                if (parentFolderNode == null)
                {
                    nodeFolder = treeView.Nodes.Add($"Folder.{currentFolder.Path}", currentFolder.Name);
                }
                else
                {
                    nodeFolder = parentFolderNode.Nodes.Add($"Folder.{currentFolder.Path}", currentFolder.Name);
                }
                nodeFolder.Tag = currentFolder;
                nodeFolder.ContextMenuStrip = cmsFolder;

                // Process sub-folders
                if (includeSubFolders)
                {
                    DisplayFileSystemFolder(currentFolder.Path, treeView, includeSubFolders, nodeFolder);
                }
                else
                {
                    // Check if sub-folders and add a dummy node so that we can populate the sub-folders when the user
                    // expands the node. This is more efficient then displaying the whole Android file system.
                    var subFolders = _androidFileSystem.GetFolders(currentFolder.Path);
                    if (subFolders.Any())
                    {
                        var nodeDummy = nodeFolder.Nodes.Add("Dummy");
                    }
                }
            }
        }

        private void tscCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tscCategory.SelectedIndex)
            {
                case 0:
                    RefreshFileSystem();
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
            if (e.Node != null &&
                e.Node.Nodes.Count > 0 &&
                e.Node.Nodes[0].Text.Equals("Dummy"))
            {
                // Clear dummy node                
                e.Node.Nodes.Clear();

                // Display folder
                FolderDetails folder = (FolderDetails)e.Node.Tag;
                DisplayFileSystemFolder(folder.Path, tvwFileSystem, false, e.Node);
            }
        }

        private void tvwFileSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();

            if (e.Node.Tag is FolderDetails)
            {
                DisplayStatus("Displaying files");

                var folder = (FolderDetails)e.Node.Tag;

                var folderControl = new FolderControl(_androidFileSystem);
                folderControl.Dock = DockStyle.Fill;
                folderControl.ModelToView(folder);
                splitContainer1.Panel2.Controls.Add(folderControl);

                DisplayStatus("Ready");
            }
        }

        //private void PrepareFolderContextMenu(TreeNode node)
        //{
        //    var nodeType = GetNodeType(node);
        //    if (nodeType == MyNodeTypes.FolderDetails)
        //    {

        //    }
        //}

        private void copyLocalFilesToFolderToolStripMenuItem_Click(object sender, EventArgs e)
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
                        _androidFileSystem.CopyLocalFileTo(localFile, remoteFile);
                    }

                    DisplayStatus("Ready");

                    MessageBox.Show("Files copied", "Copy Local Files To");

                    RefreshFileSystem();
                }
            }
        }

        private MyNodeTypes GetNodeType(TreeNode node)
        {
            if (node.Tag is FolderDetails) return MyNodeTypes.FolderDetails;
            if (node.Tag is FileDetails) return MyNodeTypes.FileDetails;
            return MyNodeTypes.Unknown;
        }

        private void tvwFileSystem_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void copyLocalFolderToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderDetails folder = (FolderDetails)tvwFileSystem.SelectedNode.Tag;

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
                    _androidFileSystem.CopyLocalFolderTo(localFolder, remoteFolder);
                    DisplayStatus("Ready");
                    
                    MessageBox.Show("Folder copied", "Copy Local Folder To");

                    RefreshFileSystem();
                }
            }
        }
    }
}
