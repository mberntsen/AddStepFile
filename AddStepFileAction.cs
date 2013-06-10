using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
//using Autodesk.Connectivity.Explorer.ExtensibilityTools;
using Inventor;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using System.Collections;

namespace AddStepFile
{
    class ItemStatusUpdateEventArgs : EventArgs
    {
        public ItemStatusUpdateEventArgs(Item item, String status)
        {
            newitem = item;
            newstatus = status;
        }

        public Item newitem { get; private set; }
        public String newstatus { get; private set; }
    }

    class ItemFileUpdateEventArgs : EventArgs
    {
        public ItemFileUpdateEventArgs(Item item, String filename, String revision)
        {
            newitem = item;
            newfilename = filename;
            newrevision = revision;
        }

        public Item newitem { get; private set; }
        public String newfilename { get; private set; }
        public String newrevision { get; private set; }
    }

    /// <summary>
    /// Helper class to locate orphaned files = files which are not references by other files.
    /// </summary>
    class AddStepFileAction
    {
        #region Fields

        //private List<FilePath> _files = new List<FilePath>();

        #endregion

        public AddStepFileAction()
        {
        }

        /// <summary>
        /// The event is used as callback to check if processing can continue or not.
        /// </summary>
        public event EventHandler<CancelEventArgs> ContinueProcessing;

        /// <summary>
        /// The event is fired when orphaned files is found.
        /// </summary>
        public event EventHandler<ItemStatusUpdateEventArgs> ItemStatusUpdate;
        public event EventHandler<ItemFileUpdateEventArgs> ItemFileUpdate;

        public bool IncludeSubitems { get; set; }
        public WebServiceManager ServiceManager { get; set; }

        /// <summary>
        /// Returns result of operation.
        /// </summary>
        /*public IEnumerable<FilePath> Files
        {
            get { return _files; }
        }*/

        /// <summary>
        /// Finds orphaned files in given folders.
        /// </summary>
        /// <param name="folders"></param>
        public void GenerateAndAdd(IEnumerable<Item> items)
        {
            //_files.Clear();
            foreach (Item item in items)
            {
                if (CanContinue() == false)
                {
                    return;
                }
                Autodesk.Connectivity.WebServices.File iptfile = FindIptFile(item);
                if (iptfile != null)
                {
                    ProcessItem(item, iptfile);
                }
            }
        }

        /// <summary>
        /// Finds orphaned files in given folder.
        /// </summary>
        /// <param name="folder"></param>
        private bool CanContinue()
        {
            EventHandler<CancelEventArgs> handler = ContinueProcessing;
            bool result = true;

            if (handler != null)
            {
                CancelEventArgs e = new CancelEventArgs
                {
                    Cancel = false,
                };

                handler(this, e);
                result = (e.Cancel ? false : true);
            }
            return result;
        }

