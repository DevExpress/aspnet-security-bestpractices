using DevExpress.Utils;
using DevExpress.XtraPrinting;
using System;

namespace SecurityBestPractices.Export {
    public partial class ExportToCsv : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
        }

        // For the Toolbar Button
        protected void ASPxGridView1_BeforeExport(object sender, DevExpress.Web.ASPxGridBeforeExportEventArgs e) {
            (e.ExportOptions as CsvExportOptionsEx).EncodeExecutableContent = DefaultBoolean.True;
        }

        // General Approach
        protected void Button_Click(object sender, EventArgs e) {
            var options = new CsvExportOptionsEx();
            options.EncodeExecutableContent = DefaultBoolean.True;
            ASPxGridView1.ExportCsvToResponse(options);
        }
    }
}