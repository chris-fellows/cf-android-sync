using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFAndroidSync.Models
{
    public abstract class FileSystemItem
    {
        public string Name { get; set; } = String.Empty;

        public string Path { get; set; } = String.Empty;
    }
}
