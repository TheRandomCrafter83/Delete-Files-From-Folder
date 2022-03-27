using System;
using System.Windows.Forms;
using System.IO;

namespace Delete_Files_From_Folder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = fd.SelectedPath;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //deleteFilesInFolder(txtDirectory.Text);
            FileManager fileManager = new FileManager();
            fileManager.Error += FileManager_Error;
            fileManager.FileDeleted += FileManager_FileDeleted;
            fileManager.StatusUpdate += FileManager_StatusUpdate;
            fileManager.deleteFilesInDirectory(txtDirectory.Text);
        }

        private void FileManager_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            addThreadSafe(e.message);
        }

        private void FileManager_FileDeleted(object sender, FileDeletedEventArgs e)
        {
            addThreadSafe(e.fileName);
        }

        private void FileManager_Error(object sender, ErrorEventArgs e)
        {
            addThreadSafe(e.message);
        }


        private void addThreadSafe(String message)
        {
            lbStatus.BeginInvoke(
                    (Action)(() =>
                        lbStatus.Items.Add("[!] - " + message)
                    )
                );
        }

        private void deleteFilesInFolder(String path)
        {
            lbStatus.Items.Clear();           
            lbStatus.Items.Add("[!] - Checking for Directory");
            if (!Directory.Exists(path)) return;

            FileInfo fileInfo = new FileInfo(path);
            String[] files = Directory.GetFiles(path);
            lbStatus.Items.Add("[!] - There are [" + files.Length.ToString() + "] to be deleted.");
            foreach (var file in files)
            {
                String filename = Path.GetFileNameWithoutExtension(file);
                File.Delete(file);
                if (File.Exists(file))
                {
                    lbStatus.Items.Add("[!] - There was an error deleting file: " + filename);
                }
                else
                {
                    lbStatus.Items.Add("[!] - File Deleted: " + filename);
                }
                Application.DoEvents();
            }
            lbStatus.Items.Add("[!] - All files have been deleted.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chkRemember.Checked = Properties.Settings.Default.isRememberChecked;
            txtDirectory.Text = Properties.Settings.Default.previousDirectory;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.isRememberChecked = chkRemember.Checked;
            Properties.Settings.Default.Save();
            if (!chkRemember.Checked) return;
            Properties.Settings.Default.previousDirectory = txtDirectory.Text;
            Properties.Settings.Default.Save();
        }

        private void chkRemember_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.previousDirectory = "";
            Properties.Settings.Default.Save();
        }
    }
}
