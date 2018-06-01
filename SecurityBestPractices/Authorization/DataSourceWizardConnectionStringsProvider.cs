using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Web;

namespace SecurityBestPractices.Authorization {
    public class DataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider {

        public Dictionary<string, string> GetConnectionDescriptions() {
            Dictionary<string, string> connections =
                new Dictionary<string, string> { { "nwindConnection", "NWind database" } };

            // Customize the connections list for your needs

            // Access restriction logic
            //if(GetIdentityName() == "Admin")
            //    connections.Add("secretConnection", "Admin only database");

            return connections;
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name) {
            return AppConfigHelper.LoadConnectionParameters(name);
        }
    }
}