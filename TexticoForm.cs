using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Textico.TexticoForm.ESaveChanges;

namespace Textico
{
    public partial class TexticoForm : Form
    {
        string TitleText = "Textico v1.0";
        string NoFileName = "Untitled";
        string FileName = "";
        string SavedContent = "";
        string FileFilter = "Text files(.txt)|*.txt|All files|*.*";

        public enum ESaveChanges { Continue, Abort }
        public TexticoForm()
        {
            FileName = NoFileName;
            InitializeComponent();
            richTextBox.Clear();
            UpdateTitle();
        }

        ESaveChanges AskAndSaveChanges()
        {
            if (!HasChanges()) return Continue;
            var choice = MessageBox.Show("Save changes" + (!String.IsNullOrEmpty(FileName) ? " to " + FileName : ""), "Save",
                MessageBoxButtons.YesNoCancel);
            if (choice == DialogResult.Cancel)
                return Abort;
            if (choice != DialogResult.Yes)
                return Continue;

            SaveChanges();
            return Continue;
        }

        private void SaveChanges()
        {
            if (!String.IsNullOrEmpty(FileName) && FileName != NoFileName)
                SaveFile(FileName);
            else
                SaveAs();
        }

        private void SaveFile(string fileName)
        {
            SavedContent = richTextBox.Text;
            File.WriteAllText(fileName, richTextBox.Text);
            UpdateTitle();
        }

        private void SaveAs()
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = FileNameWithoutPath(FileName);
            dlg.Filter = FileFilter;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileName = dlg.FileName;
                SaveFile(FileName);
            }
        }

        private void UpdateTitle()
        {
            Text = (HasChanges() ? "*" : "") + FileNameWithoutPath(FileName) + (String.IsNullOrEmpty(FileName) ? "" : " - ") + TitleText;
        }

        private bool HasChanges()
        {
            return SavedContent != richTextBox.Text;
        }

        string FileNameWithoutPath(string fileName)
        {
            var slashIndex = fileName.LastIndexOf("\\");
            return slashIndex < 0 ? fileName : fileName.Substring(slashIndex + 1);
        }


        #region Menu commands

        private void HelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A very simple text editor by ZorbaGyrka");
        }

        private void FileNew_Click(object sender, EventArgs e)
        {
            if (AskAndSaveChanges() == Abort)
                return;
            FileName = NoFileName;
            SavedContent = "";
            richTextBox.Clear();
            UpdateTitle();
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = FileFilter;
            if (dlg.ShowDialog() != DialogResult.OK)
                return;
            FileName = dlg.FileName;
            SavedContent = File.ReadAllText(FileName);
            richTextBox.Text = SavedContent;
            UpdateTitle();
        }

        private void FileSave_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void FileSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTitle();
        }
    }
}
