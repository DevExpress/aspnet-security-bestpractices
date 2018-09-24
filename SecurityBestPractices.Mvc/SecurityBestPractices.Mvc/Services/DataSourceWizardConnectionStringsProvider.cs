using System.Collections.Generic;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Web;

namespace SecurityBestPractices.Mvc.Services {
    public class DataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider {

        public Dictionary<string, string> GetConnectionDescriptions() {
            Dictionary<string, string> connections =
                new Dictionary<string, string> { { "nwindConnectionXpo", "NWind database" } };

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