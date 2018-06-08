<%@ Page Title="" Language="C#" MasterPageFile="~/UsingAntiForgegyToken/MasterPageWithAntiForgeryToken.Master" AutoEventWireup="true" CodeBehind="EditableControl.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgegyToken.EditableControl" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" namespace="DevExpress.Web" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" KeyFieldName="CategoryID">
        <SettingsAdaptivity>
            <AdaptiveDetailLayoutProperties ColCount="1">
            </AdaptiveDetailLayoutProperties>
        </SettingsAdaptivity>
        <EditFormLayoutProperties ColCount="1">
        </EditFormLayoutProperties>
        <Columns>
            <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0">
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="CategoryID" ReadOnly="True" VisibleIndex="1">
                <EditFormSettings Visible="False" />
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="CategoryName" VisibleIndex="2">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="3">
            </dx:GridViewDataTextColumn>
        </Columns>
    </dx:ASPxGridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\nwind.mdb;Persist Security Info=True" DeleteCommand="DELETE FROM [Categories] WHERE [CategoryID] = ?" InsertCommand="INSERT INTO [Categories] ([CategoryID], [CategoryName], [Description], [Picture], [Icon_17], [Icon_25]) VALUES (?, ?, ?, ?, ?, ?)" ProviderName="System.Data.OleDb" SelectCommand="SELECT * FROM [Categories]" UpdateCommand="UPDATE [Categories] SET [CategoryName] = ?, [Description] = ?, [Picture] = ?, [Icon_17] = ?, [Icon_25] = ? WHERE [CategoryID] = ?">
        <DeleteParameters>
            <asp:Parameter Name="CategoryID" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="CategoryID" Type="Int32" />
            <asp:Parameter Name="CategoryName" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Picture" Type="Object" />
            <asp:Parameter Name="Icon_17" Type="Object" />
            <asp:Parameter Name="Icon_25" Type="Object" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="CategoryName" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Picture" Type="Object" />
            <asp:Parameter Name="Icon_17" Type="Object" />
            <asp:Parameter Name="Icon_25" Type="Object" />
            <asp:Parameter Name="CategoryID" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
</asp:Content>
