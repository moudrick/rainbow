<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Control Language="c#" %>
<script runat="server">
    private void Page_Load(object sender, System.EventArgs e)
    {
        ThreePanes.DataBind();
    }
</script>
<CC1:DESKTOPPANES id="ThreePanes" runat="server" cellspacing="2" Cellpadding="0" ShowFirstSeparator="False" ShowLastSeparator="False">
	<HORIZONTALSEPARATORTEMPLATE><TRA:IMAGE runat="server" ImageUrl="<%=LayoutManager.WebPath%>/CustomMenuSolPart/images/spacer.gif" height=4></TRA:IMAGE></HORIZONTALSEPARATORTEMPLATE>
	<VERTICALSEPARATORTEMPLATE></VERTICALSEPARATORTEMPLATE>
	<CONTENTPANESTYLE verticalalign="Top" horizontalalign="left" cssclass="ContentPane"></CONTENTPANESTYLE>
	<RIGHTPANESTYLE cssclass="RightPane" verticalalign="Top"></RIGHTPANESTYLE>
	<LEFTPANESTYLE cssclass="LeftPane" verticalalign="Top" width="1px"></LEFTPANESTYLE>
</CC1:DESKTOPPANES>