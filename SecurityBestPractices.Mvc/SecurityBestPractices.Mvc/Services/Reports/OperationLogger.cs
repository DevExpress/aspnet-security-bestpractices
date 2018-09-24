using System;
using System.Collections.Generic;
using System.Web;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace SecurityBestPractices.Mvc.Services.Reports {
    public class OperationLogger : WebDocumentViewerOperationLogger, IWebDocumentViewerAuthorizationService,
        IExportingAuthorizationService {
        const string ReportDictionaryName = "reports";
        const string DocumentDictionaryName = "documents";
        const string ExportedDocumentDictionaryName = "exportedDocuments";


        static readonly Dictionary<string, Dictionary<string, HashSet<string>>> authDictionary =
            new Dictionary<string, Dictionary<string, HashSet<string>>>();

        static OperationLogger() {
            authDictionary.Add("Public", new Dictionary<string, HashSet<string>> {
                {ReportDictionaryName, new HashSet<string>()},
                {DocumentDictionaryName, new HashSet<string>()},
                {ExportedDocumentDictionaryName, new HashSet<string>()}
            });
            authDictionary.Add("Admin", new Dictionary<string, HashSet<string>> {
                {ReportDictionaryName, new HashSet<string>()},
                {DocumentDictionaryName, new HashSet<string>()},
                {ExportedDocumentDictionaryName, new HashSet<string>()}
            });
            authDictionary.Add("John", new Dictionary<string, HashSet<string>> {
                {ReportDictionaryName, new HashSet<string>()},
                {DocumentDictionaryName, new HashSet<string>()},
                {ExportedDocumentDictionaryName, new HashSet<string>()}
            });
        }

        #region WebDocumentViewerOperationLogger

        public override void ReportOpening(string reportId, string documentId, XtraReport report) {
            if(report == null) {
                var identityName = IdentityHelper.GetIdentityName();
                if(string.IsNullOrEmpty(identityName))
                    identityName = "Public";

                SaveUsedEntityId(ReportDictionaryName, identityName, reportId);
                SaveUsedEntityId(DocumentDictionaryName, identityName, documentId);
            } else
            if(report is PublicReport) {
                SaveUsedEntityId(ReportDictionaryName, "Public", reportId);
                SaveUsedEntityId(DocumentDictionaryName, "Public", documentId);
            }
            else if(report is AdminReport) {
                SaveUsedEntityId(ReportDictionaryName, "Admin", reportId);
                SaveUsedEntityId(DocumentDictionaryName, "Admin", documentId);
            }
            else if(report is JohnReport) {
                SaveUsedEntityId(ReportDictionaryName, "John", reportId);
                SaveUsedEntityId(DocumentDictionaryName, "John", documentId);

                SaveUsedEntityId(ReportDictionaryName, "Admin", reportId);
                SaveUsedEntityId(DocumentDictionaryName, "Admin", documentId);
            }
        }

        public override void BuildStarted(string reportId, string documentId, ReportBuildProperties buildProperties) {
            if(IsEntityAuthorized("Public", ReportDictionaryName, reportId)) {
                SaveUsedEntityId(DocumentDictionaryName, "Public", documentId);
            }

            if(IsEntityAuthorized("Admin", ReportDictionaryName, reportId)) {
                SaveUsedEntityId(DocumentDictionaryName, "Admin", documentId);
            }

            if(IsEntityAuthorized("John", ReportDictionaryName, reportId)) {
                SaveUsedEntityId(DocumentDictionaryName, "John", documentId);
            }
        }

        public override ExportedDocument ExportDocumentStarting(string documentId, string asyncExportOperationId,
            string format, ExportOptions options, PrintingSystemBase printingSystem,
            Func<ExportedDocument> doExportSynchronously) {
            if(!IsEntityAuthorizedForCurrentUser(DocumentDictionaryName, documentId))
                throw new UnauthorizedAccessException();

            return base.ExportDocumentStarting(documentId, asyncExportOperationId, format, options, printingSystem,
                doExportSynchronously);
        }

        public override void ReleaseDocument(string documentId) {
        }

        #endregion WebDocumentViewerOperationLogger

        #region IWebDocumentViewerAuthorizationService

        bool IWebDocumentViewerAuthorizationService.CanCreateDocument() {
            return true;
        }

        bool IWebDocumentViewerAuthorizationService.CanCreateReport() {
            return true;
        }

        bool IWebDocumentViewerAuthorizationService.CanReadDocument(string documentId) {
            return IsEntityAuthorizedForCurrentUser(DocumentDictionaryName, documentId);
        }

        bool IWebDocumentViewerAuthorizationService.CanReadReport(string reportId) {
            return IsEntityAuthorizedForCurrentUser(ReportDictionaryName, reportId);
        }

        bool IWebDocumentViewerAuthorizationService.CanReleaseDocument(string documentId) {
            return IsEntityAuthorizedForCurrentUser(DocumentDictionaryName, documentId);
        }

        bool IWebDocumentViewerAuthorizationService.CanReleaseReport(string reportId) {
            return IsEntityAuthorizedForCurrentUser(ReportDictionaryName, reportId);
        }

        #endregion IWebDocumentViewerAuthorizationService

        void SaveUsedEntityId(string dictionaryName, string user, string id) {
            if(string.IsNullOrEmpty(id))
                return;
            
            lock(authDictionary)
                authDictionary[user][dictionaryName].Add(id);
        }

        bool IsEntityAuthorizedForCurrentUser(string dictionaryName, string id) {
            return IsEntityAuthorized(IdentityHelper.GetIdentityName(), dictionaryName, id);
        }

        bool IsEntityAuthorized(string user, string dictionaryName, string id) {
            if(string.IsNullOrEmpty(id))
                return false;

            lock(authDictionary)
                return authDictionary["Public"][dictionaryName].Contains(id) || !string.IsNullOrEmpty(user) && authDictionary[user][dictionaryName].Contains(id);
        }

        public bool CanReadExportedDocument(string id) {
            return IsEntityAuthorizedForCurrentUser(ExportedDocumentDictionaryName, id); // for DevExpress.Report.Preview.AsyncExportApproach = true;
        }
    }
}