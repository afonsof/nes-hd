/*
This file is part of My Nes
A Nintendo Entertainment System Emulator.

 Copyright © 2009 - 2010 Ala Hadid (AHD)

My Nes is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

My Nes is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace NesHd.Ui.Misc
{
    public class MFolder
    {
        string _Name = "";
        string _Path = "";
        int _Mapper = 0;
        List<MFolder> _Folders = new List<MFolder>();
        string _ImagesFolder = "";
        string _InfosFolder = "";
        FolderFilter _Filter = FolderFilter.SupportedMappersOnly;
        public string Name
        {
            get { return this._Name; }
            set
            {
                this._Name = value;
                if (this.NameChanged != null)
                { this.NameChanged(this, null); }
            }
        }
        public string Path
        { get { return this._Path; } set { this._Path = value; } }
        public string ImagesFolder
        { get { return this._ImagesFolder; } set { this._ImagesFolder = value; } }
        public string InfosFolder
        { get { return this._InfosFolder; } set { this._InfosFolder = value; } }
        public FolderFilter Filter
        { get { return this._Filter; } set { this._Filter = value; } }
        public int Mapper
        { get { return this._Mapper; } set { this._Mapper = value; } }
        public event EventHandler<EventArgs> NameChanged;
        public List<MFolder> MFolders
        { get { return this._Folders; } }
        public void FindMFolders()
        {
            string[] dirs = Directory.GetDirectories(this._Path);
            foreach (string dir in dirs)
            {
                MFolder fol = new MFolder();
                fol.Path = dir;
                fol.Name = System.IO.Path.GetFileName(dir);
                fol.FindMFolders();
                this._Folders.Add(fol);
            }
        }
    }
    public class TreeNode_Folder : TreeNode
    {
        MFolder _Folder = new MFolder();
        public TreeNode_Folder()
        {
            this._Folder.NameChanged += new EventHandler<EventArgs>(this._Folder_NameChanged);
        }
        void _Folder_NameChanged(object sender, EventArgs e)
        {
            this.Text = this._Folder.Name;
        }
        public MFolder Folder
        {
            get { return this._Folder; }
            set
            {
                this._Folder = value;
                this.Text = this._Folder.Name;
                this.FindFolders();
            }
        }
        public void FindFolders()
        {
            this.Nodes.Clear();
            foreach (MFolder fol in this._Folder.MFolders)
            {
                TreeNode_Folder TR = new TreeNode_Folder();
                TR.ImageIndex = this.ImageIndex;
                TR.SelectedImageIndex = this.SelectedImageIndex;
                TR.Folder = fol;
                this.Nodes.Add(TR);
            }
        }
    }
    public class ListViewItem_Rom : ListViewItem
    {
        string _RomPath;
        public string RomPath
        { get { return this._RomPath; } set { this._RomPath = value; } }
    }
    public class MFolderCollection : List<MFolder>
    {

    }
    public enum FolderFilter
    { All, SupportedMappersOnly, Mapper }
}
