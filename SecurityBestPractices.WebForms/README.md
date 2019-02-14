# ASP.NET WebForms Security Best Practices

This document provides information on some of the best practices developers should follow to avoid certain security breaches. Each section of this document describes a possible use-case scenario that exposes the security issue and the vulnerabilities associated with it, together with information on how to mitigate the security problems.

The security issues are all shown using a simple Visual Studio solution. Fully commented code samples are provided as part of that solution to show how to avoid each security breach. You will need to have DevExpress ASP.NET controls installed in order to load and compile the solution. You can download the installer from the [DevExpress website](https://devexpress.com).

---

- [1. Uploading Files](#1-uploading-files)
- [2. Uploading and Displaying Binary Images](#2-uploading-and-displaying-binary-images)
- [3. Authorization](#3-authorization)
- [4. Preventing Cross-Site Request Forgery (CSRF)](#4-preventing-cross-site-request-forgery-csrf)

## 1. Uploading Files

**Related Controls**: [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview), [ASPxUploadControl](https://documentation.devexpress.com/AspNet/4040/ASP-NET-WebForms-Controls/File-Management/File-Upload/Overview/ASPxUploadControl-Overview), [ASPxFileManager](https://documentation.devexpress.com/AspNet/9030/ASP-NET-WebForms-Controls/File-Management/File-Manager/Overview/ASPxFileManager-Overview), [ASPxHtmlEditor](https://documentation.devexpress.com/AspNet/4024/ASP-NET-WebForms-Controls/HTML-Editor), [ASPxRichEdit](https://documentation.devexpress.com/AspNet/17721/ASP-NET-WebForms-Controls/Rich-Text-Editor), [ASPxSpreadsheet](https://documentation.devexpress.com/AspNet/16157/ASP-NET-WebForms-Controls/Spreadsheet)

**Security Risks**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html), [CWE-434](https://cwe.mitre.org/data/definitions/434.html)
This section provides information on how to provide file upload capabilities within your web application. There are several separate scenarios to cover:

- [1.1. Stop Malicious Files Being Uploaded](#11-prevent-uploading-malicious-files)
- [1.2. Avoid Uncontrolled Resource Consumption](#12-prevent-uncontrolled-resource-consumption)
- [1.3. Protect Temporary Files](#13-protect-temporary-files)

### 1.1. Stop Malicious Files Being Uploaded

Visit the **[UploadingFiles\UploadControl.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControl.aspx.cs)** page for a full code sample.

Consider the situation where your web application allows for files to be uploaded. These files are then accessed using a specific URL (for example: _example.com/uploaded/uploaded-filename_).

The possible security breach here occurs when a malicious file is uploaded that can then be executed on the server side. For example, a malefactor could upload an ASPX file containing malicious code and guess at its URL. If the malefactor is correct and requests this URL, the file would be executed on the server as if it were part of the application.

To familiarize yourself with the issue:

1. Comment out the following lines to disable protection.

   [\UploadingFiles\UploadControl.aspx:](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControl.aspx)

   ```aspx
   <%-- <ValidationSettings AllowedFileExtensions =".jpg,.png">
       </ValidationSettings> --%>

   ```

   [\UploadingFiles\UploadControl.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControl.aspx.cs):

   ```cs
   using (var stream = file.FileContent) {
   // In case additional checks are needed perform them here before saving the file
   //if (!IsValidImage(stream)) {
   //    file.IsValid = false;
   //    e.ErrorText = "Validation failed!";
   //}
   //else {
       string fileName = string.Format("{0}{1}", MapPath("~/UploadingFiles/Images/"), file.FileName);
       file.SaveAs(fileName, true);
   //}
   //}
   ```

2. Run the solution and open the **UploadingFiles/UploadControl.aspx** page.
3. Upload the **\App_Data\TestData\Malicious.aspx** file.
4. Execute the uploaded file on the server by visiting the following URL: **/UploadingFiles/Images/Malicious.aspx**

To mitigate this vulnerability, do the following:

1. Initialize the [AllowedFileExtensions](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_AllowedFileExtensionstopic) setting with a list of allowed file extensions. The server will then validate the type of the uploaded file:

```aspx
<ValidationSettings AllowedFileExtensions=".jpg, .png">
</ValidationSettings>
```

2. Disable file execution in the upload folder ([relevant StackOverflow question](https://stackoverflow.com/questions/3776847/how-to-restrict-folder-access-in-asp-net)):

```aspx
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

The default file extensions allowed by various controls that allow for file uploads:

| Control               | Allowed Extensions                                                    |
| --------------------- | --------------------------------------------------------------------- |
| **ASPxUploadControl** | _any_                                                                 |
| **ASPxBinaryImage**   | _any_                                                                 |
| **ASPxFileManager**   | _any_                                                                 |
| **ASPxHtmlEditor**    | .jpe, .jpeg, .jpg, .gif, .png <br> .mp3, .ogg <br> .swf <br> .mp4     |
| **ASPxRichEdit**      | .doc, .docx, .epub, .html, .htm, .mht, .mhtml, .odt, .txt, .rtf, .xml |
| **ASPxSpreadsheet**   | .xlsx, .xlsm, .xls, .xltx, .xltm, .xlt, .txt, .csv                    |

### 1.2. Prevent Uncontrolled Resource Consumption

Visit the **[UploadingFiles/UploadControlMemory.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControlMemory.aspx.cs)** page for a full code sample.

Consider the situation where the web application allows files of any size to be uploaded.

The possible security breach here occurs when a malefactor performs a denial of service ([DoS](https://cwe.mitre.org/data/definitions/400.html)) attack by uploading very large files, thereby using up server memory and disk space.

To mitigate this vulnerability:

1. Access file contents using the [FileContent](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileContenttopic) property (a Stream) rather than the [FileBytes](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileBytestopic) property (a byte array). This will avoid memory overflow and other issues when processing large uploaded files.

```cs
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

2. Specify the maximum size for uploaded files using the [UploadControlValidationSettings.MaxFileSize](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_MaxFileSizetopic) property.

Note that in the **Advanced** uploading mode, files are loaded in small fragments (200KB by default), thus setting the **httpRuntime**>**maxRequestLength** and **requestLimits**>**maxAllowedContentLength** options in **web.config** is not sufficient to prevent attacks.

See the [Uploading Large Files](https://documentation.devexpress.com/AspNet/9822/ASP-NET-WebForms-Controls/File-Management/File-Upload/Concepts/Uploading-Large-Files) documentation topic for more information.

The following controls limit the maximum size for uploaded files by default:

- The **ASPxHtmlEditor** defines a 31,457,280 byte limit for uploaded file size.
- The **ASPxSpreadsheet** and **ASPxRichEdit** do not set a maximum size for an uploaded file, however there is a 31,457,280 byte limit for images to be inserted into a document. Note that both controls must be specifically enabled to allow for the uploading of files.

The File Manager control automatically allows files to be uploaded, and does not impose any limitations on the file size and extension. You can disable file uploading with this code:

```aspx
<ASPxFileManager ... >
    ...
    <SettingsUpload Enabled="false">
</ASPxFileManager>
```

Other operations on files organized by the File Manager (copy, delete, download, etc.) are configured using the [SettingsEditing](http://help.devexpress.com/#AspNet/DevExpressWebASPxFileManager_SettingsEditingtopic) property. All such operations are disabled by default.

### 1.3. Protect Temporary Files

Visit the **[UploadingFiles/UploadControlTempFileName.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControlTempFileName.aspx.cs)** page for a full code sample.

Consider the situation where you store temporary files on the server, prior to processing them on the server (for example, to extract the data within to write to a database).

To avoid a security breach, you will need to ensure that these files are inaccessible to third parties.

To mitigate this vulnerability:

1. Store temporary files in a folder unreachable by URL (for example, _App_Data_).
2. Use a dedicated file extension for temporary files on the server (for example \*".mytmp").
3. Consider assigning random file names using the [GetRandomFileName](<https://msdn.microsoft.com/en-us/library/system.io.path.getrandomfilename(v=vs.110).aspx>) method.

```cs
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

You can also define security permissions for folders and files accessible through the File Manager control. The [Access Rules](https://documentation.devexpress.com/AspNet/119542/ASP-NET-WebForms-Controls/File-Management/File-Manager/Concepts/Access-Control-Overview/Access-Rules) documentation topic has further information.

---

## 2. Uploading and Displaying Binary Images

**Related Controls**: [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview), [ASPxUploadControl](https://documentation.devexpress.com/AspNet/4040/ASP-NET-WebForms-Controls/File-Management/File-Upload/Overview/ASPxUploadControl-Overview)

**Security Risks**: [CWE-79](https://cwe.mitre.org/data/definitions/79.html)

Consider the situation where an image is uploaded to the server. The server generates a page that contains the image, and a user opens that page in their browser.

The possible security breach is this: a malefactor creates a file containing a malicious script. This file has an image file extension. The file is added to the generated page by the server, and that page is downloaded by the user's browser. This results in the malicious script being run in the browser. Essentially this is an example of XSS (Cross-site Scripting) via content-sniffing, a particular case of [CWE-79](https://cwe.mitre.org/data/definitions/79.html).

To familiarize yourself with the issue:

1. Comment out the call of the IsValidImage method in the example project's [UploadingBinaryImage/UploadControl.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx.cs) to disable protection:
   ```cs
   // if(!IsValidImage(stream)) return;
   ```
2. Run the example solution and open the **[UploadingBinaryImage/UploadControl.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx)** page.
3. Upload the **[\App_Data\TestData\Content-Sniffing-XSS.jpg](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/App_Data/TestData/Content-Sniffing-XSS.jpg)** file. This is nominally a JPEG image, but in fact is a JavaScript file that emulates a malicious script.
4. Open the **[UploadingBinaryImage/BinaryImageViewer.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/BinaryImageViewer.aspx)** page. As with every ASPX request, the markup is generated by the server on request, and the uploaded file will be added to the [code behind](https://github.com/DevExpress/aspnet-security-bestpractices/blob/fd40850d01330a3d16f1a5a8c3cfd80cbe831c60/SecurityBestPractices/UploadingBinaryImages/BinaryImageViewer.aspx.cs#L17-L18).
5. The JavaScript code from the uploaded file is executed by the browser:

![malicious-image](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/uploading-binary-image-1.png?raw=true)

To mitigate this vulnerability:

1. Programmatically check whether the uploaded file is really an image before saving it to any server-side storage (see the [IsValidImage](https://github.com/DevExpress/aspnet-security-bestpractices/blob/fd40850d01330a3d16f1a5a8c3cfd80cbe831c60/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx.cs#L22-L31) method implementation).

```cs
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

2. Use the [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview) control for uploading images. This control automatically implements an image file type check.

\[Aspx\]

```aspx
<dx:ASPxBinaryImage ID="ASPxBinaryImage1" runat="server">
    <EditingSettings Enabled="True">
    </EditingSettings>
</dx:ASPxBinaryImage>
<dx:ASPxButton ID="ASPxButton1" runat="server" OnClick="ASPxButton1_Click" Text="Save">
</dx:ASPxButton>
```

\[C#\]

```cs
protected void ASPxButton1_Click(object sender, EventArgs e) {
    byte[] contentBytes = ASPxBinaryImage1.ContentBytes;
    // Here contentBytes should be saved to a database

    // For demonstration purposes, content is saved to a file
    string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
    File.WriteAllBytes(fileName, contentBytes);
}

```

Another strong recommendation is to _always_ specify the exact content type when you add binary data to the response:
**Correct:** `Response.ContentType = "image/jpeg"`;
**Potential security breach:** `Response.ContentType = "image"`.

Additionally, it is a good practice to add the `X-CONTENT-TYPE-OPTIONS="nosniff"` response header:

```cs
protected void ASPxButton1_Click(object sender, EventArgs e) {
    Response.Headers.Add("X-Content-Type-Options", "nosniff");
}
```

### Notes:

1. Microsoft Edge automatically detects a file's type based on its content, which prevents the execution of malicious scripts as described here.
2. Make sure to specify the maximum uploaded file size to prevent those DoS attacks based on uploading large files.

---

## 3. Authorization

This section provides information on using DevExpress controls in web applications that implement authorization and access control. The following features are considered:

- [3.1. Reporting](#31-reporting)
- [3.2. Dashboard](#32-dashboard)
- [3.3. Query Builder](#33-query-builder)

### 3.1. Reporting

Normally, when you create a reporting application with access restrictions using one of the standard Microsoft mechanisms, you grant or restrict access to particular pages based on a user's identity:

```aspx
  <location path="Authorization/Reports">
    <system.web>
      <authorization>
        <deny users="?" />
      </authorization>
    </system.web>
  </location>
```

However, please note that, by restricting access to certain pages that contain the [Document Viewer](https://documentation.devexpress.com/XtraReports/17738/Creating-End-User-Reporting-Applications/Web-Reporting/Document-Viewer/HTML5-Document-Viewer) control, that access restriction is not automatically passed on to the report files that these pages might display. These files can still be accessed by the Document Viewer control from other pages through the client-side API. If a malefactor knows (or guesses) a report ID, they can open it by calling the client-side [OpenReport](http://help.devexpress.com/#XtraReports/DevExpressXtraReportsWebScriptsASPxClientWebDocumentViewer_OpenReporttopic) method:

```js
documentViewer.OpenReport("ReportID");
```

To mitigate this particular vulnerability, you should define your authorization rules in the server code. This is implemented by creating a custom report storage derived from the [ReportStorageWebExtension](http://help.devexpress.com/#XtraReports/clsDevExpressXtraReportsWebExtensionsReportStorageWebExtensiontopic) class. As a starting point, you can copy the reference implementation of such a storage class from the example project's [ReportStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs) file to your application and fine-tune it for your needs. The following customizations would have to be considered:

#### A. Viewing Reports

In the sample project, the [GetViewableReportDisplayNamesForCurrentUser](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L25-L38) method returns a list of reports that can be viewed by the currently logged-in user:

```cs
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

This method is then called from the overridden [GetData](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L60-L70) method and other methods that need to interact with the report storage:

```cs
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

In the sample project, the [GetEditableReportNamesForCurrentUser](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L41-L53) method returns a list of reports that can be edited by the currently logged-in user:

```cs
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

This method is then called from the overridden [IsValidUrl](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L80-L83) method and other methods that need to write report data.

```cs
public override bool IsValidUrl(string url) {
    var reportNames = GetEditableReportNamesForCurrentUser();
    return reportNames.Contains(url);
}
```

To prevent errors in the browser when handling unauthorized access attempts, check the access rights on the page's [PageLoad](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportDesignerPage.aspx.cs#L6-L13) event. If the user is not authorized to open the report, redirect them to a public page.

```cs
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

After implementing your custom report storage with the required access rules, you must register it in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Global.asax.cs) file:

```cs
DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageWithAccessRules());
```

#### Make Sure that Authentication Rules are Applied

In the example project, you can check whether the customization has had any effect by using the following steps:

- Open the [PublicReportPage.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/PublicPages/PublicReportPage.aspx) page without logging in. This page contains a Report Viewer.
- Try to open a report using the client API in the browser console. The example report has restricted access:

```
>documentViewer.OpenReport("Admin Report");
```

The browser console will respond with the following error.

![console-output](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authoriazation-reports-accessdenied.png?raw=true)

#### Provide an Operation Logger

The Web Document Viewer control maintains communication with the server side to obtain any additional document data when it is required, for example when a user switches pages or exports the document. Because of this feature, a user could navigate through a given report's pages even after having been logged out.

The possible security breach here occurs when a malefactor can forge a request that emulates page switching in order to get access to any protected information.

To mitigate this vulnerability, implement a custom Operation Logger in order to enforce fine-grained control over which operations are available to a user. Extend the [WebDocumentViewerOperationLogger](http://help.devexpress.com/#XtraReports/clsDevExpressXtraReportsWebWebDocumentViewerWebDocumentViewerOperationLoggertopic) class and override this class's methods to implement the required access control rules (see the [OperationLogger.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Reports/OperationLogger.cs) file of the example project).

Register the operation logger in [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/586084eda743711618c8d3e01d52736aac70a8c8/SecurityBestPractices.WebForms/SecurityBestPractices/Global.asax.cs#L25):

```cs
DefaultWebDocumentViewerContainer.Register<WebDocumentViewerOperationLogger, OperationLogger>();
```

Note that in order to simplify the example project the logger implementation obtains the required user account data from a static property. This is not a recommended solution: such an implementation could have difficulties running in a cloud environment or on a web farm. Instead, we recommend that you store authentication information in some appropriate data storage.

To familiarize yourself with the solution:

1. Run the example application, log in using the [/Login.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Login.aspx) page.
2. Open the report preview ([/Authorization/Reports/ReportViewerPage.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Reports/ReportViewerPage.aspx)) in a separate browser tab.
3. Log out.
4. Try switching report pages.

The following error will be signalled:

![Operation Logger Error](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authorization-reportopertaionlogger.png?raw=true)

#### Restrict Access to Data Connections and Data Tables

The [Report Designer](https://documentation.devexpress.com/XtraReports/17103/Creating-End-User-Reporting-Applications/Web-Reporting/Report-Designer) control allows a user to browse available data connection and data tables using the integrated [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder). Refer to the [Query Builder](#33-query-builder) subsection in this document to learn how to restrict access to this information, based on authorization rules.

### 3.2. Dashboard

The [DevExpress Web Dashboard](https://devexpress.github.io/dotnet-eud/dashboard-for-web/articles/index.html) can operate in one of two supported modes:

**1) Callbacks are processed by the ASPx page**

The ASPx page contains the ASPxDashboard control and the [UseDashboardConfigurator](http://help.devexpress.com/#Dashboard/DevExpressDashboardWebASPxDashboard_UseDashboardConfiguratortopic) property is set to false. This is the default mode for the dashbaord, and you can use the standard ASP.NET access restriction mechanisms:

```aspx
<location path="Authorization/Dashboards">
    <system.web>
        <authorization>
           <deny users="?" />
        </authorization>
    </system.web>
</location>
```

**2) Callbacks are processed by the Dashboard Configurator**

Here the callbacks are processed by the DevExpress HTTP handler, and the [UseDashboardConfigurator](http://help.devexpress.com/#Dashboard/DevExpressDashboardWebASPxDashboard_UseDashboardConfiguratortopic) property is set to true.

This is the recommended mode as it is considerably faster and much more flexible. In this mode though, any access restriction rules defined using the default mechanisms have no effect. Instead, the access control should be performed by a custom dashboard storage class that implements the [IEditableDashboardStorage](https://docs.devexpress.com/Dashboard/DevExpress.DashboardWeb.IEditableDashboardStorage?tabs=tabid-csharp%2Ctabid-T392813_7_52373613) interface.

As a starting point, you can copy the reference implementation of such a storage class from the example project's [DashboardStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Dashboards/DashboardStorageWithAccessRules.cs) file to your application and fine-tune it for your needs.

The [DashboardStorageWithAccessRules](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Dashboards/DashboardStorageWithAccessRules.cs#L12-L126) class implementation defines the access restrictions:

```cs
// Register dashboard layouts
var adminId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/AdminDashboard.xml")), "Admin Dashboard");
var johnId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(@"/App_Data/JohnDashboard.xml")), "John Dashboard");
this.publicDashboardId = AddDashboardCore(XDocument.Load(HttpContext.Current.Server.MapPath(publicDashboardPath)), "Public Dashboard");
```

The following code defines which user should have access to which dashboards:

```cs
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

Register the custom dashboard storage class in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Global.asax.cs) file as shown below.

```cs
DashboardConfigurator.Default.SetDashboardStorage(new DashboardStorageWithAccessRules());
DashboardConfigurator.Default.CustomParameters += (o, args) => {
if (!new DashboardStorageWithAccessRules().IsAuthorized(args.DashboardId))
    throw new UnauthorizedAccessException();
};
```

With this custom implementation of a dashboard storage, if a user named 'John' tries to use the client API to open a report with restricted access (e.g., a report with id='1'), the handler will return error 404, File Not Found:

```js
dashboard.LoadDashboard("1"); // Load a dashboard available only to Admin.
```

```
GET http://localhost:65252/Authorization/Dashboards/DXDD.axd?action=DashboardAction/1&_=1525787741461 404 (Not Found)
```

![console-output](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authorization-dashboard-404.png?raw=true)

#### Restrict Access to Data Connections and Data Tables

The Web Dashboard control allows a user to browse available data connection and data tables using the integrated [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder). Refer to the [Query Builder](#33-query-builder) subsection in this document to learn how to restrict access to this information based on authorization rules.

### 3.3. Query Builder

The standalone [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder) as well as the Query Builder integrated into the Report and Dashboard designers allows an end-user to browse a web application's data connections and the data tables available through those connections. In a web application that uses access control, you will to write code to restrict a user's access to the available connections and data tables.

To restrict access to connection strings, implement a custom connection string provider:

```cs
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

To restrict access to data tables, implement a custom database schema provider:

```cs
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

```cs
public class DataSourceWizardDBSchemaProviderExFactory : DevExpress.DataAccess.Web.IDataSourceWizardDBSchemaProviderExFactory {
    public IDBSchemaProviderEx Create() {
        return new DBSchemaProviderEx();
    }
}
```

As a starting point, you can copy the reference implementation from the example project's [DataSourceWizardConnectionStringsProvider.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/DataSourceWizardConnectionStringsProvider.cs) and [DataSourceWizardDBSchemaProviderExFactory.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/DataSourceWizardDBSchemaProviderExFactory.cs) files to your application and fine-tune it for your needs.

Register the implemented classes for the Report Designer, Dashboard Designer, or standalone Query Builder in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Global.asax.cs) file, as shown here:

**Report Designer:**

```cs
DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider<DataSourceWizardConnectionStringsProvider>();
DefaultReportDesignerContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```

**Dashboard Designer:**

```cs
DefaultQueryBuilderContainer.Register<IDataSourceWizardConnectionStringsProvider, DataSourceWizardConnectionStringsProvider>();
DefaultQueryBuilderContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```

**Query Builder:**

```cs
DashboardConfigurator.Default.SetConnectionStringsProvider(new DataSourceWizardConnectionStringsProvider());
DashboardConfigurator.Default.SetDBSchemaProvider(new DBSchemaProviderEx());
```

---

## 4. Preventing Cross-Site Request Forgery (CSRF)

**Related Controls**: Controls with data editing available by default (e.g., [ASPxGridView](http://help.devexpress.com/#AspNet/clsDevExpressWebASPxGridViewtopic), [ASPxCardView](http://help.devexpress.com/#AspNet/clsDevExpressWebASPxCardViewtopic), [ASPxVerticalGrid](http://help.devexpress.com/#AspNet/clsDevExpressWebASPxVerticalGridtopic), [ASPxTreeList](http://help.devexpress.com/#AspNet/clsDevExpressWebASPxTreeListASPxTreeListtopic), etc.)

**Security Risks**: [CWE-352](https://cwe.mitre.org/data/definitions/352.html)

This section provides information on how to prevent cross-site request forgery (CSRF) attacks on your web application. The vulnerability affects those controls that support data editing through AJAX. Although there are authorization mechanisms that allow you to deny access by Insecure Direct Object References (for example: _example.com/app/SecureReport.aspx?id=1_), they do not protect you from CSRF attacks.

The possible security breach could occur as follows:

1. A malefactor implements a phishing page.
2. A user inadvertently visits this phishing page, which then sends a malicious request to your web application using the user's cookies.
3. As a result, the malicious action is performed on the user's behalf, allowing the malefactor to access or modify the user's data or account info.

For more information on the vulnerability, refer to the [CWE-352 - Cross-Site Request Forgery (CSRF)](https://cwe.mitre.org/data/definitions/352.html) article.

To mitigate the vulnerability, use the **AntiForgeryToken** pattern. Refer to the [AntiForgery.Validate](<https://msdn.microsoft.com/ru-ru/library/gg548011(v=vs.111).aspx>) MSDN article to learn more.

The best practice to prevent CSRF is to create a MasterPage that:

1. Generates an AntiForgery token, and
2. Checks this token within the Pre_Load event.

```xml
<form id="form1" runat="server">
    <%= System.Web.Helpers.AntiForgery.GetHtml() %>
```

On the client, generate a token and a cookie signed with a machine key:

```html
<input
  name="__RequestVerificationToken"
  type="hidden"
  value="SKZi1uvLbg_G1P-KoK2AJdmeorX1fBgdCbVhLUDim9sk6AFwReVEY6XsuPrvsXJLq5MWOVaGXMnpx09srXkLM_Yjtcfg_4tpc1747jOgo941"
/>
```

On the server, check the cookie and token in the Validate method:

```cs
protected override void OnInit(EventArgs e) {
    base.OnInit(e);
    Page.PreLoad += OnPreLoad;
}
protected void OnPreLoad(object sender, EventArgs e) {
    if (IsPostBack)
        System.Web.Helpers.AntiForgery.Validate();
}
```

If the validation fails, the MasterPage will generate an error:

![AntiForgeryError](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/anti-forgery-error.png?raw=true)

The sample project illustrates how to ensure your web application's security in two use-case scenarios:

### Preventing Anauthorized CRUD Operations

In this scenario, an attack attempts to perform a CRUD operation on the server side by emulating a request from a data aware control (an [ASPxGridView](http://help.devexpress.com/#AspNet/clsDevExpressWebASPxGridViewtopic) in the example).

![AntiForgeryGrid](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/anti-forgery-grid.png?raw=true)

### Preventing Anauthorized Changes to User Account Information

In this scenario, an attack attempts to modify the user account information (the email address in the example).

![AntiForgeryEmail](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/anti-forgery-email.png?raw=true)

**See Also:** [Stack Overflow - preventing cross-site request forgery (csrf) attacks in asp.net web forms](https://stackoverflow.com/questions/29939566/preventing-cross-site-request-forgery-csrf-attacks-in-asp-net-web-forms)

---

![Analytics](https://ga-beacon.appspot.com/UA-129603086-1/aspnet-security-bestpractices-webforms-page?pixel)
