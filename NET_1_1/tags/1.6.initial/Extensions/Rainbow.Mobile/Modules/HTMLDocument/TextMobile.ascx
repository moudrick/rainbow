<%@ Control Language="c#" Inherits="Rainbow.Modules.Text.TextMobile" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/Mobile/WebUserControl" %>
<%@ Register TagPrefix="traMobile" Namespace="Rainbow.UI.MobileControls.Globalized" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="rMobile" Namespace="Rainbow.UI.MobileControls" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>
<rMobile:StyleSheet Id="MobileStyleSheet" runat="server" />
<mobile:Panel id="summary" runat="server" BreakAfter="True" StyleReference="Style_Module">
	<mobile:DeviceSpecific id="DeviceSpecific1" runat="server">
		<Choice Filter="supportsJavaScript">
			<CONTENTTEMPLATE>
				<rMobile:MobileTitle runat="server" id="MobileTitle1" BreakAfter="true"  StyleReference="Style_Title" />
				<mobile:TextView ID=mobileSummary StyleReference="Style_Text" Runat="server"><%# mobileSummary %></mobile:TextView>
				<traMobile:LinkCommand id=LinkCommand2 runat="server" CommandName="COMMAND_DETAILS" Text="more" TextKey="MORE" StyleReference="Style_Link" Visible="<%# mobileDetails != String.Empty %>" />
				<BR />
			</CONTENTTEMPLATE>
		</Choice>
	</mobile:DeviceSpecific>
</mobile:Panel>
<rMobile:MobileTitle runat="server" id="MobileTitle2" BreakAfter="true" StyleReference="Style_Title" />
<mobile:TextView runat="server" Text="<%# mobileDetails %>" StyleReference="Style_Text" id=TextView1 />
<traMobile:LinkCommand runat="server" Text="back" TextKey="BACK" CommandName="COMMAND_SUMMARY" id="LinkCommand1" StyleReference="Style_Link" />
