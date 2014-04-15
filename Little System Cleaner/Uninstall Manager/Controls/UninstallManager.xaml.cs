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

using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel;
using Little_System_Cleaner.Uninstall_Manager.Helpers;

namespace Little_System_Cleaner.Uninstall_Manager.Controls
{
	public partial class UninstallManager
	{
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

		public UninstallManager()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
            PopulateListView();

            // Manually sort listview
            Sort((this.listViewProgs.View as GridView).Columns[0].Header as string, _lastDirection);
            _lastDirection = ((_lastDirection == ListSortDirection.Ascending) ? (ListSortDirection.Descending) : (ListSortDirection.Ascending));
		}

        private void PopulateListView()
        {
            List<ProgramInfo> listProgInfo = new List<ProgramInfo>();

            // Clear listview
            this.listViewProgs.Items.Clear();

            // Turn textbox into regex pattern
            Regex regex = new Regex("", RegexOptions.IgnoreCase);

            if (this.textBoxSearch.HasText)
            {
                StringBuilder result = new StringBuilder();
                foreach (string str in this.textBoxSearch.Text.Split(' '))
                {
                    result.Append(Regex.Escape(str));
                    result.Append(".*");
                }

                regex = new Regex(result.ToString(), RegexOptions.IgnoreCase);
            }

            // Get the program info list
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if (regKey != null)
                {
                    foreach (string strSubKeyName in regKey.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = regKey.OpenSubKey(strSubKeyName))
                        {
                            if (subKey != null)
                                listProgInfo.Add(new ProgramInfo(subKey));
                        }
                    }
                }
            }

            // (x64 registry keys)
            if (Utils.Is64BitOS)
            {
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (regKey != null)
                    {
                        foreach (string strSubKeyName in regKey.GetSubKeyNames())
                        {
                            using (RegistryKey subKey = regKey.OpenSubKey(strSubKeyName))
                            {
                                if (subKey != null)
                                    listProgInfo.Add(new ProgramInfo(subKey));
                            }
                        }
                    }
                }
            }


            // Populate list view
            foreach (ProgramInfo progInfo in listProgInfo)
            {
                if ((!string.IsNullOrEmpty(progInfo._displayName))
                    && (string.IsNullOrEmpty(progInfo._parentKeyName))
                    && (!progInfo._systemComponent))
                {

                    if (regex.IsMatch(progInfo.Program))
                        this.listViewProgs.Items.Add(progInfo);
                }
            }

            // Resize columns
            Utils.AutoResizeColumns(this.listViewProgs);
        }

        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {
            PopulateListView();
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }


                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(this.listViewProgs.Items);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (this.listViewProgs.SelectedItems.Count > 0)
            {
                ProgramInfo progInfo = this.listViewProgs.SelectedItems[0] as ProgramInfo;

                if (MessageBox.Show(Application.Current.MainWindow, "Are you sure you want to remove this program from the registry?", Utils.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    progInfo.RemoveFromRegistry();

                PopulateListView();
            }
        }

        private void buttonUninstall_Click(object sender, RoutedEventArgs e)
        {
            if (this.listViewProgs.SelectedItems.Count > 0)
            {
                ProgramInfo progInfo = this.listViewProgs.SelectedItems[0] as ProgramInfo;

                if (MessageBox.Show(Application.Current.MainWindow, "Are you sure you want to remove this program?", Utils.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    progInfo.Uninstall();

                PopulateListView();
            }
        }

	}
}