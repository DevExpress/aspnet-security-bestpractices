<%@ Page Title="Using AntiForgeryToken" Language="C#" MasterPageFile="~/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master" AutoEventWireup="true" CodeBehind="EditForm.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgeryToken.EditForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Editable Grid</h2>
    <dx:ASPxGridView ID="GridView" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" KeyFieldName="ProductID">
        <SettingsAdaptivity>
            <AdaptiveDetailLayoutProperties ColCount="1">
            </AdaptiveDetailLayoutProperties>
        </SettingsAdaptivity>
        <SettingsPager PageSize="5">
        </SettingsPager>
        <EditFormLayoutProperties ColCount="1">
        </EditFormLayoutProperties>
        <Columns>
            <dx:GridViewCommandColumn ShowEditButton="True" ShowDeleteButton="true" VisibleIndex="0">
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="ProductID" ReadOnly="True" VisibleIndex="1">
                <EditFormSettings Visible="False" />
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="ProductName" VisibleIndex="2">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="UnitPrice" VisibleIndex="3">
            </dx:GridViewDataTextColumn>
        </Columns>
        <ClientSideEvents Init="function(s,e){ s.StartEditRow(0); }" />
    </dx:ASPxGridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:nwindConnection %>" DeleteCommand="DELETE FROM [Products] WHERE [ProductID] = ?" InsertCommand="INSERT INTO [Products] ([ProductID], [ProductName], [UnitPrice]) VALUES (?, ?, ?)" ProviderName="<%$ ConnectionStrings:nwindConnection.ProviderName %>" SelectCommand="SELECT [ProductID], [ProductName], [UnitPrice] FROM [Products]" UpdateCommand="UPDATE [Products] SET [ProductName] = ?, [UnitPrice] = ? WHERE [ProductID] = ?">
        <DeleteParameters>
            <asp:Parameter Name="ProductID" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="ProductID" Type="Int32" />
            <asp:Parameter Name="ProductName" Type="String" />
            <asp:Parameter Name="UnitPrice" Type="Decimal" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="ProductName" Type="String" />
            <asp:Parameter Name="UnitPrice" Type="Decimal" />
            <asp:Parameter Name="ProductID" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <br />
    <hr />
    <br />
    <h2>Account Details</h2>
    <dx:ASPxTextBox ID="EmailTextBox" runat="server" Caption="Email">
        <ValidationSettings RequiredField-IsRequired="true"></ValidationSettings>
    </dx:ASPxTextBox>
    <br />
    <br />
    <dx:ASPxButton ID="UpdateButton" runat="server" Text="Update" OnClick="UpdateButton_Click" />
    <br />
    <dx:ASPxLabel runat="server" ID="UpdateStatusLabel" />
</asp:Content>
