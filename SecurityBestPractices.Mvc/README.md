# ASP.NET MVC Security Best Practices

This document provides information on some of the best practices developers should follow to avoid certain security breaches. Each section of this document describes a possible use-case scenario that exposes the security issue and the vulnerabilities associated with it, together with information on how to mitigate the security problems.

The security issues are all shown using a simple Visual Studio solution. Fully commented code samples are provided as part of that solution to show how to avoid each security breach. You will need to have DevExpress ASP.NET MVC extensions installed in order to load and compile the solution. You can download the installer from the [DevExpress website](https://devexpress.com).

---

- [1. Uploading Files](#1-uploading-files)
- [2. Uploading and Displaying Binary Images](#2-uploading-and-displaying-binary-images)
- [3. Authorization](#3-authorization)
- [4. Preventing Cross-Site Request Forgery (CSRF)](#4-preventing-cross-site-request-forgery-csrf)
- [5. Preventing Sensitive Information Exposure](#5-preventing-sensitive-information-exposure)
- [6. Preventing Cross-Site Scripting (XSS) Attacks with Encoding](#6-preventing-cross-site-scripting-xss-attacks-with-encoding)
- [7. User Input Validation](#7-user-input-validation)
- [8. Export to CSV](#8-export-to-csv)

## 1. Uploading Files

**Related extensions**: [Binary Image](https://documentation.devexpress.com/AspNet/9010/ASP-NET-MVC-Extensions/Data-Editors-Extensions/BinaryImage/Overview/Overview-BinaryImage), [Upload Control](https://documentation.devexpress.com/AspNet/9002/ASP-NET-MVC-Extensions/File-Management/File-Upload/Overview/Overview-UploadControl), [File Manager](https://documentation.devexpress.com/AspNet/15115/ASP-NET-MVC-Extensions/File-Management/File-Manager/Overview/Overview-FileManager), [Html Editor](https://documentation.devexpress.com/AspNet/8999/ASP-NET-MVC-Extensions/HTML-Editor/Overview/Overview-HtmlEditor), [Rich Edit](https://documentation.devexpress.com/AspNet/114046/ASP-NET-MVC-Extensions/Rich-Text-Editor/Overview/Overview-Rich-Text-Editor), [Spreadsheet](https://documentation.devexpress.com/AspNet/17114/ASP-NET-MVC-Extensions/Spreadsheet/Overview/Overview-Spreadsheet)

**Security Risks**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html), [CWE-434](https://cwe.mitre.org/data/definitions/434.html)

This section provides information on how to provide file upload capabilities within your web application. There are several separate scenarios to cover:

- [1.1. Stop Malicious Files Being Uploaded](#11-prevent-uploading-malicious-files)
- [1.2. Avoid Uncontrolled Resource Consumption](#12-prevent-uncontrolled-resource-consumption)
- [1.3. Protect Temporary Files](#13-protect-temporary-files)

### 1.1. Stop Malicious Files Being Uploaded

See the [controller code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/72492709fd0c53fef25414fc77df9a37fc4dc5a7/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingFiles/UploadingFiles.cs#L12-L57) in the **Controllers\UploadingFiles\UploadingFiles.cs** file of the example project for a full code sample.

Consider the situation where your web application allows for files to be uploaded. These files are then accessed using a specific URL (for example: _example.com/uploaded/uploaded-filename_).

The possible security breach here occurs when a malicious file is uploaded that can then be executed on the server side. For example, a malefactor could upload a file containing malicious code and guess at its URL. If the malefactor is correct and requests this URL, the file would be executed on the server as if it were part of the application.

To mitigate this vulnerability, do the following:

1. Implement a custom model binder. In the model binder implementation, initialize the [AllowedFileExtensions](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_AllowedFileExtensionstopic) setting with a list of allowed file extensions. The server will then validate the type of the uploaded file. Provide the [FilesUploadCompleteHandler](http://help.devexpress.com/#AspNet/DevExpressWebMvcBinderSettingsUploadControlBinderSettings_FileUploadCompleteHandlertopic) to perform any additional checks:

```cs
public class UploadValidationBinder : DevExpressEditorsBinder {
    public UploadValidationBinder() {
        // Specify allowed file extensions
        UploadControlBinderSettings.ValidationSettings.AllowedFileExtensions = new[] { ".jpg", ".png" };
        UploadControlBinderSettings.FilesUploadCompleteHandler = uploadControl_FilesUploadComplete;
    }
    private void uploadControl_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e) {
        var uploadedFiles = ((MVCxUploadControl)sender).UploadedFiles;
        if(uploadedFiles != null && uploadedFiles.Length > 0) {
            for(int i = 0; i < uploadedFiles.Length; i++) {
                UploadedFile file = uploadedFiles[i];
                if(file.IsValid && file.FileName != "") {
                    using(var stream = file.FileContent) {
                        // In case additional checks are needed, perform them here before saving the file
                        if(!IsValidImage(stream)) {
                            file.IsValid = false;
                            e.ErrorText = "Validation failed!";
                        } else {
                            string fileName = string.Format("{0}{1}", HostingEnvironment.MapPath("~/Upload/Images/"), file.FileName);
                            file.SaveAs(fileName, true);
                        }
                    }
                }
            }
        }
    }
}
```

Note that it is not enough to specify the allowed file extension on the View side:

```cs
@using(Html.BeginForm()) {
    @Html.DevExpress().UploadControl(
        settings => {
            settings.Name = "uploadControl";
            ...
            settings.ValidationSettings.AllowedFileExtensions = new[] { ".jpg", ".png" };
    }).GetHtml()
}
```

This option only affects the client behavior. It prevents input errors but does not strictly prohibit sending a malicious file to the server.

2. Additionally, you can disable file execution in the upload folder ([relevant StackOverflow question](https://stackoverflow.com/questions/3776847/how-to-restrict-folder-access-in-asp-net)):

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

| Control            | Allowed Extensions                                                    |
| ------------------ | --------------------------------------------------------------------- |
| **Upload Control** | _any_                                                                 |
| **Binary Image**   | _any_                                                                 |
| **File Manager**   | _any_                                                                 |
| **Html Editor**    | .jpe, .jpeg, .jpg, .gif, .png <br> .mp3, .ogg <br> .swf <br> .mp4     |
| **Rich Edit**      | .doc, .docx, .epub, .html, .htm, .mht, .mhtml, .odt, .txt, .rtf, .xml |
| **Spreadsheet**    | .xlsx, .xlsm, .xls, .xltx, .xltm, .xlt, .txt, .csv                    |

### 1.2. Prevent Uncontrolled Resource Consumption

#### 1.2.1 Prevent Uncontrolled Memory Consumption

See the [controller code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/72492709fd0c53fef25414fc77df9a37fc4dc5a7/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingFiles/UploadingFiles.cs#L59-L99) in the **Controllers\UploadingFiles\UploadingFiles.cs** file for a full code sample.

Consider the situation where the web application allows files of any size to be uploaded.

The possible security breach here occurs when a malefactor performs a denial of service ([DoS](https://cwe.mitre.org/data/definitions/400.html)) attack by uploading very large files, thereby using up server memory and disk space.

To mitigate this vulnerability:

1. Access file contents using the [FileContent](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileContenttopic) property (a Stream) rather than the [FileBytes](http://help.devexpress.com/#AspNet/DevExpressWebUploadedFile_FileBytestopic) property (a byte array). This will avoid memory overflow and other issues when processing large uploaded files.

```cs
public class UploadInMemoryProcessingBinder : DevExpressEditorsBinder {
    public UploadInMemoryProcessingBinder() {
        UploadControlBinderSettings.FilesUploadCompleteHandler = uploadControl_FilesUploadInMemoryProcessingComplete;
    }

    private void uploadControl_FilesUploadInMemoryProcessingComplete(object sender, FilesUploadCompleteEventArgs e) {
        var uploadedFiles = ((MVCxUploadControl)sender).UploadedFiles;
        if(uploadedFiles != null && uploadedFiles.Length > 0) {
            for(int i = 0; i < uploadedFiles.Length; i++) {
                UploadedFile file = uploadedFiles[i];
                if(file.IsValid && file.FileName != "") {
                    // Good approach - use stream for large files
                    using(var stream = file.FileContent) {
                        DoProcessing(stream);
                    }
                }
            }
        }
    }
}

```

2. Specify the maximum size for uploaded files using the [UploadControlValidationSettings.MaxFileSize](http://help.devexpress.com/#AspNet/DevExpressWebUploadControlValidationSettings_MaxFileSizetopic) property.

```cs
public class UploadValidationBinder : DevExpressEditorsBinder {
    public UploadValidationBinder() {
        UploadControlBinderSettings.ValidationSettings.MaxFileSize = "1000000"
    }
}

```

> Note that in the **Advanced** uploading mode, files are loaded in small fragments (200KB by default), thus setting the **httpRuntime**>**maxRequestLength** and **requestLimits**>**maxAllowedContentLength** options in **web.config** is not sufficient to prevent attacks.

See the [Uploading Large Files](https://documentation.devexpress.com/AspNet/9822/ASP-NET-WebForms-Controls/File-Management/File-Upload/Concepts/Uploading-Large-Files) documentation topic for more information.

The following extensions limit the maximum size for uploaded files by default:

- The **Html Editor** defines a 31,457,280 byte limit for uploaded file size.
- The **Spreadsheet** and **Rich Edit** do not set a maximum size for an uploaded file, however there is a 31,457,280 byte limit for images to be inserted into a document. Note that both controls must be specifically enabled to allow for the uploading of files.

The File Manager automatically allows files to be uploaded, and does not impose any limitations on the file size and extension. You can disable file uploading with this View code:

```cs
@Html.DevExpress().FileManager(settings =>
{
    settings.Name = "FileManager";
    settings.SettingsUpload.Enabled = false;
    ...
}).BindToFolder(Model).GetHtml()
```

Other operations on files organized by the File Manager (copy, delete, download, etc.) are configured using the [SettingsEditing](http://help.devexpress.com/#AspNet/DevExpressWebMvcFileManagerSettings_SettingsEditingtopic) property. All such operations are disabled by default.

#### 1.2.2 Prevent Uncontrolled Disk Space Consumption

**Security Risks**: [CWE-400](https://cwe.mitre.org/data/definitions/400.html)

You should always monitor the total size of files uploaded by end-users, otherwise a malefactor can perform a DoS attack by uploading too many files and using up the available disk space. The best practice is to set a limitation on the total size of uploaded files.

Check the upload directory size before saving the uploaded files:

```cs
private void uploadControl_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e) {
    var uploadedFiles = ((MVCxUploadControl)sender).UploadedFiles;
    if(uploadedFiles != null && uploadedFiles.Length > 0) {
        for(int i = 0; i < uploadedFiles.Length; i++) {
            UploadedFile file = uploadedFiles[i];
            if(file.IsValid && file.FileName != "") {
                using(var stream = file.FileContent) {
                    // Check limit for total size for uploaded files in upload folder and its sub folders
                    const long DirectoryFileSizesLimit = 10000000; // bytes
                    long totalFilesSize = GetDirectoryFileSizes(HostingEnvironment.MapPath("~/UploadingFiles/Images/"));
                    if(file.ContentLength + totalFilesSize > DirectoryFileSizesLimit) {
                        file.IsValid = false;
                         e.ErrorText = "Total files size exceeded!";
                    } else {
                        // In case additional checks are needed perform them here before saving the file
                        if(!IsValidImage(stream)) {
                        ...
```

See the example project's [Controllers/UploadingFiles/UploadingFiles.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingFiles/UploadingFiles.cs#L149) file.

### 1.3. Protect Temporary Files

See the [controller code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/72492709fd0c53fef25414fc77df9a37fc4dc5a7/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingFiles/UploadingFiles.cs#L101-L130) in the **Controllers\UploadingFiles\UploadingFiles.cs** file for a full code sample.

Consider the situation where you store temporary files on the server, prior to processing them on the server (for example, to extract the data within to write to a database).

To avoid a security breach, you will need to ensure that these files are inaccessible to third parties.

To mitigate this vulnerability:

1. Store temporary files in a folder unreachable by URL (for example, _App_Data_).
2. Use a dedicated file extension for temporary files on the server (for example "\*.mytmp").
3. Consider assigning random file names using the [GetRandomFileName](<https://msdn.microsoft.com/en-us/library/system.io.path.getrandomfilename(v=vs.110).aspx>) method.

```cs
public class UploadSavingTemporaryFilesBinder : DevExpressEditorsBinder {
    public UploadSavingTemporaryFilesBinder() {
        UploadControlBinderSettings.FilesUploadCompleteHandler = uploadControl_FilesUploadSavingTemporaryFilesComplete;
    }

    private void uploadControl_FilesUploadSavingTemporaryFilesComplete(object sender, FilesUploadCompleteEventArgs e) {
        var uploadedFiles = ((MVCxUploadControl)sender).UploadedFiles;
        if(uploadedFiles != null && uploadedFiles.Length > 0) {
            for(int i = 0; i < uploadedFiles.Length; i++) {
                UploadedFile file = uploadedFiles[i];
                if(file.IsValid && file.FileName != "") {
                    // Assign a random file name
                    string fileName = string.Format("{0}{1}", HostingEnvironment.MapPath("~/Upload/Processing/"),
                        Path.GetRandomFileName() + ".tmp");
                    file.SaveAs(fileName, true);
                    // DoFileProcessing(fileName);
                }
            }
        }
    }
}

```

You can also define security permissions for folders and files accessible through the File Manager control. The [Access Rules](https://documentation.devexpress.com/AspNet/119542/ASP-NET-WebForms-Controls/File-Management/File-Manager/Concepts/Access-Control-Overview/Access-Rules) documentation topic has further information.

---

## 2. Uploading and Displaying Binary Images

**Related Extension**: [Binary Image](https://documentation.devexpress.com/AspNet/9010/ASP-NET-MVC-Extensions/Data-Editors-Extensions/BinaryImage/Overview/Overview-BinaryImage), [Upload Control](https://documentation.devexpress.com/AspNet/9002/ASP-NET-MVC-Extensions/File-Management/File-Upload/Overview/Overview-UploadControl)

**Security Risks**: [CWE-79](https://cwe.mitre.org/data/definitions/79.html)

Consider the situation where an image is uploaded to the server. The server generates a page that contains the image, and a user opens that page in their browser.

The possible security breach is this: a malefactor creates a file containing a malicious script. This file has an image file extension. The file is added to the generated page by the server, and that page is downloaded by the user's browser. This results in the malicious script being run in the browser. Essentially this is an example of XSS (Cross-site Scripting) via content-sniffing, a particular case of [CWE-79](https://cwe.mitre.org/data/definitions/79.html).

To familiarize yourself with the issue:

1. Comment out the call of the **IsValidImage** method in the example project's [Controllers/UploadingBinaryImages/UploadControl.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingBinaryImages/UploadingBinaryImages.cs) to disable protection:
   ```cs
    // Here contentBytes should be saved to a database
    using(var stream = e.UploadedFile.FileContent) {
    //     if(!IsValidImage(stream)) {
    //        e.ErrorText = "Invalid image.";
    //        e.IsValid = false;
    //    } else {
            string fileName = HostingEnvironment.MapPath("~/App_Data/UploadedData/avatar.jpg");
            e.UploadedFile.SaveAs(fileName, true);
    //    }
    }
   ```
2. Run the example solution and open the **[UploadingBinaryImages/UploadControl](https://github.com/DevExpress/aspnet-security-bestpractices/blob/77534a317c3d685810e57cf034994d99a2d1c58f/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingBinaryImages/UploadingBinaryImages.cs#L14)** page.
3. Upload the **[\App_Data\TestData\Content-Sniffing-XSS.jpg](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/App_Data/TestData/Content-Sniffing-XSS.jpg)** file. This is nominally a JPEG image, but in fact is a JavaScript file that emulates a malicious script.
4. Open the **[UploadingBinaryImage/BinaryImageViewer](https://github.com/DevExpress/aspnet-security-bestpractices/blob/77534a317c3d685810e57cf034994d99a2d1c58f/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingBinaryImages/UploadingBinaryImages.cs#L79-L90)** page. As with every ASPX request, the markup is generated by the server on request, and the uploaded file will be added to the code behind.
5. The JavaScript code from the uploaded file is executed by the browser:

![malicious-image](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/uploading-binary-image-mvc.png?raw=true)

To mitigate this vulnerability:

1. Programmatically check whether the uploaded file is really an image before saving it to any server-side storage (see the [IsValidImage](https://github.com/DevExpress/aspnet-security-bestpractices/blob/77534a317c3d685810e57cf034994d99a2d1c58f/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UploadingBinaryImages/UploadingBinaryImages.cs#L42-L50) method implementation).

```cs
public class UploadControlBinder : DevExpressEditorsBinder {
    public UploadControlBinder() {
        UploadControlBinderSettings.ValidationSettings.AllowedFileExtensions = new[] { ".jpg", ".jpeg" };
        UploadControlBinderSettings.FileUploadCompleteHandler = uploadControl_FileUploadComplete;
    }

    private void uploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
        if(!e.UploadedFile.IsValid) return;

        // Here contentBytes should be saved to a database
        using(var stream = e.UploadedFile.FileContent) {
            if(!IsValidImage(stream)) {
                e.ErrorText = "Invalid image.";
                e.IsValid = false;
            } else {
                // We save it to a file for demonstration purposes
                string fileName = HostingEnvironment.MapPath("~/App_Data/UploadedData/avatar.jpg");
                e.UploadedFile.SaveAs(fileName, true);
            }
        }
    }
}

static bool IsValidImage(Stream stream) {
    try {
        using(var image = Image.FromStream(stream)) {
            return true;
        }
    } catch(Exception) {
        return false;
    }
}
```

2. Use the [Binary Image](http://help.devexpress.com/#AspNet/CustomDocument9010) extension for uploading images. This control automatically implements an image file type check.

\[View\]

```cs
@Html.DevExpress().BinaryImage(biSettings => {
    biSettings.Name = "BinaryImage";
    biSettings.Properties.EditingSettings.Enabled = true;
    biSettings.Properties.EditingSettings.UploadSettings.UploadValidationSettings.MaxFileSize = 4194304;
    biSettings.CallbackRouteValues = new { Controller = "UploadingBinaryImages", Action = "UpdateBinaryImagePartial" };
    biSettings.Properties.ClientSideEvents.ValueChanged = "function () { SubmitButton.SetEnabled(true); }";
}).Bind(Model).GetHtml()
```

\[Controller\]

```cs
[HttpPost]
public ActionResult SaveBinaryImage() {
    byte[] contentBytes = BinaryImageEditExtension.GetValue<byte[]>("BinaryImage"); // Uploaded file content are valided by Binary Image

    // Here contentBytes should be saved to a database
    // We will save it to a file for demonstration purposes
    string fileName = HostingEnvironment.MapPath("~/App_Data/UploadedData/avatar.jpg");
    System.IO.File.WriteAllBytes(fileName, contentBytes != null ? contentBytes : new byte[0] { });

    return RedirectToAction("BinaryImage");
}

```

Another strong recommendation is to _always_ specify the exact content type when you add binary data to the response:

**Correct:** `Response.ContentType = "image/jpeg"`;

**Potential security breach:** `Response.ContentType = "image"`.

Additionally, it is a good practice to add the `X-CONTENT-TYPE-OPTIONS="nosniff"` response header:

```cs
Response.Headers.Add("X-Content-Type-Options", "nosniff");
```

### Notes:

1. Microsoft Edge automatically detects a file's type based on its content, which prevents the execution of malicious scripts as described here.
2. Make sure to specify the maximum uploaded file size to prevent those DoS attacks based on uploading large files.

---

## 3. Authorization

**Related Extension**: [Document Viewer](http://help.devexpress.com/#AspNet/CustomDocument114491), [Web Dashboard](https://documentation.devexpress.com/Dashboard/16977/Building-the-Designer-and-Viewer-Applications/Web-Dashboard/ASP-NET-MVC-Dashboard-Extension), [Query Builder](http://help.devexpress.com/#AspNet/CustomDocument120076)

**Security Risks**: [CWE-285](https://cwe.mitre.org/data/definitions/285.html)

This section provides information on using DevExpress controls in web applications that implement authorization and access control. The following features are considered:

- [3.1. Using Authorization Attributes](#31-using-authorization-attributes)
- [3.2. Reporting](#32-reporting)
- [3.3. Dashboard](#33-dashboard)
- [3.4. Query Builder](#34-query-builder)

### 3.1 Using Authorization Attributes

In ASP.NET MVC, authorization rules for a web application's pages are defined on the controller side using the **[Authorize]** attribute. This attribute can be applied either to a controller action or to the whole controller class.

For maximum security, it is recommended that you apply the **[Authorize]** attribute to the controller and allow anonymous access to actions that should be public using the **[AllowAnonymous]** attribute.

```cs
namespace SecurityBestPractices.Mvc.Controllers {
    [Authorize]
    public partial class AuthorizationController : Controller {
        // GET: /Authorization/Reports/PublicReport
        [AllowAnonymous]
        public ActionResult PublicReport() {
            return View("Reports/PublicReport");
        }

        // GET: /Authorization/Reports/ReportViewer
        public ActionResult ReportViewer() {
            return View("Reports/ReportViewer",
                new ReportNameModel() { ReportName =
                ReportStorageWithAccessRules.GetViewableReportDisplayNamesForCurrentUser().FirstOrDefault()});
        }
...
```

### 3.2. Reporting

To implement authorization logic for the Document Viewer, provide a custom report storage derived from the [ReportStorageWebExtension](http://help.devexpress.com/#XtraReports/clsDevExpressXtraReportsWebExtensionsReportStorageWebExtensiontopic) class. This storage will be used by the HTTP handler to access your reports and here you can define your authorization rules. As a starting point, you can copy the reference implementation of such a storage class from the example project's [ReportStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.WebForms/SecurityBestPractices/Authorization/Reports/ReportStorageWithAccessRules.cs) file to your application and fine-tune it for your needs. The following customizations would have to be considered:

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
// GET: /Authorization/Reports/ReportDesigner/<Report Url>
public ActionResult ReportDesigner(string name) {
    var reportNames = ReportStorageWithAccessRules.GetEditableReportNamesForCurrentUser();
    if(reportNames.Contains(name))
        return View("Reports/ReportDesigner", new ReportNameModel() { ReportName = name });
    else
        return RedirectToAction("ReportViewer", "Authorization");
}
```

#### Register the Custom Report Storage

After implementing your custom report storage with the required access rules, you must register it in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Global.asax.cs) file:

```cs
DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageWithAccessRules());
```

#### Make Sure that Authentication Rules are Applied

In the example project, you can check whether the customization has had any effect by using the following steps:

- Open the [PublicReportPage](https://github.com/DevExpress/aspnet-security-bestpractices/blob/77534a317c3d685810e57cf034994d99a2d1c58f/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/Authorization/AuthorizationController.Reports.cs#L11-L13) page without logging in. This page contains a Report Viewer.
- Try to open a report using the client API in the browser console. The example report has restricted access:

```
>documentViewer.OpenReport("Admin Report");
```

The browser console will respond with the following error.

![console-output](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authoriazation-reports-accessdenied.png?raw=true)

#### Provide an Operation Logger

By default, the Web Document Viewer control access a report by a GUID, which serves as an authorization token. If required, you can provide additional protection by implementing a custom Operation Logger in order to enforce fine-grained control over which operations are available to a user. Extend the [WebDocumentViewerOperationLogger](http://help.devexpress.com/#XtraReports/clsDevExpressXtraReportsWebWebDocumentViewerWebDocumentViewerOperationLoggertopic) class and override this class's methods to implement the required access control rules (see the [OperationLogger.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Services/Reports/OperationLogger.cs) file of the example project).

Register the operation logger in [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Services/Reports/OperationLogger.cs):

```cs
DefaultWebDocumentViewerContainer.Register<WebDocumentViewerOperationLogger, OperationLogger>();
```

> Note that in order to simplify the example project, the logger implementation obtains the required user account data from a static property. This is not a recommended solution: such an implementation could have difficulties running in a cloud environment or on a web farm. Instead, we recommend that you store authentication information in some appropriate data storage.

To familiarize yourself with the solution:

1. Run the example application, log in using the [Login](https://github.com/DevExpress/aspnet-security-bestpractices/blob/77534a317c3d685810e57cf034994d99a2d1c58f/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/AccountController.cs#L21-L24) page.
2. Open the report preview ([Authorization/Reports/ReportViewer](https://github.com/DevExpress/aspnet-security-bestpractices/blob/77534a317c3d685810e57cf034994d99a2d1c58f/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/Authorization/AuthorizationController.Reports.cs#L16-L20)) in a separate browser tab.
3. Log out.
4. Try switching report pages.

The following error will be signaled:

![Operation Logger Error](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/authorization-reportopertaionlogger.png?raw=true)

#### Restrict Access to Data Connections and Data Tables

The [Report Designer](https://documentation.devexpress.com/XtraReports/17103/Creating-End-User-Reporting-Applications/Web-Reporting/Report-Designer) allows a user to browse available data connection and data tables using the integrated [Query Builder](http://help.devexpress.com/#AspNet/CustomDocument120076). Refer to the [Query Builder](#33-query-builder) subsection in this document to learn how to restrict access to this information, based on authorization rules.

### 3.3. Dashboard

The [DevExpress Web Dashboard](https://documentation.devexpress.com/Dashboard/16977/Building-the-Designer-and-Viewer-Applications/Web-Dashboard/ASP-NET-MVC-Dashboard-Extension) requires a controller, that inherits the **DashboardController** class. This class provides predefined actions that handle all AJAX requests from the dashboard.

The access control should be performed by a custom dashboard storage class that implements the [IEditableDashboardStorage](https://docs.devexpress.com/Dashboard/DevExpress.DashboardWeb.IEditableDashboardStorage?tabs=tabid-csharp%2Ctabid-T392813_7_52373613) interface.

As a starting point, you can copy the reference implementation of such a storage class from the example project's [DashboardStorageWithAccessRules.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Services/Dashboards/DashboardStorageWithAccessRules.cs) file to your application and fine-tune it for your needs.

The [DashboardStorageWithAccessRules](https://github.com/DevExpress/aspnet-security-bestpractices/blob/77534a317c3d685810e57cf034994d99a2d1c58f/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Services/Dashboards/DashboardStorageWithAccessRules.cs#L10-L117) class implementation defines the access restrictions:

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

Register the custom dashboard storage class in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Global.asax.cs) file as shown below.

```cs
DashboardConfigurator.Default.SetDashboardStorage(new DashboardStorageWithAccessRules());
```

With this custom implementation of a dashboard storage, if a user named 'John' tries to use the client API to open a report with restricted access (e.g., a report with id='1'), the handler will return error 404, File Not Found:

```js
dashboard.LoadDashboard("1"); // Load a dashboard available only to Admin.
```

```
GET http://localhost:2088/dashboardDesigner/dashboards/1?_=1536758969547 404 (Not Found)
```

#### Restrict Access to Data Connections and Data Tables

The Web Dashboard control allows a user to browse available data connection and data tables using the integrated [Query Builder](http://help.devexpress.com/#AspNet/CustomDocument120076). Refer to the [Query Builder](#33-query-builder) subsection in this document to learn how to restrict access to this information based on authorization rules.

### 3.4. Query Builder

The standalone [Query Builder](http://help.devexpress.com/#AspNet/CustomDocument120076) as well as the Query Builder integrated into the Report and Dashboard designers allows an end-user to browse a web application's data connections and the data tables available through those connections. In a web application that uses access control, you will to write code to restrict a user's access to the available connections and data tables.

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

As a starting point, you can copy the reference implementation from the example project's [DataSourceWizardConnectionStringsProvider.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Services/DataSourceWizardConnectionStringsProvider.cs) and [DataSourceWizardDBSchemaProviderExFactory.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Services/DataSourceWizardDBSchemaProviderExFactory.cs) files to your application and fine-tune it for your needs.

Register the implemented classes for the Report Designer, Dashboard Designer, or standalone Query Builder in the [Global.asax.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Global.asax.cs) file, as shown here:

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

**Related Extension**: Extensions with data editing available by default (e.g.,
[Grid View](https://documentation.devexpress.com/AspNet/8998/ASP-NET-MVC-Extensions/Grid-View/Overview/Overview-GridView), [Card View](https://documentation.devexpress.com/AspNet/114559/ASP-NET-MVC-Extensions/Card-View/Overview/Overview-CardView), [Vertical Grid](https://documentation.devexpress.com/AspNet/116060/ASP-NET-MVC-Extensions/Vertical-Grid/Overview/Overview-VerticalGrid), [Tree List](https://documentation.devexpress.com/AspNet/13766/ASP-NET-MVC-Extensions/Tree-List/Overview/Overview-TreeList), [Dashboard Designer](https://documentation.devexpress.com/Dashboard/116518/Basic-Concepts-and-Terminology/Dashboard-Designer), etc.)

**Security Risks**: [CWE-352](https://cwe.mitre.org/data/definitions/352.html)

This section provides information on how to prevent cross-site request forgery (CSRF) attacks on your web application. The vulnerability affects applications performing data editing requests including those using DevExpress AJAX-enabled controls such as the Grid View. Although there are authorization mechanisms that allow you to deny access by Insecure Direct Object References (for example: _example.com/app/SecureReport.aspx?id=1_), they do not protect you from CSRF attacks.

The possible security breach could occur as follows:

1. A malefactor implements a phishing page.
2. A user inadvertently visits this phishing page, which then sends a malicious request to your web application using the user's cookies.
3. As a result, the malicious action is performed on the user's behalf, allowing the malefactor to access or modify the user's data or account info.

### Get Familiar With the Issue

To familiarize yourself with the issue:

1. Comment out the `[ValidateAntiForgeryToken]` attribute in the example project's [Controllers/UsingAntiForgeryToken/UsingAntiForgeryTokenController.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UsingAntiForgeryToken/UsingAntiForgeryTokenController.cs#L26) file to disable protection:

   ```cs
        [Authorize]
        [ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult EditFormDeletePartial(int id = -1) {
            if(id >= 0)
                EditFormItems.Delete(id);

            return EditFormPartial();
        }
   ```

2. Open the [Static/Postback.html](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Static/Postback.html) page in the browser and click the link on this page. The page will perform a POST request to the `EditFormDeletePartial` controller action and remove a data item (Row Id=3).

For more information on the vulnerability, refer to the [CWE-352 - Cross-Site Request Forgery (CSRF)](https://cwe.mitre.org/data/definitions/352.html) article.

### How to Mitigate

To mitigate the vulnerability, use the **AntiForgeryToken** pattern as described below:

1. Generate an anti-forgery token for a form in the [view code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/UsingAntiForgeryToken/EditForm.cshtml#L21) as shown below:
   ```cs
   @using(Html.BeginForm()) {
       @Html.AntiForgeryToken()
       ...
   }
   ```
   This generates the following hidden field:
   ```html
   <input
     name="__RequestVerificationToken"
     type="hidden"
     value="[Token Value]"
   />
   ```
2. Be aware that complex AJAX-enabled controls do not automatically include the token field's value in their AJAX requests. You need to manually add the token value to such requests. You can achieve this using the code below:

   ```cs
   <script>
    // sending the __RequestVerificationToken value manually
    if (window.jQuery) {
        $.ajaxPrefilter(function (options, originalOptions, xhr) {
            if (options.dataType && options.dataType !== "html")
                return;

            var tokenValue = $('input:hidden[name="__RequestVerificationToken"]').val();
            if (tokenValue && options && options.data && options.data.indexOf('RequestVerificationToken') === -1)
                options.data += "&__RequestVerificationToken=" + tokenValue;
        });
    }
   </script>
   ```

3. To automatically check the request token on the server side, apply the `[ValidateAntiForgeryToken]` attribute to the corresponding controller action:
   ```cs
   [ValidateAntiForgeryToken]
   public ActionResult EditFormDeletePartial(int id = -1) {
       if(id >= 0)
           EditFormItems.Delete(id);
       return EditFormPartial();
   }
   ```

If the validation fails, the server will generate an error:

![AntiForgeryError](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/anti-forgery-error.png?raw=true)

> Note that you should never perform data modifying requests using the **GET** method.

### Use Anti-Forgery Tokens With the Dashboard Designer

1. Use the following [view code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/UsingAntiForgeryToken/EditDashboard.cshtml) to inject a anti-forgery token into a Dashboard's AJAX request header.

   ```cs
    <script type="text/javascript">
        function onBeforeRender(s, e) {
            var dashboardControl = s.GetDashboardControl();
            dashboardControl.remoteService.beforeSend = function (jqXHR, settings) {
                jqXHR.setRequestHeader('__RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
            }
        }
    </script>

    @using(Html.BeginForm()) {
        @Html.AntiForgeryToken()

        @Html.DevExpress().Dashboard(settings => {
        settings.Name = "Dashboard";
        settings.ControllerName = "DashboardWithAntiForgeryToken"; // see class DashboardWithAntiForgeryTokenController
        settings.InitialDashboardId = "editId";
        settings.ClientSideEvents.BeforeRender = "onBeforeRender";
        settings.ClientSideEvents.Init = "onBeforeRender";
    }).GetHtml()
    }
   ```

2. Define [a custom attribute](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UsingAntiForgeryToken/UsingAntiForgeryTokenController.cs#L58) to validate the anti-forgery token on requests.

   ```cs
    public sealed class DashboardValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter {
        public void OnAuthorization(AuthorizationContext filterContext) {
            if(filterContext == null) {
                throw new ArgumentNullException(nameof(filterContext));
            }

            HttpContextBase httpContext = filterContext.HttpContext;
            HttpCookie cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            AntiForgery.Validate(cookie?.Value, httpContext.Request.Headers["__RequestVerificationToken"]);
        }
    }
   ```

3. You can apply this attribute to a controller action that handles the Dashboard's callbacks as shown below.

   ```cs
    [DashboardValidateAntiForgeryToken]
    public class DashboardWithAntiForgeryTokenController : DevExpress.DashboardWeb.Mvc.DashboardController {
        static readonly DashboardConfigurator dashboardConfigurator;
        static DashboardWithAntiForgeryTokenController() {
            // sample data
            var dashboardInMemoryStorage = new DashboardInMemoryStorage();
            dashboardInMemoryStorage.RegisterDashboard("editId", XDocument.Load(HostingEnvironment.MapPath(@"~/App_Data/PublicDashboard.xml")));

            dashboardConfigurator = new DashboardConfigurator();
            dashboardConfigurator.SetDashboardStorage(dashboardInMemoryStorage);
        }

        public DashboardWithAntiForgeryTokenController() : base(dashboardConfigurator) {
        }
    }
   ```

   If a malefactor tries to forge a request to this controller action, an error will occur:

   ![Dashboard Anti Forgery](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/anti-forgery-dashboard.png)

## 5. Preventing Sensitive Information Exposure

**Security Risks**: [CWE-209](https://cwe.mitre.org/data/definitions/209.html)

This section describes security vulnerabilities that can make some sensitive information available to untrusted parties.

### 5.1 Information Exposure Through Error Messages

The possible security breach can occur when the server generates an exception. If an application is configured incorrectly, detailed information on the error is displayed to an end-user. This information can include sensitive parts giving a malefactor an insight on the application's infrastructure, file names and so on.

This behavior is controlled by the [customErrors](<https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/h0hfz6fc(v=vs.100)>) web.config option. This option accepts the following values:

- `RemoteOnly` (default) - In this mode, detailed errors are displayed only for connections from the local machine.
- `On` - Ensures that private messages are never displayed.
- `Off` - Forces private messages for all connections.

The following image demonstrates an error message exposing sensitive information:

![Unsafe Error Message](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/error-message-exposed.png)

In the secure configuration, the error message will be substituted by a more generic one:

![Safe Error Message](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/error-message-generic.png)

#### Manually Displaying Error Messages

It is recommended that you never display exception messages (Exception.Message) on your application's view because this text can contain sensitive information. For example, the following [code sample](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/InformationExposure/InformationExposureController.cs#L28) demonstrates an unsafe approach:

```cs
public ActionResult FormWithErrorMessage(EditFormItem item) {
    if(ModelState.IsValid) {
        try {
            // DoSomething()
            throw new InvalidOperationException("Some sensitive information");
        } catch(Exception ex) {
            ViewData[UpdateStatusKey] = ex.Message;
        }
    } else
        ViewData[UpdateStatusKey] = "Please, correct all errors.";
    return View(item);
}
```

Consider displaying custom error messages if you want to inform end-users about occurred errors:

```cs
public ActionResult FormWithErrorMessage(EditFormItem item) {
    if(ModelState.IsValid) {
        try {
            // DoSomething()
            throw new InvalidOperationException("Some sensitive information");
        } catch(Exception ex) {
            if(ex is InvalidOperationException) {
                ViewData[UpdateStatusKey] = "Some error occured...";
            } else {
                ViewData[UpdateStatusKey] = "General error occured...";
            }
        }
    } else
        ViewData[UpdateStatusKey] = "Please, correct all errors.";
    return View(item);
}
```

### 5.2 Availability of Invisible Column Values Through the Client-Side API

#### Prevent Access to Hidden Column Data

This vulnerability is associated with grid-based controls. Consider a situation, in which a control has a hidden column bound to some sensitive data that is not displayed to an end-user and is only used on the server. A malefactor can still request a value of such column using the control's client API:

```js
gridView.GetRowValues(0, "UnitPrice", function(Value) {
  alert(Value);
});
```

The image below demonstrates how a hidden column's value can be accessed using a browser's console:

![Unprotected Column Values](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/access-hidden-columns-no-protection.png)

Set the **AllowReadUnexposedColumnsFromClientApi** property to false to disable this behavior:

```cs
settings.SettingsDataSecurity.AllowReadUnexposedColumnsFromClientApi = DefaultBoolean.False;
```

See the example project's [Views/ClientSideApi/GridView.cshtml](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/ClientSideApi/GridView.cshtml#L9-L12) file to familiarize yourself with the issue.

#### Prevent Access by Field Name

Another vulnerability can occur when a malefactor tries to get a row value for a data field for which there is no column in the control:

```js
gridView.GetRowValues(0, "GuidField", function(Value) {
  alert(Value);
});
```

The capability to do so is controlled by the **AllowReadUnlistedFieldsFromClientApi** property and is disabled by default (safe configuration):

![Protected Column Values](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/access-hidden-columns-use-protection.png)

When it comes to protecting a grid control's data source data, a general recommendation to perform separate queries for data sources whose data is displayed in UI. These queries should never request data that should be kept in secret.

### 5.3 Information Exposure Through Source Code

**Security Risks**: [CWE-540](https://cwe.mitre.org/data/definitions/540.html)

The DevExpress default HTTP handler (DXR.axd) serves static files including images, scripts and styles. We assume that these static files are intended for public access and do not expose any sensitive information or server-side code. However there are additional recommendation for writing custom scripts and styles:

- Do not hardcode any credentials in scripts.
- Consider obfuscating these files in the following cases:
  - The file contains code that should be protected as an intellectual property.
  - The file's content can give a malefactor an idea about the backend system's architecture and possible vulnerabilities.

For example, consider the following code:

```js
function GetSystemState() {
...
}
```

The minified version gives considerably less information about the backend structure:

```js
function s1(){...}
```

---

## 6. Preventing Cross-Site Scripting (XSS) Attacks with Encoding

**Security Risks**: [CWE-80](https://cwe.mitre.org/data/definitions/80.html), [CWE-20](https://cwe.mitre.org/data/definitions/20.html)

The vulnerability occurs when a web page is rendered using a content specified by an end user. If user input is not properly sanitized, a resulting page can be injected with a malicious script.

It is strongly suggested that you always sanitize page contents that can be specified by a user. You should be aware of what kind of sanitization you use so it is both compatible with the displayed content and provides the intended level of safety. For example, you can remove HTML tags throughout the content but it can corrupt text that is intended to contain code samples. A more generic approach would be to substitute all '<', '>' and '&' symbols with `&lt;`, `&gt;` and `&amp;` character codes.

Microsoft provides the standard [HttpUtility](https://docs.microsoft.com/ru-ru/dotnet/api/system.web.httputility.htmlencode?view=netframework-4.7.2) class that you can use to encode data in various use-case scenarios. It provides the following methods:

| Method              | Usage                                                     |
| ------------------- | --------------------------------------------------------- |
| HtmlEncode          | Sanitizes an untrusted input inserted into an HTML output |
| HtmlAttributeEncode | Sanitizes an untrusted input assigned to a tag attribute  |
| JavaScriptEncode    | Sanitizes an untrusted input used within a script         |
| UrlEncode           | Sanitizes an untrusted input used to generate a URL       |

In ASP.NET MVC, any content inserted into a Razor view via the `@` operator is sanitized by default:

```cs
<p>Entered value: @Model.ProductName</p>
```

If you want to insert an unencoded value from a trusted source, use the `@Html.Raw` method.

To insert user input into a JavaScript block, you need to first prepare the string using the `HttpUtility.JavaScriptStringEncode` and then insert this string into the script using `@Html.Raw`:

```html
<script>
  var s =
    "@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(Model.ProductName))";
  // DoSomething(s);
</script>
```

Input text containing unsafe symbols will be converted to a safe form. In this example, if a user specifies the following unsafe string...

```
"<b>'test'</b>
```

... the script will be rendered as shown below:

```js
var s = "\"\u003cb\u003e'test'\u003c/b\u003e";
```

### 6.1 Encoding in DevExpress Controls

By default, DevExpress controls encode displayed values that can be obtained from a data source. Refer to the [HTML-Encoding](https://documentation.devexpress.com/AspNet/117902/Common-Concepts/HTML-Encoding) document for more information.

This behavior is specified by a control's EncodeHtml property. If a control displays a value that can be modified by an untrusted party, we recommend that you never disable this setting or sanitize the displayed content manually.

To get familiar with the vulnerability, open the example project's EncodeHtmlInControlsPartial.cshtm view and uncomment the [following line](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/HtmlEncoding/EncodeHtmlInControlsPartial.cshtml#L14):

```cs
column.PropertiesEdit.EncodeHtml = false;
```

Launch the project and open the page in the browser. A data field's content will be interpreted as a script and you well see an alert popup.

![Devexpress Controls - No Encoding](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/grid-columns-no-encoding.png?raw=true)

In the safe configuration, the field's content would be interpreted as text and correctly displayed:

![Devexpress Controls - Use Encoding](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/grid-columns-use-encoding.png?raw=true)


### 6.2 Encoding in Templates

If you inject data field values in templates, we recommend that you always [sanitize](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/HtmlEncoding/EncodeHtmlInTemplatesPartial.cshtml#L14) the data field values:

```cs
settings.SetItemTemplateContent(
     c => {
        ViewContext.Writer.Write(
            "<b>ProductID</b>: " + DataBinder.Eval(c.DataItem, "Id") +
            "<br/>" +
            //"<b>ProductName</b>: " + DataBinder.Eval(c.DataItem, "ProductName")
            "<b>ProductName</b>: " + System.Web.HttpUtility.HtmlEncode(DataBinder.Eval(c.DataItem, "ProductName"))
        );
    }
);

```

Inserting unsanitized content can open your application for XSS attacks:

![Templates - Unsanitized Content](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/templates-no-encoding.png?raw=true)

With encoding, the content would be interpreted as text and correctly displayed:

![Templates - Sanitized Content](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/templates-use-encoding.png?raw=true)

By default, DevExpress controls wrap templated contents with a `HttpUtility.HtmlEncode` method call.

### 6.4 Encoding Callback Data

When a client displays data received from the server via a callback, a security breach can take place if this data has not been properly encoded. For example, in the [code below](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/HtmlEncoding/EncodeAjaxResponse.cshtml), such content is assigned to an element's `innerHTML`:

##### View:

```html
<script>
  function sendRequest() {
    $.ajax({
      type: "POST",
      url: '@Url.Action("EncodeAjaxResponseCallback", "HtmlEncoding")',
      success: function(response) {
        $("#content").html(response);
      }
    });
  }
</script>
<a href="javascript:sendRequest()">Send Ajax Request</a>
```

##### Controller:

```cs
public ActionResult EncodeAjaxResponseCallback() {
    // return Content("1</title><script>alert(1);</script><title>");  // Not secure
    return Content(HttpUtility.HtmlEncode("1</title><script>alert(1);</script><title>")); // Secure
}
```

### 6.5 Dangerous Links

It is potentially dangerous to use render a hyperlink's HREF attribute using a value from a database or user input.

DevExpress grid based controls remove all potentially dangerous contents (for example, "javascript:") from HREF values when rendering hyperlink columns:

![Unsafe Link](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/url-no-encoding.png?raw=true)

This behavior is controlled by a column's `RemovePotentiallyDangerousNavigateUrl` option (true by default):

```cs
settings.Columns.Add(c => {
    ...
    var hyperLinkProperties = c.PropertiesEdit as HyperLinkProperties;
    hyperLinkProperties.RemovePotentiallyDangerousNavigateUrl = DefaultBoolean.False;
});

```

We recommend that you never set this setting to `false` if the URL value in the database can be modified by untrusted parties.

See the example project's [Views/UsingAntiForgeryToken/EditFormPartial.cshtml](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/UsingAntiForgeryToken/EditFormPartial.cshtml#L35) file.

---

## 7. User Input Validation

**Security Risks**: [CWE-20](https://cwe.mitre.org/data/definitions/20.html)

### 7.1 General Recommendations

It is strongly recommended that you validate values obtained from an end user before you save them to a database or use in any other way. The values should be validated on several levels:

1. Specify the required restrictions for inputs on the client.

2. Validate submitted values on the server before saving.

3. Specifies the required data integrity conditions on the database level.

4. Validate the values in the code that directly uses them.

> Note that client validation is for optimization only. To ensure safety, always use client validation in conjunction with server validation.

![Validation Diagram](https://raw.githubusercontent.com/DevExpress/aspnet-security-bestpractices/wiki-static-resources/validation-diagram.png)

The ASP.NET MVC framework allows you to assign validation attributes to model properties. On the client, these attributes are taken into account for all model-bound input elements.

For example, below is a [simple model definition](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Models/UserInputValidationModel.cs#L12):

```cs
public class UserProfile {
    [Required, MaxLength(50)]
    public string Name { get; set; }
```

[View code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/UserInputValidation/General.cshtml):

```cs
@Html.DevExpress().TextBoxFor(m => m.Name, settings => {
    settings.Properties.Caption = "Name";
}).GetHtml()
```

On form submit, the corresponding [controller action](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UserInputValidation/UserInputValidation.cs#L20) will receive the model-bound input's value. Here you can check for the user input's validity using the `ModelState.IsValid` property:

```cs
public ActionResult AdditionalDataAnnotationAttributes(EmployeeItem employeeItem) {
    if(ModelState.IsValid) {
        // DoSomethig(employeeItem);
    }
}
```

On the client, if a user's input does not meet limitations specified by validation attributes, validation warnings are displayed next to corresponding inputs. For the client validation to be performed automatically based on validation attributes, you need to enable client validation and unobtrusive validation. You can do this in **web.config**:

```xml
<add key="ClientValidationEnabled" value="true" />
<add key="UnobtrusiveJavaScriptEnabled" value="true" />
```

The image below demonstrates how validation errors are indicated by DevExpress controls:

![Validation Errors](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/mvc-input-validation.png?raw=true)

### 7.2 Model Binding Restrictions

In many cases, your input forms do not provide access to all properties of the model because some properties are not intended to be edited by a user. For example, the model contains a _salary_ field that should never be modifiable by an end-user.

A malefactor can forge a request that attempts to modify this field (name="Salary" value="100000"):

```html
<form
  name="test"
  method="post"
  action="http://localhost:2088/UserInputValidation/General"
>
  <input type="text" hidden="hidden" name="Name" value="name 1" />
  <input type="text" hidden="hidden" name="Salary" value="100000" />
</form>
<a href="#" onclick="test.submit();">Send Postback</a>
```

You can prohibit model binding for a specific model property using the Bind attribute's Exclude property as [shown below](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Models/UserInputValidationModel.cs#L10):

```cs
[Bind(Exclude = "Salary")]
public class EmployeeItem {
    [Required]
    public string Name { get; set; }
    ...
    public double Salary { get; set; }
}
```

### 7.3 Additional Data Annotation Attributes

In addition to data annotation attributes supported by ASP.NET MVC, DevExpress ASP.NET MVC extensions provide supplementary attributes that can be used for validation both on the server and client:

#### [Mask Check](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Models/UserInputValidationModel.cs#L20)

The `Mask` attribute allows you to check a value against a mask both on the client and server. This attribute allows you to reject invalid values on server even if a malefactor succeeds to get around the mask check on the client.

```cs
[Mask("+1 (999) 000-0000",
  IncludeLiterals = MaskIncludeLiteralsMode.None,
  ErrorMessage = "Invalid Phone Number")]
public string Phone { get; set; }
```

#### [Date Range Check](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Models/UserInputValidationModel.cs#L27)

The `DateRange` attribute ties two DateTime properties together allowing you to check whether a value falls within a specific date range.

```cs
[Display(Name = "Start Date")]
public DateTime StartDate { get; set; }

[Display(Name = "End Date")]
[DateRange(StartDateEditFieldName = "StartDate", MinDayCount = 1, MaxDayCount = 30)]
public DateTime EndDate { get; set; }
```

![Additional Validation Attributes](https://github.com/DevExpress/aspnet-security-bestpractices/blob/wiki-static-resources/mvc-validation-attributes.png?raw=true)

### 7.4 Custom Validation

In some cases, you may need to validate values of inputs that are not bound to a model. For example, you may want to validate a password that a user types as a plain string but that is stored as hash. The password is entered via an editor without model binding:

```cs
@Html.DevExpress().TextBox(settings => {
    settings.Name = "UserPassword";
    settings.Properties.Password = true;
    settings.Properties.Caption = "Password";
}).GetHtml()
```

See the [view code](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/UserInputValidation/General.cshtml#L15-L19).

On server you can access this editor's value through a controller action's parameter with the same name as the input. You can validate this value using any custom logic. If validation did not pass, you can register a validation error using the `ModelState.AddModelError` method. Calling this method automatically sets the `IsValid` property to false.

```cs
public ActionResult General(UserProfile userProfile, string UserPassword) {
    if(ModelState.IsValid) {
        if(IsUserPasswordCorrect(UserPassword)) { // Custom Validation
            DoSomethig(userProfile);
        } else {
            ModelState.AddModelError(nameof(UserPassword), "Password is not correct!");
        }
    }
    return View("General", userProfile);
}
```

See the example project's [Controllers/UserInputValidation/UserInputValidation.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UserInputValidation/UserInputValidation.cs#L22) file.

To display an error message to a user, you need to add a custom markup to the view:

```cs
@{
    var errorState = ViewContext.ViewData.ModelState["UserPassword"];
    if(errorState != null && errorState.Errors.Count > 0) {
        @Html.DevExpress().Label(s => {
            s.Text = errorState.Errors[0].ErrorMessage;
            s.ControlStyle.CssClass = "general-error-text";
        }).GetHtml()
    }
}
```

### 7.5 Validation in List Editors

A special case of custom validation is validation of list items. Consider a use-case scenario when a online shop application allows a user to select a gift option from a list based on the state of their bonus account (see the example project's **UserInputValidation/ListEditors**) page.

To familiarize yourself with the possible vulnerability:

1. Open the example project's **Controllers/UserInputValidation** controller, locate the ListEditors action and comment out the following lines:

   ```cs
   if(ProductItems.GetAvailableForUserList().FindIndex(i=>i.Id == productItemId) != -1) {
   // DoSomethig(productItemId);
   }
   else {
       ModelState.AddModelError("", "Not correct input. Looks like hacker attack.");
   }
   ```

2. Run the project and open the **Static/ComboBoxForgeryTest.html** page.

3. Using this page, submit a forged request with the Id=3, which should not be available to an end user.

To prevent request forgery, you should manually check if the selected list item falls within the allowed range:

```cs
public ActionResult ListEditors(int productItemId) {
    if(ModelState.IsValid) {
        // Custom Validation: combobox value should be in the list
        if(ProductItems.GetAvailableForUserList().FindIndex(i=>i.Id == productItemId) != -1) {
            // DoSomethig(productItemId);
        } else {
            ModelState.AddModelError("", "Not correct input. Looks like hacker attack.");
        }
    }
    return View("ListEditors", productItemId);
}
```

See the example project's [Controllers/UserInputValidation/UserInputValidation.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UserInputValidation/UserInputValidation.cs#L66-L79) file.

### 7.6 Disable Inbuilt Request Value Checks

ASP.NET checks input values for potentially dangerous content. For example, if an end-user types `<b>` into a text input and submits the form, they will be redirected to an error page with the following message:

```
System.Web.HttpRequestValidationException: A potentially dangerous Request value was detected from the client (Property="<b>")
```

In many cases such checks can have undesired side effects:

- An end user cannot enter text containing elements that are common in technical texts, for example "Use the &lt;b&gt; tag to make text bold".

- Requests are intersected before they reach a controller action so you cannot handle them and render user-friendly error messages.

Because of these reasons, the inbuilt request value checks are commonly disabled. You can do this using the `ValidateInputAttribute`:

```cs
[ValidateInput(false)]
public class UserInputValidationController : Controller {
...
```

You can assign this attribute to the controller class or a controller's action method.

> Regardless or whether or not request checks are enabled, you should use encoding to protect your application from XSS attacks. Refer to the [section 6](#6-preventing-cross-site-scripting-xss-attacks-with-encoding) of this document to learn more.

See the example project's [Controllers/UserInputValidation/UserInputValidation.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UserInputValidation/UserInputValidation.cs#L8) file.

### 7.7 Using Non-Action Controller Methods

You should always keep in mind that any public method of a controller can be triggered by an HTTP request. For example, assume that your controller that has the following public method:

```cs
public void ChangePassword(string newPassword) {
    ...
}
```

This method can be accessed from the client as shown below:

```html
<form name="test" method="post" action="/UserInputValidation/ChangePassword">
  <input type="text" hidden="hidden" name="newPassword" value="test1" />
</form>
<a href="#" onclick="test.submit();">Send Postback</a>
```

To mitigate this, assign the `NonActionAttribute` to the method:

```cs
[NonAction]
protected void ChangePassword(string newPassword) {
    ...
}
```

> We strongly recommend not to use non-action methods in controllers. Consider implementing separate classes for such code.

See the example project's [Controllers/UserInputValidation/UserInputValidation.cs](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Controllers/UserInputValidation/UserInputValidation.cs#L81-L84) file.

### 7.8 Inlining SVG Images

SVG markup can contain scripts that will be executed if this SVG is inlined into a page. For example, the code below executes a script embedded into SVG markup:

```html
@{ var svgImageWithJavaScriptCode =
<svg height="100" width="100">
  <circle cx="50" cy="50" r="40" stroke="black" stroke-width="2" fill="red" />
  <script>
    alert('XXS')
  </script></svg
>;

<div style="width:100px;">
  @Html.Raw(svgImageWithJavaScriptCode)
</div>
}
```

Because of this, you should never inline SVG images obtained from untrusted sources.

See the example project's [Views/UserInputValidation/SvgInline.cshtml](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/UserInputValidation/SvgInline.cshtml) file.

---

## 8. Export to CSV

**Security Risks**: [CWE-74](https://cwe.mitre.org/data/definitions/74.html)

Control data exported to CSV can include content that spreadsheet software such as Microsoft Excel interprets as formula. Such formulas can potentially execute shell commands. For example the following formula runs the Windows Calculator when evaluated:

```
=cmd|' /C calc'!'!A1'
```

This may cause a security issue if an end-user opens the exported file in a spreadsheet program and allows the program to run executable content. Microsoft Excel displays a message explicitly asking a user whether or not to run executable content.

To prevent possible security issues, set the `EncodeCsvExecutableContent` property to true. In this mode, the exported content is encoded so it cannot be interpreted as executable.

This behavior is controlled by the following properties:

- At the control level - `DevExpress.XtraPrinting.CsvExportOptions.EncodeExecutableContent`:

  ```cs
  var grid = Html.DevExpress().GridView<EditFormItem>(settings => {
      settings.Name = "grid";
      settings.CallbackRouteValues = new { Controller = "Export", Action = "ExportToCsvPartial" };

      settings.SettingsExport.EnableClientSideExportAPI = true;
      settings.SettingsExport.ExcelExportMode = DevExpress.Export.ExportType.DataAware;
      settings.SettingsExport.BeforeExport = (s, e) => {
          (e.ExportOptions as DevExpress.XtraPrinting.CsvExportOptionsEx).EncodeExecutableContent = DefaultBoolean.True;
      };
  }
  ```

- At the application level (a Global.asax setting) - `DevExpress.Export.ExportSettings.EncodeCsvExecutableContent`:

  ```cs
  DevExpress.Export.ExportSettings.EncodeCsvExecutableContent = DevExpress.Utils.DefaultBoolean.True;
  ```

This setting is not enabled by default because escaped content (everything that starts with "=", "-", "+", "@", "") in CSV can be unacceptable in many use-case scenarios. For example, escaping will brake text values that happed to start with "=" or negative numbers. For example:

```
Product Name,Quantity Per Unit,Unit Price,Units In Stock, Status
"""=Chai""",10 boxes x 20 bags,$18.00,39,$702.00, """-10%"""
Chang,24 - 12 oz bottles,$19.00,17,$323.00, 5%
```

Because Excel requires a user's permission to run executable content, we do not enable this setting by default and allow a user to enable this settings if it suites the use case.

See the following article to learn more about CSV injections: [https://www.owasp.org/index.php/CSV_Injection](https://www.owasp.org/index.php/CSV_Injection)

---

## 8. Unauthorized Server Operation via Client-Sided API

**Security Risks**: [CWE-284](https://cwe.mitre.org/data/definitions/284.html), [CWE-285](https://cwe.mitre.org/data/definitions/285.html)

### 9.1. Unauthorized CRUD Operations in the View Mode

**Related Extension**: Grid View, Card View, Vertical Grid, Tree List

Grid-based controls (Grid View, Card View, etc.) expose client methods that trigger CRUD operations on the server. For example, you can call call the [ASPxClientGridView.DeleteRow](https://docs.devexpress.com/AspNet/js-ASPxClientGridView.DeleteRow(visibleIndex)) method on the client to delete a grid row. If a control is configured incorrectly, these methods can be used to alter data even if the control is intended to display data in view-only mode (the data editting UI is hidden).

The best practices to mitigate this vulnerability are:

- If you intend to use a grid-based control in view-only mode, make sure that the it does not have the DeleteRowRouteValues setting specified.

  ```cs
  //settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "ClientSideApi", Action = "GridViewDeletePartial" };
  ```

- The controller does not have CRUD method as the one shown below:

  ```cs
  //[HttpPost]
  //public ActionResult GridViewDeletePartial(int id = -1) {
  //    if(id >= 0)
  //        EditFormItems.Delete(id);
  //    return GridViewPartial();
  //}
  ```

> Note that an attacker can call controller methods without a View by making a POST request, as demonstrated in the example application:

<span style="color: red">IMAGE</span>

### 9.2. Using Spreadsheet in Read-Only Mode


If you intend the Spreadsheet control to work in read-only mode (the `SettingsView.Mode` option is set to "Reading"), make sure that the ability to switch to edit mode is disabled:

```cs
settings.Settings.Behavior.SwitchViewModes = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
```

It is also recommended that you set the `ReadOnly` property to true:

```cs
settings.ReadOnly = true;
```

See the example projects [Views/ClientSideApi/SpreadsheetReadingModeOnlyPartial.cshtml](https://github.com/DevExpress/aspnet-security-bestpractices/blob/master/SecurityBestPractices.Mvc/SecurityBestPractices.Mvc/Views/ClientSideApi/SpreadsheetReadingModeOnlyPartial.cshtml#L16-L18) file.


### 9.3 File Selector Commands in the ReachEdit and Spreadsheet

In one of the popular use case scenarios, the ReachEdit or Spreadsheet control's **File** tab is hidden to prevent an end-user from accessing the FileSelector's commands (New, Open, Save, etc.) In this case, a document is opened and saved programmatically. 

Note that it is not enough to just hide the File tab because the commands from this tab can still be executed using JavaScript or keyboard shortcuts (for example, the Ctrl+O shortcut can invoke the Open dialog).

When you want to disable file-related commands, the best practice is to also disable file operations by disabling the corresponding Behavior options (CreateNew, Open, Save, SaveAs, SwitchViewModes).

##### Spreadsheet 

```cs
// Disable File Selector operations
settings.Settings.Behavior.CreateNew = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
settings.Settings.Behavior.Open = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
settings.Settings.Behavior.Save = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
settings.Settings.Behavior.SaveAs = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
```

##### Rich Edit

```cs
// Disable File Selector operations
settings.Settings.Behavior.CreateNew = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
settings.Settings.Behavior.Open = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
settings.Settings.Behavior.Save = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
settings.Settings.Behavior.SaveAs = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
```

---

![Analytics](https://ga-beacon.appspot.com/UA-129603086-1/aspnet-security-bestpractices-mvc-page?pixel)
