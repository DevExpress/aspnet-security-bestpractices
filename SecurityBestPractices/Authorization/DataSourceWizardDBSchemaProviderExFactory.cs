using System;
using System.Linq;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;

namespace SecurityBestPractices.Authorization {
    public class DataSourceWizardDBSchemaProviderExFactory : DevExpress.DataAccess.Web.IDataSourceWizardDBSchemaProviderExFactory {
        public IDBSchemaProviderEx Create() {
            return new DBSchemaProviderEx();
        }
    }

    public class DBSchemaProviderEx : IDBSchemaProviderEx {
        public DBTable[] GetTables(SqlDataConnection connection, params string[] tableList) {
            // here you can check permission

            var dbTables = connection.GetDBSchema().Tables;
            return dbTables.Where(t => t.Name == "Categories" || t.Name == "Products").ToArray();
        }

        public DBTable[] GetViews(SqlDataConnection connection, params string[] viewList) {
            return Array.Empty<DBTable>();
        }

        public DBStoredProcedure[] GetProcedures(SqlDataConnection connection, params string[] procedureList) {
            return Array.Empty<DBStoredProcedure>();
        }

        public void LoadColumns(SqlDataConnection connection, params DBTable[] tables) {
        }
    }
}