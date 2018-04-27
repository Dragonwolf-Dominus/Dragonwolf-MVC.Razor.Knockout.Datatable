using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragonwolf.Razor.Helpers.Common;

namespace Dragonwolf.Razor.Helpers.Models
{
    public class ColumnsModelsCollection : Collection<ColumnModel>
    {
        #region Public Methods
        public string GetHeader()
        {
            var result = string.Empty;

            var cellsGroups = string.Empty;
            if (this.Any(cell => !string.IsNullOrEmpty(cell.ColumnGroup)))
            {
                cellsGroups = "<tr>" + Environment.NewLine;

                var currentGroup = string.Empty;
                var currentGroupIndex = 0;
                var currentGroupCell = this.First().GetGroupHeaderCell();
                foreach (var cell in this)
                {
                    if ((string.IsNullOrEmpty(currentGroup) && string.IsNullOrEmpty(cell.ColumnGroup)) || currentGroup.Equals(cell.ColumnGroup, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (cell.IsVisible)
                        {
                            currentGroupIndex++;
                        }
                    }
                    else
                    {
                        if (currentGroupIndex > 0)
                        {
                            var toAddCell = currentGroupCell.SpecialFormat(currentGroupIndex);
                            cellsGroups += toAddCell + Environment.NewLine;
                        }

                        currentGroupIndex = cell.IsVisible ? 1 : 0;
                        currentGroup = cell.ColumnGroup;
                        currentGroupCell = cell.GetGroupHeaderCell();
                    }
                }

                if (currentGroupIndex > 0)
                {
                    var toAddCell = currentGroupCell.SpecialFormat(currentGroupIndex);
                    cellsGroups += toAddCell + Environment.NewLine;
                }

                cellsGroups += "</tr>";
            }

            var cellsHeaders = "<tr>" + Environment.NewLine + this.Aggregate(string.Empty, (s, cell) => s + (string.IsNullOrEmpty(s) ? string.Empty : Environment.NewLine) + cell.GetHeaderCell()) + Environment.NewLine + "</tr>";

            result = ("<thead>{0}{1}</thead>").SpecialFormat(cellsGroups,  cellsHeaders);
            
            return result;
        }

        public string GetRowTemplate()
        {
            var result = string.Empty;

            var cells = this.Aggregate(string.Empty, (s, cell) => s + (string.IsNullOrEmpty(s) ? string.Empty : Environment.NewLine) + cell.GetRowCellTemplate());

            var line =  "<!-- ko if: Display -->" + Environment.NewLine +
                        "   <tr>{0}</tr>" + Environment.NewLine +
                        "<!-- /ko -->";

            result = line.SpecialFormat(cells);

            return result;
        }
        #endregion
    }
}
