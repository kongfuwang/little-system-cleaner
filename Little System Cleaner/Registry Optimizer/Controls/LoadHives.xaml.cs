﻿using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using Little_System_Cleaner.Registry_Optimizer.Helpers;
using Microsoft.Win32;

namespace Little_System_Cleaner.Registry_Optimizer.Controls
{
    /// <summary>
    /// Interaction logic for LoadHives.xaml
    /// </summary>
    public partial class LoadHives
    {
        readonly Wizard _scanBase;

        public LoadHives(Wizard sb)
        {
            InitializeComponent();

            _scanBase = sb;

            Thread t = new Thread(InitHives);
            t.Start();
        }

        private void InitHives()
        {
            RegistryKey rkHives = null;
            int i = 0;
            Wizard.RegistryHives = new ObservableCollection<Hive>();

            using (rkHives = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\hivelist"))
            {
                if (rkHives == null)
                    throw new ApplicationException("Unable to open hive list... this can be a problem!");

                foreach (string strValueName in rkHives.GetValueNames())
                {
                    Dispatcher.Invoke(new Action(() => label1.Text = $"Loading {++i}/{rkHives.ValueCount} Hives"));

                    // Don't touch these hives because they are critical for Windows
                    if (strValueName.Contains("BCD") || strValueName.Contains("HARDWARE"))
                        continue;

                    string strHivePath = rkHives.GetValue(strValueName) as string;

                    if (string.IsNullOrEmpty(strHivePath))
                        continue;

                    if (strHivePath[strHivePath.Length - 1] == 0)
                        strHivePath = strHivePath.Substring(0, strHivePath.Length - 1);

                    if (string.IsNullOrEmpty(strValueName) || string.IsNullOrEmpty(strHivePath))
                        continue;

                    Hive h = new Hive(strValueName, strHivePath);

                    if (h.IsValid)
                        Wizard.RegistryHives.Add(h);
                }
            }

            _scanBase.HivesLoaded = true;

            _scanBase.MoveNext();
        }
    }
}
