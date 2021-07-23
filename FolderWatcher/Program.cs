using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderWatcher;

namespace FolderWatcher
{
    public class NSSFolderWatcher
    {
        FileSystemWatcher watcher;

        List<string> OnChangeAll { get; set; }
        string OnChangeLast { get; set; }
        
        public NSSFolderWatcher(string path)
        {
            OnChangeAll = new List<string>();

            this.watcher = new FileSystemWatcher(path);
            this.watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            this.watcher.Changed += OnChanged;
            this.watcher.Created += OnCreated;
            this.watcher.Deleted += OnDeleted;
            this.watcher.Renamed += OnRenamed;
            this.watcher.Error += OnError;

            this.watcher.Filter = "*";
            this.watcher.IncludeSubdirectories = true;
            this.watcher.EnableRaisingEvents = true;
        }

        public NSSFolderWatcher(string path, List<NotifyFilters> notify)
        {
            OnChangeAll = new List<string>();

            this.watcher = new FileSystemWatcher(path);
            foreach (var notif in notify)
            {
                this.watcher.NotifyFilter = notif;

            }

            this.watcher.Changed += OnChanged;
            this.watcher.Created += OnCreated;
            this.watcher.Deleted += OnDeleted;
            this.watcher.Renamed += OnRenamed;
            this.watcher.Error += OnError;

            this.watcher.Filter = "*";
            this.watcher.IncludeSubdirectories = true;
            this.watcher.EnableRaisingEvents = true;
        }
        
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                try
                {
                    using (FileStream fs = File.OpenRead(e.FullPath))
                    {
                        if (fs.CanRead)
                        {
                            this.OnChangeLast = string.Format("\nLast event: {0} at ({1}) with the type: {2} and the name: {3}\n", e.FullPath, DateTime.Now, e.ChangeType, e.Name);

                        }
                    }
                }
                catch { }

                var result = string.Format("\nLast event: {0} at ({1}) with the type: {2} and the name: {3}\n", e.FullPath, DateTime.Now, e.ChangeType, e.Name);

                this.OnChangeAll.Add(result);
            }
            catch (Exception ex)
            {   
                this.OnChangeLast = $"Error: {ex.Message}";
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var info = $"Created ({DateTime.Now}): {e.FullPath}";
            this.OnChangeAll.Add(info);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            this.OnChangeAll.Add(($"Deleted ({DateTime.Now}): {e.FullPath}"));
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var result = string.Format("Renamed ({0}): \nOld:{1} \nNew: {2}\n", DateTime.Now, e.OldFullPath, e.FullPath);
            this.OnChangeAll.Add(result);
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private void PrintException(Exception ex)
        {
            if (ex != null)
            {
                this.OnChangeAll.Add(string.Format($"\nMessage ({DateTime.Now}): {0} \nStacktrace: {1} \n{2}", ex.Message, ex.StackTrace, ex.InnerException));
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\kemgang\Downloads";

            NSSFolderWatcher folderWatcher = new NSSFolderWatcher(path);

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
        /*
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                using (FileStream fs = File.OpenRead(e.FullPath))
                {
                    Console.WriteLine($"can read: {fs.CanRead}");
                    Console.WriteLine($"can write: {fs.CanWrite}");
                    Console.WriteLine($"Change Type: {e.ChangeType}");
                    Console.WriteLine($"Name: {e.Name}");
                    Console.WriteLine($"Is Async: {fs.IsAsync}");
                }

                Console.WriteLine($"Changed: {e.FullPath}");
                Console.WriteLine($"/////////////////////////////////////////////");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
        */
    }
}
