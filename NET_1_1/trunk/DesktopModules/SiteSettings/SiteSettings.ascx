<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="cnf" Namespace="Rainbow.Configuration" Assembly="Rainbow" %>
<%@ Control Inherits="Rainbow.DesktopModules.SiteSettings" CodeBehind="SiteSettings.ascx.cs" Language="c#" AutoEventWireup="false" targetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0">
	<tr>
		<td width="140" class="Subhead" vAlign="top">
			<tra:Literal TextKey="SITESETTINGS_SITE_TITLE" id="site_title" Text="Site Title" runat="server"></tra:Literal>
		</td>
		<td colspan="2" class="NormalTextBox">
			<asp:Textbox id="siteName" runat="server" width="240" CssClass="NormalTextBox"></asp:Textbox>
		</td>
	</tr>
	<TR>
		<td class="Subhead" vAlign="top" width="140">
			<tra:Literal TextKey="SITESETTINGS_SITE_PATH" Text="Site Path" id="site_path" runat="server"></tra:Literal>
		</td>
		<td class="Normal" colSpan="2">
			<asp:Label id="sitePath" width="240" runat="server" />
		</td>
	</TR>
</table>
<cnf:SettingsTable id="EditTable" runat="server"></cnf:SettingsTable>
<tra:LinkButton id="updateButton" class="CommandButton" TextKey="APPLY" Text="Apply Changes" runat="server" />
<script type="text/javascript" language="javascript">
	var tpg1 = new xTabPanelGroup('tpg1', 700, 350, 50, 'tabPanel', 'tabGroup', 'tabDefault', 'tabSelected');
	var tabgroupd = xGetElementById('tpg1');
	var pareTbl = xGetElementById(tabgroupd.id);
	//pareTbl.style.posHeight= (xHeight(tabgroupd)+50);
	pareTbl.style["height"] = 380;
	pareTbl.style["overflow"] = 'hidden';
	//xResizeTo(pareTbl, xWidth(pareTbl), 380);
//	alert(xHeight(tabgroupd));
</script>
