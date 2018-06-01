using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Xml.Linq;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Web;

namespace SecurityBestPractices.Authorization.Dashboards {
    public class DashboardStorageWithAccessRules : IEditableDashboardStorage {
        readonly DataSet dashboards = new DataSet();

        const string dashboardLayoutColumn = "DashboardXml";
        const string nameColumn = "DashboardName";
        const string dashboardIDColumn = "DashboardID";
        readonly Dictionary<string, HashSet<string>> authDictionary = new Dictionary<string, HashSet<string>>();

        readonly string publicDashboardId;
        readonly string publicDashboardPath = @"/App_Data/PublicDashboard.xml";

        public DashboardStorageWithAccessRules() {
            DataTable table = new DataTable("Dashboards");
            DataColumn idColumn = new DataColumn(dashboardIDColumn, typeof(int)) {
                AutoIncrement = true,
                AutoIncrementSeed = 1,
                Unique = true,
                AllowDBNull = false
            };

            table.Columns.Add(idColumn);
            table.Columns.Add(dashboardLayoutColumn, typeof(string));
            table.Columns.Add(nameColumn, typeof(string));
            table.PrimaryKey = new[] { idColumn };
            dashboards.Tables.Add(table);

            // Register dashboard layouts
            var adminId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/AdminDashboard.xml")), "Admin Dashboard");
            var johnId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/JohnDashboard.xml")), "John Dashboard");
            this.publicDashboardId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(publicDashboardPath)), "Public Dashboard");

            // Authorization logic
            authDictionary.Add("Admin", new HashSet<string>(new [] { adminId, johnId, publicDashboardId })); // Admin can view/edit all dashboards
            authDictionary.Add("John", new HashSet<string>(new[] { johnId })); // John can view/edit only his dashboard
        }
        string AddDashboardCore(XDocument dashboard, string dashboardName) {
            DataRow newRow = dashboards.Tables[0].NewRow();
            newRow[nameColumn] = dashboardName;
            newRow[dashboardLayoutColumn] = dashboard;
            dashboards.Tables[0].Rows.Add(newRow);
            return newRow[dashboardIDColumn].ToString();
        }

        public bool IsAuthorized(string dashboardId) {
            var identityName = GetIdentityName();
            if(!string.IsNullOrEmpty(identityName)) {
                return authDictionary.ContainsKey(identityName) && authDictionary[identityName].Contains(dashboardId);
            }

            return false;
        }

        static string GetIdentityName() {
            return HttpContext.Current.User?.Identity?.Name;
        }

        // Storage implementation
        XDocument IDashboardStorage.LoadDashboard(string dashboardId) {
            if (!IsAuthorized(dashboardId))
                return null;

            DataRow currentRow = dashboards.Tables[0].Rows.Find(dashboardId);
            if (currentRow == null)
                return null;

            XDocument dashboardXml = XDocument.Parse(currentRow[dashboardLayoutColumn].ToString());
            return dashboardXml;
        }


        IEnumerable<DashboardInfo> IDashboardStorage.GetAvailableDashboardsInfo() {
            List<DashboardInfo> dashboardInfos = new List<DashboardInfo>();
            foreach (DataRow row in dashboards.Tables[0].Rows) {
                var dashboardId = row[dashboardIDColumn].ToString();
                if (IsAuthorized(dashboardId)) {
                    DashboardInfo dashboardInfo = new DashboardInfo {
                        ID = row[dashboardIDColumn].ToString(),
                        Name = row[nameColumn].ToString()
                    };
                    dashboardInfos.Add(dashboardInfo);
                }
            }

            return dashboardInfos;
        }

        void IDashboardStorage.SaveDashboard(string dashboardId, XDocument dashboard) {
            if (!IsAuthorized(dashboardId))
                return;

            if(dashboardId == this.publicDashboardId) {
                dashboard.Save(HttpContext.Current.Server.MapPath(publicDashboardPath));
            }

            DataRow currentRow = dashboards.Tables[0].Rows.Find(dashboardId);
            if (currentRow == null)
                return;

            currentRow[dashboardLayoutColumn] = dashboard;
        }
        string IEditableDashboardStorage.AddDashboard(XDocument dashboard, string dashboardName) {
            var id = AddDashboardCore(dashboard, dashboardName);
            var identityName = GetIdentityName();

            if(string.IsNullOrEmpty(identityName))
                throw new UnauthorizedAccessException();

            if (!authDictionary.ContainsKey(identityName)) {
                authDictionary.Add(identityName, new HashSet<string>());
            }
            authDictionary[identityName].Add(id);

            return id;
        }
    }
}