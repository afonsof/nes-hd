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
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using NesHd.Core.Memory;
using NesHd.Ui.Misc;

namespace NesHd.Ui.WinForms
{
    public partial class Frm_Browser : Form
    {
        Frm_Main _TheMainForm;
        TreeNode_Folder _SelectedFolder;
        public Frm_Browser(Frm_Main TheMainForm)
        {
            this._TheMainForm = TheMainForm;
            this.InitializeComponent();
            //Load settings
            this.checkBox1.Checked = Program.Settings.ShowBrowser;
            this.Location = new Point(Program.Settings.Browser_X, Program.Settings.Browser_Y);
            this.Size = new Size(Program.Settings.Browser_W, Program.Settings.Browser_H);
            this.splitContainer1.SplitterDistance = Program.Settings.Container1;
            this.splitContainer2.SplitterDistance = Program.Settings.Container2;
            this.columnHeader1_Name.Width = Program.Settings.Column_Name;
            this.columnHeader2_Size.Width = Program.Settings.Column_Size;
            this.columnHeader3_Mapper.Width = Program.Settings.Column_Mapper;
            this.listView1.View = Program.Settings.View;
            switch (Program.Settings.View)
            {
                case View.Details:
                    this.tilesToolStripMenuItem.Checked = false;
                    this.detailsToolStripMenuItem.Checked = true;
                    this.listToolStripMenuItem.Checked = false;
                    break;
                case View.Tile:
                    this.tilesToolStripMenuItem.Checked = true;
                    this.detailsToolStripMenuItem.Checked = false;
                    this.listToolStripMenuItem.Checked = false;
                    break;
                case View.List:
                    this.tilesToolStripMenuItem.Checked = false;
                    this.detailsToolStripMenuItem.Checked = false;
                    this.listToolStripMenuItem.Checked = true;
                    break;
            }
            //Load folders
            if (Program.Settings.Folders == null)
                Program.Settings.Folders = new MFolderCollection();
            this.RefreshFolders();
        }
        void RefreshFolders()
        {
            this.treeView1.Nodes.Clear();
            foreach (MFolder fol in Program.Settings.Folders)
            {
                TreeNode_Folder TR = new TreeNode_Folder();
                TR.ImageIndex = 0;
                TR.SelectedImageIndex = 1;
                TR.Folder = fol;
                this.treeView1.Nodes.Add(TR);
            }
        }
        void SaveSettings()
        {
            Program.Settings.ShowBrowser = this.checkBox1.Checked;
            Program.Settings.Browser_X = this.Location.X;
            Program.Settings.Browser_Y = this.Location.Y;
            Program.Settings.Browser_H = this.Height;
            Program.Settings.Browser_W = this.Width;
            Program.Settings.Container1 = this.splitContainer1.SplitterDistance;
            Program.Settings.Container2 = this.splitContainer2.SplitterDistance;
            Program.Settings.Column_Name = this.columnHeader1_Name.Width;
            Program.Settings.Column_Size = this.columnHeader2_Size.Width;
            Program.Settings.Column_Mapper = this.columnHeader3_Mapper.Width;
            Program.Settings.Save();
        }
        void AddRootFolder()
        {
            FolderBrowserDialog fo = new FolderBrowserDialog();
            fo.Description = "Add roms folder";
            fo.ShowNewFolderButton = true;
            if (fo.ShowDialog(this) == DialogResult.OK)
            {
                MFolder folderr = new MFolder();
                folderr.Path = fo.SelectedPath;
                folderr.Name = Path.GetFileName(fo.SelectedPath);
                folderr.FindMFolders();
                Program.Settings.Folders.Add(folderr);
                this.RefreshFolders();
            }
        }
        void AddFolder()
        {
            if (this.treeView1.SelectedNode == null)
                return;
            FolderBrowserDialog fo = new FolderBrowserDialog();
            fo.Description = "Add roms folder";
            fo.ShowNewFolderButton = true;
            if (fo.ShowDialog(this) == DialogResult.OK)
            {
                MFolder folderr = new MFolder();
                folderr.Path = fo.SelectedPath;
                folderr.Name = Path.GetFileName(fo.SelectedPath);
                folderr.FindMFolders();
                ((TreeNode_Folder)this.treeView1.SelectedNode).Folder.MFolders.Add(folderr);
                ((TreeNode_Folder)this.treeView1.SelectedNode).FindFolders();
                this.treeView1.SelectedNode.Expand();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.SaveSettings();
            this.Close();
        }
        private void DeleteFolder(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;
            if (this.treeView1.SelectedNode.Parent == null)//It's a root folder
            {
                if (MessageBox.Show("Are you sure you want to delete selected folder from the list ?", "Delete folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.Settings.Folders.RemoveAt(this.treeView1.SelectedNode.Index);
                    this.RefreshFolders();
                }
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to delete selected folder from the list ?", "Delete folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ((TreeNode_Folder)this.treeView1.SelectedNode.Parent).Folder.MFolders.RemoveAt(this.treeView1.SelectedNode.Index);
                    ((TreeNode_Folder)this.treeView1.SelectedNode.Parent).FindFolders();
                }
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;
            this.listView1.Items.Clear();
            this.pictureBox1.Image = Properties.Resources.NESDoc;
            this.richTextBox1.Text = "";
            TreeNode_Folder TR = (TreeNode_Folder)this.treeView1.SelectedNode;
            this._SelectedFolder = TR;
            //Properties
            this.listView2.Items.Clear();
            this.listView2.Items.Add("Name");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
            this.listView2.Items.Add("Path");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
            this.listView2.Items.Add("Snapshots Path");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
            this.listView2.Items.Add("Info Texts Path");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            this.TextBox1_mapper.Text = this._SelectedFolder.Folder.Mapper.ToString();
            switch (this._SelectedFolder.Folder.Filter)
            {
                case FolderFilter.All: this.ComboBox1_nav.SelectedIndex = 0; this.TextBox1_mapper.Enabled = false; break;
                case FolderFilter.SupportedMappersOnly: this.ComboBox1_nav.SelectedIndex = 1; this.TextBox1_mapper.Enabled = false; break;
                case FolderFilter.Mapper: this.ComboBox1_nav.SelectedIndex = 2; this.TextBox1_mapper.Enabled = true; break;
            }
            if (!Directory.Exists(TR.Folder.Path))
            {
                MessageBox.Show("This folder isn't exist on the disk !!");
                this.DeleteFolder(this, null);
            }
            else
            {
                string[] Dirs = Directory.GetFiles(TR.Folder.Path);
                foreach (string Dir in Dirs)
                {
                    string EXT = Path.GetExtension(Dir);
                    switch (EXT.ToLower())
                    {
                        case ".nes":
                            ListViewItem_Rom IT = new ListViewItem_Rom();
                            IT.RomPath = Dir;
                            IT.ImageIndex = 2;
                            IT.Text = Path.GetFileName(Dir);
                            IT.SubItems.Add(Program.GetFileSize(Dir));
                            INESHeaderReader header = new INESHeaderReader(Dir);
                            if (header.validRom)
                                IT.SubItems.Add(header.MemoryMapper.ToString() + ", " + header.GetMapperName());
                            switch (TR.Folder.Filter)
                            {
                                case FolderFilter.All: this.listView1.Items.Add(IT); break;
                                case FolderFilter.SupportedMappersOnly:
                                    if (header.SupportedMapper())
                                        this.listView1.Items.Add(IT);
                                    break;
                                case FolderFilter.Mapper:
                                    if (header.MemoryMapper == Convert.ToInt32(this.TextBox1_mapper.Text))
                                        this.listView1.Items.Add(IT);
                                    break;
                            }
                            break;
                        case ".rar":
                            ListViewItem_Rom IT1 = new ListViewItem_Rom();
                            IT1.ImageIndex = 3;
                            IT1.RomPath = Dir;
                            IT1.Text = Path.GetFileName(Dir);
                            IT1.SubItems.Add(Program.GetFileSize(Dir));
                            IT1.SubItems.Add("N/A");
                            this.listView1.Items.Add(IT1);
                            break;
                        case ".zip":
                            ListViewItem_Rom IT2 = new ListViewItem_Rom();
                            IT2.ImageIndex = 3;
                            IT2.RomPath = Dir;
                            IT2.Text = Path.GetFileName(Dir);
                            IT2.SubItems.Add(Program.GetFileSize(Dir));
                            IT2.SubItems.Add("N/A");
                            this.listView1.Items.Add(IT2);
                            break;
                        case ".7z":
                            ListViewItem_Rom IT3 = new ListViewItem_Rom();
                            IT3.ImageIndex = 3;
                            IT3.RomPath = Dir;
                            IT3.Text = Path.GetFileName(Dir);
                            IT3.SubItems.Add(Program.GetFileSize(Dir));
                            IT3.SubItems.Add("N/A");
                            this.listView1.Items.Add(IT3);
                            break;
                    }
                }
                this.label1_status.Text = this.listView1.Items.Count.ToString() + " items found.";
            }
        }
        private void RenameSelected(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;
            this.treeView1.SelectedNode.BeginEdit();
        }
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
            { e.CancelEdit = true; return; }
            if (e.Label == null)
            { return; }
            TreeNode_Folder fol = (TreeNode_Folder)this.treeView1.SelectedNode;
            fol.Folder.Name = e.Label;
            fol.Text = e.Label;
            //Properties
            this.listView2.Items.Clear();
            this.listView2.Items.Add("Name");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.Name);
            this.listView2.Items.Add("Path");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.Path);
            this.listView2.Items.Add("Snapshots Path");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.ImagesFolder);
            this.listView2.Items.Add("Info Texts Path");
            this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(((TreeNode_Folder)e.Node).Folder.InfosFolder);
        }
        //Play
        private void PlayRom(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count != 1)
                return;
            ListViewItem_Rom IT = (ListViewItem_Rom)this.listView1.SelectedItems[0];
            if (File.Exists(IT.RomPath))
            {
                this.SaveSettings();
                this._TheMainForm.OpenRom(IT.RomPath);
                this._TheMainForm.Select();
            }
        }
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                this.PlayRom(this, null);
        }
        private void Frm_Browser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                this.PlayRom(this, null);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.PlayRom(this, null);
        }
        //snapshotes
        private void ChangeImageFolder(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;
            TreeNode_Folder TR = (TreeNode_Folder)this.treeView1.SelectedNode;
            FolderBrowserDialog Fol = new FolderBrowserDialog();
            Fol.Description = "Images folder";
            Fol.ShowNewFolderButton = true;
            Fol.SelectedPath = TR.Folder.ImagesFolder;
            if (Fol.ShowDialog() == DialogResult.OK)
            {
                TR.Folder.ImagesFolder = Fol.SelectedPath;
                //Properties
                this.listView2.Items.Clear();
                this.listView2.Items.Add("Name");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
                this.listView2.Items.Add("Path");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
                this.listView2.Items.Add("Snapshots Path");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
                this.listView2.Items.Add("Info Texts Path");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            }
        }
        //text
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;
            TreeNode_Folder TR = (TreeNode_Folder)this.treeView1.SelectedNode;
            FolderBrowserDialog Fol = new FolderBrowserDialog();
            Fol.Description = "Info files folder";
            Fol.ShowNewFolderButton = true;
            Fol.SelectedPath = TR.Folder.InfosFolder;
            if (Fol.ShowDialog() == DialogResult.OK)
            {
                TR.Folder.InfosFolder = Fol.SelectedPath;
                //Properties
                this.listView2.Items.Clear();
                this.listView2.Items.Add("Name");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
                this.listView2.Items.Add("Path");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
                this.listView2.Items.Add("Snapshots Path");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
                this.listView2.Items.Add("Info Texts Path");
                this.listView2.Items[this.listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Image = Properties.Resources.NESDoc;
            this.richTextBox1.Text = "";
            if (this.listView1.SelectedItems.Count != 1)
                return;
            if (this.treeView1.SelectedNode == null)
                return;
            if (this._SelectedFolder == null)
                return;
            #region Load Image
            try
            {
                string[] Exs = { ".jpg", ".JPG", ".png", ".PNG", ".bmp", ".BMP", ".gif", ".GIF", ".jpeg", ".JPEG", ".tiff", ".TIFF" };
                string[] Names = Directory.GetFiles(this._SelectedFolder.Folder.ImagesFolder);
                string FileName = Path.GetFileNameWithoutExtension(this.listView1.SelectedItems[0].Text);
                foreach (string STR in Names)
                {
                    if (Path.GetFileNameWithoutExtension(STR).Length >= Path.GetFileNameWithoutExtension(this.listView1.SelectedItems[0].Text).Length)
                    {
                        if (Path.GetFileNameWithoutExtension(STR).Substring(0, Path.GetFileNameWithoutExtension(this.listView1.SelectedItems[0].Text).Length) == Path.GetFileNameWithoutExtension(this.listView1.SelectedItems[0].Text))
                        { FileName = Path.GetFileNameWithoutExtension(STR); break; }
                    }
                }
                foreach (string STR in Exs)
                {
                    if (File.Exists(this._SelectedFolder.Folder.ImagesFolder + "//" +
                        FileName + STR))
                    {
                        this.pictureBox1.Image = Image.FromFile(this._SelectedFolder.Folder.ImagesFolder + "//" +
                        FileName + STR); break;
                    } this.pictureBox1.Image = Properties.Resources.NESDoc;
                }
            }
            catch { this.pictureBox1.Image = Properties.Resources.NESDoc; }
            #endregion
            #region Load Info Text
            try
            {
                string FileName = Path.GetFileNameWithoutExtension(this.listView1.SelectedItems[0].Text);
                this.richTextBox1.Text = ""; //Clear
                this.richTextBox1.Lines = File.ReadAllLines(this._SelectedFolder.Folder.InfosFolder + "//" + FileName + ".txt");
            }
            catch
            {
                this.richTextBox1.Text = "";
            }
            #endregion
        }
        private void Frm_Browser_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.SaveSettings();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count != 1)
                return;
            if (this.treeView1.SelectedNode == null)
                return;
            if (this._SelectedFolder == null)
                return;
            string FileName = Path.GetFileNameWithoutExtension(this.listView1.SelectedItems[0].Text);
            if (Directory.Exists(this._SelectedFolder.Folder.InfosFolder))
            {
                File.WriteAllLines(this._SelectedFolder.Folder.InfosFolder + "//" + FileName
                    + ".txt", this.richTextBox1.Lines, Encoding.Unicode);
            }
            else
            {
                if (MessageBox.Show("This folder hasn't a folder for info files, do you want to browse for it now ?", "No Infos Folder !!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    FolderBrowserDialog Fol = new FolderBrowserDialog();
                    Fol.Description = "Info files folder";
                    Fol.ShowNewFolderButton = true;
                    Fol.SelectedPath = this._SelectedFolder.Folder.InfosFolder;
                    if (Fol.ShowDialog() == DialogResult.OK)
                    {
                        this._SelectedFolder.Folder.InfosFolder = Fol.SelectedPath;
                        File.WriteAllLines(this._SelectedFolder.Folder.InfosFolder + "//" + FileName
                   + ".txt", this.richTextBox1.Lines, Encoding.Unicode);
                    }
                }
            }
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listView1.SelectedItems.Count != 1)
                    return;
                if (this.treeView1.SelectedNode == null)
                    return;
                if (this._SelectedFolder == null)
                    return;
                string FileName = Path.GetFileNameWithoutExtension(this.listView1.SelectedItems[0].Text);
                this.richTextBox1.Text = ""; //Clear
                this.richTextBox1.Lines = File.ReadAllLines(this._SelectedFolder.Folder.InfosFolder + "//" + FileName + ".txt");
            }
            catch
            {
                this.richTextBox1.Text = "";
            }
        }
        private void TextBox1_mapper_Click(object sender, EventArgs e)
        {
            try
            {
                Convert.ToInt32(this.TextBox1_mapper.Text);
                if (Convert.ToInt32(this.TextBox1_mapper.Text) > 255)
                    this.TextBox1_mapper.Text = "255";
                if (this._SelectedFolder != null)
                    this._SelectedFolder.Folder.Mapper = Convert.ToInt32(this.TextBox1_mapper.Text);
            }
            catch { this.TextBox1_mapper.Text = "0"; }
        }
        private void TextBox1_mapper_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Convert.ToInt32(this.TextBox1_mapper.Text);
                if (Convert.ToInt32(this.TextBox1_mapper.Text) > 255)
                    this.TextBox1_mapper.Text = "255";
                if (this._SelectedFolder != null)
                    this._SelectedFolder.Folder.Mapper = Convert.ToInt32(this.TextBox1_mapper.Text);
            }
            catch { this.TextBox1_mapper.Text = "0"; }
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            this.treeView1_AfterSelect(this, null);
        }
        private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Settings.View = View.LargeIcon;
            this.tilesToolStripMenuItem.Checked = true;
            this.detailsToolStripMenuItem.Checked = false;
            this.listToolStripMenuItem.Checked = false;
            this.listView1.View = Program.Settings.View;
        }
        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Settings.View = View.Details;
            this.tilesToolStripMenuItem.Checked = false;
            this.detailsToolStripMenuItem.Checked = true;
            this.listToolStripMenuItem.Checked = false;
            this.listView1.View = Program.Settings.View;
        }
        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Settings.View = View.List;
            this.tilesToolStripMenuItem.Checked = false;
            this.detailsToolStripMenuItem.Checked = false;
            this.listToolStripMenuItem.Checked = true;
            this.listView1.View = Program.Settings.View;
        }
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count == 0)
                return;
            List<ListViewItem_Rom> founded = new List<ListViewItem_Rom>();
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                if (this.listView1.Items[i].Text.Length >= this.TextBox1_search.Text.Length &
                    this.listView1.Items[i].Text.ToLower().Substring(0, this.TextBox1_search.Text.Length) == this.TextBox1_search.Text.ToLower())
                {
                    founded.Add((ListViewItem_Rom)this.listView1.Items[i]);
                }
            }
            this.listView1.Items.Clear();
            foreach (ListViewItem_Rom innn in founded)
                this.listView1.Items.Add(innn);
            this.label1_status.Text = this.listView1.Items.Count.ToString() + " item(s) matched.";
        }
        private void ComboBox1_nav_DropDownClosed(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;
            if (this._SelectedFolder == null)
                return;
            switch (this.ComboBox1_nav.SelectedIndex)
            {
                case 0:
                    this._SelectedFolder.Folder.Filter = FolderFilter.All;
                    this.TextBox1_mapper.Enabled = false;
                    break;
                case 1:
                    this._SelectedFolder.Folder.Filter = FolderFilter.SupportedMappersOnly;
                    this.TextBox1_mapper.Enabled = false;
                    break;
                case 2:
                    this._SelectedFolder.Folder.Filter = FolderFilter.Mapper;
                    this.TextBox1_mapper.Enabled = true;
                    break;
            }
            this.treeView1_AfterSelect(this, null);
        }
        //folder properties double click
        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;
            if (this.listView2.SelectedItems.Count != 1)
                return;
            switch (this.listView2.SelectedItems[0].Text)
            {
                case "Snapshots Path":
                    this.ChangeImageFolder(this, null);
                    break;
                case "Info Texts Path":
                    this.toolStripButton7_Click(this, null);
                    break;
                case "Name":
                    this.RenameSelected(this, null);
                    break;
            }
        }
        private void rootFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddRootFolder();
        }
        private void folderInSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddFolder();
        }
    }
}
