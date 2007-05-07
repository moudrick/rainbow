<%@ Page Language="c#" Inherits="Rainbow.MobileDefault" AutoEventWireup="False" EnableViewState="True" enableViewStateMac="False"%>
<%@ Register TagPrefix="rMobile" Namespace="Rainbow.UI.MobileControls" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="traMobile" Namespace="Rainbow.UI.MobileControls.Globalized" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile" %>
<body Xmlns:mobile="http://schemas.microsoft.com/Mobile/WebForm">
	<rMobile:StyleSheet Id="MobileStyleSheet" runat="server" /><mobile:form 
					id="PortalForm" 
					runat="server" 
					StyleReference="Style_Form" 
					Paginate="True" 
					Method="Post" 
					PagerStyle-StyleReference="Style_Pager"
					PagerStyle-PreviousPageText="Previous Page"
					PagerStyle-NextPageText="Next Page" 
					>
	<mobile:DeviceSpecific id=DeviceSpecific1 runat="server">
		<Choice Filter="isHTML32">
			<HEADERTEMPLATE>
				<mobile:Label id="Label1" runat="server"  StyleReference="Style_PortalTitle"><%=portalSettings.PortalName%> [title]</mobile:Label>
				[title2]<%=portalSettings.PortalName%>
				<TABLE width="100%" height="100%" cellSpacing="0" cellPadding="0" border="0">
					<tr>
						<td width="1"><rMobile:Image id="Image1" runat="server" imageUrl="~/images/spacer.gif" Visible="true" EnableViewState="False" /></td>
						<td width="100%" vAlign="top">
			</HEADERTEMPLATE>
			<FOOTERTEMPLATE>
						</td>
						<td width="1"><rMobile:Image id="Image2" runat="server" imageUrl="~/images/spacer.gif" Visible="true" EnableViewState="False" /></td>
					</tr>
					<TR><td colspan="3" 
						vAlign="top"><hr style="height:1px">
							<rMobile:Image id="Image4" runat="server" ImageUrl="~~/MobRain.gif"  Visible="true" Alignment=Center /></td>
					</TR>
				</TABLE>
			</FOOTERTEMPLATE>
		</Choice>		
	</mobile:DeviceSpecific>
	<rMobile:TabbedPanel id=TabView runat="server" Paginate="False" StyleReference="Style_TabView" BreakAfter="True" TabsPerRow="7" TabStyleReference="Style_Tabs" ActiveTabStyleReference="Style_ActiveTab"></rMobile:TabbedPanel>
	</mobile:form>
</body>
