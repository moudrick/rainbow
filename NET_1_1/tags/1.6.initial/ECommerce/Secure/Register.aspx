<%@ Page Language="c#" codebehind="Register.aspx.cs" autoeventwireup="false" Inherits="Rainbow.ECommerce.Register" %>
<%@ Register TagPrefix="secure" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="secure" TagName="Footer" Src="Footer.ascx" %>
<%@ import Namespace="Esperantus" %>
<html>
<head>
		<title>Rainbow Secure Server</title>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <link href="msdefault.css" type="text/css" rel="stylesheet" />
</head>
<body leftmargin="0" topmargin="0" marginheight="0" marginwidth="0">
    <table cellspacing="0" cellpadding="0" width="100%" border="0">
        <tbody>
			<tr valign="top">
				<td>
                    <secure:Header id="Header1" runat="server"></secure:Header>
				</td>
			</tr>
			<tr>
				<td align="middle">
					<form id="Register" method="post" runat="server">
                        <table cellspacing="0" cellpadding="0" width="650" align="center" border="0">
                            <tbody>
							<tr>
								<td>
									<!-- Start Register control -->
									<asp:PlaceHolder id="registerPlaceHolder" runat="server"></asp:PlaceHolder>
                                        <!-- End Register control --></td>
							</tr>
                            </tbody>
						</table>
					</form>
				</td>
			</tr>
			<tr valign="top">
				<td>
                    <secure:Footer id="Footer1" runat="server"></secure:Footer>
				</td>
			</tr>
        </tbody>
		</table>
</body>
</html>
