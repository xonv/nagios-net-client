using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace ServiceInstaller
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            //System.Diagnostics.Debugger.Launch();
            string cmd = "";
            string args = "";
            try
            {
                var setupDir = this.Context.Parameters["setupDir"];
                System.Diagnostics.Debug.WriteLine(setupDir);
                System.IO.FileInfo fi = new System.IO.FileInfo((setupDir + "NagiosNetClient.exe").Replace(@"\\", @"\"));
                if (fi.Exists == false)
                    throw new Exception("NagiosNetClient.exe file not found");
                cmd = string.Format("{0}{1}", System.Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe").Replace(@"\\", @"\");
                args = string.Format("{0}\"{1}{2}\"", @"/LogToConsole=false ", setupDir, "NagiosNetClient.exe").Replace(@"\\", @"\");
                System.Diagnostics.Process.Start(cmd, args).WaitForExit();
                System.Diagnostics.Process.Start("net", "start \"NagiosNetClient\"").WaitForExit();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(cmd + System.Environment.NewLine + args + System.Environment.NewLine + ex.Message + System.Environment.NewLine + ex.StackTrace);
            }
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            //System.Diagnostics.Debugger.Launch();
            string cmd = "";
            string args = "";
            try
            {
                var setupDir = this.Context.Parameters["setupDir"];
                System.Diagnostics.Debug.WriteLine(setupDir);
                System.IO.FileInfo fi = new System.IO.FileInfo((setupDir + "NagiosNetClient.exe").Replace(@"\\", @"\"));
                if (fi.Exists == false)
                    throw new Exception("NagiosNetClient.exe file not found");

                System.Diagnostics.Process.Start("net", "stop \"NagiosNetClient\"").WaitForExit();

                cmd = string.Format("{0}{1}", System.Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe").Replace(@"\\", @"\");
                args = string.Format("{0}\"{1}{2}\"", @"/u /LogToConsole=false ", setupDir, "NagiosNetClient.exe").Replace(@"\\", @"\");
                System.Diagnostics.Process.Start(cmd, args).WaitForExit();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(cmd + System.Environment.NewLine + args + System.Environment.NewLine + ex.Message + System.Environment.NewLine + ex.StackTrace);
            }
        }

    }
}
