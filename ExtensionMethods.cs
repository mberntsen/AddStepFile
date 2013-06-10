using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.Connectivity.WebServicesTools;

namespace AddStepFile
{
    static class Util
    {
        private static List<Exception> ExceptionCollection;

        internal static void Init()
        {
            ExceptionCollection = new List<Exception>();
        }

        public static void DoAction(Action a)
        {
            try
            {
                a();
            }
            catch (Exception ex)
            {
                if (!Thread.CurrentThread.IsBackground)
                    MessageBox.Show(ex.Message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    lock (ExceptionCollection)
                    {
                        ExceptionCollection.Add(ex);
                    }
                }
            }
        }

        public static void PrintErrors()
        {
            lock (ExceptionCollection)
            {
                if (!ExceptionCollection.Any())
                    return;

                System.Text.StringBuilder output = new System.Text.StringBuilder();

                output.AppendLine("Errors found");
                foreach (Exception ex in ExceptionCollection)
                {
                    output.AppendLine(ex.Message);
                }

                ExceptionCollection.Clear();

                MessageBox.Show(output.ToString(), "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    static class ExtensionMethods
    {
        public static IWebServiceCredentials GetCredentials(this VaultContext context)
        {
            return new UserIdTicketCredentials(context.RemoteBaseUrl.ToString(),
                context.VaultName, context.UserId, context.Ticket);
        }

        public static void InvokeIfRequired(this Control control, MethodInvoker method)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(method);
            }
            else
            {
                method();
            }
        }

        
    }
}
