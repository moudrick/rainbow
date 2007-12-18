<%@ Register TagPrefix="portal" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SimpleMenu.SimpleMenuType" %>
<!-- START DHTML Menu -->
<portal:MenuNavigation 
						id="NavigationMenu" 
						runat="server"  
						visible="True"  
						Horizontal="<%#(MenuRepeatDirection==0)%>"
						ParentTabId="<%#ParentPageID%>" 
						Bind="<%#MenuBindOption%>"
						AutoBind="False" 
						BorderWidth="0" 
						Width="190px">
	<ControlItemStyle CssClass="MenuItem"></ControlItemStyle>
	<ControlSubStyle CssClass="MenuSubItem"></ControlSubStyle>
	<ControlHiStyle CssClass="MenuHiItem"></ControlHiStyle>
	<ControlHiSubStyle CssClass="MenuHiSubItem"></ControlHiSubStyle>
	<ArrowImage Width="5px" Height="10px" ImageUrl="tri.gif"></ArrowImage>
	<ArrowImageLeft Width="5px" Height="10px" ImageUrl="trileft.gif"></ArrowImageLeft>
	<ArrowImageDown Width="10px" Height="5px" ImageUrl="tridown.gif"></ArrowImageDown>
</portal:MenuNavigation>
<!-- END DHTML Menu -->
