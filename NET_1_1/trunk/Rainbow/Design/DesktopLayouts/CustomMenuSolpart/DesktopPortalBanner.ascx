<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ import Namespace="System.Collections" %>
<%@ import Namespace="Rainbow.Design" %>
<%@ import Namespace="Rainbow.Configuration" %>
<%@ Control Language="c#" %>
<script language="C#" runat="server">
	public Hashtable userProfile;
    private void Page_Load(object sender, System.EventArgs e) 
    {
		PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
			
		// Obtain the Profile details for the current user
		userProfile = PortalSettings.GetCurrentUserProfile(portalSettings.PortalID);

        PortalHeaderMenu.DataBind();
        PortalImage.DataBind();
        PortalTitle.DataBind();
        PortalMenu.DataBind();
    }
</script> <!-- Portal BANNER -->
<table id="Table1" cellSpacing="0" cellPadding="0" width="100%" border="0">
<tr>
  <td><table class="HeadBg" border="0" cellpadding="0" cellspacing="0" width="100%">
	<tr height=22>
		<td colspan=2 background="<%=LayoutManager.WebPath%>/CustomMenuSolPart/images/header_top_bkd.gif" nowrap><CC1:HEADERMENU id="PortalHeaderMenu" runat="server" Bind="BindOptionTop" RepeatDirection="Horizontal"
									BackColor="" ShowWelcome="false" ShowHome="false" CssClass="SiteLink" CellPadding="2">
									<SEPARATORSTYLE forecolor="White"></SEPARATORSTYLE>
									<ITEMTEMPLATE><%# Container.DataItem %></ITEMTEMPLATE>
									<SEPARATORTEMPLATE>|</SEPARATORTEMPLATE>
								</CC1:HEADERMENU>
		</td>
	</tr>
	<tr>
		<td width="5%" nowrap><CC1:HEADERIMAGE id="PortalImage" runat="server" EnableViewState="false"></CC1:HEADERIMAGE></td>	  
		<td width="95%" height="100%" colspan=2 nowrap><table border="0" cellpadding="0" cellspacing="0" width="100%" height=100%>
			<tr><td width="100%" height=10%><img src="<%=LayoutManager.WebPath%>/CustomMenuSolPart/images/spacer.gif" width="1" height="1" border="0" alt=""></td></tr>
			<tr><td align=right valign=bottom><CC1:HEADERTITLE id="PortalTitle" runat="server" EnableViewState="false" CssClass="SiteTitle"></CC1:HEADERTITLE></td></tr>
  			<tr><td align=right class="headerUserInfo" nowrap><%=PortalSettings.CurrentUser.Identity.Name%><br><%=userProfile["Title"]%></td></tr>
			</table>
		</td>
	</tr>
	</table></td>
</tr>
<tr>
	<td><table cellpadding=0 cellspacing=0 border=0 width="100%">
		<tr height=4><td colspan=2 background="<%=LayoutManager.WebPath%>/CustomMenuSolPart/images/nav_top_bkd.gif" border="0"></td></tr>
		<tr height=24>
			<td class="navigationBar spm_MenuBar"><CC1:SOLPARTNAVIGATION 
					id="PortalMenu" 
					runat="server" 
					Bind="BindOptionTop" 
					ParentTabID="0"
				
					MenuCSSPlaceHolderControl="spMenuStyle"
					MenuCSS-MenuBar="spm_MenuBar" 
					MenuCSS-MenuItemSel="spm_MenuItemSel" 
					MenuCSS-MenuItem="spm_MenuItem"
					MenuCSS-MenuContainer="spm_MenuContainer" 
					MenuCSS-SubMenu="spm_SubMenu" 
					MenuCSS-MenuBreak="spm_MenuBreak" 
					MenuCSS-RootMenuArrow="spm_RootMenuArrow"
					MenuCSS-MenuIcon="spm_MenuIcon" 
					MenuCSS-MenuArrow="spm_MenuArrow"
					
					MenuCSS-MenuDefaultItem="spm_DefaultItem"
					MenuCSS-MenuDefaultItemHighLight="spm_DefaultItemHighlight"
					
					MenuEffects-MenuTransition="None"
					MenuEffects-MouseOverDisplay="None"
					MenuEffects-MenuTransitionStyle="None"
					MenuEffects-MouseOverExpand="True" 
					MenuEffects-MouseOutHideDelay=500
					
					SelectedBorderColor="#cccccc"
					SelectedColor="transparent" 
					SelectedForeColor="transparent" 
					ShadowColor="transparent"

					MenuBarHeight="23" 
					MenuBorderWidth="0"
					MenuItemHeight="23"
					IconWidth="15"	
					
					RootArrow="False" 
					MenuAlignment="Left"
					Display="Horizontal" 

					Moveable="False" 
					ToolTip="" 
					Visible="True" 
					ForceDownlevel="False" 
					AutoBind="False" 
					MenuBarLeftHTML=""
					MenuBarRightHTML="">
				</CC1:SOLPARTNAVIGATION></td>
			<td align=right width="99%" class="navigationBar navigationNonSelected" nowrap><img src="<%=LayoutManager.WebPath%>/CustomMenuSolPart/images/spacer.gif" width="20" height="1" border="0" alt=""><span class="dateCurrent"><%=System.DateTime.Now.ToLongDateString()%></span><span class="dateLastVisit"><%=(userProfile["user_last_visit"] != null)? "<br>Last visit was "+DateTime.Parse(userProfile["user_last_visit"].ToString()).ToShortDateString():""%></span></td>
		</tr>
		</table></td>
</tr>
<tr>
  <td class="bc_Row"><cc1:BreadCrumbs TextCSSClass="bc_Text" LinkCSSClass="bc_Link" id="Breadcrumbs1" runat="server"></cc1:BreadCrumbs></td>
</tr>
</table>
<!-- END Portal BANNER -->
