<%@ Control language="c#" Inherits="Rainbow.DesktopModules.Signin" CodeBehind="Signin.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<table cellSpacing="1" cellPadding="1" width="100%" border=0 align="center">
	<tr>
		<td noWrap class="Normal"><tra:literal id="EmailLabel" runat="server" Text="Email" TextKey="EMAIL" LabelForControl="email"></tra:literal></td>
	</tr>
	<tr>
		<td noWrap><asp:textbox id="email" runat="server" columns="24" cssclass="NormalTextBox"></asp:textbox></td>
	</tr>
	<tr>
		<td noWrap class="Normal"><tra:literal id="PasswordLabel" runat="server" Text="Password" TextKey="PASSWORD" LabelForControl="password"></tra:literal></td>
	</tr>
	<tr>
		<td noWrap><asp:textbox id="password" runat="server" columns="24" cssclass="NormalTextBox" textmode="password"></asp:textbox></td>
	</tr>
	<tr>
		<td noWrap><tra:checkbox id="RememberCheckBox" runat="server" Text="Remember Login" TextKey="REMEMBER_LOGIN"
				CssClass="Normal"></tra:checkbox></td>
	</tr>
	<tr>
		<td noWrap><tra:button id="LoginBtn" runat="server" Text="Sign in" TextKey="SIGNIN" EnableViewState="False"
				CssClass="CommandButton"></tra:button></td>
	</tr>
	<tr>
		<td noWrap><tra:button id="RegisterBtn" runat="server" Text="Register" TextKey="REGISTER" EnableViewState="False"
				CssClass="CommandButton"></tra:button></td>
	</tr>
	<tr>
		<td noWrap><tra:button id="SendPasswordBtn" runat="server" Text="Forgotten Password?" TextKey="SIGNIN_SEND_PWD"
				EnableViewState="False" CssClass="CommandButton"></tra:button></td>
	</tr>
	<tr>
		<td><tra:label id="Message" runat="server" CssClass="Error"></tra:label></td>
	</tr>
</table>
