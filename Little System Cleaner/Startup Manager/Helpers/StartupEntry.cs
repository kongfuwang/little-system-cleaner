﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Little_System_Cleaner.Startup_Manager.Helpers
{
    public class StartupEntry : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion

        private string _cmd;

        public ObservableCollection<StartupEntry> Children { get; } = new ObservableCollection<StartupEntry>();

        public RegistryKey RegKey { get; set; }

        public StartupEntry Parent { get; set; }

        public bool IsLeaf => (Children.Count == 0);

        public string SectionName { get; set; }
        public string Path { get; set; }
        public string Args { get; set; }

        public string Command
        {
            get
            {
                if (_cmd != null)
                    return _cmd;

                if (!IsLeaf)
                {
                    _cmd = string.Empty;
                    return _cmd;
                }

                if (string.IsNullOrWhiteSpace(Path) && string.IsNullOrWhiteSpace(Args))
                {
                    _cmd = string.Empty;
                    return _cmd;
                }

                string cmd = Path.Trim();
                string args = Args.Trim();

                if (!string.IsNullOrEmpty(args))
                    cmd = cmd + " " + args;

                _cmd = cmd;

                return _cmd;
            }
        }

        public Image bMapImg { get; set; }
    }
}
