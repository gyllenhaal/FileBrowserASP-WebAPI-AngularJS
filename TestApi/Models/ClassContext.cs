using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using System.Web;

namespace TestApi.Models
{
    public class ClassContext
    {
        public List<DirectoryClass> Directories { get; set; }
        public List<FileClass> Files { get; set; }
        public string FolderUpSign { get; set; }
        public string CurrentPath { get; set; }
        public string FolderUpPath { get; set; }
        public int small_count { get; set; }
        public int middle_count { get; set; }
        public int big_count { get; set; }
    }
}