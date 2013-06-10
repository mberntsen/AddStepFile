using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;

namespace AddStepFile
{
    public class Extension : IExtension
    {

        #region Fields

        private static string s_ItemContextMenuCommandSiteId = "AddStepFile.ContextMenu";
        private static string s_AddStepFileCommandId = "AddStepFile.AddStepFileCommand";

        #endregion

        public Extension()
        {
        }

        #region IExtension Members

        public IEnumerable<CommandSite> CommandSites()
        {
            CommandSite itemContextMenu = new CommandSite(s_ItemContextMenuCommandSiteId, "AddStepFile")
            {
                DeployAsPulldownMenu = false,
                Location = CommandSiteLocation.ItemContextMenu,
            };
            CommandItem AddStepFileCmd = new CommandItem(s_AddStepFileCommandId, "Add Step File ...")
            {
                Description = "Adds STEP file to an item",
                Hint = "Add STEP file to an item",
                MultiSelectEnabled = true,
                NavigationTypes = new SelectionTypeId[] { SelectionTypeId.Item },
                ToolbarPaintStyle = PaintStyle.TextAndGlyph,
                Image = Resources.Resource.OnesAndZeros
            };

            AddStepFileCmd.Execute += new EventHandler<CommandItemEventArgs>(Handle_AddStepFileCmd_Execute);

            itemContextMenu.AddCommand(AddStepFileCmd);
            return new CommandSite[] { itemContextMenu };
        }

        public IEnumerable<CustomEntityHandler> CustomEntityHandlers()
        {
            return null;
        }

        public IEnumerable<DetailPaneTab> DetailTabs()
        {
            return null;
        }

        public IEnumerable<string> HiddenCommands()
        {
            return null;
        }

        public void OnLogOff(IApplication application)
        {
            ReleaseServiceManager();
        }

        public void OnLogOn(IApplication application)
        {
            ReleaseServiceManager();
            IWebServiceCredentials credentials = application.VaultContext.GetCredentials();

            ServiceManager = new WebServiceManager(credentials);
        }

        public void OnShutdown(IApplication application)
        {
        }

        public void OnStartup(IApplication application)
        {
            Util.Init();
        }

        #endregion

        private WebServiceManager ServiceManager { get; set; }

        private void ReleaseServiceManager()
        {
            if (ServiceManager != null)
            {
                ServiceManager.Dispose();
                ServiceManager = null;
            }
        }

        private void Handle_AddStepFileCmd_Execute(object sender, CommandItemEventArgs e)
        {
            Util.DoAction(delegate
            {
                IEnumerable<long> ItemIds = from s in e.Context.CurrentSelectionSet
                                              where s.TypeId == SelectionTypeId.Item
                                              select s.Id;

                if (ItemIds.Any() == false)
                {
                    return;
                }
                Item[] items = ServiceManager.ItemService.GetItemsByIds(ItemIds.ToArray());

                using (AddStepFileForm frm = new AddStepFileForm())
                {
                    frm.Credentials = e.Context.Application.VaultContext.GetCredentials();
                    frm.Items.AddRange(items);
                    if (frm.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }
            });
        }
    }
}
