<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SimpleMenu.SimpleMenuType" %>
<%@ Register TagPrefix="portal" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
	<!--simple static menu-->
	<portal:DESKTOPNAVIGATION 
						id=NavigationMenu 
						runat="server"  
						visible="True" 
						CssClass="sm_SimpleMenu" 
						ShowHeader="False" 
						ShowFooter="False" 
						RepeatDirection="<%#MenuRepeatDirection%>"
						ParentPageID="<%#ParentPageID%>" 
						Bind="<%#MenuBindOption%>"
						AutoBind="False" 
						EnableViewState="False">
		<SelectedItemStyle CssClass="sm_SelectedTab" />
		<SelectedItemTemplate>
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl(((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %>
				&#160;
	</SelectedItemTemplate>
		<AlternatingItemStyle CssClass="sm_OtherSubTabsAlt" />
		<AlternatingItemTemplate>
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl(((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %>&#160; 
	</AlternatingItemTemplate>
		<ItemStyle CssClass="sm_OtherSubTabs" />
		<ItemTemplate>
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl(((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %>
			</a>&#160;
	</ItemTemplate>
		<FooterStyle CssClass="sm_Footer" />
		<FooterTemplate></FooterTemplate>
		<HeaderStyle CssClass="sm_Header" />
		<HeaderTemplate></HeaderTemplate>
	</portal:DESKTOPNAVIGATION>
	<!--END: simple static menu-->
