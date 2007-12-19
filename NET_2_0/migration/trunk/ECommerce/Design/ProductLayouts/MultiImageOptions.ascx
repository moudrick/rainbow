<%@ Control Language="c#" AutoEventWireup="true" Codebehind="Options.ascx.cs" Inherits="Rainbow.ECommerce.Design.Options" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<SCRIPT language="C#" runat="server">
		private void ddOptions1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ImageButton m = this.ImageProductButton;
			if(m != null)
			{
				m.ImageUrl = m.ImageUrl.Replace(".jpg", ddOptions1.SelectedValue + ".jpg");
				m.Visible=true;
			}
		}
		
		private void OnLoad(object sender, System.EventArgs e)
		{
			ImageButton m = this.ImageProductButton;
			if(m != null)
			{
				m.ImageUrl = m.ImageUrl.Replace(".jpg", ddOptions1.SelectedValue + ".jpg");
				m.Visible=true;
			}
		}
</SCRIPT>
<TABLE id="Table1" cellSpacing="0" cellPadding="0" border="0">
	<TR>
		<TD noWrap align="left" class="smalltext">
			<asp:Label id="lblOptions" runat="server">Select Options :</asp:Label></TD>
	</TR>
	<TR>
		<TD noWrap align="left">
			<asp:DropDownList Autopostback="true" OnSelectedIndexChanged="ddOptions1_SelectedIndexChanged" id="ddOptions1"
				runat="server" Width="240px">
				<asp:ListItem Value="No Options">No Options</asp:ListItem>
			</asp:DropDownList></TD>
	</TR>
	<TR>
		<TD noWrap align="left">
			<asp:DropDownList id="ddOptions2" runat="server" Width="240px">
				<asp:ListItem Value="No Options">No Options</asp:ListItem>
			</asp:DropDownList></TD>
	</TR>
	<TR>
		<TD noWrap align="left">
			<asp:DropDownList id="ddOptions3" runat="server" Width="240px">
				<asp:ListItem Value="No Options">No Options</asp:ListItem>
			</asp:DropDownList></TD>
	</TR>
</TABLE>
