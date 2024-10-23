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
    public partial class FolderControl : UserControl
    {
        private readonly IAndroidFileSystem _androidFileSystem;
        public FolderControl()
        {
            InitializeComponent();
        }

        public FolderControl(IAndroidFileSystem androidFileSystem)
        {
            InitializeComponent();

            _androidFileSystem = androidFileSystem;
        }

        public void ModelToView(FolderDetails folder)
        {
            dgvItems.Rows.Clear();
            dgvItems.Columns.Clear();

            int columnIndex = dgvItems.Columns.Add("Name", "Name");

            var files = _androidFileSystem.GetFiles(folder.Path);
            foreach(var file in files)
            {                
                dgvItems.Rows.Add(GetRow(file));
            }            
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
