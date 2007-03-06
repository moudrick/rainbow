<%@ Register TagPrefix="secure" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="secure" TagName="Header" Src="Header.ascx" %>
<%@ Page language="c#" CodeBehind="Logon.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.ECommerce.LogonPage" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<HTML>
	<HEAD>
		<title>
			<%= Titulo %>
		</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<LINK href="msdefault.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body leftMargin="0" topMargin="0" marginheight="0" marginwidth="0">
		<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
			<tr vAlign="top">
				<td><secure:header id="Header1" runat="server"></secure:header></td>
			</tr>
			<tr>
				<td>
					<form id="Logon" runat="server">
						<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td height="*">
									<TABLE cellSpacing="1" cellPadding="1" width="230" align="center">
										<TBODY>
											<TR>
												<TD><tra:label id="Label3" runat="server" TextKey="ECOMMERCE_ACCOUNT_LOGIN">Account Login </tra:label>
													<hr width="100%" noShade SIZE="1">
												</TD>
											</TR>
											<TR>
												<td noWrap><tra:literal id="Literal1" runat="server" Text="Email" LabelForControl="email" TextKey="EMAIL"></tra:literal>:</td>
											</TR>
											<TR>
												<TD><asp:textbox id="email" runat="server" columns="35" width="200" cssclass="NormalTextBox"></asp:textbox></TD>
											</TR>
											<TR>
												<td noWrap><tra:literal id="Literal2" runat="server" Text="Password" LabelForControl="password" TextKey="PASSWORD"></tra:literal>:</td>
											</TR>
											<TR>
												<TD><asp:textbox id="password" runat="server" columns="35" width="200" cssclass="NormalTextBox" textmode="password"></asp:textbox></TD>
											</TR>
											<TR>
												<td noWrap><tra:checkbox id="Checkbox1" runat="server" Text="Remember Login" TextKey="REMEMBER_LOGIN"></tra:checkbox></td>
											</TR>
											<TR>
												<td noWrap align="right"><tra:button id="LoginBtn" runat="server" CssClass="CommandButton" Text="Sign in" EnableViewState="False"
														TextKey="SIGNIN"></tra:button></td>
											</TR>
											<tr>
												<td align="left"><tra:linkbutton id="SendPasswordBtn" runat="server" CssClass="CommandButton" Text="Forgotten Password?"
														EnableViewState="False" TextKey="SIGNIN_SEND_PWD"></tra:linkbutton></td>
											</tr>
											<TR>
												<TD align="left"><BR>
													<tra:linkbutton id="RegisterBtn" runat="server" CssClass="CommandButton" Text="Register" EnableViewState="False"
														TextKey="REGISTER"></tra:linkbutton></TD>
												<tra:label id="Label1" runat="server" TextKey="ECOMMERCE_LOGON_MSGREGISTER">, if you do not have a account, please register.</tra:label>
								</td>
							</tr>
							<TR>
								<TD align="left"><BR>
									<tra:linkbutton id="CancelBtn" runat="server" CssClass="CommandButton" Text="Cancel" EnableViewState="False"
										TextKey="CANCEL"></tra:linkbutton><tra:label id="Label2" runat="server" TextKey="ECOMMERCE_LOGON_MSGCANCEL">, back to the home page</tra:label></TD>
							</TR>
							<TR>
								<TD><br>
									<asp:label id="Message" runat="server" CssClass="NormalRed"></asp:label></TD>
							</TR>
						</table>
				</td>
			</tr>
		</table>
		</FORM></TD></TR>
		<tr valign="top">
			<td>
				<secure:Footer runat="server" id="Footer1" />
			</td>
		</tr>
		</TBODY></TABLE>
	</body>
</HTML>
