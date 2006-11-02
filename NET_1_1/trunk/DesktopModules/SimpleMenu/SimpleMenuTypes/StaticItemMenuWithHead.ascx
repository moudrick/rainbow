<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SimpleMenu.SimpleMenuType" %>
<%@ Register TagPrefix="portal" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
	<!--Simple Item Menu-->
	<portal:DESKTOPNAVIGATION 
					id=NavigationMenu 
					runat="server"  
					visible="true"  
					CssClass="sm_SimpleMenu" 
					ShowHeader="true" 
					ShowFooter="false" 
					RepeatDirection="<%#MenuRepeatDirection%>"
					ParentPageID="<%#ParentPageID%>" 
					Bind="<%#MenuBindOption%>"
					AutoBind="false" 
					EnableViewState="false">
		<SelectedItemStyle CssClass="sm_SelectedTab" />
		<SelectedItemTemplate>
<!-- 
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx" , GlobalPortalSettings.ActivePage.PageID,"ItemId="  + ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %>
				&#160;
-->
<!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx" , ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %></a>
				&#160;
<!-- end of modification -->
	</SelectedItemTemplate>
		<AlternatingItemStyle CssClass="sm_OtherSubTabsAlt" />
		<AlternatingItemTemplate>
<!--  
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx" , GlobalPortalSettings.ActivePage.PageID,"ItemId=" + ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %>&#160; 
-->   
<!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx" , ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %></a>&#160; 
<!-- end of modification -->	
	</AlternatingItemTemplate>
		<ItemStyle CssClass="sm_OtherSubTabs" />
		<ItemTemplate>
<!-- 
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx" ,GlobalPortalSettings.ActivePage.PageID, "ItemId=" + ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %>
			</a>&#160;
-->   
<!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
			&#160;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl("~/Default.aspx", ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
				<%# ((Rainbow.Configuration.PageStripDetails) Container.DataItem).PageName %>
			</a>&#160;
<!-- end of modification -->
	</ItemTemplate>
		<FooterStyle CssClass="sm_Footer" />
		<FooterTemplate></FooterTemplate>
		<HeaderStyle CssClass="sm_Header" />
		<HeaderTemplate>&#160;<a href='<%=Rainbow.HttpUrlBuilder.BuildUrl(GlobalPortalSettings.ActivePage.PageID)%>'><%=GlobalPortalSettings.ActivePage.PageName%></a>&#160;</HeaderTemplate>
	</portal:DESKTOPNAVIGATION>
	<!--END:Simple Item Menu-->