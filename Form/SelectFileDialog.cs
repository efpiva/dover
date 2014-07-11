using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Dover.Framework.Form
{
    public enum DialogType
    {
        SAVE,
        OPEN,
        FOLDER
    };

    public class SelectFileDialog
    {
        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        public string SelectedFile { get ; private set; }
        public string SelectedFolder { get ; private set; }

        private string folder, file, filter;
        private DialogType type;

        public SelectFileDialog(string folder, string file, string filter,
            DialogType type)
        {
            if (folder == null || file == null || filter == null)
                throw new ArgumentException(Messages.AllArgumentsMandatory);

            this.folder = folder;
            this.file = file;
            this.filter = filter;
            this.type = type;
        }

        private void InternalSelectFileDialog()
        {
            var form = new System.Windows.Forms.Form();

            form.TopMost = true;
            form.Height = 0;
            form.Width = 0;
            form.WindowState = FormWindowState.Minimized;
            form.Visible = true;

            switch (type)
            {
                case DialogType.FOLDER:
                    FolderDialog(form);
                    break;
                case DialogType.OPEN:
                    OpenDialog(form);
                    break;
                case DialogType.SAVE:
                    SaveDialog(form);
                    break;
            }
            shutdownEvent.Set();
        }

        private void FolderDialog(System.Windows.Forms.Form form)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Description = Messages.FolderDialogTitle;
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            //----------------------------------------------------------------//
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                form.Close();
                SelectedFolder = dialog.SelectedPath;
            }
            else
            {
                form.Close();
                SelectedFolder = "";
            }
        }

        private void OpenDialog(System.Windows.Forms.Form form)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            OpenOrSaveDialog(dialog, form);
        }

        private void SaveDialog(System.Windows.Forms.Form form)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            OpenOrSaveDialog(dialog, form);
        }

        private void OpenOrSaveDialog(FileDialog dialog, System.Windows.Forms.Form form)
        {
            dialog.Title = Messages.FileDialogTitle;
            dialog.Filter = filter; //"TXT files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = folder;
            dialog.FileName = file;
            //----------------------------------------------------------------//
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                form.Close();
                SelectedFile = dialog.FileName;
            }
            else
            {
                form.Close();
                SelectedFile = "";
            }
        }

        public void Open()
        {
            Thread t = new Thread(new ThreadStart(this.InternalSelectFileDialog));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            shutdownEvent.WaitOne();
        }
    }
}
