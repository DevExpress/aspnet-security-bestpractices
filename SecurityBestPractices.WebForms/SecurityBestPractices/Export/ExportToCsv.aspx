<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportToCsv.aspx.cs" Inherits="SecurityBestPractices.Export.ExportToCsv" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" KeyFieldName="ProductID" ClientInstanceName="gridView" OnBeforeExport="ASPxGridView1_BeforeExport">
            <SettingsExport EnableClientSideExportAPI="true" ExcelExportMode="DataAware" />

            <SettingsPager PageSize="5">
            </SettingsPager>
            <SettingsDataSecurity AllowEdit="False" AllowInsert="False" AllowDelete="False" AllowReadUnexposedColumnsFromClientApi="False" />
            <Columns>
                <dx:GridViewCommandColumn VisibleIndex="0">
                </dx:GridViewCommandColumn>
                <dx:GridViewDataTextColumn FieldName="ProductID" ReadOnly="True" VisibleIndex="1">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="ProductName" VisibleIndex="2">
                </dx:GridViewDataTextColumn>

                <dx:GridViewDataTextColumn FieldName="StatusNumeric" Caption="Status (Numeric)" VisibleIndex="3" />
                <dx:GridViewDataTextColumn FieldName="StatusText" Caption="Status (Text)" VisibleIndex="4" />
            </Columns>
            <Toolbars>
                <dx:GridViewToolbar>
                    <Items>
                        <dx:GridViewToolbarItem Command="ExportToCsv"></dx:GridViewToolbarItem>
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
        </dx:ASPxGridView>

        <hr />
        <dx:ASPxButton ID="Button" runat="server" Text="Export to CSV" OnClick="Button_Click" />


        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:nwindConnection %>" ProviderName="<%$ ConnectionStrings:nwindConnection.ProviderName %>" SelectCommand="SELECT [ProductID], [ProductName], [StatusNumeric], [StatusText] FROM [Products]"></asp:SqlDataSource>
    </form>
</body>
</html>
