using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dragonwolf.Razor.Helpers.Common;

namespace Dragonwolf.Razor.Helpers.Models
{
    public class ColumnModel
    {
        #region Properties
        #region Settable
        public string TableJSName { get; private set; }

        public string ColumnName
        {
            get;
            set;
        }

        public string PropertyName
        {
            get;
            set;
        }

        public Type ColumnType
        {
            get;
            set;
        }

        public bool IsSortable
        {
            get;
            set;
        }

        public bool IsEditable
        {
            get;
            set;
        }

        public string ColumnTemplate
        {
            get;
            set;
        }

        public string HeaderTemplate
        {
            get;
            set;
        }

        public string HeaderClass
        {
            get;
            set;
        }

        public string CellClass
        {
            get;
            set;
        }

        public string ColumnGroup
        {
            get;
            set;
        }

        public string ColumnGroupClass
        {
            get;
            set;
        }

        public string ColumnGroupName
        {
            get;
            set;
        }

        public string ColumnGroupTemplate
        {
            get;
            set;
        }

        public bool IsVisible
        {
            get;
            set;
        }

        public string DropDownDataSourceAccess
        {
            get;
            set;
        }

        public string DropDownDataTextBinding
        {
            get;
            set;
        }

        public string DropDownDataValueBinding
        {
            get;
            set;
        }

        public string DropDownDataVisibleOptionBinding
        {
            get;
            set;
        }
        #endregion

        #region Read Only
        private string VisibleClass
        {
            get
            {
                return !IsVisible ? "hidden" : string.Empty; 
            }
        }

        private string SortableClass
        {
            get
            {
                return !IsSortable ? "Sort" : string.Empty;
            }
        }

        private string Name
        {
            get
            {
                return !string.IsNullOrEmpty(ColumnName) ? ColumnName : PropertyName;
            }
        }

        private string GroupName
        {
            get
            {
                return !string.IsNullOrEmpty(ColumnGroupName) ? ColumnGroupName : (ColumnGroup ?? string.Empty);
            }
        }

        private Type Type
        {
            get
            {
                return ColumnType != null ? ColumnType : typeof(string);
            }
        }

        public string SortingColumnAccess
        {
            get
            {
                return IsSortable ? ("data-sortingproperty=\"{0}\"").SpecialFormat(PropertyName) : string.Empty;
            }
        }
        #endregion
        #endregion

        #region Constructors
        public ColumnModel(string tableJSName, string propertyName, string columnName = null, Type columnType = null, bool isSortable = false, bool isEditable = false, string columnTemplate = null, string headerTemplate = null, string headerClass = null, string cellClass = null, string columnGroup = null, string columnGroupClass = null, string columnGroupName = null, string columnGroupTemplate = null, bool isVisible = true, string dropDownDataSourceAccess = null, string dropDownDataValueBinding = null, string dropDownDataTextBinding = null, string dropDownDataVisibleOptionBinding = null) : this(propertyName, columnName, columnType, isSortable, isEditable, columnTemplate, headerTemplate, headerClass, cellClass, columnGroup, columnGroupClass, columnGroupName, columnGroupTemplate, isVisible, dropDownDataSourceAccess, dropDownDataValueBinding, dropDownDataTextBinding, dropDownDataVisibleOptionBinding)
        {
            this.SetTableJSName(tableJSName);
        }

