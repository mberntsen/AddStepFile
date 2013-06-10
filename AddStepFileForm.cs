using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using System.Drawing;

namespace AddStepFile
{
    public partial class AddStepFileForm : Form
    {
        public AddStepFileForm()
        {
            InitializeComponent();
            Items = new List<Item>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing && (Worker != null))
            {
                Worker.Dispose();
                Worker = null;
            }
            base.Dispose(disposing);
        }

        public IWebServiceCredentials Credentials { get; set; }

        public List<Item> Items { get; private set; }
        public FilePath SelectedFile { get; private set; }

        private bool IncludeSubitems { get; set; }
        private BackgroundWorker Worker { get; set; }

        private void Handle_Form_Load(object sender, EventArgs e)
        {
            Util.DoAction(delegate
            {
                StringBuilder sb = new StringBuilder();

                _listView.BeginUpdate();
                foreach (Item item in Items)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(";");
                    }
                    sb.Append(item.Title);
                    ListViewItem newItem = new ListViewItem();

                    newItem.Text = item.ItemNum;
                    //newItem.SubItems.Add(file.Cat.CatName);
                    newItem.SubItems.Add(item.RevNum);
                    //newItem.SubItems.Add(item.LfCycStateId.ToString);
                    newItem.SubItems.Add(item.Title);
                    newItem.SubItems.Add("-");
                    newItem.SubItems.Add("-");
                    newItem.SubItems.Add("-");
                    //newItem.ImageIndex = GetImageIndex(file.Name);
                    _listView.Items.Add(newItem);
                }
                _listView.EndUpdate();
                //_txtStatus.Text = file);
                Worker = new BackgroundWorker();
                Worker.WorkerSupportsCancellation = true;
                Worker.WorkerReportsProgress = true;
                Worker.DoWork += new DoWorkEventHandler(Handle_Worker_DoWork);
                Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Handle_Worker_RunWorkerCompleted);
            });
        }

        private void Handle_BtnStart_Click(object sender, EventArgs e)
        {
            Util.DoAction(delegate
            {
                Worker.RunWorkerAsync();
            });
            Util.PrintErrors();
        }

        private void Handle_BtnStop_Click(object sender, EventArgs e)
        {
            Util.DoAction(delegate
            {
                Worker.CancelAsync();
            });
            Util.PrintErrors();
        }

        private void Handle_BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Handle_BtnGoToFile_Click(object sender, EventArgs e)
        {
            Util.DoAction(delegate
            {
                if (_listView.SelectedItems.Count > 0)
                {
                    ListViewItem selectedItem = _listView.SelectedItems[0];

                    SelectedFile = selectedItem.Tag as FilePath;
                }
                DialogResult = DialogResult.OK;
                Close();
            });
        }

        private void Handle_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Util.DoAction(delegate
            {
                _btnStart.InvokeIfRequired(() => _btnStart.Enabled = false);
                _btnClose.InvokeIfRequired(() => _btnClose.Enabled = false);
                _btnStop.InvokeIfRequired(() => _btnStop.Enabled = true);
                //_listView.InvokeIfRequired(() => _listView.Items.Clear());
                IncludeSubitems = _checkBoxIncludeSubitems.Checked;
                // the method is run in separate thread so we create separate instance of WebServiceManager
                using (WebServiceManager serviceMgr = new WebServiceManager(Credentials))
                {
                    AddStepFileAction StepAction = new AddStepFileAction
                    {
                        ServiceManager = serviceMgr,
                        IncludeSubitems = _checkBoxIncludeSubitems.Checked,
                    };

                    StepAction.ContinueProcessing += new EventHandler<CancelEventArgs>(Handle_StepAction_ContinueProcessing);
                    StepAction.ItemStatusUpdate += new EventHandler<ItemStatusUpdateEventArgs>(Handle_StepAction_ItemStatusUpdate);
                    StepAction.ItemFileUpdate += new EventHandler<ItemFileUpdateEventArgs>(Handle_StepAction_ItemFileUpdate);
                    StepAction.GenerateAndAdd(Items);
                }
            });
        }

        private void Handle_StepAction_ContinueProcessing(object sender, CancelEventArgs e)
        {
            if (Worker.IsBusy && Worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void Handle_StepAction_ItemStatusUpdate(object sender, ItemStatusUpdateEventArgs e)
        {
            Util.DoAction(delegate
            {
                Item item = e.newitem;
                String status = e.newstatus;

                if (item == null)
                {
                    return;
                }
                foreach (ListViewItem listitem in _listView.Items)
                {
                    if (listitem.Text == item.ItemNum)
                    {
                        listitem.SubItems[5].Text = status;
                    }
                }
            });
        }

        private void Handle_StepAction_ItemFileUpdate(object sender, ItemFileUpdateEventArgs e)
        {
            Util.DoAction(delegate
            {
                Item item = e.newitem;
                String filename = e.newfilename;
                String filerev = e.newrevision;

                if (item == null)
                {
                    return;
                }
                foreach (ListViewItem listitem in _listView.Items)
                {
                    if (listitem.Text == item.ItemNum)
                    {
                        listitem.SubItems[3].Text = filename;
                        listitem.SubItems[4].Text = filerev;
                    }
                }
            });
        }

        private void Handle_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _btnStart.InvokeIfRequired(() => _btnStart.Enabled = true);
            _btnClose.InvokeIfRequired(() => _btnClose.Enabled = true);
            _btnStop.InvokeIfRequired(() => _btnStop.Enabled = false);
            Util.PrintErrors();
        }

        private void Handle_ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listView = sender as ListView;
            bool enable = false;

            if (listView != null)
            {
                if (listView.SelectedItems.Count > 0)
                {
                    enable = true;
                }
            }
            _btnGoToFile.Enabled = enable;
        }

        private int GetImageIndex(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName);

            if (_imageList.Images.ContainsKey(ext) == false)
            {
                Image fileIcon = ShellFileIcon.GetFileIcon(fileName, ShellFileIcon.FileIconSize.Small);

                _imageList.Images.Add(ext, fileIcon);
            }
            return _imageList.Images.IndexOfKey(ext);
        }

        private static string GetFolderName(string path)
        {
            int index = path.LastIndexOf('/');

            if (index > 0)
            {
                return path.Substring(0, index);
            }
            return path;
        }

    }
}
