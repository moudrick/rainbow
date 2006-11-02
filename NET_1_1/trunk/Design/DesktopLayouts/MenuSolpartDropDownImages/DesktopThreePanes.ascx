<%@ Control Language="c#" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<script runat="server">
    private void Page_Load(object sender, System.EventArgs e)
    {
        ThreePanes.DataBind();
    }
</script>
<CC1:DESKTOPPANES id="ThreePanes" runat="server" cellspacing="0" Cellpadding="4" ShowFirstSeparator="False" ShowLastSeparator="False">
	<ContentPaneTemplate>
		<P class="Normal">
			<tra:Label id="Label1" runat="server" TextKey="IMAGEMENU_INSTRUCTIONS">This is a sample use for image menus. You can select ahother image for this tab changing the default "Custom Image Menu" in PageLayout settings.</tra:Label><BR>
		</P>
	</ContentPaneTemplate>
	<CONTENTPANESTYLE verticalalign="Top" horizontalalign="left" cssclass="ContentPane"></CONTENTPANESTYLE>
	<RIGHTPANESTYLE cssclass="RightPane" verticalalign="Top"></RIGHTPANESTYLE>
	<LEFTPANESTYLE cssclass="LeftPane" verticalalign="Top" width="1px"></LEFTPANESTYLE>
</CC1:DESKTOPPANES>