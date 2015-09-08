﻿/*
    Little System Cleaner
    Copyright (C) 2008 Little Apps (http://www.little-apps.com/)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;
using Little_System_Cleaner.Misc;
using Little_System_Cleaner.Properties;
using MessageBox = System.Windows.MessageBox;

namespace Little_System_Cleaner.Tab_Controls.Options
{
    public partial class Options : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public int SelectedUpdateDelay
        {
            get
            {
                switch (Settings.Default.optionsUpdateDelay)
                {
                    case 3:
                        return 0;
                    case 5:
                        return 1;
                    case 7:
                        return 2;
                    default:
                        return 3;
                }
            }
            set
            {
                int index = value;

                switch (index)
                {
                    case 0:
                        Settings.Default.optionsUpdateDelay = 3;
                        break;
                    case 1:
                        Settings.Default.optionsUpdateDelay = 5;
                        break;
                    case 2:
                        Settings.Default.optionsUpdateDelay = 7;
                        break;
                    default:
                        Settings.Default.optionsUpdateDelay = 14;
                        break;
                }

                OnPropertyChanged("SelectedUpdateDelay");
            }
        }

        public bool? AutoUpdate
        {
            get { return Settings.Default.updateAuto; }
            set
            {
                Settings.Default.updateAuto = value.GetValueOrDefault();

                OnPropertyChanged("AutoUpdate");
            }
        }

        public bool? SysRestore
        {
            get { return Settings.Default.optionsSysRestore; }
            set
            {
                Settings.Default.optionsSysRestore = value.GetValueOrDefault();

                OnPropertyChanged("SysRestore");
            }
        }

        public bool? UsageStats
        {
            get { return Settings.Default.optionsUsageStats; }
            set
            {
                Settings.Default.optionsUsageStats = value.GetValueOrDefault();

                OnPropertyChanged("UsageStats");
            }
        }

        public string LogDirectory
        {
            get { return Settings.Default.OptionsLogDir; }
            set
            {
                Settings.Default.OptionsLogDir = value;

                OnPropertyChanged("LogDirectory");
            }
        }

        public bool? NoProxy
        {
            get { return (Settings.Default.optionsUseProxy == 0); }
            set
            {
                if (value.GetValueOrDefault())
                    Settings.Default.optionsUseProxy = 0;

                OnPropertyChanged("NoProxy");
                OnPropertyChanged("IEProxy");
                OnPropertyChanged("Proxy");
                OnPropertyChanged("ShowProxySettings");
            }
        }

        public bool? IeProxy
        {
            get { return (Settings.Default.optionsUseProxy == 1); }
            set
            {
                if (value.GetValueOrDefault())
                    Settings.Default.optionsUseProxy = 1;

                OnPropertyChanged("NoProxy");
                OnPropertyChanged("IEProxy");
                OnPropertyChanged("Proxy");
                OnPropertyChanged("ShowProxySettings");
            }
        }

        public bool? Proxy
        {
            get { return (Settings.Default.optionsUseProxy == 2); }
            set
            {
                if (value.GetValueOrDefault())
                    Settings.Default.optionsUseProxy = 2;

                OnPropertyChanged("NoProxy");
                OnPropertyChanged("IEProxy");
                OnPropertyChanged("Proxy");
                OnPropertyChanged("ShowProxySettings");
            }
        }

        public Visibility ShowProxySettings => Proxy.GetValueOrDefault() ? Visibility.Visible : Visibility.Hidden;

        public string ProxyAddress
        {
            get { return Settings.Default.optionsProxyHost; }
            set
            {
                Settings.Default.optionsProxyHost = value;

                OnPropertyChanged("ProxyAddress");
            }
        }

        public int? ProxyPort
        {
            get { return Settings.Default.optionsProxyPort; }
            set
            {
                Settings.Default.optionsProxyPort = value.GetValueOrDefault();

                OnPropertyChanged("ProxyPort");
            }
        }

        public bool? ProxyAuthenticate
        {
            get { return Settings.Default.optionsProxyAuthenticate; }
            set
            {
                Settings.Default.optionsProxyAuthenticate = value.GetValueOrDefault();

                OnPropertyChanged("ProxyAuthenticate");
            }
        }

        public string ProxyUser
        {
            get { return Settings.Default.optionsProxyUser; }
            set
            {
                Settings.Default.optionsProxyUser = value;

                OnPropertyChanged("ProxyUser");
            }
        }

        public string ProxyPassword
        {
            get
            {
                using (SecureString secureStr = Utils.DecryptString(Settings.Default.optionsProxyPassword))
                {
                    return Utils.ToInsecureString(secureStr);
                }
            }

            set
            {
                string encryptedPassword = Utils.EncryptString(proxyPassword.SecurePassword);
                if (Settings.Default.optionsProxyPassword != encryptedPassword)
                    Settings.Default.optionsProxyPassword = encryptedPassword;
            }
        }


		public Options()
		{
            InitializeComponent();

            using (SecureString secureStr = Utils.DecryptString(Settings.Default.optionsProxyPassword)) {
                proxyPassword.Password = Utils.ToInsecureString(secureStr);
            }
		}

        public void ShowAboutTab()
        {
            tabControl1.SelectedIndex = tabControl1.Items.IndexOf(tabItemAbout);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (!Utils.LaunchUri(e.Uri.ToString()))
                MessageBox.Show(App.Current.MainWindow, "Unable to detect web browser to open link", Utils.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void buttonSupportThisProject_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.LaunchUri(@"http://www.little-apps.com/?donate"))
                MessageBox.Show(App.Current.MainWindow, "Unable to detect web browser to open link", Utils.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void buttonWebsite_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.LaunchUri(@"http://www.little-apps.com/"))
                MessageBox.Show(App.Current.MainWindow, "Unable to detect web browser to open link", Utils.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog())
            {
                folderBrowserDlg.Description = "Select the folder where the log files will be placed";
                folderBrowserDlg.SelectedPath = textBoxLog.Text;
                folderBrowserDlg.ShowNewFolderButton = true;

                if (folderBrowserDlg.ShowDialog() == DialogResult.OK)
                    LogDirectory = folderBrowserDlg.SelectedPath;
            }
        }

        private void proxyPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            string encryptedPassword = Utils.EncryptString(proxyPassword.SecurePassword);
            if (Settings.Default.optionsProxyPassword != encryptedPassword)
                Settings.Default.optionsProxyPassword = encryptedPassword;
        }
	}
}