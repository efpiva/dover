/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
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
