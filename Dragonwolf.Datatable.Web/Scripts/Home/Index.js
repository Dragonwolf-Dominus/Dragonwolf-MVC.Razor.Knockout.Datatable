var HomeIndexClass = function ()
{
    var self = this;

    this.ListeDonnees = ko.observableArray([]);

    this.ListeDonnees.push({ Id: 1, Libelle: 'Toto', Display: true });
    this.ListeDonnees.push({ Id: 2, Libelle: 'Tata', Display: true });
    this.ListeDonnees.push({ Id: 3, Libelle: 'Titi', Display: true });
    this.ListeDonnees.push({ Id: 4, Libelle: 'TomTom', Display: false });
    this.ListeDonnees.push({ Id: 5, Libelle: 'TombeTombe', Display: true });

    this.MyTable1 = KODataTable.GetTable("MyTable1");
    this.DataListe = [];
    $.each(this.MyTable1.Datas(), function (dataIndex, data)
    {
        var tempData = ko.mapping.toJS(data);
        delete tempData.Display;

        self.DataListe.push(tempData);
    });

    this.MyTable2 = KODataTable.GetTable("MyTable2");
    $.each(this.DataListe, function (dataIndex, data)
    {
        var tempData = ko.mapping.fromJS(data);

        self.MyTable2.Datas.push(tempData);
    });
    self.MyTable2.GestionPaging();

    this.MyTable3 = KODataTable.GetTable("MyTable3");

    ko.applyBindings(this);
};

var homeIndex = null;
$(document).ready(function ()
{
    homeIndex = new HomeIndexClass();
});