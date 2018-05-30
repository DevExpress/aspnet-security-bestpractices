# ASP.NET Security Best Practices

This README document provides information on best practices that you should follow when you develop your applications to avoid introducing any security breaches. The document is divided into sections. Each section describes a use case scenario and possible vulnerabilities associated with it along with information on how to make sure that these vulnerabilities do not take place in you applications. Thus, you can use this document as check list to verify your applications security.

The example solution illustrates the described vulnerabilities and provides code samples with comprehensive comments. To launch this solution, you need to have DevExpress ASP.NET controls installed. You can download the installer from the DevExpress site.

---

## 1. Uploading Files

**Related Controls**: [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview), [ASPxUploadControl](https://documentation.devexpress.com/AspNet/4040/ASP-NET-WebForms-Controls/File-Management/File-Upload/Overview/ASPxUploadControl-Overview), [ASPxFileManager](https://documentation.devexpress.com/AspNet/9030/ASP-NET-WebForms-Controls/File-Management/File-Manager/Overview/ASPxFileManager-Overview), [ASPxHtmlEditor](https://documentation.devexpress.com/AspNet/4024/ASP-NET-WebForms-Controls/HTML-Editor), [ASPxRichEdit](https://documentation.devexpress.com/AspNet/17721/ASP-NET-WebForms-Controls/Rich-Text-Editor), [ASPxSpreadsheet](https://documentation.devexpress.com/AspNet/16157/ASP-NET-WebForms-Controls/Spreadsheet)

**Security Risks**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html), [CWE-434](https://cwe.mitre.org/data/definitions/434.html)

This document provides information on security best practices that you should consider when organizing the file uploading functionality in your web applications. Document sections describe use-case scenarios along with related security concerns and best practices that you should follow to avoid introducing any security breaches. The following sections are included:

* [Prevent Uploading Malicious Files](#prevent-uploading-malicious-files)
* [Prevent Uncontrolled Resource Consumption](#prevent-uncontrolled-resource-consumption)
* [Protect Temporary Files](#protect-temporary-files)

### Prevent Uploading Malicious Files
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
2. Disable file execution in the upload folder ([https://stackoverflow.com/questions/3776847/how-to-restrict-folder-access-in-asp-net])
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



### Prevent Uncontrolled Resource Consumption
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


### Protect Temporary Files
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
### 3.1 XtraReports
Normally when you create a reporting application with access restrictions via one of the standard Microsoft mechanisms, you grant or restrict access to particular pages based on a user’s identity: 

``` xml
  <location path="Authorization/Reports">
    <system.web>
      <authorization>
        <deny users="?" />
      </authorization>
    </system.web>
  </location>

```

However, note that by restricting access to certain pages, containing the Report Viewer control, you don’t automatically protect the report files that these pages display. These files can still be accessed by report viewers contained by other pages through client side API. A malefactor can open a report through API, by guessing the report’s name:

``` js
documentViewer.OpenReport("ReportTypeName"); 
```

The best practice when implementing a web reporting application is to restrict access to particular report files on the server by implementing a custom report storage by overriding methods of the base **ReportStorageWebExtension** class. In your custom method implementations, perform access rights check for a particular user. To implement this functionality in your application, you can copy the sample code from the **ReportStorageWithAccessRules.cs** file of the example project and fine-tune it for your needs.

Based on your use-case scenario, the following customizations are required:


#### A. Viewing Reports

In the sample project, the **GetViewableReportDisplayNamesForCurrentUser** method returns a list of reports available to be viewed by the currently logged in user:

``` cs
// Logic for getting reports available for viewing
public static IEnumerable<string> GetViewableReportDisplayNamesForCurrentUser() {
    var identityName = GetIdentityName();

    var result = new List<string> { reports[typeof(PublicReport)] }; // for unauthenticated users (ie public)

    if (identityName == "Admin") {
        result.AddRange(new[] { reports[typeof(AdminReport)], reports[typeof(JohnReport)] });
    } else if (identityName == "John") {
        result.Add(reports[typeof(JohnReport)]);
    }
    return result;
}
```

This method is then called from the overridden **GetData()** method and other methods interacting with the report storage:

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

Copy this code from the example project and change it for your needs.



#### B. Editing Reports
In the sample project, the **GetEditableReportNamesForCurrentUser** method returns a list of reports available to be edited by the currently logged in user:

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

This method is then called from the overridden **IsValidUrl()** method and other methods related to writing report data.

``` cs
public override bool IsValidUrl(string url) {
    var reportNames = GetEditableReportNamesForCurrentUser();
    return reportNames.Contains(url);
}
```

Copy this code from the example project and change it for your needs.


To prevent errors in an end-user’s browser when handling unauthorized access attempts, check the access rights on the page’s **PageLoad** event. If the user is not authorized to open the report, redirect to a public page.

(ReportViewerPage.aspx):
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



### 3.2 Dashboards 

The DevExpress Dashboards suite can operate in one of the two supported modes:

#####1)	Callbacks are processed by an ASPx page containing the ASPxDashboard control  (the  UseDashboardConfigurator property is set to false)

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

This mode is used by default.

#####2)	Callbacks are processed by the DashboardConfigurator on the DevExpress HTTP Handler side (the UseDashboardConfigurator property is set to true) 

In this mode, access restriction rules defined using the default mechanisms have no effect. The access control logic should be manually implemented by a custom dashboard storage class registered using the **DashboardConfigurator.Default.SetDashboardStorage** method:

``` cs
DashboardConfigurator.Default.SetDashboardStorage(new DashboardStorageWithAccessRules());
```

Note that this mode of operation is recommended, because it is considerably faster and more flexible.


``` cs
// Initialize the Dashboard for using authorization
DashboardConfigurator.Default.SetDashboardStorage(new DashboardStorageWithAccessRules());
DashboardConfigurator.Default.CustomParameters += (o, args) => {
if (!new DashboardStorageWithAccessRules().IsAuthorized(args.DashboardId))
    throw new UnauthorizedAccessException();
};
DashboardConfigurator.Default.SetConnectionStringsProvider(new DataSourceWizardConnectionStringsProvider()); // provide connections for dashboard designer
DashboardConfigurator.Default.SetDBSchemaProvider(new DBSchemaProviderEx()); // provide only nessesary dbtables
```

In the **DashboardStorageWithAccessRules** class implementation define the access restrictions.  

``` cs
// Register dashboard layouts
var adminId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/AdminDashboard.xml")), "Admin Dashboard");
var johnId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/JohnDashboard.xml")), "John Dashboard");
this.publicDashboardId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(publicDashboardPath)), "Public Dashboard");
```

The code below defines which user should have access to which dashboards.

``` cs
// Authorization logic
authDictionary.Add("Admin", new HashSet<string>(new [] { adminId, johnId, publicDashboardId })); // admin can view/edit all dashboards
authDictionary.Add("John", new HashSet<string>(new[] { johnId })); // john can view/edit only his dashboard

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

With this custom implementation, if a user John will try to use the client API to open a report with restricted access (e.g., a report with id=’1’), the handler will return the error 404:

``` js
dashboard.LoadDashboard('1') // Load a dashboard available only to Admin
```


```
GET http://localhost:65252/Authorization/Dashboards/DXDD.axd?action=DashboardAction/1&_=1525787741461 404 (Not Found)
```

You can copy the code from the example project and change user names and authorization logic based on your requirements.



### 3.3 Query Builder
Both the standalone Query Builder and the Query Builder integrated into the Report and Dashboard designers require you to restrict an end-user’s access to the available connections and data tables in code.
 
See the example project’s **Global.asax** file to see how these customizations are registered fort Reports, Dashboards and standalone Query Builder

To restrict access to connection strings, implement a custom connection string provider (see the example implementation):

``` cs
public class DataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider {

    public Dictionary<string, string> GetConnectionDescriptions() {
        Dictionary<string, string> connections =
            new Dictionary<string, string> { { "nwindConnection", "NWind database" } };

        // Customize the loaded connections list.  

        // here restrict access
        //if(GetIdentityName() == "Admin")
        //    connections.Add("secretConnection", "Admin only database");

        return connections;
    }

    public DataConnectionParametersBase GetDataConnectionParameters(string name) {
        return AppConfigHelper.LoadConnectionParameters(name);
    }
}
```

Implement a custom database schema provider to restrict access to data tables:

``` cs
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
```


The Dashboard Designer requires the database schema provider to be created by a factory object:

``` cs
public class DataSourceWizardDBSchemaProviderExFactory : DevExpress.DataAccess.Web.IDataSourceWizardDBSchemaProviderExFactory {
    public IDBSchemaProviderEx Create() {
        return new DBSchemaProviderEx();
    }
}
```

Register the implemented classes for the Report Designer, Dashboard Designer and standalone Query Builder as shown below.

**Reports:**
``` cs
DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider<DataSourceWizardConnectionStringsProvider>();
DefaultReportDesignerContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```

**Dashboards:**
``` cs
DefaultQueryBuilderContainer.Register<IDataSourceWizardConnectionStringsProvider, DataSourceWizardConnectionStringsProvider>();
DefaultQueryBuilderContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```


**Query Builder:**
``` cs
DashboardConfigurator.Default.SetConnectionStringsProvider(new DataSourceWizardConnectionStringsProvider());
DashboardConfigurator.Default.SetDBSchemaProvider(new DBSchemaProviderEx());
```

**See the example implementation.**










