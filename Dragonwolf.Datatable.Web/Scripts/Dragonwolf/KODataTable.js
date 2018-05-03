/*
 * Dragonwolf internationalization
 */
var KODataTableLabels = function (culture)
{
    culture = culture ? culture.toLowerCase() : "";

    this.ResultatsParPage = ko.observable("");
    this.Page = ko.observable("");
    this.SurPages = ko.observable("");

    this.PageSuivante = ko.observable("");
    this.PagePrecedante = ko.observable("");
    this.PremierePage = ko.observable("");
    this.DernierePage = ko.observable("");

    this.Ascendant = ko.observable("");
    this.Descendant = ko.observable("");

    switch (culture)
    {
        case "es":
        case "es-es":
        case "es-ar":
        case "es-bo":
        case "es-cl":
        case "es-co":
        case "es-cr":
        case "es-sv":
        case "es-ec":
        case "es-gt":
        case "es-hn":
        case "es-mx":
        case "es-ni":
        case "es-pa":
        case "es-py":
        case "es-pe":
        case "es-pr":
        case "es-tt":
        case "es-uy":
        case "es-ve":
            this.ResultatsParPage = ko.observable("Resultados por página : ");
            this.Page = ko.observable("Página");
            this.SurPages = ko.observable("sobre");

            this.PageSuivante = ko.observable("Página Siguiente");
            this.PagePrecedante = ko.observable("Page Precedente");
            this.PremierePage = ko.observable("Primera Página");
            this.DernierePage = ko.observable("Última Página");

            this.Ascendant = ko.observable("Ascendiente");
            this.Descendant = ko.observable("Descendante");
            break;
        case "fr":
        case "fr-fr":
        case "fr-be":
        case "fr-ca":
        case "fr-lu":
        case "fr-ch":
            this.ResultatsParPage = ko.observable("Résultats par page : ");
            this.Page = ko.observable("Page");
            this.SurPages = ko.observable("sur");

            this.PageSuivante = ko.observable("Page Suivante");
            this.PagePrecedante = ko.observable("Page Précédante");
            this.PremierePage = ko.observable("Première Page");
            this.DernierePage = ko.observable("Dernière Page");

            this.Ascendant = ko.observable("Ascendant");
            this.Descendant = ko.observable("Descendant");
            break;
        default:
            this.ResultatsParPage = ko.observable("Results count on page");
            this.Page = ko.observable("Page");
            this.SurPages = ko.observable("over");

            this.PageSuivante = ko.observable("Next page");
            this.PagePrecedante = ko.observable("Previous Page");
            this.PremierePage = ko.observable("First page");
            this.DernierePage = ko.observable("Last Page");

            this.Ascendant = ko.observable("Ascending");
            this.Descendant = ko.observable("Descending");
            break;
    }
};

/*
 *  Dragonwolf's Knockout DataTable
 */
