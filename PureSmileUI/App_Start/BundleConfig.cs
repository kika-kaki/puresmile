using System.Web.Optimization;

namespace PureSmileUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery.js")
                .Include("~/Scripts/bootstrap-datetimepicker.min.js")
                .Include("~/Scripts/spin.js")
                .Include("~/Scripts/progress2.js")
                .Include("~/Scripts/ErrorLogger.js")
                .Include("~/Scripts/moment.min.js")
                .IncludeDirectory("~/Scripts", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqGrid")
                .Include("~/Scripts/jquery.jqGrid.min.js",
                         "~/Scripts/i18n/grid.locale-en.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymin")
              .Include("~/Scripts/jquery.js")
              .Include("~/Scripts/spin.js")
              .Include("~/Scripts/progress2.js")
              .Include("~/Scripts/ErrorLogger.js")
           );

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/site.css",
                         "~/Content/style.css",
                         "~/Content/themes/base/jquery-ui.css",
                         "~/Content/jquery.jqGrid/ui.jqgrid-bootstrap.css")
               .Include("~/Content/themes/base/bootstrap-datetimepicker.min.css")
               .Include("~/Content/themes/base/font-awesome.min.css")
               .IncludeDirectory("~/Content/themes/base", "*.css"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}