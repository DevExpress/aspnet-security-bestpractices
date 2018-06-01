# ASP.NET Security Best Practices

This README document provides information on best practices that you should follow when you develop your applications to avoid introducing any security breaches. The document is divided into sections. Each section describes a use case scenario and possible vulnerabilities associated with it along with information on how to make sure that these vulnerabilities do not take place in you applications. Thus, you can use this document as check list to verify your applications security.

The example solution illustrates the described vulnerabilities and provides code samples with comprehensive comments. To launch this solution, you need to have DevExpress ASP.NET controls installed. You can download the installer from the DevExpress site.

---

## 1. Uploading Files

**Related Controls**: [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview), [ASPxUploadControl](https://documentation.devexpress.com/AspNet/4040/ASP-NET-WebForms-Controls/File-Management/File-Upload/Overview/ASPxUploadControl-Overview), [ASPxFileManager](https://documentation.devexpress.com/AspNet/9030/ASP-NET-WebForms-Controls/File-Management/File-Manager/Overview/ASPxFileManager-Overview), [ASPxHtmlEditor](https://documentation.devexpress.com/AspNet/4024/ASP-NET-WebForms-Controls/HTML-Editor), [ASPxRichEdit](https://documentation.devexpress.com/AspNet/17721/ASP-NET-WebForms-Controls/Rich-Text-Editor), [ASPxSpreadsheet](https://documentation.devexpress.com/AspNet/16157/ASP-NET-WebForms-Controls/Spreadsheet)

**Security Risks**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html), [CWE-434](https://cwe.mitre.org/data/definitions/434.html)

This section provides information on security best practices that you should consider when organizing the file uploading functionality in your web applications. Document sections describe use-case scenarios along with related security concerns and best practices that you should follow to avoid introducing any security breaches. The following sections are included:

* [1.1. Prevent Uploading Malicious Files](#11-prevent-uploading-malicious-files)
* [1.2. Prevent Uncontrolled Resource Consumption](#12-prevent-uncontrolled-resource-consumption)
* [1.3. Protect Temporary Files](#13-protect-temporary-files)

### 1.1. Prevent Uploading Malicious Files
See the **[UploadingFiles\UploadControl.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices/UploadingFiles/UploadControl.aspx.cs)** page source code for a full code sample with commentaries.

Consider a situation in which your web application supports uploading files, which are then available under a URL. A security concern occurs when a web application allows uploading executable files, which than can be executed on the server side. For example, a malefactor can upload an ASPX file containing malicious code and guess its URL. If the malefactor requests this URL, the file will be executed on the server as if it was a part of the application.

In this example solution, you can can get familiar with the issue using the following steps:

1. Run the solution and open the **UploadingFiles/UploadControl.aspx** page.
2. Upload the **\App_Data\TestData\Malicious.aspx**, file.
3. Now it is possible to execute the uploaded file on the server side by requesting it using the following address: **/UploadingFiles/Images/Malicious.aspx**

Take into account the following rules to mitigate this vulnerability:

1. Perform server-side validation of the uploaded file type by specifying the [AllowedExtensions](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_AllowedFileExtensionstopic) property.
``` asp
<ValidationSettings AllowedFileExtensions=".jpg, .png">
</ValidationSettings>
```
2. Disable file execution in the upload folder ([https://stackoverflow.com/questions/3776847/how-to-restrict-folder-access-in-asp-net](https://stackoverflow.com/questions/3776847/how-to-restrict-folder-access-in-asp-net))
``` xml
  <location path="UploadingFiles/Images">
    <system.webServer>
      <handlers>
        <clear />
        <add
          name="StaticFile"
          path="*" verb="*"
          modules="StaticFileModule"
          resourceType="Either"
          requireAccess="Read" />
      </handlers>
    </system.webServer>
  </location>
```
The table below lists the default file extensions allowed by various controls with the file uploading functionality. 

| Control                 |Allowed Extensions                                                                                                                      
| ----------------------- | ------------------                                                                                
| **ASPxUploadControl**   | *any*                                                                                                                  
| **ASPxBinaryImage**     | *any*                                                                
| **ASPxFileManager**     | *any*                                                             
| **ASPxHtmlEditor**      | .jpe, .jpeg, .jpg, .gif, .png <br> .mp3, .ogg <br> .swf <br> .mp4    
| **ASPxRichEdit**        | .doc, .docx, .epub, .html, .htm, .mht, .mhtml, .odt, .txt, .rtf, .xml 
| **ASPxSpreadsheet**     | .xlsx, .xlsm, .xls, .xltx, .xltm, .xlt, .txt, .csv        



### 1.2. Prevent Uncontrolled Resource Consumption
See the **[UploadingFiles/UploadControlMemory.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices/UploadingFiles/UploadControlMemory.aspx.cs)** page source code for a full code sample with commentaries.

If the application does not restrict the maximum uploaded file size, there is a security breach allowing a malefactor to perform a denial of service ([DoS](https://cwe.mitre.org/data/definitions/400.html)) attack by cluttering up server memory and disk space.

Take into account the following rules to mitigate this vulnerability:

1. To prevent memory overflow when working with massive uploaded files, access file contents using the [FileContent](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileContenttopic) property (a Stream) rather than [FileBytes](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileBytestopic) property (a byte array).
``` cs
protected void uploadControl_FilesUploadComplete(object sender, DevExpress.Web.FilesUploadCompleteEventArgs e) {
    for(int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
        UploadedFile file = uploadControl.UploadedFiles[i];
 	
        // Recommended approach - use stream for large files
        using (var stream = file.FileContent) {
            DoProcessing(stream);
        }
    }
}

```
2. Specify the maximum size for uploaded files using the [UploadControlValidationSettings.MaxFileSize](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_MaxFileSizetopic) property.  Note that in the **Advanced** uploading mode, files are loaded in small fragments (200KB by default), thus setting the **httpRuntime**>**maxRequestLength** and **requestLimits**>**maxAllowedContentLength** options in **web.config** is not enough to prevent attacks.

See the [Uloading Large Filtes](https://documentation.devexpress.com/AspNet/9822/ASP-NET-WebForms-Controls/File-Management/File-Upload/Concepts/Uploading-Large-Files) documentation topic for more information.

The following controls limit the maximum size for uploaded files by default:
* The **ASPxHtmlEditor** defines the *31457280* byte limit for uploaded file size.
* The **ASPxSpreadsheet** and **ASPxRichEdit** define the *31457280* byte limit for images to be inserted into a document. The uploaded document size is not limited. However, document uploading is disabled by default.

Be aware that the file manager control allows uploading files and does not impose any limitations on the file size and extension by default. You can disable file uploading as shown below:
``` asp
<ASPxFileManager ... >
    ...
    <SettingsUpload Enabled="false">
</ASPxFileManager>
```  
The availability of other operations on files (such as copying, deleting, downloading, etc.) is configured using the [SettingsEditing](http://help.devexpress.com/#AspNet/DevExpressWebASPxFileManager_SettingsEditingtopic) property. By default, all operations are disabled.


### 1.3. Protect Temporary Files
See the **[UploadingFiles/UploadControlTempFileName.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices/UploadingFiles/UploadControlTempFileName.aspx.cs)** page source code for a full code sample with commentaries. 

If you are storing temporary files on the server (e.g., to process the uploaded content before it is moved to a database), you need to make sure that these files are inaccessible for third parties.  

Take into account the following rules to prevent security concerns:

1. Store temporary files in a folder unreachable by URL (e.g., *App_Data*).
2. Additionally use a dedicated file extension for temporary files on the server (for example *“.tmp”*). 
3. Consider assigning random file names using the [GetRandomName](https://msdn.microsoft.com/en-us/library/system.io.path.getrandomfilename(v=vs.110).aspx) method.
``` cs
protected void uploadControl_FilesUploadComplete(object sender, DevExpress.Web.FilesUploadCompleteEventArgs e) {
    if (uploadControl.UploadedFiles != null && uploadControl.UploadedFiles.Length > 0) {
        for (int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
            UploadedFile file = uploadControl.UploadedFiles[i];
            if (file.FileName != "") { 
                string fileName = string.Format("{0}{1}", MapPath("~/UploadingFiles/Processing/"),
                    Path.GetRandomFileName() + ".tmp");
                file.SaveAs(fileName, true);
                // DoFileProcessing(fileName);
                ...

```



You can also define security permissions for folders and files accessible through the File Manager control. To learn how to achieve this, refer to the [Access Rules](https://documentation.devexpress.com/AspNet/119542/ASP-NET-WebForms-Controls/File-Management/File-Manager/Concepts/Access-Control-Overview/Access-Rules) documentation topic.


---


## 2. Uploading and Displaying Binary Images
**Related Controls**: [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview), [ASPxUploadControl](https://documentation.devexpress.com/AspNet/4040/ASP-NET-WebForms-Controls/File-Management/File-Upload/Overview/ASPxUploadControl-Overview)

**Security Risks**: [CWE-79](https://cwe.mitre.org/data/definitions/79.html)

If there is a potential for a file containing a malicious script disguised as an image to be uploaded and sent to an end-user, an opportunity is open to run malicious code in the end-user’s browser (XSS via Content-sniffing, a particular case of [CWE-79](https://cwe.mitre.org/data/definitions/79.html)).

You can familiarize yourself with the issue using the following steps:
1. Run the example solution and open the **[UploadingBinaryImage/UploadControl.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx)** page.
2. Upload the **[\App_Data\TestData\Content-Sniffing-XSS.jpg](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices/App_Data/TestData/Content-Sniffing-XSS.jpg)** file, which is a JavaScript file emulating a malicious script disguised as a JPEG image.
3. Open the **[UploadingBinaryImage/BinaryImageViewer.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices/UploadingBinaryImages/BinaryImageViewer.aspx)** page, which writes the uploaded file to the server response in the [code behind](https://github.com/DevExpress/aspnet-security-bestpractices/blob/fd40850d01330a3d16f1a5a8c3cfd80cbe831c60/SecurityBestPractices/UploadingBinaryImages/BinaryImageViewer.aspx.cs#L17-L18).
4. As the result, JavaScript code from the uploaded file is executed by the browser:

![malicious-image](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/uploading-binary-image-1.png?raw=true)

To prevent possible security issues, consider one of the following solutions:
1.	Programmatically check whether the uploaded file is really an image before saving it to the server-side storage (see the [IsValidImage](https://github.com/DevExpress/aspnet-security-bestpractices/blob/fd40850d01330a3d16f1a5a8c3cfd80cbe831c60/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx.cs#L22-L31) method implementation).
``` cs
protected void ASPxUploadControl1_FileUploadComplete(object sender, 
  DevExpress.Web.FileUploadCompleteEventArgs e) {
    // Here contentBytes should be saved to a database
    using(var stream = e.UploadedFile.FileContent) {
        if(!IsValidImage(stream)) return;

        // For demonstration purposes, content is saved to a file
        string fileName = Server.MapPath("~/App_Data/TestData/avatar.jpg");
        e.UploadedFile.SaveAs(fileName, true);
    }
}

static bool IsValidImage(Stream stream) {
    try {
        using(var image = Image.FromStream(stream)) {
            return true;
        }
    }
    catch(Exception) {
        return false;
    }
}
```
2.	Use the [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview) control for image uploading. This control implements automatic file type check.

\[Aspx\]
``` asp
<dx:ASPxBinaryImage ID="ASPxBinaryImage1" runat="server">
    <EditingSettings Enabled="True">
    </EditingSettings>
</dx:ASPxBinaryImage>
<dx:ASPxButton ID="ASPxButton1" runat="server" OnClick="ASPxButton1_Click" Text="Save">
</dx:ASPxButton>
```
\[C#\]
``` cs
protected void ASPxButton1_Click(object sender, EventArgs e) {
    byte[] contentBytes = ASPxBinaryImage1.ContentBytes;
    // Here contentBytes should be saved to a database
 
    // For demonstration purposes, content is saved to a file
    string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
    File.WriteAllBytes(fileName, contentBytes);
}

```

It is also recommended that you always specify the exact content type when you write binary data to the response:

**Correct:** `Response.ContentType = "image/jpeg"`;

**Potential security breach:** `Response.ContentType = "image"`.

### Notes:
1. Microsoft Edge automatically detects a file's type based on its content, which prevents execution of malicious scripts.
2. Make sure to strictly specify the maximum uploaded file size to prevent DoS attacks based on uploading large files.
---




## 3. Authorization 
This section provides information on security best practices to consider when using DevExpress controls in web applications with authorization and access control.

* [3.1. Reporting](#31-reporting)
* [3.2. Dashboard](#32-dashboard)
* [3.3. Query Builder](#33-query-builder)

### 3.1. Reporting
Normally, when you create a reporting application with access restrictions using one of the standard Microsoft mechanisms, you grant or restrict access to particular pages based on a user’s identity: 

``` xml
  <location path="Authorization/Reports">
    <system.web>
      <authorization>
        <deny users="?" />
      </authorization>
    </system.web>
  </location>
```

However, note that by restricting access to certain pages, containing the [Document Viewer](https://documentation.devexpress.com/XtraReports/17738/Creating-End-User-Reporting-Applications/Web-Reporting/Document-Viewer/HTML5-Document-Viewer) control, you don’t automatically protect the report files that these pages display. These files can still be accessed by the Document Viewer control's instances from other pages through client side API. Knowing a report's name, a malefactor can open it, by calling the client [OpenReport](http://help.devexpress.com/#XtraReports/DevExpressXtraReportsWebScriptsASPxClientWebDocumentViewer_OpenReporttopic) method:

``` js
documentViewer.OpenReport("ReportTypeName"); 
```

The best practice when developing a web reporting application is to define authorization rules in server code by implementing a custom report storage derived from the [ReportStorageWebExtension](http://help.devexpress.com/#XtraReports/clsDevExpressXtraReportsWebExtensionsReportStorageWebExtensiontopic) class. You can copy the reference implementation of from the example project's [ReportStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs) file to your application and fine-tune it for your needs. Based on your use-case scenario, the following customizations are required:


#### A. Viewing Reports

In the sample project, the [GetViewableReportDisplayNamesForCurrentUser](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L25-L38) method returns a list of reports available to be viewed by the currently logged in user:

``` cs
// Logic for getting reports available for viewing
public static IEnumerable<string> GetViewableReportDisplayNamesForCurrentUser() {
    var identityName = GetIdentityName();

    var result = new List<string> { reports[typeof(PublicReport)] }; // For unauthenticated users (i.e., public)

    if (identityName == "Admin") {
        result.AddRange(new[] { reports[typeof(AdminReport)], reports[typeof(JohnReport)] });
    } else if (identityName == "John") {
        result.Add(reports[typeof(JohnReport)]);
    }
    return result;
}
```

This method is then called from the overridden [GetData](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L60-L70) method and other methods interacting with the report storage:

``` cs
public override byte[] GetData(string url) {
    var reportNames = GetViewableReportDisplayNamesForCurrentUser();
    if (!reportNames.Contains(url))
        throw new UnauthorizedAccessException();

    XtraReport publicReport = CreateReportByDisplayName(url);
    using (MemoryStream ms = new MemoryStream()) {
        publicReport.SaveLayoutToXml(ms);
        return ms.GetBuffer();
    }
}
```


#### B. Editing Reports
In the sample project, the [GetEditableReportNamesForCurrentUser](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L41-L53) method returns a list of reports available to be edited by the currently logged in user:

``` cs
// Logic for getting reports available for editing
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
```

This method is then called from the overridden [IsValidUrl](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L80-L83) method and other methods related to writing report data.

``` cs
public override bool IsValidUrl(string url) {
    var reportNames = GetEditableReportNamesForCurrentUser();
    return reportNames.Contains(url);
}
```

To prevent errors in an end-user’s browser when handling unauthorized access attempts, check the access rights on the page’s [PageLoad](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportDesignerPage.aspx.cs#L6-L13) event. If the user is not authorized to open the report, redirect to a public page.

``` cs
protected void Page_Load(object sender, EventArgs e) {
    var name = Request.QueryString["name"];
    var reportNames = ReportStorageWithAccessRules.GetEditableReportNamesForCurrentUser();
    if(reportNames.Contains(name))
        ASPxReportDesigner1.OpenReport(name);
    else
        Response.Redirect("~/Authorization/Reports/ReportViewerPage.aspx");
}
``` 


#### Register the Custom Report Storage
After implementing you custom report storage with access rules, register it in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Global.asax.cs) file: 

``` cs
DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageWithAccessRules());
```     

#### Make Sure that Authentication Rules are Applied
In the example project, you can check whether the customization has effect using the following steps:
* Open the [PublicReportPage.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Authorization/PublicPages/PublicReportPage.aspx) page with a Report Viewer without logging in.
* Try to open a report with restricted access using client API in the browser console:
```
>documentViewer.OpenReport("Admin Report");
```
The browser console will respond with the following error.

![console-output](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authoriazation-reports-accessdenied.png?raw=true)


#### Restrict Access to Data Conntections and Data Tables
The [Report Designer](https://documentation.devexpress.com/XtraReports/17103/Creating-End-User-Reporting-Applications/Web-Reporting/Report-Designer) control allows an end-user to browse available data connection and data tables using the integrated [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder). Refer to the [Query Builder](#33-query-builder) subsection to learn how to restrict access to this information based on authorization rules.





### 3.2. Dashboard

The [DevExpress Web Dashboard](https://devexpress.github.io/dotnet-eud/dashboard-for-web/articles/index.html) can operate in one of the two supported modes:

**1) Callbacks are processed by an ASPx page containing the ASPxDashboard control (the [UseDashboardConfigurator](http://help.devexpress.com/#Dashboard/DevExpressDashboardWebASPxDashboard_UseDashboardConfiguratortopic) property is set to false)**

Use the standard ASP.NET access restriction mechanisms:

``` xml
<location path="Authorization/Dashboards">
    <system.web>
        <authorization>
           <deny users="?" />
        </authorization>
    </system.web>
</location>
```

This mode is active by default.

**2) Callbacks are processed by the Dashboard Configurator on the DevExpress HTTP Handler side (the [UseDashboardConfigurator](http://help.devexpress.com/#Dashboard/DevExpressDashboardWebASPxDashboard_UseDashboardConfiguratortopic) property is set to true)**

This is the recommended mode, as it is considerably faster and much more flexible. However, in this mode, access restriction rules defined using the default mechanisms have no effect. The access control should be performed by a custom dashboard storage implementing the [IEditableDashboardStorage](https://docs.devexpress.com/Dashboard/DevExpress.DashboardWeb.IEditableDashboardStorage?tabs=tabid-csharp%2Ctabid-T392813_7_52373613) interface.

You can copy the reference implementation of a dashboard storage from the example project's [DashboardStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Authorization/Dashboards/DashboardStorageWithAccessRules.cs) and fine-tune it for your needs.

The [DashboardStorageWithAccessRules](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Dashboards/DashboardStorageWithAccessRules.cs#L12-L126) class implementation defines the access restrictions:

``` cs
// Register dashboard layouts
var adminId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/AdminDashboard.xml")), "Admin Dashboard");
var johnId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/JohnDashboard.xml")), "John Dashboard");
this.publicDashboardId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(publicDashboardPath)), "Public Dashboard");
```

The code below defines which user should have access to which dashboards.

``` cs
// Authorization logic
authDictionary.Add("Admin", new HashSet<string>(new [] { adminId, johnId, publicDashboardId })); // Admin can view/edit all dashboards.
authDictionary.Add("John", new HashSet<string>(new[] { johnId })); // John can view/edit only his dashboard.

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
```

Register the custom dashboard storage in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Global.asax.cs) file as shown below.

``` cs
DashboardConfigurator.Default.SetDashboardStorage(new DashboardStorageWithAccessRules());
DashboardConfigurator.Default.CustomParameters += (o, args) => {
if (!new DashboardStorageWithAccessRules().IsAuthorized(args.DashboardId))
    throw new UnauthorizedAccessException();
};
```

With this custom implementation of a dashboard storage, if a user named 'John' tries to use the client API to open a report with restricted access (e.g., a report with id=’1’), the handler will return the error 404:

``` js
dashboard.LoadDashboard('1') // Load a dashboard available only to Admin.
```


```
GET http://localhost:65252/Authorization/Dashboards/DXDD.axd?action=DashboardAction/1&_=1525787741461 404 (Not Found)
```

![console-output](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authorization-dashboard-404.png?raw=true)

#### Restrict Access to Data Conntections and Data Tables
The Web Dashboard control allows an end-user to browse available data connection and data tables using the integrated [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder). Refer to the [Query Builder](#33-query-builder) subsection to learn how to restrict access to this information based on authorization rules.



### 3.3. Query Builder

The standalone [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder) as well as the Query Builder integrated into the Report and Dashboard designers allows an end-user to browse a web application's data connections and a data tables available through these connections. In a web application with access control, you need to restrict an end-user’s access to the available connections and data tables in code.

To restrict the access to connection strings, implement a custom connection string provider:

``` cs
public class DataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider {

    public Dictionary<string, string> GetConnectionDescriptions() {
        Dictionary<string, string> connections =
            new Dictionary<string, string> { { "nwindConnection", "NWind database" } };

        // Customize the loaded connections list.  

        // Access restriction logic.
        //if(GetIdentityName() == "Admin")
        //    connections.Add("secretConnection", "Admin only database");

        return connections;
    }

    public DataConnectionParametersBase GetDataConnectionParameters(string name) {
        return AppConfigHelper.LoadConnectionParameters(name);
    }
}
```

Implement a custom database schema provider to restrict the access to data tables:

``` cs
public class DBSchemaProviderEx : IDBSchemaProviderEx {
    public DBTable[] GetTables(SqlDataConnection connection, params string[] tableList) {
        // Check permissions.

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
```


The Dashboard Designer requires the database schema provider to be created by a factory object:

``` cs
public class DataSourceWizardDBSchemaProviderExFactory : DevExpress.DataAccess.Web.IDataSourceWizardDBSchemaProviderExFactory {
    public IDBSchemaProviderEx Create() {
        return new DBSchemaProviderEx();
    }
}
```
You can copy the reference implementation from the example project's [DataSourceWizardConnectionStringsProvider.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Authorization/DataSourceWizardConnectionStringsProvider.cs) and [DataSourceWizardDBSchemaProviderExFactory.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Authorization/DataSourceWizardDBSchemaProviderExFactory.cs) files to your application and fine-tune it for your needs.

Register the implemented classes for the Report Designer, Dashboard Designer or standalone Query Builder as shown below. (See the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/develop/SecurityBestPractices/Global.asax.cs) file)

**Report Designer:**
``` cs
DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider<DataSourceWizardConnectionStringsProvider>();
DefaultReportDesignerContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```

**Dashboard Designer:**
``` cs
DefaultQueryBuilderContainer.Register<IDataSourceWizardConnectionStringsProvider, DataSourceWizardConnectionStringsProvider>();
DefaultQueryBuilderContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```

**Query Builder:**
``` cs
DashboardConfigurator.Default.SetConnectionStringsProvider(new DataSourceWizardConnectionStringsProvider());
DashboardConfigurator.Default.SetDBSchemaProvider(new DBSchemaProviderEx());
```
---