var KODataTable = function (tableDomId, dataArray, sortingColumns, datasUrl)
{
    var self = this;

    var userLang = navigator.language || navigator.userLanguage;

    this.Culture = ko.observable(userLang);

    this.Libelles = ko.computed(function()
    {
        return new KODataTableLabels(self.Culture());
    }).extend({ deferred: true });

    this.DatasUrl = ko.observable(datasUrl ? datasUrl : null);

    this.AdditionalSendingDatas = ko.observable(null);
    this.AdditionalCallbackFunction = null;

    this.Datas = ko.observableArray([]);
    this.SortingColumns = ko.observable({});

    if (sortingColumns)
    {
        $.each(sortingColumns, function (columnIndex, column) {
            self.SortingColumns()[column] = ko.observable({ order: 0, colName: column, value: null });
        });
    }

    if (dataArray && dataArray.length > 0)
    {
        $.each(dataArray, function (dataIndex, data)
        {
            self.Datas.push(ko.mapping.fromJS(data));
        });
    }

    this.ResultsCount = ko.observable(self.Datas().length);
    this.ResultatsParPage = ko.observable(10);
    this.DataSizesList = ko.observableArray([5, 10, 15, 20, 40, 50, 100, 150, 200, 300]);

    this.ResultatsParPage.subscribe(function (e) {
        self.GestionPaging();
    });

    this.Page = ko.observable(0);
    this.NombrePages = ko.computed(function () {
        var result = 0;

        var tempResult = self.ResultsCount() / self.ResultatsParPage();
        var tempResultArrondi = Math.ceil(tempResult);

        result = tempResultArrondi;

        return result;
    }).extend({ deferred: true });

    this.GestionPaging();

    this.AffichePrecedant = ko.computed(function () {
        var result = false;

        if (self.ResultsCount() > self.ResultatsParPage()) {
            result = self.Page() > 0;
        }

        return result;
    });

    this.AfficheSuivant = ko.computed(function () {
        var result = false;

        if (self.ResultsCount() > self.ResultatsParPage()) {
            result = (self.Page() + 1) < self.NombrePages();
        }

        return result;
    });

    this.ComptePage = ko.computed(function () {
        return self.Libelles().Page() + " " + (self.Page() + 1).toString() + " " + self.Libelles().SurPages() + " " + self.NombrePages().toString();
    });

    this.Table = $("#" + tableDomId);

    if (this.Table && this.Table.length > 0)
    {
        var $parentContainer = this.Table.parent();

        $parentContainer.find('[name="btnPagePrecedante"]').off("click", $.proxy(self.PagePrecedanteClicked, self));
        $parentContainer.find('[name="btnPagePrecedante"]').on("click", $.proxy(self.PagePrecedanteClicked, self));
        $parentContainer.find('[name="btnPageSuivante"]').off("click", $.proxy(self.PageSuivanteClicked, self));
        $parentContainer.find('[name="btnPageSuivante"]').on("click", $.proxy(self.PageSuivanteClicked, self));

        $parentContainer.find('[name="btnPremierePage"]').off("click", $.proxy(self.PremierePageClicked, self));
        $parentContainer.find('[name="btnPremierePage"]').on("click", $.proxy(self.PremierePageClicked, self));
        $parentContainer.find('[name="btnDernierePage"]').off("click", $.proxy(self.DernierePageClicked, self));
        $parentContainer.find('[name="btnDernierePage"]').on("click", $.proxy(self.DernierePageClicked, self));
    }
};

KODataTable.prototype.SortingColumnClicked = ko.command({
    execute: function (complete, button) {
        var self = this;

        var propertyName = $(button.currentTarget).data("property");

        if (propertyName)
        {
            var valueContainer = this.SortingColumns()[propertyName]();

            if (valueContainer)
            {
                var value = valueContainer ? valueContainer.value : null;

                if (valueContainer.value === null)
                {
                    valueContainer.order = 9999;
                    valueContainer.value = true;
                }
                else if (valueContainer.value === true)
                {
                    valueContainer.value = false;
                }
                else
                {
                    valueContainer.order = 0;
                    valueContainer.value = null;
                }

                this.SortingColumns()[propertyName](valueContainer);

                var sortedSortings = self.GetOrderedActiveSorting();

                var i = 0;
                $.each(sortedSortings, function (elementIndex, element)
                {
                    i = i + 1;
                    element.order = i;

                    self.SortingColumns()[element.colName](element);
                });
            }

            this.GestionPaging();
        }
    },
    canExecute: function (isExecuting, toto, tata) {
        return !isExecuting;
    }

});

KODataTable.prototype.PagePrecedanteClicked = function () {
    this.Page(this.Page() > 0 ? this.Page() - 1 : 0);
    this.GestionPaging();
};

KODataTable.prototype.PageSuivanteClicked = function () {
    this.Page((this.Page() + 1) < this.NombrePages() ? this.Page() + 1 : (this.NombrePages() - 1));
    this.GestionPaging();
};

KODataTable.prototype.PremierePageClicked = function ()
{
    this.Page(0);
    this.GestionPaging();
};

KODataTable.prototype.DernierePageClicked = function ()
{
    this.Page(this.NombrePages() - 1);
    this.GestionPaging();
};

