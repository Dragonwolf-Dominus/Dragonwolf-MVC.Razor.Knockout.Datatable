using System.Web;
using System.Web.Optimization;

namespace Dragonwolf.Datatable.Web
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/Knockout").Include(
                      "~/Scripts/knockout-{version}.js",
                      "~/Scripts/knockout.activity.js",
                      "~/Scripts/knockout.command.js",
                      "~/Scripts/knockout.dirtyFlag.js",
                      "~/Scripts/knockout.mapping-latest.js"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération (bluid) sur http://modernizr.com pour choisir uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/Dragonwolf").Include(
                      "~/Scripts/Dragonwolf/KODataTable.js"));
            #endregion

            #region Styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/Dragonwolf").Include(
                      "~/Content/Dragonwolf/Dragonwolf.css"));

            bundles.Add(new StyleBundle("~/Content/Sample1").Include(
                      "~/Content/SampleCssThemes/Sample1.css"));

            bundles.Add(new StyleBundle("~/Content/Sample2").Include(
                      "~/Content/SampleCssThemes/Sample2.css"));
            #endregion

        }
    }
}
