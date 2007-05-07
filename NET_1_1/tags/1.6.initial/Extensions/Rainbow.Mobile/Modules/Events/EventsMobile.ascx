<%@ Control Language="c#" Inherits="Rainbow.Modules.Events.EventsMobile" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/Mobile/WebUserControl" %>
<%@ Register TagPrefix="traMobile" Namespace="Rainbow.UI.MobileControls.Globalized" Assembly="Rainbow.Mobile" %>
<%@ Register TagPrefix="mobile" Namespace="System.Web.UI.MobileControls" Assembly="System.Web.Mobile, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>
<%@ Register TagPrefix="rMobile" Namespace="Rainbow.UI.MobileControls" Assembly="Rainbow.Mobile" %>
<rMobile:StyleSheet Id="MobileStyleSheet" runat="server" />
<mobile:Panel runat="server" id="summary">
	<mobile:DeviceSpecific id="DeviceSpecific1" runat="server">
		<CHOICE Filter="supportsJavaScript">
			<CONTENTTEMPLATE>
				<rMobile:MobileTitle id="Mobiletitle3" runat="server" StyleReference="Style_Title"></rMobile:MobileTitle>
					<asp:Repeater id=Repeater1 runat="server" OnItemCommand="SummaryView_OnItemCommand" DataSource="<%# ds %>">
						<ItemTemplate>
							<mobile:Label id="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>' StyleReference="Style_SubTitle"></mobile:Label>
							<mobile:Label id="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "WhereWhen") %>' StyleReference="Style_Text"></mobile:Label>
							<traMobile:LinkCommand id=LinkCommand1 runat="server" Text="more" TextKey="MORE" StyleReference="Style_Link" />
						</ItemTemplate>
					</asp:Repeater>
			</CONTENTTEMPLATE>
		</CHOICE>
	</mobile:DeviceSpecific>
</mobile:Panel>
<rMobile:MultiPanel runat="server" id="MainView" >
	<rMobile:ChildPanel id="EventsList" runat="server">
		<rMobile:MobileTitle id="Mobiletitle1" runat="server" StyleReference="Style_Title"></rMobile:MobileTitle>
		<mobile:List id=List1 runat="server" OnItemCommand="EventsList_OnItemCommand" DataSource="<%# ds %>" DataTextField="Title" StyleReference="Style_Text"></mobile:List>
	</rMobile:ChildPanel>
	<rMobile:ChildPanel id="EventDetails" runat="server">
		<rMobile:MobileTitle id="Mobiletitle2" runat="server" StyleReference="Style_Title"></rMobile:MobileTitle>
		<mobile:Label id=Label3 runat="server" Text='<%# FormatChildField("Title") %>' StyleReference="Style_SubTitle"></mobile:Label>
		<mobile:Label id=Label4 runat="server" Text='<%# FormatChildField("WhereWhen") %>' StyleReference="Style_Text"></mobile:Label>
		<mobile:TextView id=TextView1 runat="server" Text='<%# FormatChildField("Description") %>' StyleReference="Style_Text"></mobile:TextView>
		<traMobile:LinkCommand id="LinkCommand2" onclick="DetailsView_OnClick" runat="server" Text="back" TextKey="BACK" StyleReference="Style_Link"></traMobile:LinkCommand>
	</rMobile:ChildPanel>
</rMobile:MultiPanel>
