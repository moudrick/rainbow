<%@ Control Language="c#" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<script runat="server">

    private void Page_Load(object sender, System.EventArgs e)
    {
        PortalHeaderMenu.DataBind();
        PortalTitle.DataBind();
        PortalImage.DataBind();
        //PortalTabs.DataBind();
    }

</script>
<!-- Portal BANNER -->
<table class="HeadBg" id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
    <tbody>
        <tr>
            <td class="DefaultBanner1" colspan="2" height="1">
                <TRA:IMAGE ImageUrl="~/design/Themes/default/img/shim.gif" runat="server"></TRA:IMAGE></td>
        </tr>
        <tr>
            <td valign="center" align="left" rowspan="2">
                <table cellspacing="0" cellpadding="0">
                    <tbody>
                        <tr>
                            <td>
                                <CC1:HEADERIMAGE id="PortalImage" style="MARGIN-TOP: 0px; MARGIN-RIGHT: 0px" runat="server" EnableViewState="false"></CC1:HEADERIMAGE>
                            </td>
                            <!--headerimage=/_rainbow/images/logo-default.gif-->
                            <td height="50">
                                <CC1:HEADERTITLE id="PortalTitle" runat="server" EnableViewState="false" CssClass="SiteTitle"></CC1:HEADERTITLE>
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
                                <CC1:HEADERMENU class="SiteLink" id="PortalHeaderMenu" runat="server" RepeatDirection="Horizontal" CellPadding="4" ShowHome="False" ShowHelp="True">
                                    <SEPARATORSTYLE></SEPARATORSTYLE>
                                    <ITEMTEMPLATE>
                                        <span class="SiteLink"><%# Container.DataItem %></span>
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
                <TRA:IMAGE ImageUrl="~/design/Themes/default/img/shim.gif" runat="server"></TRA:IMAGE></td>
        </tr>
        <tr>
            <td class="DefaultBanner2" colspan="2" height="1">
                <TRA:IMAGE ImageUrl="~/design/Themes/default/img/shim.gif" runat="server"></TRA:IMAGE></td>
        </tr>
        <tr>
            <td class="DefaultTD" colspan="2">
                <CC1:MENUNAVIGATION id="HWMenu" runat="server" borderstyle="none" Horizontal="True" Width="140px" AutoBind="True" Bind="BindOptionTop" ImagesPath="/design/Themes/default/img" height="21" RightPadding="5" LeftPaddng="5">
                    <CONTROLITEMSTYLE cssclass="HWMenuItem"></CONTROLITEMSTYLE>
                    <CONTROLSUBSTYLE cssclass="HWMenuSub"></CONTROLSUBSTYLE>
                    <CONTROLHISTYLE cssclass="HWMenuItemHi"></CONTROLHISTYLE>
                    <CONTROLHISUBSTYLE cssclass="HWMenuSubHi"></CONTROLHISUBSTYLE>
                    <ARROWIMAGEDOWN height="5px" width="10px" imageurl="arrow_down.gif"></ARROWIMAGEDOWN>
                    <ARROWIMAGE height="9px" width="7px" imageurl="arrow.gif"></ARROWIMAGE>
                    <ARROWIMAGELEFT height="9px" width="5px" imageurl="arrow_left.gif"></ARROWIMAGELEFT>
                </CC1:MENUNAVIGATION>
            </td>
        </tr>
        <tr>
            <td class="DefaultBanner2" colspan="2" height="1">
                <TRA:IMAGE ImageUrl="~/design/Themes/default/img/shim.gif" runat="server"></TRA:IMAGE></td>
        </tr>
    </tbody>
</table>
<!-- END Portal BANNER -->
