<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DangerousNavigateUrl.aspx.cs" Inherits="SecurityBestPractices.ClientSideApi.DangerousNavigateUrl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" KeyFieldName="ID" ClientInstanceName="gridView">
            <SettingsPager PageSize="5">
            </SettingsPager>
            <SettingsDataSecurity AllowEdit="False" AllowInsert="False" AllowDelete="False" AllowReadUnexposedColumnsFromClientApi="False" />
            <Columns>
                <dx:GridViewDataTextColumn FieldName="ID" ReadOnly="True" VisibleIndex="0">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataHyperLinkColumn FieldName="Url" VisibleIndex="1" Caption="HyperLinkColumn">
                    <PropertiesHyperLinkEdit RemovePotentiallyDangerousNavigateUrl="true" TextField="Caption" />
                </dx:GridViewDataHyperLinkColumn>
                <dx:GridViewDataTextColumn FieldName="Url" VisibleIndex="2">
                </dx:GridViewDataTextColumn>
            </Columns>
        </dx:ASPxGridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:nwindConnection %>" ProviderName="<%$ ConnectionStrings:nwindConnection.ProviderName %>" SelectCommand="SELECT * FROM [Links]">
        </asp:SqlDataSource>
    </form>
</body>
</html>
