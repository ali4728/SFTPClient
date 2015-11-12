using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFTPClient
{
    public class FileObject
    {
        public FileObject(string name, long size, DateTime lastmodified )
        {
            Name = name;
            Size = size;
            LastModified = lastmodified;
        }
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }

    }
}
