<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MerchantManagerEditGateway.ascx.cs" Inherits="Rainbow.ECommerce.MerchantManagerEditGateway" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:textbox id="txtPortalID" visible="false" runat="server" />
<table>
	<tr>
		<td class="editcontrol-field-title">Merchant ID</td>
		<td><asp:textbox id="txtMerchantID" width="200" enabled="True" maxlength="25" runat="server" CssClass="NormalTextBox" /></td>
		<td><asp:RequiredFieldValidator id="RequiredFieldMerchantID" ControlToValidate="txtMerchantID" ErrorMessage="Merchant ID is a required value." Display="Static" runat="server">*</asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="editcontrol-field-title" style="HEIGHT: 21px">
			Bank Gateway</td>
		<td style="HEIGHT: 21px">
			<asp:DropDownList id="cmbGateway" runat="server" Width="200px" CssClass="NormalTextBox"></asp:DropDownList></td>
		<td style="HEIGHT: 21px"></td>
	</tr>
	<tr>
		<td class="editcontrol-field-title">Name</td>
		<td><asp:textbox id="txtName" width="200" enabled="True" maxlength="50" runat="server" CssClass="NormalTextBox" /></td>
		<td><asp:RequiredFieldValidator id="RequiredFieldName" ControlToValidate="txtName" ErrorMessage="Name is a required value." Display="Static" runat="server">*</asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="editcontrol-field-title">Merchant Email</td>
		<td><asp:textbox id="txtMerchantEmail" width="200" enabled="True" maxlength="50" runat="server" CssClass="NormalTextBox" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="editcontrol-field-title">Technical Email</td>
		<td><asp:textbox id="txtTechnicalEmail" width="200" enabled="True" maxlength="50" runat="server" CssClass="NormalTextBox" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="editcontrol-field-title">Metadata Xml</td>
		<td><asp:textbox id="txtMetadataXml" width="200" enabled="True" textmode="MultiLine" rows="8" runat="server" CssClass="NormalTextBox" /></td>
		<td></td>
	</tr>
	<tr>
		<td colspan="3">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><asp:button id="btnAdd" cssclass="edit-button" text="Add" runat="server" />&nbsp;<asp:button id="btnUpdate" cssclass="edit-button" text="Update" runat="server" />&nbsp;<asp:button id="btnDelete" cssclass="edit-button" text="Delete" runat="server" />&nbsp;<asp:button id="btnCancel" cssclass="edit-button" text="Cancel" runat="server" /></td>
	</tr>
	<tr>
		<td colspan="3">&nbsp;</td>
	</tr>
</table>
<asp:label id="lblError" cssclass="error-message" runat="server" />
