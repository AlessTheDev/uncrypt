using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Terminal
{
    public class FileSystem
    {
        [System.Serializable]
        public class Directory
        {
            private string _name;

            public string Name
            {
                get => _name;
                set => _name = value;
            }

            private Dictionary<string, Directory> _directories = new(StringComparer.OrdinalIgnoreCase);
            private Dictionary<string, File> _files = new();
            private Directory _parentDirectory;

            public Directory(string name)
            {
                _name = name.Trim();
            }

            public void AddDirectory(Directory dir)
            {
                _directories.Add(dir.Name, dir);
                dir._parentDirectory = this;
            }

            public void AddFile(File file)
            {
                _files.Add(file.Name.Trim(), file);
            }

            public Directory GetDirWithName(string name)
            {
                return _directories.GetValueOrDefault(name.Trim(), null);
            }

            public File GetFileWithName(string name)
            {
                return _files.GetValueOrDefault(name.Trim(), null);
            }

            public string GetPath()
            {
                if (_parentDirectory == null)
                {
                    return "~";
                }

                return Parent.GetPath() + "/" + _name;
            }


            public Directory Parent => _parentDirectory;
            public Directory[] Children => _directories.Values.ToArray();
            public File[] Files => _files.Values.ToArray();
        }

        [System.Serializable]
        public class File
        {
            public string Name;
            public string Content;

            public File(string name, string content)
            {
                Name = name.Trim();
                Content = content;
            }
        }

        public Directory Root { get; private set; } = new("Home");
        private Directory _currentDir;

        public FileSystem()
        {
            Directory utils = new("utils");

            utils.AddFile(new File("README.txt", "Ciao :)"));

            Root.AddDirectory(utils);

            _currentDir = Root;
        }

        public Directory GetDirectory(string path)
        {
            Directory targetDir = _currentDir;

            // If the dir is null
            if (path.StartsWith("/") || path.StartsWith("\\") || path.StartsWith("~"))
            {
                targetDir = Root;
            }

            var dirs = Regex.Split(path,
                @"[\\/]+"); // Splits the string by the following characters: \ or / returning the individual dir names

            foreach (var dir in dirs)
            {
                if (dir == "." || string.IsNullOrEmpty(dir)) continue;

                // If the dir is ".." go back to the parent directory, otherwise get the child dir
                targetDir = dir == ".." ? targetDir.Parent : targetDir.GetDirWithName(dir);

                if (targetDir == null)
                {
                    throw new Exception("Folder not found, make sure the path is correct.");
                }
            }

            return targetDir;
        }
        
        public Directory GetDirectoryOrNull(string path)
        {
            try
            {
                return GetDirectory(path);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void ChangeDirectory(string path)
        {
            _currentDir = GetDirectory(path);
        }

        public void Save()
        {
            SaveSystem.GameData data = SaveSystem.GetData();
            data.FileSystemRoot = Root;
            SaveSystem.Save(data);
        }

        public void LoadFromSaveFile()
        {
            SaveSystem.GameData data = SaveSystem.GetData();
            Root = data.FileSystemRoot;
            _currentDir = Root.GetDirWithName(_currentDir.Name) ?? Root;
        }

        public Directory CurrentDir => _currentDir;
    }
}