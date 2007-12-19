<%@ Control Language="c#" Inherits="Rainbow.Modules.Contacts.ContactsMobile" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/Mobile/WebUserControl" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>
<%@ Register TagPrefix="rMobile" Namespace="Rainbow.UI.MobileControls" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="traMobile" Namespace="Rainbow.UI.MobileControls.Globalized" Assembly="Rainbow.Mobile" %>
<rMobile:StyleSheet Id="MobileStyleSheet" runat="server" />
<mobile:Panel runat="server" id="summary">
	<mobile:DeviceSpecific id="DeviceSpecific1" runat="server">
		<CHOICE Filter="supportsJavaScript">
			<CONTENTTEMPLATE>
				<rMobile:MobileTitle id="Mobiletitle1" runat="server" StyleReference="Style_Title" />
				<traMobile:LinkCommand id=LinkCommand1 runat="server"  OnClick="SummaryView_OnClick" TextKey="MOBILE_CLICK_MORE_FOR_CONTACTS" Text="Click here to access the directory of contacts." StyleReference="Style_Link" />
				<BR>
			</CONTENTTEMPLATE>
		</CHOICE>
	</mobile:DeviceSpecific>
</mobile:Panel>
<rMobile:MultiPanel runat="server" id="MainView" StyleReference="Style_Module">
	<rMobile:ChildPanel id="ContactsList" runat="server">
		<rMobile:MobileTitle id="Mobiletitle2" runat="server" StyleReference="Style_Title" />
		<mobile:List id=List1 runat="server" DataSource="<%# ds %>" DataTextField="Name" OnItemCommand="ContactsList_OnItemCommand">
			<DeviceSpecific>
				<Choice Filter="supportsJavaScript">
					<HeaderTemplate>
						<table width="95%" border="0" cellspacing="0" cellpadding="0">
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td><a href='<%# "mailto:" + DataBinder.Eval(((MobileListItem)Container).DataItem, "Email") %>'>
									<%# DataBinder.Eval(((MobileListItem)Container).DataItem, "Name") %>
									</a>
							</td>
							<td align="right"><traMobile:LinkCommand id="LinkCommand2"  runat="server" Text="more" TextKey="MORE" StyleReference="Style_Link" /></td>
						</tr>
					</ItemTemplate>
					<FooterTemplate>
						</table>
						<traMobile:LinkCommand id="Linkcommand3" StyleReference="Style_Link"  onclick="MainView_OnClick" runat="server" Text="back" TextKey="BACK"></traMobile:LinkCommand>
					</FooterTemplate>
				</Choice>
			</DeviceSpecific>
		</mobile:List>
	</rMobile:ChildPanel>
	<rMobile:ChildPanel id="ContactDetails" runat="server"  StyleReference="Style_Module">
		<rMobile:MobileTitle id=Mobiletitle3 runat="server" StyleReference="Style_Title" Text='<%# FormatChildField("Name") %>' />
		<traMobile:Label  StyleReference="Style_SubTitle" id="LabelA1" runat="server" Text="Role" breakAfter="false" TextKey="ROLE"/>: 
		<mobile:Label id=Label1 StyleReference="Style_Text" runat="server" Text='<%# FormatChildField("Role") %>' />
		<traMobile:Label id="Label2" StyleReference="Style_SubTitle" runat="server" Text="Email" breakAfter="false" TextKey="EMAIL" />: 
		<mobile:Link id=Link1 StyleReference="Style_Text"  runat="server" Text='<%# FormatChildField("Email") %>' NavigateUrl='<%# "mailto:" + FormatChildField("Email") %>' />
		<traMobile:Label id="Label3" StyleReference="Style_SubTitle" runat="server" Text="Contacts" breakAfter="false" TextKey="CONTACTS"></traMobile:Label>:<br>
		<mobile:PhoneCall id=PhoneCall1 StyleReference="Style_Link" runat="server" Text='<%# FormatChildField("Contact1") %>' AlternateFormat="{0}" PhoneNumber='<%# GetPhoneNumber(FormatChildField("Contact1")) %>' Visible='<%# FormatChildField("Contact1") != String.Empty %>' />
		<mobile:PhoneCall id=PhoneCall2 StyleReference="Style_Link"  runat="server" Text='<%# FormatChildField("Contact2") %>' AlternateFormat="{0}" PhoneNumber='<%# GetPhoneNumber(FormatChildField("Contact2")) %>' Visible='<%# FormatChildField("Contact2") != String.Empty %>' />
		<traMobile:LinkCommand id="LinkCommand4" StyleReference="Style_Link"  onclick="DetailsView_OnClick" runat="server" Text="back" TextKey="BACK"></traMobile:LinkCommand>
	</rMobile:ChildPanel>
</rMobile:MultiPanel>