KODataTable.prototype.GestionPaging = function () {
    var self = this;

    var sortingTable = [];

    var sortedSortings = self.GetOrderedActiveSorting();

    $.each(sortedSortings, function (elementIndex, element) {
        sortingTable.push({
            key: element.colName,
            ascendant: element.value
        });
    });

    if (self.DatasUrl() && self.DatasUrl() != "")
    {
        var datasTosend = {};

        var pagingSortingFilter = {
            paging: {
                page: self.Page(),
                resultatsParPage: self.ResultatsParPage(),
            },
            sorting: sortingTable
        };

        if (self.AdditionalSendingDatas())
        {
            datasTosend = ko.mapping.toJS(self.AdditionalSendingDatas());
        }

        datasTosend.PagingSortingFilter = pagingSortingFilter;

        $.ajax({
            url: self.DatasUrl(),
            method: 'POST',
            data: datasTosend,
            success: function (datasResult)
            {
                self.Datas.removeAll();

                if (datasResult && datasResult.Results)
                {
                    self.Page(datasResult.Page);
                    self.ResultatsParPage(datasResult.NombreResultatsParPage);
                    self.ResultsCount(datasResult.TotalResultats);

                    $.each(datasResult.Results, function (dataIndex, data)
                    {
                        var tempKoData = ko.mapping.fromJS(data);

                        if (tempKoData.Display === undefined)
                        {
                            tempKoData.Display = ko.observable(true);
                        }

                        tempKoData.Display(true);

                        self.Datas.push(tempKoData);
                    });

                    if (self.AdditionalCallbackFunction)
                    {
                        self.AdditionalCallbackFunction();
                    }
                }
            }
        });
    }
    else
    {
        self.ResultsCount(self.Datas().length);

        if (sortingTable && sortingTable.length > 0)
        {
            self.Datas(self.Datas().sort(function (a, b)
            {
                var result = 0;

                $.each(sortingTable, function (sortValueIndex, sortValue)
                {
                    var sortComplement = 0;

                    var coef = Math.pow(10, (sortingTable.length - sortValueIndex) - 1);

                    if (a[sortValue.key]() < b[sortValue.key]())
                    {
                        sortComplement = - (1 * coef);
                    }
                    else if (a[sortValue.key]() > b[sortValue.key]())
                    {
                        sortComplement = (1 * coef);
                    }
                    else
                    {
                        sortComplement = 0;
                    }

                    if (sortValue.ascendant !== true)
                    {
                        sortComplement = -sortComplement;
                    }

                    result = result + sortComplement;
                });

                return result;
            }));
        }

        if (self.Datas().length <= self.ResultatsParPage()) {
            $.each(self.Datas(), function (dataIndex, data) {
                if (data.Display === undefined) {
                    data.Display = ko.observable(true);
                }

                data.Display(true);
            });
        }
        else {
            var firstDisplayIndex = (self.Page() * self.ResultatsParPage());
            var lastDisplayIndex = firstDisplayIndex + self.ResultatsParPage() - 1;

            $.each(self.Datas(), function (dataIndex, data) {
                if (data.Display === undefined) {
                    data.Display = ko.observable(true);
                }

                data.Display(dataIndex >= firstDisplayIndex && dataIndex <= lastDisplayIndex);
            });
        }
    }
};

KODataTable.prototype.GetOrderedActiveSorting = function ()
{
    var self = this;

    var tempSortings = [];
    $.each(self.SortingColumns(), function (sortingElementIndex, sortingElement)
    {
        tempSortings.push(sortingElement());
    });

    var activeSortings = $.grep(tempSortings, function (e) { return e.order !== 0; });
    var sortedSortings = activeSortings.sort(function (a, b) { return a.order - b.order });

    return sortedSortings;
};

KODataTable.SetTable = function (tableVarName, tableDomId, dataArray, sortableColumns, datasUrl) {
    KODataTable.Tables = KODataTable.Tables || {};

    KODataTable.Tables[tableVarName] = new KODataTable(tableDomId, dataArray, sortableColumns, datasUrl);
};

KODataTable.GetTable = function (tableVarName) {
    var result = null;

    KODataTable.Tables = KODataTable.Tables || {};

    result = KODataTable.Tables[tableVarName];

    delete KODataTable.Tables[tableVarName];

    return result;
};