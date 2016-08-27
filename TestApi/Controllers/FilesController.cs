using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TestApi.Models;
using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace TestApi.Controllers
{
    public class FilesController : ApiController
    {
        private ClassContext db = new ClassContext();
        public ClassContext Get( string path)
        {
            List<string> disks = new List<string>();
            List<string> directories = new List<string>();
            List<string> files = new List<string>();
            db.Directories = new List<DirectoryClass>();
            db.Files = new List<FileClass>();
            if (path==null||path=="null")
            {

                foreach (DriveInfo item in DriveInfo.GetDrives())
                {
                    if (item.IsReady)
                    {
                        disks.Add(item.ToString());
                    }
                }
                directories.AddRange(disks);

                for (var i = 0; i < directories.Count; i++)
                {
                    string str = directories[i];

                    DirectoryClass dir = new DirectoryClass()
                    {
                        Name = str,
                        Path = str
                    };

                    db.Directories.Add(dir);
                }
                db.small_count = 0;
                db.middle_count = 0;
                db.big_count = 0;
                db.FolderUpPath = null;
                db.FolderUpSign = null;
                return db;
            }

            DriveInfo drives = new DriveInfo(path);
            if (!drives.IsReady)
            {
                path = String.Empty;
                return Get(path);
            }
            if (System.IO.File.Exists(path))
            {
                var lastIndex = path.LastIndexOf("\\");
                path = path.Substring(0, lastIndex);
            }

            directories.AddRange(Directory.GetDirectories(path));
            for (var i = 0; i < directories.Count; i++)
            {
                string fullName = directories[i];
                var lastIndex = directories[i].LastIndexOf("\\");

                string name = fullName.Substring(lastIndex + 1);

                DirectoryClass dir = new DirectoryClass()
                {
                    Name = name,
                    Path = fullName
                };
                
                db.Directories.Add(dir);
            }

            files.AddRange(Directory.GetFiles(path));
            for (var i = 0; i < files.Count; i++)
            {
                string fullName = files[i];
                var lastIndex = files[i].LastIndexOf("\\");

                string name = fullName.Substring(lastIndex + 1);

                FileClass file = new FileClass()
                {
                    Name = name,
                    Path = fullName
                };

                db.Files.Add(file);
            }
            int[] count = FilesCount(path);
            db.small_count = count[0];
            db.middle_count = count[1];
            db.big_count = count[2];
            string[] tmpList = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (tmpList.Count() == 2)
            {
                db.FolderUpPath = path.Substring(0, path.LastIndexOf("\\") + 1);
            }
            else if (tmpList.Count() > 1)
            {
                db.FolderUpPath = path.Substring(0, path.LastIndexOf("\\"));
            }
            else
            {
                db.FolderUpPath = null;
            }
            db.CurrentPath = path;
            db.FolderUpSign = "..";
            return db;
        }
        private static int[] FilesCount(string path)
        {
            int count_big = 0, count_small = 0, count_middle = 0;
            int[] count_all = new int[3];
            Stack<string> dirs = new Stack<string>(20);

            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException();
            }
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {

                    continue;
                }

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {

                    continue;
                }
                System.IO.FileInfo fi = null;
                foreach (string file in files)
                {
                    try
                    {
                        if (file.Length < 248)
                        {
                            try
                            {
                                fi = new System.IO.FileInfo(file);
                            }
                            catch
                            {
                                continue;
                            }
                            if (fi.Length <= 10000000)
                            {
                                count_small++;
                            }
                            if (fi.Length <= 50000000 && fi.Length >= 10000000)
                            {
                                count_middle++;
                            }
                            if (fi.Length >= 100000000)
                            {
                                count_big++;
                            }
                        }
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        continue;
                    }
                }

                foreach (string str in subDirs)
                    dirs.Push(str);
            }
            count_all[0] = count_small;
            count_all[1] = count_middle;
            count_all[2] = count_big;
            return count_all;
        }
    }
}
