# ASP.NET Web Forms Security Best Practices

This article details security-related best practices for the ASP.NET Web Forms platform.   

Each section within this document includes a specific security-related use-case scenario, with possible mitigation strategies.  

The Visual Studio solutions herein include fully commented code samples. You will need to have DevExpress ASP.NET controls installed to load and compile each solution. If you are new to DevExpress, you can download our ASP.NET product installer from the [DevExpress website](https://devexpress.com).

---

- [1. Uploading Files](#1-uploading-files)
- [2. Uploading and Displaying Binary Images](#2-uploading-and-displaying-binary-images)
- [3. Authorization](#3-authorization)
- [4. Preventing Cross-Site Request Forgery (CSRF)](#4-preventing-cross-site-request-forgery-csrf)
- [5. Preventing Sensitive Information Exposure](#5-preventing-sensitive-information-exposure)
- [6. Preventing Cross-Site Scripting (XSS) Attacks with Encoding](#6-preventing-cross-site-scripting-xss-attacks-with-encoding)
- [7. User Input Validation](#7-user-input-validation)
- [8. Export to CSV](#8-export-to-csv)
- [9. Unauthorized Operations on Server Through Client API](#9-unauthorized-operations-on-the-server-through-client-api)
- [10. Downloading Files From External URLs](#10-downloading-files-from-external-urls)

## 1. Uploading Files

**Related Controls**: [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview), [ASPxUploadControl](https://documentation.devexpress.com/AspNet/4040/ASP-NET-WebForms-Controls/File-Management/File-Upload/Overview/ASPxUploadControl-Overview), [ASPxFileManager](https://documentation.devexpress.com/AspNet/9030/ASP-NET-WebForms-Controls/File-Management/File-Manager/Overview/ASPxFileManager-Overview), [ASPxHtmlEditor](https://documentation.devexpress.com/AspNet/4024/ASP-NET-WebForms-Controls/HTML-Editor), [ASPxRichEdit](https://documentation.devexpress.com/AspNet/17721/ASP-NET-WebForms-Controls/Rich-Text-Editor), [ASPxSpreadsheet](https://documentation.devexpress.com/AspNet/16157/ASP-NET-WebForms-Controls/Spreadsheet)

**Security Risks**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html), [CWE-434](https://cwe.mitre.org/data/definitions/434.html)

This section covers secure file upload operations and explores the following usage scenarios:

- [1.1. Stop Malicious Files from Being Uploaded](#11-stop-malicious-files-from-being-uploaded)
- [1.2. Avoid Uncontrolled Resource Consumption](#12-prevent-uncontrolled-resource-consumption)
- [1.3. Protect Temporary Files](#13-protect-temporary-files)

### 1.1. Stop Malicious Files from Being Uploaded

> Visit **[UploadingFiles\UploadControl.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControl.aspx.cs)** for a full code sample.

Consider a web application that allows users to upload files. In this scenario, a user can use a URL to access the uploaded files (for example: `example.com/uploaded/uploaded-filename`).

A security risk can be introduced if users are allowed to upload malicious files – files that can be executed on the server side. For example, a threat actor could upload an ASPX file that contains malicious code and guess its URL. If the threat actor is correct and requests this URL, the file would be executed on the server as if it were part of the application. 

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
   // If additional checks are needed, execute them here before saving the file
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

4. Visit the following URL to execute the uploaded file on the server: **/UploadingFiles/Images/Malicious.aspx**

To mitigate this risk, you must:

1. Initialize the [`AllowedFileExtensions`](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_AllowedFileExtensionstopic) setting with a list of allowed file extensions. The server will then validate uploaded file type:

```aspx
<ValidationSettings AllowedFileExtensions=".jpg, .png">
</ValidationSettings>
```

2. Disable file execution within the upload folder ([see relevant StackOverflow question](https://stackoverflow.com/questions/3776847/how-to-restrict-folder-access-in-asp-net)):

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

The table below lists default file extensions allowed by various UI controls that support file upload operations:

| Control               | Allowed Extensions                                                    |
| --------------------- | --------------------------------------------------------------------- |
| **ASPxUploadControl** | _any_                                                                 |
| **ASPxBinaryImage**   | _any_                                                                 |
| **ASPxFileManager**   | _any_                                                                 |
| **ASPxHtmlEditor**    | .jpe, .jpeg, .jpg, .gif, .png <br> .mp3, .ogg <br> .swf <br> .mp4     |
| **ASPxRichEdit**      | .doc, .docx, .epub, .html, .htm, .mht, .mhtml, .odt, .txt, .rtf, .xml |
| **ASPxSpreadsheet**   | .xlsx, .xlsm, .xls, .xltx, .xltm, .xlt, .txt, .csv                    |

### 1.2. Prevent Uncontrolled Resource Consumption

#### 1.2.1 Prevent Uncontrolled Memory Consumption

> Visit **[UploadingFiles/UploadControlMemory.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControlMemory.aspx.cs)** for a full code sample.

Consider a web application that allows files - of any size - to be uploaded to a server. 

In this scenario, a threat actor can upload very large files to consume server memory and disk space - a form of Denial of Service  ([DoS](https://cwe.mitre.org/data/definitions/400.html)) attack.

To mitigate this vulnerability:

1. Use the [`FileContent`](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileContenttopic) property (a Stream) rather than the [`FileBytes`](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileBytestopic) property (a byte array) to access file contents. This prevents memory overflows and other issues when the server processes large files.

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

2. Use the [`UploadControlValidationSettings.MaxFileSize`](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_MaxFileSizetopic) property to specify the maximum size for uploaded files.

> Note: In the Advanced uploading mode, files are loaded in small fragments (200KB by default). As such, setting the `httpRuntime`>`maxRequestLength` and `requestLimits`>`maxAllowedContentLength` options in *web.config* is not sufficient to prevent attacks.

See the [Uploading Large Files](https://documentation.devexpress.com/AspNet/9822/ASP-NET-WebForms-Controls/File-Management/File-Upload/Concepts/Uploading-Large-Files) documentation topic for more information in this regard.

The following controls limit the maximum size for upload operations by default:

- The **ASPxHtmlEditor** defines a 31,457,280 byte limit for uploaded file size.
- The **ASPxSpreadsheet** and **ASPxRichEdit** do not set a maximum size for an uploaded file, however there is a 31,457,280 byte limit for images to be inserted into a document. Note: Both controls must be explicitly configured to accept file uploads.

The File Manager control automatically allows files to be uploaded and does not impose any limitations on file size and extension. You can disable file upload operations with this code:

```aspx
<ASPxFileManager ... >
    ...
    <SettingsUpload Enabled="false">
</ASPxFileManager>
```

Other file-related operations managed by the File Manager (copy, delete, download, etc.) are configured through the [`SettingsEditing`](http://help.devexpress.com/#AspNet/DevExpressWebASPxFileManager_SettingsEditingtopic) property. All such operations are disabled by default.

#### 1.2.2 Prevent Uncontrolled Disk Space Consumption

**Security Risks**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html)

You should always monitor the total file size uploaded by end-users, otherwise a threat actor can perform a DoS attack by uploading too many files and consuming available disk space. You should always set a limit on the total size of uploaded files. 

To restrict upload file size, check the upload directory's size in the [Upload Control](https://docs.devexpress.com/AspNet/8298/aspnet-webforms-controls/file-management/file-upload)'s [FilesUploadComplete](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxUploadControl.FilesUploadComplete) event handler:

```cs
protected void uploadControl_FilesUploadComplete(object sender, DevExpress.Web.FilesUploadCompleteEventArgs e) {
    if(uploadControl.UploadedFiles != null && uploadControl.UploadedFiles.Length > 0) {
        for(int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
            UploadedFile file = uploadControl.UploadedFiles[i];
            if(file.IsValid && file.FileName != "") {
                // Check the upload folder's size taking into account the new files.
                const long DirectoryFileSizesLimit = 10000000; // bytes
                long totalFilesSize = GetDirectoryFileSizes(MapPath("~/UploadingFiles/Images/"));
                if(file.ContentLength + totalFilesSize > DirectoryFileSizesLimit) {
                    file.IsValid = false;
                    e.ErrorText = "Total files size exceeded!";
                } else {
    ...

```

For more information, refer to the following project: [UploadingFiles/LimitDirectorySize.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/LimitDirectorySize.aspx.cs)

### 1.3. Protect Temporary Files

> Visit **[UploadingFiles/UploadControlTempFileName.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingFiles/UploadControlTempFileName.aspx.cs)** for a full code sample.

Consider an app that saves data to temporary files on the server before processing or writing results to a database.  

To avoid security issues, you need to ensure that these files are inaccessible to third parties. 

To mitigate this vulnerability: 

1. Store temporary files in a folder unreachable via URL (for example, App_Data).
2. Use a dedicated file extension for temporary files on the server (for example, `"\*.mytmp"`).
3. Consider using the [`GetRandomFileName`](<https://msdn.microsoft.com/en-us/library/system.io.path.getrandomfilename(v=vs.110).aspx>) method to assign random file names.

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
            }
        }
    }
}

```

You can also define security permissions for folders and files accessible through the File Manager control. For more information, refer to the following help topic: [Access Rules](https://documentation.devexpress.com/AspNet/119542/ASP-NET-WebForms-Controls/File-Management/File-Manager/Concepts/Access-Control-Overview/Access-Rules)

---

## 2. Uploading and Displaying Binary Images

**Related Controls**: [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview), [ASPxUploadControl](https://documentation.devexpress.com/AspNet/4040/ASP-NET-WebForms-Controls/File-Management/File-Upload/Overview/ASPxUploadControl-Overview)

**Security Risks**: [CWE-79](https://cwe.mitre.org/data/definitions/79.html)

Consider the situation where an image is uploaded to the server. The server generates a page that contains the image, and a user opens that page in their browser.

The possible security risk is as follows: a threat actor creates a file containing a malicious script. This file includes an image file extension. The file is added to the generated page by the server, and the page is downloaded by the user's browser. In this scenario, the malicious script is executed in the browser. Essentially, this is an example of XSS (Cross-site Scripting) via content-sniffing or [CWE-79](https://cwe.mitre.org/data/definitions/79.html).

> Note that newer versions of Google Chrome include embedded mechanisms to counteract this vulnerability. As of the original publication of this article, you could reproduce this vulnerability within the Mozilla Firefox browser.

To familiarize yourself with the issue

1. Disable protection: Un-comment the call to the `IsValidImage` method in the following sample project: [UploadingBinaryImage/UploadControl.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx.cs)
   ```cs
   // if(!IsValidImage(stream)) return;
   ```
2. In the [BinaryImageViewer.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/BinaryImageViewer.aspx.cs) file, un-comment `Response.ContentType = "image"` line and comment out `Response.ContentType = "image/jpeg"`.
3. Run the example solution and open the **[UploadingBinaryImage/UploadControl.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx)** page.
4. Upload the **[\App_Data\TestData\Content-Sniffing-XSS.jpg](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/App_Data/TestData/Content-Sniffing-XSS.jpg)** file. Though labeled a JPEG image, this is a JavaScript file designed to emulate a malicious script.
5. Open the **[UploadingBinaryImage/BinaryImageViewer.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/BinaryImageViewer.aspx)** page. Like any ASPX request, the markup is generated by the server on request, and the uploaded file is added to [code behind](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UploadingBinaryImages/BinaryImageViewer.aspx.cs).
6. The JavaScript code from the uploaded file is executed by the browser:

![malicious-image](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/uploading-binary-image-1.png?raw=true)

To mitigate this vulnerability:

1. Programmatically check whether the uploaded file is actually an image before you save it to server-side storage (see the [`IsValidImage`](https://github.com/DevExpress/aspnet-security-bestpractices/blob/fd40850d01330a3d16f1a5a8c3cfd80cbe831c60/SecurityBestPractices/UploadingBinaryImages/UploadControl.aspx.cs#L22-L31) method implementation).

```cs
protected void ASPxUploadControl1_FileUploadComplete(object sender,
  DevExpress.Web.FileUploadCompleteEventArgs e) {
    // Save contentBytes to a database here
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

2. Use the [ASPxBinaryImage](https://documentation.devexpress.com/AspNet/11624/ASP-NET-WebForms-Controls/Data-Editors/Editor-Types/ASPxBinaryImage/Overview/ASPxBinaryImage-Overview) control to upload images. This control automatically checks image file type.

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
    // Save contentBytes to a database here

    // For demonstration purposes, content is saved to a file
    string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
    File.WriteAllBytes(fileName, contentBytes);
}

```

We highly recommend to _always_ specify exact content type when you add binary data to the response:

**Correct:** `Response.ContentType = "image/jpeg"`;

**Potential security breach:** `Response.ContentType = "image"`.

Additionally, it is a good practice to add the `X-CONTENT-TYPE-OPTIONS="nosniff"` response header:

```cs
protected void ASPxButton1_Click(object sender, EventArgs e) {
    Response.Headers.Add("X-Content-Type-Options", "nosniff");
}
```

### Notes:

1. Microsoft Edge automatically detects file type based on its content, which prevents the execution of malicious scripts described herein.
2. Make certain to specify max upload file size to prevent DoS attacks via large file uploads.

---

## 3. Authorization

This section includes information on the use of DevExpress UI controls within web applications that include authorization and access control. The following products/features are considered:

- [3.1. Reporting](#31-reporting)
- [3.2. Dashboard](#32-dashboard)
- [3.3. Query Builder](#33-query-builder)

### 3.1. Reporting

To implement authorization logic for the Document Viewer, add a custom report storage derived from the [ReportStorageWebExtension](http://help.devexpress.com/#XtraReports/clsDevExpressXtraReportsWebExtensionsReportStorageWebExtensiontopic) class. As a starting point, you can copy the reference implementation of such a storage class from [ReportStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs) to your project and fine-tune it as needs dictate. The following customizations must be considered:

#### A. Viewing Reports

In our sample project, the [`GetViewableReportDisplayNamesForCurrentUser`](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L25-L38) method returns a list of reports that can be viewed by the current user (logged-in user):

```cs
// Logic used to obtain reports
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

This method is then called from the overridden [`GetData`](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L60-L70) method and other methods that need to interact with the report storage:

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

In our sample project, the [`GetEditableReportNamesForCurrentUser`](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L41-L53) method returns a list of reports that can be edited by the current user (logged-in user):

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

This method is then called from the overridden [`IsValidUrl`](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs#L80-L83) method and other methods that need to write report data.

```cs
public override bool IsValidUrl(string url) {
    var reportNames = GetEditableReportNamesForCurrentUser();
    return reportNames.Contains(url);
}
```

To prevent errors in the browser when you handle unauthorized access attempts, check access rights on the page's [`PageLoad`](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Reports/ReportDesignerPage.aspx.cs#L6-L13) event. If the user is not authorized to open the report, redirect them to a public page.

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

#### Register Custom Report Storage

Once you implement your custom report storage with required access rules, you must register it in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Global.asax.cs) file:

```cs
DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageWithAccessRules());
```

#### Make Certain that Authentication Rules are Applied

In our example project, follow the steps below to determine whether the customization has had any effect: 

- Open the [PublicReportPage.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/PublicPages/PublicReportPage.aspx) page without logging in. This page contains a Report Viewer.
- Try to use the client API in the browser console to open a report. The example report has restricted access:

```
>documentViewer.OpenReport("Admin Report");
```

The browser console will respond with the following error.

![console-output](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authoriazation-reports-accessdenied.png?raw=true)

#### Implement an Operation Logger

The Web Document Viewer control maintains communication with the server side to obtain any additional document data when required (for example, when a user switches pages or exports the document). Because of this feature, a user could navigate through an individual report pages even after logging out.

Security issue: A threat actor can forge a request that emulates page switching to obtain access to protected information. 

To mitigate this vulnerability, implement a custom Operation Logger to enforce control over which operations are available to a user. Extend the [WebDocumentViewerOperationLogger](http://help.devexpress.com/#XtraReports/clsDevExpressXtraReportsWebWebDocumentViewerWebDocumentViewerOperationLoggertopic) class and override class methods to implement necessary access control rules (review [OperationLogger.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Reports/OperationLogger.cs) in the example project).

Register the operation logger in [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/586084eda743711618c8d3e01d52736aac70a8c8/SecurityBestPractices.WebForms/SecurityBestPractices/Global.asax.cs#L25):

```cs
DefaultWebDocumentViewerContainer.Register<WebDocumentViewerOperationLogger, OperationLogger>();
```

> Note: To simplify the example project, our logger implementation obtains necessary user account data from a static property. This is not a recommended solution: such an implementation could fail in a cloud environment or on a web farm. Instead, we recommend that you store authentication information in an appropriate data storage. 

To familiarize yourself with the solution:

1. Run the example application and log in via [/Login.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Login.aspx).
2. Open the report preview ([/Authorization/Reports/ReportViewerPage.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Reports/ReportViewerPage.aspx)) in a separate browser tab.
3. Log out.
4. Try switching report pages.

The following error will appear on screen:

![Operation Logger Error](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authorization-reportopertaionlogger.png?raw=true)

#### Restrict Access to Data Connections and Data Tables

Our [Report Designer](https://documentation.devexpress.com/XtraReports/17103/Creating-End-User-Reporting-Applications/Web-Reporting/Report-Designer) component allows a user to browse available data connections and data tables within the Designer’s integrated [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder) UI. Refer to the [Query Builder](#33-query-builder) subsection of this document to learn how you can restrict access to this information based on internal authorization rules.

### 3.2. Dashboard

The [DevExpress Web Dashboard](https://devexpress.github.io/dotnet-eud/dashboard-for-web/articles/index.html) can be used in one of the following two modes:

**1) Callbacks are processed by the ASPx page**

The ASPx page includes the ASPxDashboard control and the [UseDashboardConfigurator](http://help.devexpress.com/#Dashboard/DevExpressDashboardWebASPxDashboard_UseDashboardConfiguratortopic) property is set to `false` (default mode). You can use standard ASP.NET access restriction mechanisms within your app:

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

In this mode, callbacks are processed by the DevExpress HTTP handler, and the [UseDashboardConfigurator](http://help.devexpress.com/#Dashboard/DevExpressDashboardWebASPxDashboard_UseDashboardConfiguratortopic) property is set to `true`.

When the Dashboard is used in this manner, access restriction rules enforced by default mechanisms have no impact. Instead, access control must be performed by a custom dashboard storage class that implements the [IEditableDashboardStorage](https://docs.devexpress.com/Dashboard/DevExpress.DashboardWeb.IEditableDashboardStorage?tabs=tabid-csharp%2Ctabid-T392813_7_52373613) interface.

As a starting point, you can copy the reference implementation of such a storage class from the [DashboardStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Dashboards/DashboardStorageWithAccessRules.cs) file to your application and fine-tune it to address your business needs.

The [DashboardStorageWithAccessRules](https://github.com/DevExpress/aspnet-security-bestpractices/blob/408c2328fc8d567281994b2bba52d0705850c0b5/SecurityBestPractices/Authorization/Dashboards/DashboardStorageWithAccessRules.cs#L12-L126) class implementation defines appropriate access restrictions:

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

With this custom dashboard storage implementation, if a user named 'John' tries to use the client API to open a dashboard with restricted access (e.g., a dashboard with id='1'), the handler returns `error 404, File Not Found`:

```js
dashboard.LoadDashboard("1"); // Load a dashboard available only to Admin.
```

```
GET http://localhost:65252/Authorization/Dashboards/DXDD.axd?action=DashboardAction/1&_=1525787741461 404 (Not Found)
```

![console-output](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authorization-dashboard-404.png?raw=true)

#### Restrict Access to Data Connections and Data Tables

Our Web Dashboard control allows a user to browse available data connections and data tables within the control’s integrated [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder) UI. Refer to the [Query Builder](#33-query-builder) subsection of this document to learn how to restrict access to this information based on internal authorization rules.

### 3.3. Query Builder

Our standalone [Query Builder](https://documentation.devexpress.com/AspNet/114930/ASP-NET-WebForms-Controls/Query-Builder) and the Query Builder integrated into DevExpress Report and Dashboard designers allows end-users to browse web application data connections and the data tables available through those connections. If your web application uses access control, you must implement custom logic to restrict user access to all available connections and data tables. 

To restrict access to connection strings, implement a custom connection string provider:

```cs
public class DataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider {

    public Dictionary<string, string> GetConnectionDescriptions() {
        Dictionary<string, string> connections =
            new Dictionary<string, string> { { "nwindConnection", "NWind database" } };

        // Customize loaded connections list.

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

As a starting point, you can copy the reference implementation from [DataSourceWizardConnectionStringsProvider.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/DataSourceWizardConnectionStringsProvider.cs) and [DataSourceWizardDBSchemaProviderExFactory.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/DataSourceWizardDBSchemaProviderExFactory.cs) to your application and fine-tune it to address your business needs.

Register the implemented classes for the Report Designer, Dashboard Designer, or standalone Query Builder in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Global.asax.cs) file, as shown below:

**Report Designer:**

```cs
DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider<DataSourceWizardConnectionStringsProvider>();
DefaultReportDesignerContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```

**Dashboard Designer:**

```cs
DashboardConfigurator.Default.SetConnectionStringsProvider(new DataSourceWizardConnectionStringsProvider());
DashboardConfigurator.Default.SetDBSchemaProvider(new DBSchemaProviderEx());

```

**Query Builder:**

```cs
DefaultQueryBuilderContainer.Register<IDataSourceWizardConnectionStringsProvider, DataSourceWizardConnectionStringsProvider>();
DefaultQueryBuilderContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
```

---

## 4. Preventing Cross-Site Request Forgery (CSRF)

**Related Controls**: Controls with data editing available by default (e.g., [ASPxGridView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView), [ASPxCardView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCardView), [ASPxVerticalGrid](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxVerticalGrid), [ASPxTreeList](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxTreeList.ASPxTreeList), [ASPxDiagram](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxDiagram.ASPxDiagram), etc.)

**Security Risks**: [CWE-352](https://cwe.mitre.org/data/definitions/352.html)

This section contains information on how to prevent cross-site request forgery (CSRF) attacks against your web application. The vulnerability affects applications that execute POST requests (which includes requests made by DevExpress AJAX-enabled controls such as the Grid View). Although authorization mechanisms allow you to deny access by Insecure Direct Object References (for example: `example.com/app/SecureReport.aspx?id=1`), they do not protect you from CSRF attacks.

The possible attacks could occur if:

1. A threat actor implements a phishing page.
2. A user inadvertently visits this phishing page, which then sends a malicious request to your web application with the user's cookie information.
3. As a result, malicious action is executed on the user's behalf and the threat actor can access or modify the user's private data or account information.

For more information on this vulnerability, refer to the following article: [CWE-352 - Cross-Site Request Forgery (CSRF)](https://cwe.mitre.org/data/definitions/352.html).

To protect against this vulnerability, use the **AntiForgeryToken** pattern. Refer to the following MSDN article to learn more: [AntiForgery.Validate](<https://msdn.microsoft.com/en-us/library/gg548011(v=vs.111).aspx>).

One option designed to prevent CSRF is to create a [MasterPage](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master#L13) which:

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

On the server, check the cookie and token within the [Validate method](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master.cs#L6-L13):

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

If validation fails, the MasterPage generates an error:

![AntiForgeryError](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/anti-forgery-error.png?raw=true)

The sample project illustrates how to maximize web application security for two specific use-case scenarios:

### Preventing Unauthorized CRUD Operations

In this scenario, an attack attempts to perform a CRUD operation on the server by emulating a request from a data aware control (an [ASPxGridView](http://help.devexpress.com/#AspNet/clsDevExpressWebASPxGridViewtopic) in the example).

![AntiForgeryGrid](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/anti-forgery-grid.png?raw=true)

Review our sample [UsingAntiForgeryToken/EditForm.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UsingAntiForgeryToken/EditForm.aspx) file to familiarize yourself with this particular vulnerability.

### Preventing Unauthorized Changes to User Account Information

In this scenario, an attack attempts to modify the user account information (the email address in the example).

![AntiForgeryEmail](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/anti-forgery-email.png?raw=true)

See the example project's [UsingAntiForgeryToken/EditForm.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/UsingAntiForgeryToken/EditForm.aspx) file to familiarize yourself with the vulnerability.

**See Also:** [Stack Overflow - preventing cross-site request forgery (csrf) attacks in asp.net web forms](https://stackoverflow.com/questions/29939566/preventing-cross-site-request-forgery-csrf-attacks-in-asp-net-web-forms)

---

## 5. Preventing Exposure of Sensitive Information

**Security Risks**: [CWE-209](https://cwe.mitre.org/data/definitions/209.html)

This section describes security vulnerabilities that can expose sensitive information to untrusted parties.

### 5.1 Information Exposure Through Error Messages

A breach or exposure can occur when the server generates an exception. If an application is configured incorrectly, detailed error information will be displayed to an end-user. This information can include information a threat actor can use to gain insight on an app’s infrastructure, file names, etc.

This behavior is controlled by the [customErrors](<https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/h0hfz6fc(v=vs.100)>) *web.config* option. This option accepts the following values:

- `RemoteOnly` (default) - In this mode, detailed errors are displayed only for connections from the local machine.
- `On` - Ensures that private messages are never displayed.
- `Off` - Forces private messages for all connections.

In the following image, sensitive information is displayed within an error message:

![Unsafe Error Message](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/error-message-exposed.png)

By using secure configuration, the error message is substituted by a more generic message:

![Safe Error Message](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/error-message-generic.png)

#### Manually Displaying Error Messages

You should never display exception messages (Exception.Message) in your application's UI because this text may contain sensitive information. For example, the following code sample is considered unsafe: 

```cs
protected void UpdateButton_Click(object sender, EventArgs e) {
    try {
        // DoSomething()
        throw new InvalidOperationException("Some sensitive information");
    } catch(Exception ex) {
        UpdateStatusLabel.Visible = true;
        UpdateStatusLabel.Text = ex.Message;
    }
}
```

Consider displaying custom error messages if you want to inform end-users about occurred errors:

```cs
protected void UpdateButton_Click(object sender, EventArgs e) {
    try {
        // DoSomething()
        throw new InvalidOperationException("Some sensitive information");
    } catch(Exception ex) {
        if(ex is InvalidOperationException)
            UpdateStatusLabel.Text = "Some error occured...";
        else
            UpdateStatusLabel.Text = "General error occured...";
    }
}
```

Refer to the sample [InformationExposure/ErrorMessage.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/InformationExposure/ErrorMessage.aspx.cs#L8) file to familiarize yourself with this issue.

### 5.2 Availability of Invisible Column Values Through the Client-Side APIs

#### Prevent Access to Hidden Column Data

This vulnerability is associated with grid-based controls. Consider an app wherein a control includes hidden columns bound to sensitive data (hidden data is not displayed on-screen and is only used on the server). A threat actor can still use the control's client API to request the value of such a column:

```js
gridView.GetRowValues(0, "UnitPrice", function (Value) {
  alert(Value);
});
```

Review our sample [ClientSideApi/GridView.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ClientSideApi/GridView.aspx#L46-L48) page for more information.

In the following image, the browser's console is used to access hidden column values

![Unprotected Column Values](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/access-hidden-columns-no-protection.png)

Set the `AllowReadUnexposedColumnsFromClientApi` property to `false` to disable this behavior:

```cs
AllowReadUnexposedColumnsFromClientApi = "False";
```

#### Prevent Access by Field Name

Another risk involves threat actors who attempt to obtain a row value for a data field for which there is no column in the control:

```js
gridView.GetRowValues(0, "GuidField", function (Value) {
  alert(Value);
});
```

This is controlled by the `AllowReadUnlistedFieldsFromClientApi` property and is disabled by default (safe configuration):

![Protected Column Values](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/access-hidden-columns-use-protection.png)

General recommendation: Always execute separate queries for data sources (data) displayed on-screen. These queries should never request data that should be kept secret. 

### 5.3 Information Exposure Through Source Code

**Security Risks**: [CWE-540](https://cwe.mitre.org/data/definitions/540.html), [CWE-615](https://cwe.mitre.org/data/definitions/615.html)

The DevExpress default HTTP handler (DXR.axd) serves static files including images, scripts and styles. We assume that these static files are intended for public access and do not expose sensitive information or server-side code. However, there are additional recommendations on custom scripts and styles:

- Do not hardcode any credentials in scripts.
- Consider obfuscating these files in the following instances:
  - The file contains code that should be protected as intellectual property.
  - File content can provide a threat actor information about the backend system, its architecture, and security vulnerabilities. 

For example, consider the following code:

```js
function GetSystemState() {
...
}
```

The minified version provides considerably less information about the backend system:

```js
function s1(){...}
```

---

## 6. Preventing Cross-Site Scripting (XSS) Attacks with Encoding

**Security Risks**: [CWE-80](https://cwe.mitre.org/data/definitions/80.html), [CWE-20](https://cwe.mitre.org/data/definitions/20.html)

Occurs when a web page is rendered based on content specified by an end user. If user input is not properly sanitized, the resulting page can be injected with a malicious script. 

It is strongly suggested that you always sanitize page content specified by a user. Note: You should be aware of what type of sanitization to use so it is both compatible with displayed content and offers the desired level of safety. For example, you can remove HTML tags throughout content but it can corrupt text that is intended to contain code samples. A more generic approach would be to substitute all '<', '>' and '&' symbols with `&lt;`, `&gt;` and `&amp;` character codes.

Microsoft supplies a standard [HttpUtility](https://docs.microsoft.com/ru-ru/dotnet/api/system.web.httputility.htmlencode?view=netframework-4.7.2) class you can use to encode data in various use-case scenarios. It exposes the following methods:

| Method                | Usage                                                     |
| --------------------- | --------------------------------------------------------- |
| `HtmlEncode`          | Sanitizes untrusted input inserted into HTML output       |
| `HtmlAttributeEncode` | Sanitizes untrusted input assigned to a tag attribute     |
| `JavaScriptEncode`    | Sanitizes untrusted input used within a script            |
| `UrlEncode`           | Sanitizes untrusted input used to generate a URL          |

To safely insert user input value into markup, wrap it with a `HttpUtility.HtmlEncode` method call:

```cs
SearchResultLiteral.Text =
  string.Format("Your search - {0} - did not match any documents.", HttpUtility.HtmlEncode(SearchBox.Text));
```

Review our sample [HtmlEncoding/General.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/General.aspx.cs) file for more information.

Before you insert user input into a JavaScript block, you need to use the `HttpUtility.JavaScriptStringEncode` method to prepare the string:

```html
<script>
  var s = "<%= HttpUtility.JavaScriptStringEncode(SearchBox.Text) %>";
  // DoSomething(s);
</script>
```

Input text that contains unsafe symbols will be converted to a safe form. In this example, if a user specifies the following unsafe string...

```
"<b>'test'</b>
```

... the script will be rendered in the following manner:

```js
var s = "\"\u003cb\u003e'test'\u003c/b\u003e";
```

Review our sample [HtmlEncoding/General.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/General.aspx#L21-L22) file for more information.

### 6.1 Encoding in DevExpress Controls

By default, DevExpress controls encode displayed values obtained from a data source. Refer to the [HTML-Encoding](https://documentation.devexpress.com/AspNet/117902/Common-Concepts/HTML-Encoding) document for more information.

This behavior is specified by a control's `EncodeHtml` property. If a control displays a value that can be modified by an untrusted party, we recommend that you never disable this setting or sanitize displayed content manually.

To learn more about this vulnerability, review the sample [EncodeHtml.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/EncodeHtml.aspx.cs) page and uncomment the following line in [code behind](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/EncodeHtml.aspx.cs):

```cs
((GridViewDataTextColumn)GridView.Columns["ProductName"]).PropertiesEdit.EncodeHtml = false;
```

Launch the project and open the page in the browser. Data field content will be interpreted as a script and you will see an alert popup.

![Devexpress Controls - No Encoding](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/grid-columns-no-encoding.png?raw=true)

In safe configuration, field content would be interpreted as text and correctly displayed:

![Devexpress Controls - Use Encoding](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/grid-columns-use-encoding.png?raw=true)

#### 6.1.1 Encoding Header Filter Items

**Related Controls**: [ASPxGridView](https://docs.devexpress.com/AspNet/5823/aspnet-webforms-controls/grid-view), [ASPxCardView](https://docs.devexpress.com/AspNet/114048/aspnet-webforms-controls/card-view), [ASPxVerticalGrid](https://docs.devexpress.com/AspNet/116045/aspnet-webforms-controls/vertical-grid), [ASPxTreeList](https://docs.devexpress.com/AspNet/7928/aspnet-webforms-controls/tree-list)

You should always encode (wrap with a `HttpUtility.HtmlEncode` method call) filter items obtained from an unsafe data source or specified by an end user.

```cs
ASPxGridViewHeaderFilterEventArgs e) {
    if(e.Column.FieldName == "ProductName") {
        e.Values.Clear();
        // Adding custom values from an unsafe data source

        // Safe approach - Display Text is encoded
        e.AddValue(HttpUtility.HtmlEncode("<b>T</b>est <img src=1 onerror=alert('XSS') />"), "1");

        // Unsafe approach - Display Text is not encoded
        //e.AddValue("<b>T</b>est <img src=1 onerror=alert('XSS') />", "1");
    }
}
```

Review our sample [HtmlEncoding/EncodeHtml.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/EncodeHtml.aspx.cs#L18) file for more information.

If filter items are not encoded, an XSS can be executed when a user opens a header filter dropdown:

![Templates - Unsanitized Content](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/header-filter-item-encoding.png?raw=true)

### 6.2 Encoding in Templates

If you inject data field values in templates, we recommend that you always [encode the data field values](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/Templates.aspx#L17):

```xml
<asp:Label ID="ProductNameLabel" runat="server"
   Text='<%# System.Web.HttpUtility.HtmlEncode(Eval("ProductName")) %>' />
```

Inserting unsanitized content can expose your application to XSS attacks:

![Templates - Unsanitized Content](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/templates-no-encoding.png?raw=true)

With encoding, content would be interpreted as text and correctly displayed:

![Templates - Sanitized Content](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/templates-use-encoding.png?raw=true)

By default, DevExpress controls wrap templated contents with a `HttpUtility.HtmlEncode` method call.

### 6.3 Encoding Callback Data

When a client displays data received from the server via a callback, a breach may occur if this data has not been properly encoded. For example, in the [code below](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/Callback.aspx#L14-L19), such content is assigned to an element's `innerHTML`:

```xml
<dx:ASPxCallback runat="server" ID="CallbackControl" OnCallback="Callback_Callback" ClientInstanceName="callbackControl" >
    <ClientSideEvents CallbackComplete="function(s, e) {
        document.getElementById('namePlaceHodler').innerHTML = e.result;
            if(callbackControl.cpSomeInfo)
                document.getElementById('someInfo').innerHTML = callbackControl.cpSomeInfo;
        }" />
</dx:ASPxCallback>
```

The safe approach is to use `HtmlEncode` in [server-side code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/Callback.aspx.cs#L14):

```cs
protected void Callback_Callback(object source, DevExpress.Web.CallbackEventArgs e) {
    // Not secure
    // e.Result = "<img src=1 onerror=alert('XSS') /> ";
    // CallbackControl.JSProperties["cpSomeInfo"] = "<video src=1 onerror=alert(document.cookie)>";

    e.Result = HttpUtility.HtmlEncode("<img src=1 onerror=alert('XSS') /> ");
    CallbackControl.JSProperties["cpSomeInfo"] = HttpUtility.HtmlEncode("<video src=1 onerror=alert(document.cookie)>");
}
```

### 6.4 Encoding Page Title

When you assign a value from a database or user input to a page title, you need to make certain that the value is properly encoded to prevent possible script injections.

For example, the [code below](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/EncodePageTitle.aspx.cs#L13) assigns a value from a database to the page title.

```cs
protected void Page_Load(object sender, EventArgs e) {
    var ds = SqlDataSource1.Select(new System.Web.UI.DataSourceSelectArguments()) as DataView;
    var value = ds[0]["ProductName"]; // value from DB = "</title><script>alert('XSS')</script>";

    //Title = "Product: " + value.ToString(); // Not secure
    Title = "Product: " + HttpUtility.HtmlEncode(value).ToString(); // Secure
}

```

If encoding was not used, the resulting markup would contain a malicious script:

```html
<head><title>
    Product: </title><script>alert('XSS')</script>
</title></head>
```

With encoding, the markup is rendered as follows:

```html
<head>
  <title>
    Product: &lt;/title&gt;&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;
  </title>
</head>
```

### 6.5 Dangerous Links

It is potentially dangerous to render a hyperlink's HREF attribute based on a value from a database or user input.

DevExpress grid based controls remove all potentially dangerous contents (for example, `javascript:`) from HREF values when they render hyperlink columns:

![Unsafe Link](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/url-no-encoding.png?raw=true)

This behavior is controlled by a column's `RemovePotentiallyDangerousNavigateUrl` option (`true` by default):

```xml
<dx:GridViewDataHyperLinkColumn FieldName="Description" VisibleIndex="1">
   <PropertiesHyperLinkEdit RemovePotentiallyDangerousNavigateUrl="True” />
</dx:GridViewDataHyperLinkColumn>
```

We recommend that you never set this option to `false` if the URL value in the database can be modified by untrusted parties.

Review our sample [HtmlEncoding/DangerousNavigateUrl.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/HtmlEncoding/DangerousNavigateUrl.aspx#L20) page for more information.

---

## 7. User Input Validation

**Security Risks**: [CWE-20](https://cwe.mitre.org/data/definitions/20.html)

### 7.1 General Recommendations

You should always validate values obtained from an end user before you save them to a database or use in any other manner. Values should be validated on multiple levels:

1. Specify required input restrictions for on the client.

2. Validate submitted values on the server before save operations.

3. Specifies required data integrity conditions at the database level.

4. Validate values in the code that directly uses the value(s).

> Note that client validation is simply server load optimization. To ensure safety, always use client validation in conjunction with server validation.

![Validation Diagram](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/validation-diagram.png)

You can use control properties such as `MaxLength`, `MinValue`, `MaxValue`, and `Required` to specify input restrictions. Server side validation does not allow a user to submit an invalid value even if a malefactor manages to send an invalid value bypassing the client validation. If the value is invalid, the editor's value is set to the previous value assigned on the editor's `init`.

> Note: starting with our v19.2 release, the value set on `init` is also validated and cannot be saved if validation fails. In earlier versions, this value would be saved without validation if it was not modified on the client.

The image below demonstrates how validation errors are indicated by DevExpress controls:

![Validation Errors](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/webforms-input-validation.png?raw=true)

Review our sample [ValidateInput/General.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ValidateInput/General.aspx) file for more information.

### 7.2 Built-in Validation in DevExpress Controls

Some DevExpress web controls have built-in validation mechanisms. These mechanisms apply restrictions to input values when certain settings are specified. The table below lists controls with built-in validation support along with properties that control associated validation logic.

| Control                                                                                                     | Validation-Related Properties                                                                                                                                                                                                                                                                                                                                                   |
| ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [ASPxTextBox](https://docs.devexpress.com/AspNet/11586/aspnet-webforms-controls/data-editors/aspxtextbox)   | [MaxLength](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxTextBoxBase.MaxLength) </br> [MaskSettings.Mask](https://docs.devexpress.com/AspNet/DevExpress.Web.MaskSettings.Mask)                                                                                                                                                                                         |
| [ASPxSpinEdit](https://docs.devexpress.com/AspNet/11664/aspnet-webforms-controls/data-editors/aspxspinedit) | [MinValue](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxSpinEdit.MinValue) </br> [MaxValue](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxSpinEdit.MaxValue) </br> [MaxLength](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxTextBoxBase.MaxLength) </br> [MaskSettings.Mask](https://docs.devexpress.com/AspNet/DevExpress.Web.MaskSettings._members) |
| [ASPxCalendar](https://docs.devexpress.com/AspNet/11649/aspnet-webforms-controls/data-editors/aspxcalendar) | [MinDate](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCalendar.MinDate) </br> [MaxDate](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCalendar.MaxDate)                                                                                                                                                                                                       |
| [ASPxDateEdit](https://docs.devexpress.com/AspNet/11654/aspnet-webforms-controls/data-editors/aspxdateedit) | [DateRangeSettings](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxDateEdit.DateRangeSettings) </br> [MinDate](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxDateEdit.MinDate) </br> [MaxDate](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxDateEdit.MaxDate)                                                                                           |
| [ASPxListBox](https://docs.devexpress.com/AspNet/11660/aspnet-webforms-controls/data-editors/aspxlistbox)   | [DataSecurityMode](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxListBox.DataSecurityMode) (if set to **Strict**)                                                                                                                                                                                                                                                       |
| [ASPxComboBox](https://docs.devexpress.com/AspNet/11653/aspnet-webforms-controls/data-editors/aspxcombobox) | [DataSecurityMode](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxListBox.DataSecurityMode) (if set to **Strict**) </br> [MaxLength](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxTextBoxBase.MaxLength) </br> [MaskSettings.Mask](https://docs.devexpress.com/AspNet/DevExpress.Web.MaskSettings.Mask)                                                         |
| [ASPxTokenBox](https://docs.devexpress.com/AspNet/16295/aspnet-webforms-controls/data-editors/aspxtokenbox) | [DataSecurityMode](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxListBox.DataSecurityMode) (if set to **Strict**)                                                                                                                                                                                                                                                       |

### 7.3 Validation in List Editors

List-based UI controls within the DevExpress ASP.NET suite expose a `DataSecurity` property. This property specifies how a control handles input values that do not exist in the list. By default, this property is set to `Default`. With this setting, an editor accepts values that aren't in the list. Set the `DataSecurity` property to `Strict` to prohibit such values.

If you use the Strict DataSecurity mode, ViewState is disabled. If data binding is handled at runtime, you should execute data binding on `Page_Init`. Binding on `Page_load` is too late because editor validation triggers earlier than when its items are populated.

Review our sample [ValidateInput/ListEditors.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ValidateInput/ListEditors.aspx#L8) page for more information.

### 7.4 Disable Built-in ASP.NET Request Validation

ASP.NET checks input values for potentially dangerous content. For example, when an end-user enters `<b>` into a text field (and submits the form), they are redirected to an error page with the following message:

```
System.Web.HttpRequestValidationException: A potentially dangerous Request value was detected from the client (Property="<b>")
```

In many instances, such checks can have undesired side effects:

- End users cannot enter text containing elements that are common in technical texts, for example, "Use the &lt;b&gt; tag to apply a bold text effect."

- On a validation error, ASP.NET raises an exception and responds with the default error page. Because of this, you cannot handle the error and display a user-friendly error message.

For these reasons, built-in request value checks are commonly disabled. If you wish to remove value checks, disable the `validateRequest` option available in the `/system.web/pages` section of [web.config](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Web.config#L23-L24):

```xml
<system.web>
  <httpRuntime requestValidationMode="2.0" />
  <pages validateRequest="false">
```

> Whether checks are enabled or not, you should use encoding to protect your application from XSS attacks. Refer to the [section 6](#6-preventing-cross-site-scripting-xss-attacks-with-encoding) of this document to learn more.

### 7.5 Inlining SVG Images

SVG markup can include scripts that will be executed if the SVG is inlined into a page. For example, the code below executes a script embedded into SVG markup:

##### [APSX file](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ValidateInput/SvgInline.aspx):

```aspx
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>Inline Svg images are not secure! You must trust the source!</p>
    <p>For example: Svg bytes uploaded by end user, stored in the data source and embedded inline. JavaScript is excecuted:</p>
    <div id="svgInlineImageContainer" runat="server"></div>
</asp:Content>
```

##### [Code-behind](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ValidateInput/SvgInline.aspx.cs):

```cs
protected void Page_Load(object sender, EventArgs e) {
    // Init Value from DataSource
    var svgImageWithJavaScriptCode = "<svg height=100 width=100><circle cx=50 cy=50 r=40 stroke=black stroke-width=2 fill=red /><script>alert('XXS')</script></svg>";
    svgInlineImageContainer.InnerHtml = svgImageWithJavaScriptCode;
}
```

---

## 8. Export to CSV

**Security Risks**: [CWE-74](https://cwe.mitre.org/data/definitions/74.html)

Control data exported to CSV can include content that spreadsheet software such as Microsoft Excel interprets as a formula. Such formulas can potentially execute shell commands. For example, the following formula runs the Windows Calculator when evaluated:

```
=cmd|' /C calc'!'!A1'
```

This may generate a risk if an end-user opens the exported file in a spreadsheet program and allows the program to run executable content. Microsoft Excel displays a message explicitly asking if a user wishes to run executable content.

To prevent possible vulnerabilities, set the `EncodeCsvExecutableContent` property to `true`. In this mode, exported content is encoded so it cannot be interpreted as executable.

This behavior is controlled by the following properties:

- At the control level - `DevExpress.XtraPrinting.CsvExportOptions.EncodeExecutableContent`:

  ```cs
  // Standard Approach
  protected void Button_Click(object sender, EventArgs e) {
    var options = new CsvExportOptionsEx();
    options.EncodeExecutableContent = DefaultBoolean.True;
    ASPxGridView1.ExportCsvToResponse(options);
  }

  // For a Toolbar Button
  protected void ASPxGridView1_BeforeExport(object sender, DevExpress.Web.ASPxGridBeforeExportEventArgs e) {
    (e.ExportOptions as CsvExportOptionsEx).EncodeExecutableContent = DefaultBoolean.True;
  }
  ```

- At the application level (a Global.asax setting) - `DevExpress.Export.ExportSettings.EncodeCsvExecutableContent`:

  ```cs
  DevExpress.Export.ExportSettings.EncodeCsvExecutableContent = DevExpress.Utils.DefaultBoolean.True;
  ```

> **IMPORTANT NOTE:** This setting is not enabled by default because escaped content (everything that starts with "=", "-", "+", "@", "") within a CSV file may be unacceptable in many use-case scenarios. For example, escaping brakes text values that begin with "=" or negative numbers:
> ```
> Product Name,Quantity Per Unit,Unit Price,Units In Stock, Status
> """=Chai""",10 boxes x 20 bags,$18.00,39,$702.00, """-10%"""
> Chang,24 - 12 oz bottles,$19.00,17,$323.00, 5%
> ```

Because Excel requires a user's permission to run executable content, we do not enable this setting by default and allow a user to enable this settings if it addresses a specific use-case.

Review the following article to learn more about CSV injections: [https://www.owasp.org/index.php/CSV_Injection](https://www.owasp.org/index.php/CSV_Injection)

---

## 9. Unauthorized Operations on the Server Through Client API

**Security Risks**: [CWE-284](https://cwe.mitre.org/data/definitions/284.html), [CWE-285](https://cwe.mitre.org/data/definitions/285.html)

### 9.1. Unauthorized CRUD Operations in View Mode

Review our sample [ClientSideApi/GridView.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ClientSideApi/GridView.aspx) page for more information.

Grid-based controls (ASPxGridView, ASPxCardView, etc.) expose client methods that trigger CRUD operations on the server. For example, you can call the [ASPxClientGridView.DeleteRow](<https://docs.devexpress.com/AspNet/js-ASPxClientGridView.DeleteRow(visibleIndex)>) method on the client to delete a grid row. If a control is configured incorrectly, these methods can be used to alter data even if the control is set to display data in view-only mode (data editting-related UI elements are hidden).

To familiarize yourself with the issue:

1. Comment out the following line in [ClientSideApi/GridView.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ClientSideApi/GridView.aspx):

   ```aspx
   <SettingsDataSecurity AllowEdit="False" AllowInsert="False" AllowDelete="False" AllowReadUnexposedColumnsFromClientApi="False" />
   ```

2. Open this page in the browser and click the **DeleteRow(0)** button or enter the following code in the browser's console:

   ```js
   >gridView.DeleteRow(0)
   ```

   ![Templates - Sanitized Content](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/unauthorized-client-crud.png?raw=true)

   This will delete a data row with index `0`. This is possible because the Grid View's data source still has the Delete statement and the grid's `SettingsDataSecurity.AllowDelete` property is set to the default value (`True`).

To address this vulnerability:

- If you intend to use a grid-based control in view-only mode, make certain that its data source does not allow data editing (for example a SqlDataSource does not have a `DeleteCommand`, `InsertCommand` and `UpdateCommand`).

-  Use the control's [`SettingsDataSecurity`](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView.SettingsDataSecurity) property to disable CRUD operations at the control level:

  ```aspx
  <SettingsDataSecurity AllowEdit="False" AllowInsert="False" AllowDelete="False" />
  ```

### 9.2. Using Spreadsheet in Read-Only Mode

If you wish to use our Spreadsheet control in read-only mode (the `SettingsView.Mode` option is set to "Reading"), you must prevent users from switching to edit mode:

```aspx
<Settings>
    <Behavior SwitchViewModes="Hidden" />
</Settings>
```

You should also set the `ReadOnly` property to `true`:

```cs
spreadsheet.ReadOnly="true"
```

Review our sample [ClientSideApi/SpreadsheetReadingModeOnly.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ClientSideApi/SpreadsheetReadingModeOnly.aspx) page for more inforamtion.

### 9.3 File Selector Commands in the RichEdit and Spreadsheet

Related Controls: [ASPxRichEdit](https://docs.devexpress.com/AspNet/17721/aspnet-webforms-controls/rich-text-editor), [ASPxSpreadsheet](https://docs.devexpress.com/AspNet/16157/aspnet-webforms-controls/spreadsheet)

In a popular use case scenario, the RichEdit or Spreadsheet control's **File** tab is hidden to prevent an end-user from accessing FileSelector commands (`New`, `Open`, `Save`, etc.) In such instances, documents are opened and saved programmatically.

**IMPORTANT**: To prevent access to FileSelector commands, you must take additional steps as these commands may still be executed with JavaScript or keyboard shortcuts (for example, Ctrl+O can invoke the Open dialog).

When you want to disable file-related commands, you must disable file operations by disabling corresponding Behavior options (`CreateNew`, `Open`, `Save`, `SaveAs`, `SwitchViewModes`).

##### ASPxSpreadsheet:

```aspx
<dx:ASPxSpreadsheet ID="spreadsheet" runat="server" WorkDirectory="~/App_Data/WorkDirectory">
    <Settings>
        <Behavior CreateNew="Hidden" Open="Hidden" Save="Hidden" SaveAs="Hidden" SwitchViewModes="Hidden"/>
    </Settings>
</dx:ASPxSpreadsheet>

```

##### ASPxRichEdit:

```aspx
<dx:ASPxRichEdit ID="ASPxRichEdit1" runat="server" WorkDirectory="~\App_Data\WorkDirectory">
    <Settings>
        <Behavior CreateNew="Hidden" Open="Hidden" Save="Hidden" SaveAs="Hidden" ></Behavior>
    </Settings>
</dx:ASPxRichEdit>
```

To familiarize yourself with the issue, open our sample project's [ClientSideApi/FileSelector.aspx](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/ClientSideApi/OfficeControlsFileOperations.aspx) file and comment out the following line:

```aspx
<Behavior CreateNew="Hidden" Open="Hidden" Save="Hidden" SaveAs="Hidden" SwitchViewModes="Hidden"/>
```

Run the application and open the page. If you press Ctrl+O, the Open dialog will be invoked, even though the File tab is hidden.

![Templates - Sanitized Content](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/spreadsheet-file-selector.png?raw=true)

---

## 10. Downloading Files From External URLs

Consider a web application that receives a URL from an end-user, downloads an image file from this URL and saves the file to a database. This file is then displayed on the application page using the [ASPxBinaryImage](https://docs.devexpress.com/AspNet/11646/components/data-editors/binaryimage) control.

Many suggest the use of the [WebClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.webclient?view=net-6.0) class to download a file in this particular usage scenario:

```cs
using(var webClient = new WebClient())
    BinaryImage.ContentBytes = webClient.DownloadData(url);
```

Unfortunately, this is an unsafe strategy as the WebClient can accept a path to a local resource on the server (for example, `c:\...\App_Data\ConfidentialImages\...`). This allows a threat actor to obtain access to confidential files (such as *web.config*, the *App_Data* folder or other files/folders with private content).

To mitigate this vulnerability, use [HttpWebRequest](https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest?view=net-7.0):

```cs
HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
using(HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
    ...
}
```

In this instance, an attempt to download a local file generates an exception.

Review our sample [DownloadingFiles/DownloadFileFromUrl.aspx.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/DownloadingFiles/DownloadFileFromUrl.aspx.cs) file for more information.

---

![Analytics](https://ga-beacon.appspot.com/UA-129603086-1/aspnet-security-bestpractices-webforms-page?pixel)
