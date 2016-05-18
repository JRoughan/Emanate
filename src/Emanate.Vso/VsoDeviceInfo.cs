using System;
using Emanate.Extensibility;

namespace Emanate.Vso
{
    public class VsoDeviceInfo : ViewModel
    {
        public Guid Id { get; } = Guid.NewGuid();

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private string uri;
        public string Uri
        {
            get { return uri; }
            set { uri = value; OnPropertyChanged(); }
        }

        private int pollingInterval;
        public int PollingInterval
        {
            get { return pollingInterval; }
            set { pollingInterval = value; OnPropertyChanged(); }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }
    }
}
