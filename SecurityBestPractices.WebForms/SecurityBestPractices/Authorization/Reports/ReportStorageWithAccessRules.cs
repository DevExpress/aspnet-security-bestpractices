using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;

namespace SecurityBestPractices.Authorization.Reports {
    public class ReportStorageWithAccessRules : ReportStorageWebExtension {
        private static readonly Dictionary<Type, string> reports = new Dictionary<Type, string> {
            {typeof(PublicReport), "Public Report"},
            {typeof(AdminReport), "Admin Report"},
            {typeof(JohnReport), "John Report"}
        };
        static string GetIdentityName() {
            return HttpContext.Current.User?.Identity?.Name;
        }
        static XtraReport CreateReportByDisplayName(string displayName) {
            Type type = reports.First(v => v.Value == displayName).Key;

            return (XtraReport)Activator.CreateInstance(type);
        }
        // Getting viewable reports
        public static IEnumerable<string> GetViewableReportDisplayNamesForCurrentUser() {
            var identityName = GetIdentityName();

            var result = new List<string>(); 

            if (identityName == "Admin") {
                result.AddRange(new[] { reports[typeof(AdminReport)], reports[typeof(JohnReport)] });
            } else if (identityName == "John") {
                result.Add(reports[typeof(JohnReport)]);
            }

            result.Add(reports[typeof(PublicReport)]); // for unauthenticated users (ie public)

            return result;
        }

        // Getting editable reports
        public static IEnumerable<string> GetEditableReportNamesForCurrentUser() {
            var identityName = GetIdentityName();

            if (identityName == "Admin") {
                return new[] { reports[typeof(AdminReport)], reports[typeof(JohnReport)] };
            }

            if (identityName == "John") {
                return new[] { reports[typeof(JohnReport)] };
            }

            return Array.Empty<string>();
        }

        // Storage override
        public override bool CanSetData(string url) {
            var reportNames = GetEditableReportNamesForCurrentUser();
            return reportNames.Contains(url);
        }
        public override byte[] GetData(string url) {
            var reportNames = GetViewableReportDisplayNamesForCurrentUser();
            if(!reportNames.Contains(url))
                throw new UnauthorizedAccessException();

            // TODO: Put your logic to get bytes from DB
            XtraReport publicReport = CreateReportByDisplayName(url);
            using(MemoryStream ms = new MemoryStream()) {
                publicReport.SaveLayoutToXml(ms);
                return ms.GetBuffer();
            }
        }
        public override Dictionary<string, string> GetUrls() {
            // Get URLs and display names for all reports available for editing in the storage.
            var result = new Dictionary<string, string>();
            var reportNames = GetEditableReportNamesForCurrentUser();
            foreach(var reportName in reportNames) {
                result.Add(reportName, reportName);                    
            }
            return result;
        }
        public override bool IsValidUrl(string url) {
            var reportNames = GetEditableReportNamesForCurrentUser();
            return reportNames.Contains(url);
        }

        public override void SetData(XtraReport report, string url) {
            // TODO: Put your logic to save bytes to DB
            // Save report layout
            // https://documentation.devexpress.com/XtraReports/17553/Creating-End-User-Reporting-Applications/Web-Reporting/Report-Designer/API-and-Customization/Implementing-a-Report-Storage
        }
        public override string SetNewData(XtraReport report, string defaultUrl) {
            // TODO: Put your logic to save bytes to DB
            // Save new report layout and return report's URL
            // https://documentation.devexpress.com/XtraReports/17553/Creating-End-User-Reporting-Applications/Web-Reporting/Report-Designer/API-and-Customization/Implementing-a-Report-Storage
            return "New name";
        }
    }
}