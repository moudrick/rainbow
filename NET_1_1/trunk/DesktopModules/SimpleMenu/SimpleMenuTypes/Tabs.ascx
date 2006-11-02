<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SimpleMenu.SimpleMenuType" %>
<%@ Register TagPrefix="portal" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<!--Simple Item Menu-->
<portal:DESKTOPNAVIGATION 
					id=NavigationMenu 
					runat="server"  
					visible="true"  
					CssClass="DefaultTD" 
					ShowHeader="false" 
					ShowFooter="false" 
					RepeatDirection="<%#MenuRepeatDirection%>"
					ParentPageID="<%#ParentPageID%>" 
					Bind="<%#MenuBindOption%>"
					AutoBind="false" 
					EnableViewState="false">
	<SelectedItemStyle CssClass="SelectedTabs" />
	<SelectedItemTemplate>
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx" , GlobalPortalSettings.ActiveTab.TabID,"ItemId="  + ((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabID)%>'>
			<%# ((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabName %>
				&#160;
	</SelectedItemTemplate>
	<ItemStyle CssClass="Tabs" />
	<ItemTemplate>
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx" ,GlobalPortalSettings.ActiveTab.TabID, "ItemId=" + ((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabID)%>'>
			<%# ((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabName %>
		</a>&#160;
	</ItemTemplate>
	<FooterStyle CssClass="sm_Footer" />
	<FooterTemplate></FooterTemplate>
	<HeaderStyle CssClass="sm_Header" />
	<HeaderTemplate></HeaderTemplate>
</portal:DESKTOPNAVIGATION>
<!--END:Simple Item Menu-->