        private void OnItemStatusUpdate(ItemStatusUpdateEventArgs e)
        {
            EventHandler<ItemStatusUpdateEventArgs> handler = ItemStatusUpdate;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnItemFileUpdate(ItemFileUpdateEventArgs e)
        {
            EventHandler<ItemFileUpdateEventArgs> handler = ItemFileUpdate;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private Autodesk.Connectivity.WebServices.File FindIptFile(Item item)
        {
            ItemFileAssoc[] associations = ServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ItemFileLnkTypOpt.Primary);
            String iptfilename = "";
            long fileid = 0;
            string ext = "";
            Autodesk.Connectivity.WebServices.File iptfile = null;
            //kunnen er meer primary associated files zijn?
            foreach (ItemFileAssoc association in associations)
            {
                iptfilename = "C:/Vault WorkingFolder/AddStpFileTemp/" + association.FileName;
                fileid = association.CldFileId;
                ext = System.IO.Path.GetExtension(iptfilename);
                if (ext == ".ipt") 
                {
                    iptfile = ServiceManager.DocumentService.GetFileById(fileid);
                    OnItemFileUpdate(new ItemFileUpdateEventArgs(item, iptfile.Name, iptfile.FileRev.Label));
                    return iptfile;
                }
            }
            OnItemFileUpdate(new ItemFileUpdateEventArgs(item, "Not found", "-"));
            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Failed"));
            return null;
        }

        private void ProcessItem(Item item, Autodesk.Connectivity.WebServices.File iptfile)
        {
            //ServiceManager.PropertyService.getp

            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Retreiving file.."));
            // get file from vault
            
            ByteArray buffer;
            String iptfilename = "C:/Vault WorkingFolder/AddStpFileTemp/" + iptfile.Name;
                
            ServiceManager.DocumentService.DownloadFile(iptfile.Id, true, out buffer);
            System.IO.File.WriteAllBytes(iptfilename, buffer.Bytes);
            
            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Opening file.."));
            Inventor.ApprenticeServerComponent appserver;
            appserver = new ApprenticeServerComponent();
            Inventor.ApprenticeServerDocument appdocu;
            appdocu = appserver.Open(iptfilename);

            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Generating step.."));
            //String stepfilename = System.IO.Path.ChangeExtension(iptfilename, ".stp");
            //stepfilename = "C:/Vault WorkingFolder/Export Files/" + System.IO.Path.GetFileName(stepfilename);
            String stepfilename = "C:/Vault WorkingFolder/Export Files/" + item.ItemNum + "-" + iptfile.FileRev.Label + ".stp";
            
            FileSaveAs sa;
            sa = appserver.FileSaveAs;
            sa.AddFileToSave(appdocu, stepfilename);
            sa.ExecuteSaveCopyAs();

            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Closing file.."));
            appdocu.Close();
            System.IO.File.Delete(iptfilename);

            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Adding step-file to vault.."));
            // add file to vault + attach

            Folder[] folders = ServiceManager.DocumentService.FindFoldersByPaths(new string [] {"$/Export Files"});
            long folderid = folders[0].Id;
            System.IO.FileInfo info = new FileInfo(stepfilename);
            buffer.Bytes = System.IO.File.ReadAllBytes(stepfilename);

        //Autodesk.Connectivity.WebServices.File newfile = ServiceManager.DocumentService.AddFile(folderid, System.IO.Path.GetFileName(stepfilename), "Added by Martijns stepfileplugin", info.LastWriteTime, null, null, FileClassification.None, false, buffer);

        //Attmt newattachment = new Attmt();
        //newattachment.FileId = newfile.Id;
        //newattachment.Pin = false;
            
            // Get the file associations for later we need
            // to pass the existing file associations back to the item
            // get all the available properties
            //PropDef[] propdefs = ServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM");
            //long[] ids = new long[propdefs.Length];
            //long idsi = 0;

            // store their ids in a list
            //foreach (PropDef propdef in propdefs)
            //    ids[idsi++] = propdef.Id;

            // get all the properties for this item
            //PropInst[] props = ServiceManager.PropertyService.GetProperties("ITEM", new long[] { item.Id }, ids);
            ////PropInst[] oldprops = ServiceManager.ItemService.GetItemProperties(new long[] { item.Id }, ids);

            //ItemFileAssoc[] assocs = ServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ItemFileLnkTypOpt.Primary | ItemFileLnkTypOpt.Secondary | ItemFileLnkTypOpt.StandardComponent | ItemFileLnkTypOpt.PrimarySub | ItemFileLnkTypOpt.SecondarySub | ItemFileLnkTypOpt.Tertiary);
            ItemFileAssoc[] assocs = ServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ItemFileLnkTypOpt.Primary | ItemFileLnkTypOpt.StandardComponent | ItemFileLnkTypOpt.PrimarySub | ItemFileLnkTypOpt.SecondarySub | ItemFileLnkTypOpt.Tertiary);
            long primaryId = 0;
            bool isPrimarySubComp = false;
            //ArrayList secondaryList = new ArrayList();
            ArrayList stdCompList = new ArrayList();
            ArrayList secSudCompList = new ArrayList();
            ArrayList tertiaryList = new ArrayList();

            foreach (ItemFileAssoc assoc in assocs)
            {
                if (assoc.Typ == ItemFileLnkTyp.Primary)
                    primaryId = assoc.CldFileId;
                if (assoc.Typ == ItemFileLnkTyp.PrimarySub)
                    isPrimarySubComp = true;
                //if (assoc.Typ == ItemFileLnkTyp.Secondary)
                //    secondaryList.Add(assoc.CldFileId);
                if (assoc.Typ == ItemFileLnkTyp.SecondarySub)
                    secSudCompList.Add(assoc.CldFileId);
                if (assoc.Typ == ItemFileLnkTyp.StandardComponent)
                    stdCompList.Add(assoc.CldFileId);
                if (assoc.Typ == ItemFileLnkTyp.Tertiary)
                    tertiaryList.Add(assoc.CldFileId);
            }
            //long[] secondary = (long[])secondaryList.ToArray(typeof(long));
            long[] stdComp = (long[])stdCompList.ToArray(typeof(long));
            long[] secSudComp = (long[])secSudCompList.ToArray(typeof(long));
            long[] tertiary = (long[])tertiaryList.ToArray(typeof(long));
            
            Attmt[] atts = ServiceManager.ItemService.GetAttachmentsByItemId(item.Id);
        //Array.Resize(ref atts, atts.Count() + 1);
        //atts[atts.Count() - 1] = newattachment;

        //ServiceManager.ItemService.UpdateAndCommitItem(item, primaryId, isPrimarySubComp, null, stdComp, secSudComp, atts, null, tertiary, 2);

            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Done"));
        }
    }
}