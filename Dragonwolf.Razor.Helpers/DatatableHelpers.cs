using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using Dragonwolf.Razor.Helpers.Models;
using Dragonwolf.Razor.Helpers.Common;

namespace Dragonwolf.Razor.Helpers
{
    public static class DatatableHelpers
    {
        #region Public Methods
        /// <summary>
        /// Creates a table from model's list definition.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper">Instance du programme d'assistance HTML étendue par cette méthode.</param>
        /// <param name="expression">Expression identifiant l'objet qui contient les propriétés à restituer.</param>
        /// <param name="tableJSName"></param>
        /// <param name="tableId">[Optional] : Html Table Id.</param>
        /// <param name="columns">[Optional] : Columns definition.</param>
        /// <param name="htmlAttributes">[Optional] : Objet qui contient les attributs HTML à définir pour l'élément.</param>
        /// <returns>Html Tags to create the table.</returns>
        public static MvcHtmlString KODataTableFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string tableJSName, string tableId = null, ColumnsModelsCollection columns = null, object htmlAttributes = null)
        {
            var result = MvcHtmlString.Create(String.Format(""));

            var attributes = GetAttributes(htmlAttributes);

            var listeResults = DatatableHelpers.GetListFromExpression(htmlHelper, expression, tableJSName, ref tableId, ref columns);

            result = CreateHtml(tableJSName, tableId, columns, attributes, listeResults);

            return result;
        }

        /// <summary>
        /// Creates a table.
        /// </summary>
        /// <param name="htmlHelper">Instance du programme d'assistance HTML étendue par cette méthode.</param>
        /// <param name="columns">Columns definition.</param>
        /// <param name="tableJSName">In js Table name.</param>
        /// <param name="tableId">Html Table Id</param>
        /// <param name="refreshDataUrl">[Optional] : Url used to refresh datas from server.</param>
        /// <param name="htmlAttributes">[Optional] : Objet qui contient les attributs HTML à définir pour l'élément.</param>
        /// <returns>Html Tags to create the table.</returns>
        public static MvcHtmlString KODataTable(this HtmlHelper htmlHelper, ColumnsModelsCollection columns, string tableJSName, string tableId, string refreshDataUrl = null, object htmlAttributes = null)
        {
            var result = MvcHtmlString.Create(String.Format(""));

            var attributes = GetAttributes(htmlAttributes);

            if (columns != null && string.IsNullOrEmpty(columns.TableJSName) && !string.IsNullOrEmpty(tableJSName))
            {
                columns.SetTableJSName(tableJSName);
            }

            result = CreateHtml(tableJSName, tableId, columns, attributes, null, refreshDataUrl);

            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Extracts attributes from html Attributes object.
        /// </summary>
        /// <param name="htmlAttributes"></param>
        /// <returns>A dictionnary of attributes.</returns>
        private static Dictionary<string, object> GetAttributes(object htmlAttributes)
        {
            var attributes = new Dictionary<string, object>();

            if (htmlAttributes != null)
            {
                var properties = htmlAttributes.GetType().GetProperties().ToList();

                properties.ForEach(p =>
                {
                    var attributeName = p.Name.Replace("_", "-");
                    var value = p.GetValue(htmlAttributes);
                    attributes.Add(attributeName, value);
                });
            }

            return attributes;
        }

        /// <summary>
        /// Transform an expression to its representative list.
        /// </summary>
        /// <typeparam name="TModel">Model's Type</typeparam>
        /// <typeparam name="TProperty">Property's Type</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="tableJSName"></param>
        /// <param name="tableId"></param>
        /// <param name="columns"></param>
        /// <returns>The list of objects</returns>
        private static List<object> GetListFromExpression<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string tableJSName, ref string tableId, ref ColumnsModelsCollection columns)
        {
            List<object> result = null;

            var expressionResultItem = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;

            if (expressionResultItem is IEnumerable)
            {
                result = new List<object>();

                var elementType = expressionResultItem.GetType().GenericTypeArguments.First();
                tableId = string.IsNullOrEmpty(tableId) ? ("Table_" + elementType.Name + "s") : tableId;
                var tempExpressionResult = (expressionResultItem as IEnumerable).GetEnumerator();

                while (tempExpressionResult.MoveNext())
                {
                    var item = tempExpressionResult.Current;

                    if (item.GetType() == elementType)
                    {
                        result.Add(item);
                    }
                }

                if (columns == null)
                {
                    var properties = elementType.GetProperties().ToList();

                    var tempColumns = new ColumnsModelsCollection(tableJSName);

                    properties.ForEach(p =>
                    {
                        tempColumns.Add(new ColumnModel(p.Name, null, p.PropertyType, true));
                    });

                    columns = tempColumns;
                }
            }

            if (columns == null)
            {
                columns = new ColumnsModelsCollection(tableJSName);
            }
            else
            {
                columns.SetTableJSName(tableJSName);
            }

            return result;
        }

