
namespace NzbDroneWatcher
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NzbDroneWatcherProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.NzbDroneWatcherServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // NzbDroneWatcherProcessInstaller
            // 
            this.NzbDroneWatcherProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.NzbDroneWatcherProcessInstaller.Password = null;
            this.NzbDroneWatcherProcessInstaller.Username = null;
            // 
            // NzbDroneWatcherServiceInstaller
            // 
            this.NzbDroneWatcherServiceInstaller.DelayedAutoStart = true;
            this.NzbDroneWatcherServiceInstaller.Description = "Restarts Nzb Drone services - Such as Radarr, Sonarr, Lidarr";
            this.NzbDroneWatcherServiceInstaller.DisplayName = "NzbDroneWatcher";
            this.NzbDroneWatcherServiceInstaller.ServiceName = "NzbDroneWatcher";
            this.NzbDroneWatcherServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.NzbDroneWatcherProcessInstaller,
            this.NzbDroneWatcherServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller NzbDroneWatcherProcessInstaller;
        private System.ServiceProcess.ServiceInstaller NzbDroneWatcherServiceInstaller;
    }
}