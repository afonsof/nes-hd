using System;
using System.Collections.Generic;
using System.IO;

namespace NesHd.Ui.Misc
{
    public class MFolder
    {
        private string _name = "";

        public MFolder()
        {
            MFolders = new List<MFolder>();
            Filter = FolderFilter.SupportedMappersOnly;
            InfosFolder = "";
            ImagesFolder = "";
            Path = "";
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (NameChanged != null)
                {
                    NameChanged(this, null);
                }
            }
        }

        public string Path { get; set; }

        public string ImagesFolder { get; set; }

        public string InfosFolder { get; set; }

        public FolderFilter Filter { get; set; }

        public int Mapper { get; set; }

        public List<MFolder> MFolders { get; private set; }

        public event EventHandler<EventArgs> NameChanged;

        public void FindMFolders()
        {
            var dirs = Directory.GetDirectories(Path);
            foreach (var dir in dirs)
            {
                var fol = new MFolder();
                fol.Path = dir;
                fol.Name = System.IO.Path.GetFileName(dir);
                fol.FindMFolders();
                MFolders.Add(fol);
            }
        }
    }
}