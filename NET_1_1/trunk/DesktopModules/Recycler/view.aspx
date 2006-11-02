<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Page CodeBehind="view.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="Rainbow.recyclerViewPage" %>
<HTML>
	<HEAD id="htmlHead" runat="server">
	</HEAD>
	<body>
		<form runat="server">
			<asp:Panel Runat="server" ID="pnlMain" Visible="True">
<TABLE class="PrintPage" cellSpacing="0" cellPadding="0" width="100%">
					<TR>
						<TD>
							<asp:placeholder id="PrintPlaceHolder" runat="server"></asp:placeholder></TD>
					</TR>
				</TABLE>
<asp:LinkButton id="updateButton" runat="server" Cssclass="CommandButton" Text="Update"></asp:LinkButton>
<asp:DropDownList id=ddTabs Runat="server" DataValueField="TabID" DataTextField="TabName" DataSource="<%# portalTabs %>" OnSelectedIndexChanged="ddTabs_SelectedIndexChanged" AutoPostBack="True">
				</asp:DropDownList>&nbsp; 
<asp:LinkButton id="restoreButton" onclick="restoreButton_Click" Runat="server" Text="Restore" CssClass="CommandButton"></asp:LinkButton>&nbsp; 
<asp:LinkButton id="cancelButton" runat="server" Cssclass="CommandButton" Text="Cancel" CausesValidation="False"></asp:LinkButton>&nbsp; 
<asp:LinkButton id="deleteButton" runat="server" Cssclass="CommandButton" Text="Delete this item"
					CausesValidation="False"></asp:LinkButton>
<HR width="500" noShade SIZE="1">
			</asp:Panel>
			<asp:Panel Runat="server" ID="pnlError" Visible="False">
				<tra:Label id="Label1" runat="server" CssClass="Head" TextKey="ERROR_403">Access / Edit rights have been denied</tra:Label>
			</asp:Panel>
		</form>
	</body>
</HTML>
