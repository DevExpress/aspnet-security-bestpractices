using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraReports.Web.QueryBuilder;

namespace SecurityBestPractices.Authorization.QueryBuilder
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ASPxQueryBuilder1.OpenConnection("nwindConnection");
        }
    }
}