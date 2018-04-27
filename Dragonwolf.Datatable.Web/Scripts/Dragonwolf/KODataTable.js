var KODataTable = function (tableDomId, dataArray, sortingColumns, datasUrl)
{
    var self = this;

    this.DatasUrl = ko.observable(datasUrl ? datasUrl : null);

    this.AdditionalSendingDatas = ko.observable(null);
    this.AdditionalCallbackFunction = null;

    this.Datas = ko.observableArray([]);
    this.SortingColumns = ko.observable({});

    if (sortingColumns)
    {
        $.each(sortingColumns, function (columnIndex, column) {
            self.SortingColumns()[column] = ko.observable(null);
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
    this.Page = ko.observable(0);
    this.NombrePages = ko.computed(function () {
        var result = 0;

        var tempResult = self.ResultsCount() / self.ResultatsParPage();
        var tempResultArrondi = Math.ceil(tempResult);

        result = tempResultArrondi;

        return result;
    });

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
        return "Page " + (self.Page() + 1).toString() + " sur " + self.NombrePages().toString();
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
            $.each(this.SortingColumns(), function (fieldName, fieldValue) {
                if (fieldName !== propertyName)
                {
                    self.SortingColumns()[fieldName](null);
                }
            });

            var value = this.SortingColumns()[propertyName]();

            if (value === null)
            {
                value = true;
            }
            else if (value === true) {
                value = false;
            }
            else
            {
                value = null;
            }

            this.SortingColumns()[propertyName](value);

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

    var sortPropertyName = null;
    var sortPropertyValue = null;

    $.each(self.SortingColumns(), function (fieldName, fieldValue) {
        if (!sortPropertyName && fieldValue() !== null)
        {
            sortPropertyName = fieldName;
            sortPropertyValue = fieldValue();
        }
    });

    if (self.DatasUrl() && self.DatasUrl() != "")
    {
        var datasTosend = {};

        var pagingSortingFilter = {
            paging: {
                page: self.Page(),
                resultatsParPage: self.ResultatsParPage(),
            },
            sorting: [{
                key: sortPropertyName,
                ascendant: sortPropertyValue
            }]
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

                    if (AdditionalCallbackFunction)
                    {
                        AdditionalCallbackFunction();
                    }
                }
            }
        });
    }
    else
    {
        self.ResultsCount(self.Datas().length);

        if (sortPropertyName) {
            self.Datas(self.Datas().sort(function (a, b) {
                var result = 0;

                if (a[sortPropertyName]() < b[sortPropertyName]()) {
                    result = -1;
                }
                else if (a[sortPropertyName]() > b[sortPropertyName]()) {
                    result = 1;
                }
                else {
                    result = 0;
                }

                return result;
            }));

            if (sortPropertyValue !== true) {
                self.Datas(self.Datas().reverse());
            }
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