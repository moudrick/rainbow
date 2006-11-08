<%@ Control AutoEventWireup="false" CodeFile="Pages.ascx.cs" Inherits="Rainbow.Content.Web.Modules.Pages" Language="c#" %>
<table cellpadding="5" cellspacing="0">
    <tr>
        <td class="SubHead" colspan="3">
            <rbfwebui:Label ID="lblHead" runat="server" Text="Pages" TextKey="AM_TABS" />
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:ListBox ID="tabList" runat="server" CssClass="NormalTextBox" DataSource="<%# portalPages %>"
                DataTextField="Name" DataValueField="ID" Rows="20" Width="400" /></td>
        <td>&nbsp;</td>
        <td>
            <table>
                <tr>
                    <td>
                        <rbfwebui:ImageButton ID="upBtn" runat="server" CommandName="up" text="Move up" TextKey="MOVE_UP" OnClick="UpDown_Click" /></td>
                </tr> 
                <tr>
                    <td>
                        <rbfwebui:ImageButton ID="downBtn" runat="server" CommandName="down" text="Move down" TextKey="MOVE_DOWN" OnClick="UpDown_Click" /></td>
                </tr>
                <tr>
                    <td>
                        <rbfwebui:ImageButton ID="EditBtn" runat="server" CommandName="Edit" text="Edit button" TextKey="EDITBTN" OnClick="EditBtn_Click"  /></td>
                </tr>
                <tr>
                    <td>
                        <rbfwebui:ImageButton ID="DeleteBtn" runat="server" CommandName="Delete" text="Delete button" TextKey="DELETEBTN" OnClick="DeleteBtn_Click" /></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="3">
            <rbfwebui:LinkButton ID="addBtn" runat="server" CssClass="CommandButton" Text="Add New Page"
                TextKey="ADDPAGE" OnClick="AddPage_Click" /></td>
    </tr>
</table>