        public ColumnModel(string propertyName, string columnName = null, Type columnType = null, bool isSortable = false, bool isEditable = false, string columnTemplate = null, string headerTemplate = null, string headerClass = null, string cellClass = null, string columnGroup = null, string columnGroupClass = null, string columnGroupName = null, string columnGroupTemplate = null, bool isVisible = true, string dropDownDataSourceAccess = null, string dropDownDataValueBinding = null, string dropDownDataTextBinding = null, string dropDownDataVisibleOptionBinding = null)
        {
            PropertyName = propertyName;
            ColumnName = string.IsNullOrEmpty(columnName) ? PropertyName : columnName;
            ColumnType = columnType != null ? columnType : typeof(string);
            IsSortable = isSortable;
            IsEditable = isEditable;
            ColumnTemplate = columnTemplate;
            HeaderTemplate = headerTemplate;
            HeaderClass = headerClass ?? string.Empty;
            CellClass = cellClass ?? string.Empty;
            ColumnGroup = columnGroup;
            ColumnGroupClass = columnGroupClass ?? string.Empty;
            ColumnGroupName = columnGroupName;
            ColumnGroupTemplate = columnGroupTemplate;
            IsVisible = isVisible;
            DropDownDataSourceAccess = dropDownDataSourceAccess;
            DropDownDataValueBinding = dropDownDataValueBinding;
            DropDownDataTextBinding = dropDownDataTextBinding;
            DropDownDataVisibleOptionBinding = dropDownDataVisibleOptionBinding;
        }
        #endregion

        #region Public Methods
        public string GetGroupHeaderCell()
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(ColumnGroupTemplate))
            {
                result = ("<th class=\"ColumnGroup text-center {0}\" colspan=\"{2}\">{1}</th>").SpecialFormat(ColumnGroupClass, GroupName, "{0}");
            }
            else
            {
                result = ColumnGroupTemplate;
            }