        /// <summary>
        /// Creates Html table.
        /// </summary>
        /// <param name="tableJSName">In js Table name.</param>
        /// <param name="tableId">Html Table Id</param>
        /// <param name="columns">Columns definition.</param>
        /// <param name="attributes">Attributes to add on Html main container.</param>
        /// <param name="listeResults">[Optional] : List of objects to display.</param>
        /// <returns>Html Tags to create the table.</returns>
        private static MvcHtmlString CreateHtml(string tableJSName, string tableId, ColumnsModelsCollection columns = null, Dictionary<string, object> attributes = null, List<object> listeResults = null, string refreshDataUrl = null)
        {
            MvcHtmlString result;
            var classNames = attributes != null && attributes.ContainsKey("class") ? attributes["class"] : string.Empty;
            var domId = attributes != null && attributes.ContainsKey("id") ? "id=\"" + attributes["id"] + "\"" : string.Empty;

            var listSpecials = new List<string>()
            {
                "class",
                "id"
            };

            var othersAttributes = attributes != null ? attributes.Where(c => !listSpecials.Contains(c.Key)).Aggregate(string.Empty, (s, c) => s + (string.IsNullOrEmpty(s) ? string.Empty : " ") + c.Key + "=\"" + c.Value + "\"") : string.Empty;

            var sortableColumns = columns != null ? columns.Where(c => c.IsSortable).Select(col => col.PropertyName).Distinct().ToList() : new List<string>();

            var tableHtml = ("<table id=\"{0}\" class=\"table table-striped table-bordered table-hover\">" + Environment.NewLine +
                                columns.GetHeader() + Environment.NewLine +
                                " <tbody data-bind=\"foreach: {1}.Datas\">" + Environment.NewLine +
                                columns.GetRowTemplate() + Environment.NewLine +
                                " </tbody>" + Environment.NewLine +
                                "</table>").SpecialFormat(tableId, tableJSName);

            var pagingHtml = ("<div class=\"row text-center\" name=\"DragonwolfPagingZone\">" + Environment.NewLine +
                
                                "    <div class=\"col-md-4 pull-left\">" + Environment.NewLine +
                                "       <div class=\"col-md-8\">" + Environment.NewLine +
                                "           <label class=\"form-control\" data-bind=\"text: {2}.Libelles().ResultatsParPage\"></label>" + Environment.NewLine +
                                "       </div>" + Environment.NewLine +
                                "       <div class=\"col-md-4\">" + Environment.NewLine +
                                "           <select class=\"form-control\" data-bind=\"foreach: {2}.DataSizesList, value: {2}.ResultatsParPage\">" + Environment.NewLine +
                                "               <option data-bind=\"value: $data, text: $data\"></option>" + Environment.NewLine +
                                "           </select>" + Environment.NewLine +
                                "       </div>" + Environment.NewLine +
                                "    </div>" + Environment.NewLine +

                                "    <div class=\"col-md-4 pull-right\" data-bind=\"visible: {2}.ResultatsParPage() !== 0 && {2}.ResultsCount() > {2}.ResultatsParPage()\">" + Environment.NewLine +
                                "        <div class=\"input-group\">" + Environment.NewLine +
                                "           <span class=\"input-group-btn\">" + Environment.NewLine +
                                "               <button name = \"btnPremierePage\" type=\"button\" class=\"btn btn-primary col-md-12\" data-bind=\"visible: {2}.AffichePrecedant, attr: { title: {2}.Libelles().PremierePage }\"><i class=\"glyphicon glyphicon-fast-backward\"></i></button>" + Environment.NewLine +
                                "           </span>" + Environment.NewLine +
                                "           <span class=\"input-group-btn\">" + Environment.NewLine +
                                "               <button name = \"btnPagePrecedante\" type=\"button\" class=\"btn btn-primary col-md-12\" data-bind=\"visible: {2}.AffichePrecedant, attr: { title: {2}.Libelles().PagePrecedante }\"><i class=\"glyphicon glyphicon-backward\"></i></button>" + Environment.NewLine +
                                "           </span>" + Environment.NewLine +
                                "           <label class=\"form-control\" data-bind=\"text: {2}.ComptePage\"></label>" + Environment.NewLine +
                                "           <span class=\"input-group-btn\">" + Environment.NewLine +
                                "               <button name=\"btnPageSuivante\" type=\"button\" class=\"btn btn-primary col-md-12\" data-bind=\"visible: {2}.AfficheSuivant, attr: { title: {2}.Libelles().PageSuivante }\"><i class=\"glyphicon glyphicon-forward\"></i></button>" + Environment.NewLine +
                                "           </span>" + Environment.NewLine +
                                "           <span class=\"input-group-btn\">" + Environment.NewLine +
                                "               <button name=\"btnDernierePage\" type=\"button\" class=\"btn btn-primary col-md-12\" data-bind=\"visible: {2}.AfficheSuivant, attr: { title: {2}.Libelles().DernierePage }\"><i class=\"glyphicon glyphicon-fast-forward\"></i></button>" + Environment.NewLine +
                                "           </span>" + Environment.NewLine +
                                "        </div>" + Environment.NewLine +
                                "    </div>" + Environment.NewLine +
                                "</div>").SpecialFormat(classNames, tableId, tableJSName);

            var javascriptHtml = string.Empty;

            if (listeResults != null)
            {
                javascriptHtml = ("<script type=\"text/javascript\">KODataTable.SetTable('{0}', '{1}', {2}, {3});</script>").SpecialFormat(tableJSName, tableId, Newtonsoft.Json.JsonConvert.SerializeObject(listeResults), Newtonsoft.Json.JsonConvert.SerializeObject(sortableColumns));
            }
            else if (!string.IsNullOrEmpty(refreshDataUrl))
            {
                javascriptHtml = ("<script type=\"text/javascript\">KODataTable.SetTable('{0}', '{1}', {2}, {3}, '{4}');</script>").SpecialFormat(tableJSName, tableId, "null", Newtonsoft.Json.JsonConvert.SerializeObject(sortableColumns), refreshDataUrl);
            }
            else
            {
                javascriptHtml = ("<script type=\"text/javascript\">KODataTable.SetTable('{0}', '{1}', {2}, {3});</script>").SpecialFormat(tableJSName, tableId, "null", Newtonsoft.Json.JsonConvert.SerializeObject(sortableColumns));
            }

            var wrappingDom = ("<div class=\"{3}\" {4}>{0}{1}{2}</div>").SpecialFormat(tableHtml, pagingHtml, javascriptHtml, classNames, othersAttributes);


            result = MvcHtmlString.Create(wrappingDom);
            return result;
        }
        #endregion
    }
}
