namespace Emanate.Service
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
            this.monitoringServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.monitoringServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // monitoringServiceProcessInstaller
            // 
            this.monitoringServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.monitoringServiceProcessInstaller.Password = null;
            this.monitoringServiceProcessInstaller.Username = null;
            // 
            // monitoringServiceInstaller
            // 
            this.monitoringServiceInstaller.DisplayName = "Emanate Monitoring Service";
            this.monitoringServiceInstaller.ServiceName = "EmanateService";
            this.monitoringServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.monitoringServiceProcessInstaller,
            this.monitoringServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller monitoringServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller monitoringServiceInstaller;
    }
}