<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SimpleMenu.SimpleMenuType" %>
<%@ Register TagPrefix="portal" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
	<!-- START SolpartMenu -->
	<portal:Solpartnavigation 
					id="NavigationMenu" 
					runat="server" 
					
					ParentTabId="<%#ParentPageID%>" 
					Bind="<%#MenuBindOption%>"
					AutoBind="False" 
					
					SeparateCSS="True"

					Display="<%#MenuRepeatDirection%>"
					EnableViewState="False"
					Visible="True" 					
					 
					MenuBarRightHTML=""
					MenuBarLeftHTML=""
					ForceDownlevel="False"
					ToolTip=""
					Moveable="False"
					MenuAlignment="Left"
					RootArrow="False"
					IconWidth="15"
					MenuItemHeight="22"
					MenuBorderWidth="0"
					MenuBarHeight="22"
					ShadowColor="transparent"
					SelectedForeColor="transparent"
					SelectedColor="transparent"
					SelectedBorderColor="transparent"
					MenuEffects-MouseOutHideDelay="500"
					MenuEffects-MouseOverExpand="True"
					MenuEffects-MenuTransitionStyle="None"
					MenuEffects-MouseOverDisplay="None"
					MenuEffects-MenuTransition="None"
					
					MenuCSS-MenuDefaultItemHighLight="spm_DefaultItemHighlight"
					MenuCSS-MenuDefaultItem="spm_DefaultItem"
					MenuCSS-MenuArrow="spm_MenuArrow"
					MenuCSS-MenuIcon="spm_MenuIcon"
					MenuCSS-RootMenuArrow="spm_RootMenuArrow"
					MenuCSS-MenuBreak="spm_MenuBreak"
					MenuCSS-SubMenu="spm_SubMenu"
					MenuCSS-MenuContainer="spm_MenuContainer"
					MenuCSS-MenuItem="spm_MenuItem"
					MenuCSS-MenuItemSel="spm_MenuItemSel"
					MenuCSS-MenuBar="spm_MenuBar"
					MenuCSSPlaceHolderControl="spMenuStyle"
					/>
		</portal:Solpartnavigation>
		<!-- END SolpartMenu -->