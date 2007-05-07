<%@ Control Language="c#" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<script runat="server">
    private void Page_Load(object sender, System.EventArgs e)
    {
        ThreePanes.DataBind();
    }
</script>
<CC1:DESKTOPPANES id="ThreePanes" height="100%" ShowLogon="False" ShowLastSeparator="False" ShowFirstSeparator="False" Cellpadding="0" cellspacing="0" runat="server">
    <LEFTPANESTYLE cssclass="LeftPane" width="190px" verticalalign="Top"></LEFTPANESTYLE>
    <CONTENTPANESTYLE cssclass="ContentPane" verticalalign="Top" horizontalalign="left"></CONTENTPANESTYLE>
    <RIGHTPANESTYLE cssclass="RightPane" verticalalign="Top"></RIGHTPANESTYLE>
</CC1:DESKTOPPANES>
