<%@ Control Language="c#" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ import Namespace="Rainbow.Design" %>
<script runat="server">
    private void Page_Load(object sender, System.EventArgs e)
    {
        PortalHeaderMenu.DataBind();
        PortalImage.DataBind();
        PortalTitle.DataBind();
        PortalMenu.DataBind();
    }
</script>
<!-- Portal BANNER -->
<table id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tbody>
		<tr>
			<td class="DefaultBanner1" colspan="2" height="1">
				<TRA:IMAGE runat="server" border="0" ImageUrl="~/design/Themes/default/img/shim.gif"></TRA:IMAGE></td>
		</tr>
		<tr>
			<td valign="center" align="left" rowspan="2">
				<table cellspacing="0" cellpadding="0">
					<tbody>
						<tr>
							<td>
								<CC1:HEADERIMAGE id="PortalImage" style="MARGIN-TOP: 0px; MARGIN-RIGHT: 0px" runat="server" EnableViewState="false"></CC1:HEADERIMAGE>
							</td>
							<!--headerimage=images/logo-default.gif-->
							<td height="50">
								<CC1:HEADERTITLE id="PortalTitle" CssClass="SiteTitle" runat="server" EnableViewState="false"></CC1:HEADERTITLE>
							</td>
						</tr>
					</tbody>
				</table>
			</td>
			<td valign="top" align="right">
				<table cellspacing="0" cellpadding="0">
					<tbody>
						<tr>
							<td valign="top">
							</td>
							<td valign="top">
								<CC1:HEADERMENU id="PortalHeaderMenu" CellPadding="4" CssClass="SiteLink" ShowHome="False" RepeatDirection="Horizontal"
									runat="server" ShowHelp="True">
									<SEPARATORSTYLE></SEPARATORSTYLE>
									<ITEMTEMPLATE>
										<span class="SiteLink">
											<%# Container.DataItem %>
										</span>
									</ITEMTEMPLATE>
									<SEPARATORTEMPLATE>
                                        | 
                                    </SEPARATORTEMPLATE>
								</CC1:HEADERMENU>
							</td>
							<asp:LinkButton id="saveConfig" Visible="False" Runat="server"></asp:LinkButton>
						</tr>
					</tbody>
				</table>
			</td>
		</tr>
		<tr valign="top">
			<td align="right">
				&nbsp;
			</td>
		</tr>
		<tr>
			<td class="DefaultBanner1" colspan="2" height="1">
				<TRA:IMAGE runat="server" border="0" ImageUrl="~/design/Themes/default/img/shim.gif"></TRA:IMAGE></td>
		</tr>
		<tr>
			<td class="DefaultBanner2" colspan="2" height="1">
				<TRA:IMAGE runat="server" border="0" ImageUrl="~/design/Themes/default/img/shim.gif"></TRA:IMAGE></td>
		</tr>
		<tr>
			<td colspan="2" class="DefaultTD">
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tbody>
						<tr height="22">
							<td class="navigationBar spm_MenuBar">
								<CC1:SOLPARTNAVIGATION id="PortalMenu" Bind="BindOptionTop" runat="server" MenuBarRightHTML="" MenuBarLeftHTML=""
									AutoBind="False" ForceDownlevel="False" Visible="True" ToolTip="" Moveable="False" Display="Horizontal"
									MenuAlignment="Left" RootArrow="False" IconWidth="15" MenuItemHeight="22" MenuBorderWidth="0" MenuBarHeight="22"
									ShadowColor="transparent" SelectedForeColor="transparent" SelectedColor="transparent" SelectedBorderColor="transparent"
									MenuEffects-MouseOutHideDelay="500" MenuEffects-MouseOverExpand="True" MenuEffects-MenuTransitionStyle="None"
									MenuEffects-MouseOverDisplay="None" MenuEffects-MenuTransition="None" MenuCSS-MenuDefaultItemHighLight="spm_DefaultItemHighlight"
									MenuCSS-MenuDefaultItem="spm_DefaultItem" MenuCSS-MenuArrow="spm_MenuArrow" MenuCSS-MenuIcon="spm_MenuIcon"
									MenuCSS-RootMenuArrow="spm_RootMenuArrow" MenuCSS-MenuBreak="spm_MenuBreak" MenuCSS-SubMenu="spm_SubMenu"
									MenuCSS-MenuContainer="spm_MenuContainer" MenuCSS-MenuItem="spm_MenuItem" MenuCSS-MenuItemSel="spm_MenuItemSel"
									MenuCSS-MenuBar="spm_MenuBar" MenuCSSPlaceHolderControl="spMenuStyle" ParentPageID="0"></CC1:SOLPARTNAVIGATION>
							</td>
							<td class="navigationBar navigationNonSelected" nowrap="nowrap" align="right">
								<img height="1" alt="Design" src="<%=LayoutManager.WebPath%>/MenuTopSolpart/images/spacer.gif" width="60" border="0" /><span class="dateCurrent"><%=System.DateTime.Now.ToLongDateString()%></span><span class="dateLastVisit"></span></td>
						</tr>
					</tbody>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="2" class="bc_Row">
				<CC1:BREADCRUMBS id="Breadcrumbs1" runat="server" LinkCSSClass="bc_Link" TextCSSClass="bc_Text"></CC1:BREADCRUMBS>
			</td>
		</tr>
	</tbody>
</table>
<!-- END Portal BANNER -->
