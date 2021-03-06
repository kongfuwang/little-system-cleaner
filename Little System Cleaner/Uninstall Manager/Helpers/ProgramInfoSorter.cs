﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Little_System_Cleaner.Uninstall_Manager.Helpers
{
    public class ProgramInfoSorter : IComparer<ProgramInfo>, IComparer
    {
        private readonly GridViewColumn _column;
        private readonly ListSortDirection _direction;

        public ProgramInfoSorter(GridViewColumn column, ListSortDirection direction)
        {
            _column = column;
            _direction = direction;
        }

        public int Compare(object x, object y)
        {
            return Compare(x as ProgramInfo, y as ProgramInfo);
        }

        public int Compare(ProgramInfo x, ProgramInfo y)
        {
            try
            {
                var priority = 0;

                switch ((string)_column.Header)
                {
                    case "Program":
                        priority = string.CompareOrdinal(x.Program, y.Program);
                        break;

                    case "Publisher":
                        priority = string.CompareOrdinal(x.Publisher, y.Publisher);
                        break;

                    case "Size":
                        priority = x.SizeBytes.CompareTo(y.SizeBytes);
                        break;
                }

                return _direction.Equals(ListSortDirection.Ascending) ? priority : -priority;
            }
            catch
            {
                return 0;
            }
        }
    }
}