<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="PlainTextFilesEncryptionPortal.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH_Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Content" runat="server">

    <div style="text-align:center">
        <asp:Label ID="status_labe" runat="server" Text=""></asp:Label>
    </div>

    <div style="text-align:right;margin-right:14px">
    </div>

    <div style="text-align:center">

        <br />
        <asp:FileUpload ID="FileUpload" runat="server" />

        <br />
        <br />

        <asp:Button ID="btn_upload" runat="server" Text="Upload" OnClick="btn_upload_Click" />
    </div>
</asp:Content>
