<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MerchantManagerShipping.ascx.cs" Inherits="Rainbow.ECommerce.MerchantManagerShipping" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="MerchantManagerEdit" Src="MerchantManagerEditShipping.ascx" %>
<table>
	<tr>
		<td valign="top"><asp:ListBox id="lbEcommerceMerchants" AutoPostBack="True" Width="200" Rows="20" runat="server" CssClass="NormalTextBox" /></td>
		<td>&nbsp;</td>
		<td valign="top"><uc1:MerchantManagerEdit id="EcommerceMerchantsEdit" runat="server" /></td>
	</tr>
</table>
<asp:label id="lblError" cssclass="error-message" runat="server" />
