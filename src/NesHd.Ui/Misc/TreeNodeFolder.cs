using System;
using System.Windows.Forms;

namespace NesHd.Ui.Misc
{
    public class TreeNodeFolder : TreeNode
    {
        private MFolder _folder = new MFolder();

        public TreeNodeFolder()
        {
            _folder.NameChanged += FolderNameChanged;
        }

        public MFolder Folder
        {
            get { return _folder; }
            set
            {
                _folder = value;
                Text = _folder.Name;
                FindFolders();
            }
        }

        private void FolderNameChanged(object sender, EventArgs e)
        {
            Text = _folder.Name;
        }

        public void FindFolders()
        {
            Nodes.Clear();
            foreach (var folder in _folder.MFolders)
            {
                var treeNodeFolder = new TreeNodeFolder
                                         {
                                             ImageIndex = ImageIndex,
                                             SelectedImageIndex = SelectedImageIndex, 
                                             Folder = folder
                                         };
                Nodes.Add(treeNodeFolder);
            }
        }
    }
}