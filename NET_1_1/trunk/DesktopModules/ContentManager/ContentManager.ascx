<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Control Language="c#" Inherits="Rainbow.DesktopModules.ContentManager" codebehind="ContentManager.ascx.cs" autoeventwireup="false" %>
<table align="center" border="0" width="600" cellpadding="0" cellspacing="0">
	<TR>
		<TD width="250">
			<SPAN class="ItemTitle">ModuleTypes</SPAN>
		</TD>
		<td width="120">&nbsp;</td>
		<TD></TD>
	</TR>
	<TR>
		<TD width="250">
			<asp:DropDownList id="ModuleTypes" AutoPostBack="true" runat="server" width="250px" style="width:250px;overflow-y:auto;overflow-x:hidden;"></asp:DropDownList>
		</TD>
		<td width="120">&nbsp;</td>
		<TD></TD>
	</TR>
	<TR>
		<td colspan="2">
			<table runat="server" id="MultiPortalTable" align="center" border="0" width="250">
				<tr>
					<TD width="250">
						<asp:label class="ItemTitle" id="SourcePortalLabel" runat="server" Text="Source Portal"></asp:label></TD>
					<td width="120">&nbsp;</td>
					<TD width="250">
						<asp:label class="ItemTitle" id="DestinationPortalLabel" runat="server" Text="Destination Portal"></asp:label></TD>
				</tr>
				<TR>
					<TD width="250">
						<asp:DropDownList id="SourcePortal" AutoPostBack="true" runat="server" width="250px" style="width:250px;overflow-y:auto;overflow-x:hidden;"></asp:DropDownList></TD>
					<td width="120">&nbsp;</td>
					<TD width="250">
						<asp:DropDownList id="DestinationPortal" AutoPostBack="true" runat="server" width="250px" style="width:250px;overflow-y:auto;overflow-x:hidden;"></asp:DropDownList></TD>
				</TR>
			</table>
		</td>
	<TR>
		<TD width="250"><SPAN class="ItemTitle">Source Module</SPAN>
		</TD>
		<td width="120">&nbsp;</td>
		<TD width="250" align="right">
			<SPAN class="ItemTitle">Destination Module</SPAN>
		</TD>
	</TR>
	<TR>
		<TD>
			<asp:DropDownList id="SourceInstance" AutoPostBack="true" runat="server" width="250px" style="width:250px;overflow-y:auto;overflow-x:hidden;"></asp:DropDownList>
		</TD>
		<td width="120">&nbsp;</td>
		<TD align="right">
			<asp:DropDownList id="DestinationInstance" AutoPostBack="true" runat="server" width="250px" style="width:250px;overflow-y:auto;overflow-x:hidden;"></asp:DropDownList>
		</TD>
	</TR>
</table>
<table align="center" border="0" width="600" cellpadding="0" cellspacing="0">
	<tr>
		<td>
			<span class="ItemTitle">Source</span>
		</td>
		<td width="120">&nbsp;</td>
		<td>
			<span class="ItemTitle">Destination</span>
		</td>
	</tr>
	<tr>
		<td valign="top" Width="250">
			<asp:ListBox id="SourceListBox" style="width:250px;overflow-y:auto;overflow-x:hidden;" width="250px"
				rows="15" runat="server" />
		</td>
		<td valign="top" width="120">
			<P>
				<tra:LinkButton id="DeleteLeft_Btn" cssclass="CommandButton" width="120px" TextKey="DeleteLeft"
					Text="<- Delete" runat="server"></tra:LinkButton></P>
			<P>
				<tra:LinkButton id="MoveLeft_Btn" cssclass="CommandButton" width="120px" TextKey="MoveItemLeft"
					Text="<- Move" runat="server"></tra:LinkButton></P>
			<P>
				<tra:LinkButton id="MoveRight_Btn" cssclass="CommandButton" width="120px" TextKey="MoveItemRight"
					Text="Move ->" runat="server"></tra:LinkButton></P>
			<P>
				<tra:LinkButton id="CopyRight_Btn" cssclass="CommandButton" width="120px" TextKey="CopyItem" Text="Copy ->"
					runat="server"></tra:LinkButton></P>
			<P>
				<tra:LinkButton id="CopyAll_Btn" cssclass="CommandButton" width="120px" TextKey="CopyAll" Text=" Copy All ->"
					runat="server"></tra:LinkButton></P>
			<P>
				<tra:LinkButton id="DeleteRight_Btn" cssclass="CommandButton" width="120px" TextKey="DeleteRight"
					Text="Delete->" runat="server"></tra:LinkButton></P>
		</td>
		<td valign="top" Width="250">
			<asp:ListBox id="DestListBox" style="width:250px;overflow-y:auto;overflow-x:hidden;" width="250px"
				rows="15" runat="server" />
		</td>
	</tr>
</table>
