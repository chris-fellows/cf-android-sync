using CFAndroidSync.Interfaces;
using CFAndroidSync.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CFAndroidSync.Controls
{    
    /// <summary>
    /// Details of a single remote folder.
    /// </summary>
    public partial class RemoteFolderControl : UserControl
    {
        private readonly IPhoneFileSystem _phoneFileSystem;

        //public RemoteFolderControl()
        //{
        //    InitializeComponent();
        //}

        public RemoteFolderControl(IPhoneFileSystem phoneFileSystem)
        {
            InitializeComponent();

            _phoneFileSystem = phoneFileSystem;
        }

        public void ModelToView(FolderDetails folder)
        {
            dgvItems.Rows.Clear();
            dgvItems.Columns.Clear();

            int columnIndex = dgvItems.Columns.Add("Name", "Name");
            dgvItems.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // Get files
            var files = _phoneFileSystem.GetFiles(folder.Path);

            // Display files
            foreach(var file in files)
            {                
                dgvItems.Rows.Add(GetRow(file));
            }

            dgvItems.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private static DataGridViewRow GetRow(FileDetails file)
        {
            DataGridViewRow row = new DataGridViewRow();

            using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
            {
                cell.Value = file.Name;
                row.Cells.Add(cell);
            }

            return row;
        }
    }
}
