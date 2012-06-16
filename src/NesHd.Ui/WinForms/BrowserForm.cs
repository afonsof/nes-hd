using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NesHd.Core.Memory;
using NesHd.Ui.Misc;
using NesHd.Ui.Properties;

namespace NesHd.Ui.WinForms
{
    public partial class BrowserForm : Form
    {
        private readonly MainForm _theMainForm;
        private TreeNodeFolder _SelectedFolder;

        public BrowserForm(MainForm theMainForm)
        {
            _theMainForm = theMainForm;
            InitializeComponent();
            //Load settings
            checkBox1.Checked = Program.Settings.ShowBrowser;
            Location = new Point(Program.Settings.Browser_X, Program.Settings.Browser_Y);
            Size = new Size(Program.Settings.Browser_W, Program.Settings.Browser_H);
            splitContainer1.SplitterDistance = Program.Settings.Container1;
            splitContainer2.SplitterDistance = Program.Settings.Container2;
            columnHeader1_Name.Width = Program.Settings.Column_Name;
            columnHeader2_Size.Width = Program.Settings.Column_Size;
            columnHeader3_Mapper.Width = Program.Settings.Column_Mapper;
            listView1.View = Program.Settings.View;
            switch (Program.Settings.View)
            {
                case View.Details:
                    tilesToolStripMenuItem.Checked = false;
                    detailsToolStripMenuItem.Checked = true;
                    listToolStripMenuItem.Checked = false;
                    break;
                case View.Tile:
                    tilesToolStripMenuItem.Checked = true;
                    detailsToolStripMenuItem.Checked = false;
                    listToolStripMenuItem.Checked = false;
                    break;
                case View.List:
                    tilesToolStripMenuItem.Checked = false;
                    detailsToolStripMenuItem.Checked = false;
                    listToolStripMenuItem.Checked = true;
                    break;
            }
            //Load folders
            if (Program.Settings.Folders == null)
                Program.Settings.Folders = new MFolderCollection();
            RefreshFolders();
        }

        private void RefreshFolders()
        {
            treeView1.Nodes.Clear();
            foreach (var fol in Program.Settings.Folders)
            {
                var TR = new TreeNodeFolder();
                TR.ImageIndex = 0;
                TR.SelectedImageIndex = 1;
                TR.Folder = fol;
                treeView1.Nodes.Add(TR);
            }
        }

        private void SaveSettings()
        {
            Program.Settings.ShowBrowser = checkBox1.Checked;
            Program.Settings.Browser_X = Location.X;
            Program.Settings.Browser_Y = Location.Y;
            Program.Settings.Browser_H = Height;
            Program.Settings.Browser_W = Width;
            Program.Settings.Container1 = splitContainer1.SplitterDistance;
            Program.Settings.Container2 = splitContainer2.SplitterDistance;
            Program.Settings.Column_Name = columnHeader1_Name.Width;
            Program.Settings.Column_Size = columnHeader2_Size.Width;
            Program.Settings.Column_Mapper = columnHeader3_Mapper.Width;
            Program.Settings.Save();
        }

        private void AddRootFolder()
        {
            var fo = new FolderBrowserDialog();
            fo.Description = "Add roms folder";
            fo.ShowNewFolderButton = true;
            if (fo.ShowDialog(this) == DialogResult.OK)
            {
                var folderr = new MFolder();
                folderr.Path = fo.SelectedPath;
                folderr.Name = Path.GetFileName(fo.SelectedPath);
                folderr.FindMFolders();
                Program.Settings.Folders.Add(folderr);
                RefreshFolders();
            }
        }

