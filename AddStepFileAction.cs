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

        public bool IgnoreKPG { get; set; }
        public bool IgnoreDS { get; set; }
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
                Item itemRev = ServiceManager.ItemService.GetLatestItemByItemNumber(item.ItemNum);
                PropInst[] props;
                props = ServiceManager.PropertyService.GetProperties("ITEM", new long[] { item.Id }, new long[] { 109 });
                String KiestraProductGroup = "";
                if (props[0].Val != null)
                {
                    KiestraProductGroup = props[0].Val.ToString();
                }
                if ((KiestraProductGroup == "1100") ||
                    (KiestraProductGroup == "3000") ||
                    (KiestraProductGroup == "4000") ||
                    (KiestraProductGroup == "7000") ||
                    IgnoreKPG)
                {
                    Autodesk.Connectivity.WebServices.File iptiamfile = FindIptIamFile(itemRev);
                    if (iptiamfile != null)
                    {
                        bool AllFinal = true;
                        if (IgnoreDS == false)
                        {
                            FilePathArray[] fparray = ServiceManager.DocumentService.GetLatestAssociatedFilePathsByMasterIds(new long[] { iptiamfile.MasterId }, FileAssociationTypeEnum.None, false, FileAssociationTypeEnum.Dependency, true, false, true, false);
                            foreach (FilePath fp in fparray[0].FilePaths)
                            {
                                props = ServiceManager.PropertyService.GetProperties("FILE", new long[] { fp.File.Id }, new long[] { 44 });
                                if (props[0].Val == null)
                                {
                                    AllFinal = false;
                                }
                                else
                                {
                                    AllFinal = AllFinal && (props[0].Val.ToString() == "Final");
                                }
                            }
                        }
                        if (AllFinal || IgnoreDS)
                        {
                            ProcessItem(itemRev, iptiamfile);
                        }
                        else
                        {
                            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Not all dependencies are Final"));
                        }
                    }
                }
                else
                {
                    OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Invalid Product Group"));
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

        private Autodesk.Connectivity.WebServices.File FindIptIamFile(Item item)
        {
            ItemFileAssoc[] associations = ServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ItemFileLnkTypOpt.Primary);
            string ext = "";
            Autodesk.Connectivity.WebServices.File iptiamfile = null;
            //kunnen er meer primary associated files zijn?
            foreach (ItemFileAssoc association in associations)
            {
                ext = System.IO.Path.GetExtension(association.FileName);
                if ((ext == ".ipt") || (ext == ".iam")) 
                {
                    iptiamfile = ServiceManager.DocumentService.GetFileById(association.CldFileId);
                    OnItemFileUpdate(new ItemFileUpdateEventArgs(item, iptiamfile.Name, iptiamfile.FileRev.Label));
                    return iptiamfile;
                }
            }
            OnItemFileUpdate(new ItemFileUpdateEventArgs(item, "Not found", "-"));
            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Failed"));
            return null;
        }

        private void ProcessItem(Item item, Autodesk.Connectivity.WebServices.File iptiamfile)
        {
            FilePath [] filepaths = ServiceManager.DocumentService.FindFilePathsByNameAndChecksum(iptiamfile.Name, iptiamfile.Cksum);
            String iptiampath = filepaths[0].Path.Replace("$", "C:/Vault WorkingFolder");
            
            String stepfilename = "C:/Vault WorkingFolder/Export Files/" + item.ItemNum + "-" + iptiamfile.FileRev.Label + ".stp";
            
            //checking if attachment exists..
            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Checking attachments.."));
            Attmt[] atts = ServiceManager.ItemService.GetAttachmentsByItemId(item.Id);
            foreach (Attmt att in atts)
            {
                //getting file object of attachment..
                Autodesk.Connectivity.WebServices.File attmtfile = ServiceManager.DocumentService.GetFileById(att.FileId);
                //checking filename..
                if (attmtfile.Name == System.IO.Path.GetFileName(stepfilename))
                {
                    OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Step-file already attached"));
                    return;
                }
            }
            //no stepfile found as attachment
            //looking for step file in vault...
            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Searching for step-file in vault.."));
            PropDef[] filePropDefs = ServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
            PropDef filenamePropDef = filePropDefs.Single(n => n.SysName == "Name");
            SrchCond isFilename = new SrchCond()
            {
                PropDefId = filenamePropDef.Id,
                PropTyp = PropertySearchType.SingleProperty,
                SrchOper = 3,
                SrchRule = SearchRuleType.Must,
                SrchTxt = System.IO.Path.GetFileName(stepfilename)
            };
            string bookmark = string.Empty;
            SrchStatus status = null;
            Autodesk.Connectivity.WebServices.File[] results = ServiceManager.DocumentService.FindFilesBySearchConditions(new SrchCond[] { isFilename }, null, null, false, true, ref bookmark, out status);

            Autodesk.Connectivity.WebServices.File newfile;
            
            if (results == null)
            {
                //no stepfile in vault, user must add
                OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, System.IO.Path.GetFileName(stepfilename) + " not found in vault"));
                return;

                

                /*
                //no stepfile in vault, downloading ipt and generating stepfile
                OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Retreiving files.."));
                
                ByteArray buffer;
                String fullpath;
                FilePathArray [] fparray = ServiceManager.DocumentService.GetLatestAssociatedFilePathsByMasterIds(new long [] {iptiamfile.MasterId}, FileAssociationTypeEnum.None, false, FileAssociationTypeEnum.Dependency, true, false, true, false);
                foreach (FilePath fp in fparray[0].FilePaths)
                {
                    fullpath = fp.Path.Replace("$", "C:/Vault WorkingFolder");
                    ServiceManager.DocumentService.DownloadFile(fp.File.Id, true, out buffer);
                    if (System.IO.File.Exists(fullpath) == true)
                    {
                        System.IO.File.SetAttributes(fullpath, FileAttributes.Normal);
                    }
                    System.IO.File.WriteAllBytes(fullpath, buffer.Bytes);
                    System.IO.File.SetAttributes(fullpath, FileAttributes.ReadOnly);
                }

                Inventor.Application m_inventorApp;
                OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Looking for Inventor.."));
                try //Try to get an active instance of Inventor
                {
                    try
                    {
                        m_inventorApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;
                    }
                    catch
                    {
                        Type inventorAppType = System.Type.GetTypeFromProgID("Inventor.Application");

                        m_inventorApp = System.Activator.CreateInstance(inventorAppType) as Inventor.Application;

                        //Must be set visible explicitly
                        m_inventorApp.Visible = true;
                    }
                }
                catch
                {
                    OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "couldn't create Inventor instance"));
                    return;
                }
                OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Opening file.."));
                _Document iptdocu = m_inventorApp.Documents.Open(iptiampath, true);
                
                //Inventor.ApprenticeServerComponent appserver;
                //appserver = new ApprenticeServerComponent();
                //Inventor.ApprenticeServerDocument appdocu;
                //appdocu = appserver.Open(iptiampath);

                OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Generating step-file.."));

                if (System.IO.File.Exists(stepfilename) == true)
                {
                    System.IO.File.SetAttributes(stepfilename, FileAttributes.Normal);
                }

                iptdocu.SaveAs(stepfilename, true);
                
                //FileSaveAs sa;
                //sa = appserver.FileSaveAs;
                //sa.AddFileToSave(appdocu, stepfilename);
                //sa.ExecuteSaveCopyAs();

                System.IO.File.SetAttributes(stepfilename, FileAttributes.ReadOnly);

                OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Closing part-file.."));
                iptdocu.Close(true);
                //appdocu.Close();

                //OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Deleting part-file.."));
                //System.IO.File.Delete(iptfilename);

                OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Adding step-file to vault.."));
                // add file to vault + attach

                Folder[] folders = ServiceManager.DocumentService.FindFoldersByPaths(new string[] { "$/Export Files" });
                long folderid = folders[0].Id;
                System.IO.FileInfo info = new FileInfo(stepfilename);
                buffer = new ByteArray();
                buffer.Bytes = System.IO.File.ReadAllBytes(stepfilename);

                //OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "addfile"));
                try
                {
                    newfile = ServiceManager.DocumentService.AddFile(folderid, System.IO.Path.GetFileName(stepfilename), "Added by Martijns stepfileplugin", info.LastWriteTime, null, null, FileClassification.None, false, buffer);
                }
                catch (Exception ex)
                {
                    OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Failed"));
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }
                //1008 addfileexists
                //1009 addfile failed
                 */
            }
            else
            {
                if (results.Count() > 1)
                {
                    OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Failed"));
                    MessageBox.Show("Error: more then 1 file with the name " + System.IO.Path.GetFileName(stepfilename + " exist in vault!"));
                    return;
                }
                newfile = results[0];
            }
            //OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "new attmt"));
            
            Attmt newattachment = new Attmt();
            newattachment.FileId = newfile.Id;
            newattachment.Pin = false;

            //OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "new edititem"));
            
            Array.Resize(ref atts, atts.Count() + 1);
            atts[atts.Count() - 1] = newattachment;

            Item edititem = ServiceManager.ItemService.EditItem(item.RevId);

            ServiceManager.ItemService.UpdateAndCommitItem(edititem, 0, false, null, null, null, atts, null, null, 2);

            OnItemStatusUpdate(new ItemStatusUpdateEventArgs(item, "Done"));
        }
    }
}