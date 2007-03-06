<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="secure" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="secure" TagName="Header" Src="Header.ascx" %>
<%@ Page language="c#" Codebehind="ok.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.ECommerce.ok" %>
<HTML>
	<HEAD>
		<title>
			<%= Titulo %>
		</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<link rel="stylesheet" href="msdefault.css" type="text/css">
	</HEAD>
	<body leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
		<table width="100%" height="100%" cellspacing="0" cellpadding="0" border="0">
			<tr valign="top">
				<td>
					<secure:Header runat="server" id="Header1" />
				</td>
			</tr>
			<tr>
				<td>
					<form id="ok" method="post" runat="server">
						<table width="100%" height="100%" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td height="*">
									<TABLE class="Normal" CELLSPACING="1" CELLPADDING="1" align="center">
										<TR>
											<TD>
												<P><b>
														<tra:Label id="Label1" runat="server" TextKey="ECOMMERCE_OK_MSG1">Your information has been updated in the database</tra:Label></b></P>
												<P>
													<tra:Label id="Label2" runat="server" TextKey="ECOMMERCE_OK_MSG2">Click on the 'continue' button to be redirected to the non-secure area. </tra:Label>
												</P>
												<P>
													<tra:Label id="Label3" runat="server" TextKey="ECOMMERCE_OK_MSG3">Depending of you browser, you may also have to click on a dialog box to acknowledge leaving the secure area.</tra:Label></P>
											</TD>
										</TR>
										<TR>
											<TD align="left">
												<BR>
												<tra:LinkButton id="ContinueBtn" TextKey="CONTINUE" runat="server" CssClass="CommandButton" EnableViewState="False">Continue</tra:LinkButton>
											</TD>
										</TR>
									</TABLE>
								</td>
							</tr>
						</table>
					</form>
				</td>
			</tr>
			<tr valign="top">
				<td>
					<secure:Footer runat="server" id="Footer1" />
				</td>
			</tr>
		</table>
	</body>
</HTML>