        private void AddFolder()
        {
            if (treeView1.SelectedNode == null)
                return;
            var fo = new FolderBrowserDialog();
            fo.Description = "Add roms folder";
            fo.ShowNewFolderButton = true;
            if (fo.ShowDialog(this) == DialogResult.OK)
            {
                var folderr = new MFolder();
                folderr.Path = fo.SelectedPath;
                folderr.Name = Path.GetFileName(fo.SelectedPath);
                folderr.FindMFolders();
                ((TreeNodeFolder) treeView1.SelectedNode).Folder.MFolders.Add(folderr);
                ((TreeNodeFolder) treeView1.SelectedNode).FindFolders();
                treeView1.SelectedNode.Expand();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void DeleteFolder(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (treeView1.SelectedNode.Parent == null) //It's a root folder
            {
                if (
                    MessageBox.Show("Are you sure you want to delete selected folder from the list ?", "Delete folder",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.Settings.Folders.RemoveAt(treeView1.SelectedNode.Index);
                    RefreshFolders();
                }
            }
            else
            {
                if (
                    MessageBox.Show("Are you sure you want to delete selected folder from the list ?", "Delete folder",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ((TreeNodeFolder) treeView1.SelectedNode.Parent).Folder.MFolders.RemoveAt(
                        treeView1.SelectedNode.Index);
                    ((TreeNodeFolder) treeView1.SelectedNode.Parent).FindFolders();
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            listView1.Items.Clear();
            pictureBox1.Image = Resources.NESDoc;
            richTextBox1.Text = "";
            var TR = (TreeNodeFolder) treeView1.SelectedNode;
            _SelectedFolder = TR;
            //Properties
            listView2.Items.Clear();
            listView2.Items.Add("Name");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
            listView2.Items.Add("Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
            listView2.Items.Add("Snapshots Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
            listView2.Items.Add("Info Texts Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            TextBox1_mapper.Text = _SelectedFolder.Folder.Mapper.ToString();
            switch (_SelectedFolder.Folder.Filter)
            {
                case FolderFilter.All:
                    ComboBox1_nav.SelectedIndex = 0;
                    TextBox1_mapper.Enabled = false;
                    break;
                case FolderFilter.SupportedMappersOnly:
                    ComboBox1_nav.SelectedIndex = 1;
                    TextBox1_mapper.Enabled = false;
                    break;
                case FolderFilter.Mapper:
                    ComboBox1_nav.SelectedIndex = 2;
                    TextBox1_mapper.Enabled = true;
                    break;
            }
            if (!Directory.Exists(TR.Folder.Path))
            {
                MessageBox.Show("This folder isn't exist on the disk !!");
                DeleteFolder(this, null);
            }
            else
            {
                var Dirs = Directory.GetFiles(TR.Folder.Path);
                foreach (var Dir in Dirs)
                {
                    var EXT = Path.GetExtension(Dir);
                    switch (EXT.ToLower())
                    {
                        case ".nes":
                            var IT = new ListViewItemRom();
                            IT.RomPath = Dir;
                            IT.ImageIndex = 2;
                            IT.Text = Path.GetFileName(Dir);
                            IT.SubItems.Add(Program.GetFileSize(Dir));
                            var header = new NesHeaderReader(Dir);
                            if (header.ValidRom)
                                IT.SubItems.Add(header.MemoryMapper.ToString() + ", " + header.GetMapperName());
                            switch (TR.Folder.Filter)
                            {
                                case FolderFilter.All:
                                    listView1.Items.Add(IT);
                                    break;
                                case FolderFilter.SupportedMappersOnly:
                                    if (header.SupportedMapper())
                                        listView1.Items.Add(IT);
                                    break;
                                case FolderFilter.Mapper:
                                    if (header.MemoryMapper == Convert.ToInt32(TextBox1_mapper.Text))
                                        listView1.Items.Add(IT);
                                    break;
                            }
                            break;
                        case ".rar":
                            var IT1 = new ListViewItemRom();
                            IT1.ImageIndex = 3;
                            IT1.RomPath = Dir;
                            IT1.Text = Path.GetFileName(Dir);
                            IT1.SubItems.Add(Program.GetFileSize(Dir));
                            IT1.SubItems.Add("N/A");
                            listView1.Items.Add(IT1);
                            break;
                        case ".zip":
                            var IT2 = new ListViewItemRom();
                            IT2.ImageIndex = 3;
                            IT2.RomPath = Dir;
                            IT2.Text = Path.GetFileName(Dir);
                            IT2.SubItems.Add(Program.GetFileSize(Dir));
                            IT2.SubItems.Add("N/A");
                            listView1.Items.Add(IT2);
                            break;
                        case ".7z":
                            var IT3 = new ListViewItemRom();
                            IT3.ImageIndex = 3;
                            IT3.RomPath = Dir;
                            IT3.Text = Path.GetFileName(Dir);
                            IT3.SubItems.Add(Program.GetFileSize(Dir));
                            IT3.SubItems.Add("N/A");
                            listView1.Items.Add(IT3);
                            break;
                    }
                }
                label1_status.Text = listView1.Items.Count.ToString() + " items found.";
            }
        }

        private void RenameSelected(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            treeView1.SelectedNode.BeginEdit();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                e.CancelEdit = true;
                return;
            }
            if (e.Label == null)
            {
                return;
            }
            var fol = (TreeNodeFolder) treeView1.SelectedNode;
            fol.Folder.Name = e.Label;
            fol.Text = e.Label;
            //Properties
            listView2.Items.Clear();
            listView2.Items.Add("Name");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(((TreeNodeFolder) e.Node).Folder.Name);
            listView2.Items.Add("Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(((TreeNodeFolder) e.Node).Folder.Path);
            listView2.Items.Add("Snapshots Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(
                ((TreeNodeFolder) e.Node).Folder.ImagesFolder);
            listView2.Items.Add("Info Texts Path");
            listView2.Items[listView2.Items.Count - 1].SubItems.Add(
                ((TreeNodeFolder) e.Node).Folder.InfosFolder);
        }

        //Play
        private void PlayRom(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;
            var IT = (ListViewItemRom) listView1.SelectedItems[0];
            if (File.Exists(IT.RomPath))
            {
                SaveSettings();
                _theMainForm.OpenRom(IT.RomPath);
                _theMainForm.Select();
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                PlayRom(this, null);
        }

        private void Frm_Browser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                PlayRom(this, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PlayRom(this, null);
        }

        //snapshotes
        private void ChangeImageFolder(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            var TR = (TreeNodeFolder) treeView1.SelectedNode;
            var Fol = new FolderBrowserDialog();
            Fol.Description = "Images folder";
            Fol.ShowNewFolderButton = true;
            Fol.SelectedPath = TR.Folder.ImagesFolder;
            if (Fol.ShowDialog() == DialogResult.OK)
            {
                TR.Folder.ImagesFolder = Fol.SelectedPath;
                //Properties
                listView2.Items.Clear();
                listView2.Items.Add("Name");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
                listView2.Items.Add("Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
                listView2.Items.Add("Snapshots Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
                listView2.Items.Add("Info Texts Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            }
        }

        //text
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            var TR = (TreeNodeFolder) treeView1.SelectedNode;
            var Fol = new FolderBrowserDialog();
            Fol.Description = "Info files folder";
            Fol.ShowNewFolderButton = true;
            Fol.SelectedPath = TR.Folder.InfosFolder;
            if (Fol.ShowDialog() == DialogResult.OK)
            {
                TR.Folder.InfosFolder = Fol.SelectedPath;
                //Properties
                listView2.Items.Clear();
                listView2.Items.Add("Name");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Name);
                listView2.Items.Add("Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.Path);
                listView2.Items.Add("Snapshots Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.ImagesFolder);
                listView2.Items.Add("Info Texts Path");
                listView2.Items[listView2.Items.Count - 1].SubItems.Add(TR.Folder.InfosFolder);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Resources.NESDoc;
            richTextBox1.Text = "";
            if (listView1.SelectedItems.Count != 1)
                return;
            if (treeView1.SelectedNode == null)
                return;
            if (_SelectedFolder == null)
                return;

            #region Load Image

            try
            {
                string[] Exs = {
                                   ".jpg", ".JPG", ".png", ".PNG", ".bmp", ".BMP", ".gif", ".GIF", ".jpeg", ".JPEG",
                                   ".tiff", ".TIFF"
                               };
                var Names = Directory.GetFiles(_SelectedFolder.Folder.ImagesFolder);
                var FileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
                foreach (var STR in Names)
                {
                    if (Path.GetFileNameWithoutExtension(STR).Length >=
                        Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text).Length)
                    {
                        if (
                            Path.GetFileNameWithoutExtension(STR).Substring(0,
                                                                            Path.GetFileNameWithoutExtension(
                                                                                listView1.SelectedItems[0].Text).
                                                                                Length) ==
                            Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text))
                        {
                            FileName = Path.GetFileNameWithoutExtension(STR);
                            break;
                        }
                    }
                }
                foreach (var STR in Exs)
                {
                    if (File.Exists(_SelectedFolder.Folder.ImagesFolder + "//" +
                                    FileName + STR))
                    {
                        pictureBox1.Image = Image.FromFile(_SelectedFolder.Folder.ImagesFolder + "//" +
                                                           FileName + STR);
                        break;
                    }
                    pictureBox1.Image = Resources.NESDoc;
                }
            }
            catch
            {
                pictureBox1.Image = Resources.NESDoc;
            }

            #endregion

            #region Load Info Text

            try
            {
                var FileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
                richTextBox1.Text = ""; //Clear
                richTextBox1.Lines =
                    File.ReadAllLines(_SelectedFolder.Folder.InfosFolder + "//" + FileName + ".txt");
            }
            catch
            {
                richTextBox1.Text = "";
            }

            #endregion
        }

        private void Frm_Browser_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;
            if (treeView1.SelectedNode == null)
                return;
            if (_SelectedFolder == null)
                return;
            var fileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
            if (Directory.Exists(_SelectedFolder.Folder.InfosFolder))
            {
                File.WriteAllLines(_SelectedFolder.Folder.InfosFolder + "//" + fileName
                                   + ".txt", richTextBox1.Lines, Encoding.Unicode);
            }
            else
            {
                if (
                    MessageBox.Show("This folder hasn't a folder for info files, do you want to browse for it now ?",
                                    "No Infos Folder !!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    var Fol = new FolderBrowserDialog();
                    Fol.Description = "Info files folder";
                    Fol.ShowNewFolderButton = true;
                    Fol.SelectedPath = _SelectedFolder.Folder.InfosFolder;
                    if (Fol.ShowDialog() == DialogResult.OK)
                    {
                        _SelectedFolder.Folder.InfosFolder = Fol.SelectedPath;
                        File.WriteAllLines(_SelectedFolder.Folder.InfosFolder + "//" + fileName
                                           + ".txt", richTextBox1.Lines, Encoding.Unicode);
                    }
                }
            }
        }

        private void ToolStripButton5Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count != 1)
                    return;
                if (treeView1.SelectedNode == null)
                    return;
                if (_SelectedFolder == null)
                    return;
                var fileName = Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text);
                richTextBox1.Text = ""; //Clear
                richTextBox1.Lines =
                    File.ReadAllLines(_SelectedFolder.Folder.InfosFolder + "//" + fileName + ".txt");
            }
            catch
            {
                richTextBox1.Text = "";
            }
        }

        private void TextBox1MapperClick(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(TextBox1_mapper.Text) > 255)
                    TextBox1_mapper.Text = 255.ToString(CultureInfo.InvariantCulture);
                if (_SelectedFolder != null)
                    _SelectedFolder.Folder.Mapper = Convert.ToInt32(TextBox1_mapper.Text);
            }
            catch
            {
                TextBox1_mapper.Text = 0.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void TextBox1MapperTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(TextBox1_mapper.Text) > 255)
                    TextBox1_mapper.Text = 255.ToString(CultureInfo.InvariantCulture);
                if (_SelectedFolder != null)
                    _SelectedFolder.Folder.Mapper = Convert.ToInt32(TextBox1_mapper.Text);
            }
            catch
            {
                TextBox1_mapper.Text = 0.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void ToolStripButton8Click(object sender, EventArgs e)
        {
            treeView1_AfterSelect(this, null);
        }

        private void TilesToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Settings.View = View.LargeIcon;
            tilesToolStripMenuItem.Checked = true;
            detailsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = false;
            listView1.View = Program.Settings.View;
        }

        private void DetailsToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Settings.View = View.Details;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = true;
            listToolStripMenuItem.Checked = false;
            listView1.View = Program.Settings.View;
        }

        private void ListToolStripMenuItemClick(object sender, EventArgs e)
        {
            Program.Settings.View = View.List;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = true;
            listView1.View = Program.Settings.View;
        }

        private void ToolStripButton9Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
                return;
            var founded = new List<ListViewItemRom>();
            for (var i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Text.Length >= TextBox1_search.Text.Length &
                    listView1.Items[i].Text.ToLower().Substring(0, TextBox1_search.Text.Length) ==
                    TextBox1_search.Text.ToLower())
                {
                    founded.Add((ListViewItemRom) listView1.Items[i]);
                }
            }
            listView1.Items.Clear();
            foreach (var innn in founded)
                listView1.Items.Add(innn);
            label1_status.Text = string.Format("{0} item(s) matched.", listView1.Items.Count.ToString(CultureInfo.InvariantCulture));
        }

        private void ComboBox1NavDropDownClosed(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (_SelectedFolder == null)
                return;
            switch (ComboBox1_nav.SelectedIndex)
            {
                case 0:
                    _SelectedFolder.Folder.Filter = FolderFilter.All;
                    TextBox1_mapper.Enabled = false;
                    break;
                case 1:
                    _SelectedFolder.Folder.Filter = FolderFilter.SupportedMappersOnly;
                    TextBox1_mapper.Enabled = false;
                    break;
                case 2:
                    _SelectedFolder.Folder.Filter = FolderFilter.Mapper;
                    TextBox1_mapper.Enabled = true;
                    break;
            }
            treeView1_AfterSelect(this, null);
        }

        //folder properties double click
        private void ListView2MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (listView2.SelectedItems.Count != 1)
                return;
            switch (listView2.SelectedItems[0].Text)
            {
                case "Snapshots Path":
                    ChangeImageFolder(this, null);
                    break;
                case "Info Texts Path":
                    toolStripButton7_Click(this, null);
                    break;
                case "Name":
                    RenameSelected(this, null);
                    break;
            }
        }

        private void RootFolderToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddRootFolder();
        }

        private void FolderInSelectedToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddFolder();
        }
    }
}