            return result;
        }

        public string GetHeaderCell()
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(HeaderTemplate))
            {
                var sortingButton = ("<span class=\"input-group-btn\">" +
                                     "  <button type=\"button\" data-property=\"{0}\" class=\"btn btn-primary\" data-bind=\"click: $.proxy($root." + TableJSName + ".SortingColumnClicked, $root." + TableJSName + "), activity: $root." + TableJSName + ".SortingColumnClicked.isExecuting, attr: { title: $root." + TableJSName + ".SortingColumns().{0}().value === null ? '' : ($root." + TableJSName + ".SortingColumns().{0}().value === true ? $root." + TableJSName + ".Libelles().Ascendant() : ($root." + TableJSName + ".SortingColumns().{0}().value === false ? $root." + TableJSName + ".Libelles().Descendant() : '')) }\">" +
                                     "      <!-- ko if: $root." + TableJSName + ".SortingColumns().{0}().order !== 0 -->" +
                                     "          <i><span><label data-bind=\"text: $root." + TableJSName + ".SortingColumns().{0}().order\"></label>&nbsp;</span></i>" +
                                     "      <!-- /ko -->" +
                                     "      <i class=\"glyphicon\" data-bind=\"css: { 'glyphicon-triangle-right': $root." + TableJSName + ".SortingColumns().{0}().value === null, 'glyphicon-triangle-bottom': $root." + TableJSName + ".SortingColumns().{0}().value === true, 'glyphicon-triangle-top': $root." + TableJSName + ".SortingColumns().{0}().value === false }\"></i>" +
                                     "  </button>" +
                                     "</span>").SpecialFormat(PropertyName);

                var cellContent =  ("<div class=\"{2}\">" + 
                             "   <label class=\"form-control\">{0}</label>" +
                             "   {1}" +
                             "</div>").SpecialFormat(Name, IsSortable ? sortingButton : string.Empty, IsSortable ? "input-group" : "row");

                result = ("<th class=\"text-center {0} {1} {2}\">{3}</th>").SpecialFormat(SortableClass, VisibleClass, HeaderClass, cellContent);
            }
            else
            {
                result = HeaderTemplate;
            }

            return result;
        }

        public string GetRowCellTemplate()
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(ColumnTemplate))
            {
                if (IsEditable)
                {
                    var input = string.Empty;

                    if (!string.IsNullOrEmpty(DropDownDataSourceAccess) && 
                        (Type == typeof(int) ||
                        Type == typeof(int?) ||
                        Type == typeof(string)))
                    {
                        input = (   "<select class=\"form-control\" data-bind=\"foreach: {1}, value: {0}\">" + Environment.NewLine +
                                    " <!-- ko if: {4} --><option data-bind=\"value: {2}, text: {3}\"></option><!-- /ko -->" + Environment.NewLine +
                                    "</select>").SpecialFormat(PropertyName, DropDownDataSourceAccess, DropDownDataValueBinding, DropDownDataTextBinding, DropDownDataVisibleOptionBinding);
                    }
                    else if (Type == typeof(int) ||
                        Type == typeof(int?))
                    {
                        input = ("<input class=\"form-control\" type=\"number\" step=\"1\" data-bind=\"value: {0}\" />").SpecialFormat(PropertyName);
                    }
                    else if (Type == typeof(float) || Type == typeof(decimal) || Type == typeof(double) ||
                        Type == typeof(float?) || Type == typeof(decimal?) || Type == typeof(double?))
                    {
                        input = ("<input class=\"form-control\" type=\"number\" step=\"0.01\" data-bind=\"value: {0}\" />").SpecialFormat(PropertyName);
                    }
                    else if (Type == typeof(DateTime) ||
                            Type == typeof(DateTime?))
                    {
                        input = (   "<div class=\"input-group\">" + Environment.NewLine +
                                    "  <input type=\"date\" class=\"form-control\" data-bind=\"value: {0}\" />" + Environment.NewLine +
                                    "  <div class=\"input-group-addon\"><span class=\"glyphicon glyphicon-calendar\"></span></div>" + Environment.NewLine +
                                    "</div>").SpecialFormat(PropertyName);
                    }
                    else if (Type == typeof(bool) ||
                            Type == typeof(bool?))
                    {
                        input = (   "<div class=\"checkbox\">" + Environment.NewLine +
                                    "   <label>" + Environment.NewLine +
                                    "       <input type=\"checkbox\" data-bind=\"checked: {0}\" />" + Environment.NewLine +
                                    "   </label>" + Environment.NewLine +
                                    "</div>").SpecialFormat(PropertyName);
                    }
                    else
                    {
                        input = ("<input class=\"form-control\" type=\"text\" data-bind=\"value: {0}\" />").SpecialFormat(PropertyName);
                    }

                    result = ("<td class=\"{0} {1} {3}\">{4}</td>").SpecialFormat(SortableClass, VisibleClass, Name, HeaderClass, input);
                }
                else
                {
                    var alignmentClass = string.Empty;

                    var tempResult = "<td class=\"{5} {0} {1} {3}\"><label class=\"control-label\" data-bind=\"text: {4}\"></label></td>";

                    if (Type == typeof(int) ||
                        Type == typeof(int?) ||
                        Type == typeof(float) || Type == typeof(decimal) || Type == typeof(double) ||
                        Type == typeof(float?) || Type == typeof(decimal?) || Type == typeof(double?))
                    {
                        alignmentClass = "text-right";
                    }
                    else if (Type == typeof(DateTime) ||
                            Type == typeof(DateTime?))
                    {
                        alignmentClass = "text-center";
                    }
                    else if (Type == typeof(bool) ||
                            Type == typeof(bool?))
                    {
                        tempResult = "<td class=\"{5} {0} {1} {3}\">" + 
                                    "   <div class=\"checkbox\">" + Environment.NewLine +
                                    "       <label>" + Environment.NewLine +
                                    "           <input readonly=\"readonly\" disabled=\"disabled\" type=\"checkbox\" data-bind=\"checked: {4}\" />" + Environment.NewLine +
                                    "       </label>" + Environment.NewLine +
                                    "   </div>" +
                                    "</td>";

                        alignmentClass = "text-center";
                    }
                    else
                    {
                        alignmentClass = "text-left";
                    }

                    result = (tempResult).SpecialFormat(SortableClass, VisibleClass, Name, HeaderClass, PropertyName, alignmentClass);
                }
            }
            else
            {
                result = ColumnTemplate;
            }

            return result;
        }

        public void SetTableJSName(string tableJSName)
        {
            if (!string.IsNullOrEmpty(tableJSName))
            {
                TableJSName = tableJSName;
            }
        }
        #endregion
    }
}
