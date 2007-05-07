<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.Modules.Announcements.AnnouncementsMobile" TargetSchema="http://schemas.microsoft.com/Mobile/WebUserControl" %>
<%@ Register TagPrefix="rMobile" Namespace="Rainbow.UI.MobileControls" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="traMobile" Namespace="Rainbow.UI.MobileControls.Globalized" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>
<rMobile:StyleSheet Id="MobileStyleSheet" runat="server" />
<mobile:panel id="summary" runat="server">
	<mobile:DeviceSpecific id="DeviceSpecific1" runat="server">
		<Choice Filter="supportsJavaScript">
			<CONTENTTEMPLATE>
				<rMobile:MobileTitle id="Title1" runat="server" StyleReference="Style_Title" />
					<asp:Repeater id=AnnouncementListRepeater runat="server" OnItemCommand="SummaryView_OnItemCommand" DataSource="<%# ds %>">
						<ItemTemplate>
							<rMobile:LinkCommand runat="server" StyleReference="Style_Link" text='<%#DataBinder.Eval(Container.DataItem, "Title") %>' ></rMobile:LinkCommand>
						</ItemTemplate>
					</asp:Repeater>
				<BR>
			</CONTENTTEMPLATE>
		</Choice>
	</mobile:DeviceSpecific>
</mobile:panel>
<rMobile:multipanel id="MainView" runat="server" StyleReference="Style_Module">
	<rMobile:ChildPanel id="AnnouncementsList" runat="server">
		<rMobile:MobileTitle id="multipanelTitle1" runat="server" StyleReference="Style_Title" />
		<mobile:List id=List1 runat="server" StyleReference="Style_Text" DataTextField="Title" DataValueField= DataSource="<%# ds %>"  OnItemCommand="AnnouncementsList_OnItemCommand">
			<Item>
				<rMobile:LinkCommand id="Linkcommand1" StyleReference="Style_Link" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Title") %>'></rMobile:LinkCommand>
			</Item>
		</mobile:List>
	</rMobile:ChildPanel>
	<rMobile:ChildPanel id="AnnouncementDetails" runat="server">
		<rMobile:MobileTitle id=multipanelTitle2 runat="server" StyleReference="Style_Title"></rMobile:MobileTitle>
		<mobile:Label id="Label" runat="server" StyleReference="Style_SubTitle" Text='<%#FormatChildField("Title") %>'></mobile:Label>
		<mobile:TextView id=TextView1 runat="server" StyleReference="Style_Text" Text='<%# FormatChildField("Description") %>'>
		</mobile:TextView>
		<traMobile:Link id=LinkMore runat="server" Text="read more"  StyleReference="Style_Link" NavigateUrl='<%# FormatChildField("MobileMoreLink") %>' Visible='<%# FormatChildField("MobileMoreLink") != String.Empty %>' TextKey="MORE" />
		<traMobile:LinkCommand id="LinkBack" onclick="DetailsView_OnClick"  StyleReference="Style_Link" runat="server" Text="back" TextKey="BACK"></traMobile:LinkCommand>
	</rMobile:ChildPanel>
</rMobile:multipanel>
