using System;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraReports.Web.QueryBuilder;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using SecurityBestPractices.Mvc.Services;
using SecurityBestPractices.Mvc.Services.Dashboards;
using SecurityBestPractices.Mvc.Services.Reports;

namespace SecurityBestPractices.Mvc {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            #region Query builder
            DefaultQueryBuilderContainer.Register<IDataSourceWizardConnectionStringsProvider, DataSourceWizardConnectionStringsProvider>();
            DefaultQueryBuilderContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();

            // MVCxQueryBuilder.StaticInitialize(); // Don't need this line if ASPxReportDesigner.StaticInitialize() has been called
            #endregion

            #region Reports
            DefaultWebDocumentViewerContainer.Register<IExportingAuthorizationService, OperationLogger>();
            DefaultWebDocumentViewerContainer.Register<WebDocumentViewerOperationLogger, OperationLogger>();
            DefaultWebDocumentViewerContainer.Register<IWebDocumentViewerAuthorizationService, OperationLogger>();

            DefaultReportDesignerContainer.Register<WebDocumentViewerOperationLogger, OperationLogger>();

            DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageWithAccessRules());
            DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider<DataSourceWizardConnectionStringsProvider>(); // Provide connections to Report Designer
            DefaultReportDesignerContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>(); // Provide only nessesary dbtables

            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Required;
            MVCxReportDesigner.StaticInitialize();
            #endregion

            #region Dashboards
            DashboardConfigurator.Default.SetDashboardStorage(new DashboardStorageWithAccessRules());
 
            DashboardConfigurator.Default.SetConnectionStringsProvider(new DataSourceWizardConnectionStringsProvider()); // Provide connections to Dashboard Designer
            DashboardConfigurator.Default.SetDBSchemaProvider(new DBSchemaProviderEx()); // Provide only nessesary dbtables
            #endregion            

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;
        }

        protected void Application_Error(object sender, EventArgs e) {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
        }
    }
}