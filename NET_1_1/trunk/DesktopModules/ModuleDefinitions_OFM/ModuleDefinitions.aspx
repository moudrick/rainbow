<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Page language="c#" CodeBehind="ModuleDefinitions.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.AdminAll.ModuleDefinitions_OFM" %>
<html>
	<head runat="server"></head>
	<body runat="server">
		<form runat="server">
			<div class="rb_AlternateLayoutDiv">
				<table class="rb_AlternateLayoutTable">
					<tr vAlign="top">
						<td class="rb_AlternatePortalHeader" valign="top"><portal:banner id="SiteHeader" runat="server"></portal:banner></td>
					</tr>
					<tr>
						<td><br>
							<table cellSpacing="0" cellPadding="4" width="98%" border="0">
								<tr vAlign="top">
									<td width="150">&nbsp;
									</td>
									<td width="*">
										<table cellSpacing="0" cellPadding="0" width="500">
											<tr>
												<td class="Head" align="left"><tra:label id="Label1" runat="server" Text="OneFileModule type definition" TextKey="MODULE_TYPE_DEFINITION_OFM"></tra:label></td>
											</tr>
											<tr>
												<td colSpan="2">
													<hr noShade SIZE="1">
												</td>
											</tr>
										</table>
										<table cellSpacing="0" cellPadding="3" width="750" border="0" runat="server">
											<tr>
												<td class="SubHead" width="105">
													<tra:label id="Label2" runat="server" TextKey="FRIENDLY_NAME" Text="Friendly Name"></tra:label>:
												</td>
												<td width="3" rowSpan="6">&nbsp;
												</td>
												<td>
													<asp:textbox id="FriendlyName" runat="server" cssclass="NormalTextBox" width="390" Columns="30" maxlength="150"></asp:textbox></td>
												<td width="10" rowSpan="6">&nbsp;
												</td>
												<td class="Normal" width="250">
													<tra:requiredfieldvalidator id="Req1" runat="server" TextKey="ERROR_ENTER_A_MODULE_NAME" Display="Dynamic" ErrorMessage="Enter a Module Name" ControlToValidate="FriendlyName" CssClass="Error" DESIGNTIMEDRAGDROP="235"></tra:requiredfieldvalidator></td>
											</tr>
											<TR>
												<td class="SubHead" noWrap width="105"><tra:label id="Label3" runat="server" Text="Desktop Source" TextKey="DESKTOP_SOURCE"></tra:label>:
												</td>
												<td><asp:textbox id="DesktopSrc" runat="server" maxlength="150" Columns="30" width="390" cssclass="NormalTextBox"></asp:textbox></td>
												<td class="Normal"><tra:requiredfieldvalidator id="Req2" runat="server" TextKey="ERROR_ENTER_A_SOURCE_PATH" CssClass="Error" ControlToValidate="DesktopSrc" ErrorMessage="You Must Enter Source Path for the Desktop Module" Display="Dynamic"></tra:requiredfieldvalidator><tra:label id="lblInvalidModule" runat="server" Text="Invalid module!" TextKey="ERROR_INVALID_MODULE" Visible="False" cssClass="Error" EnableViewState="False">
											Invalid module!</tra:label></td>
											</TR>
											<tr>
												<td class="SubHead" width="105"><tra:label id="Label4" runat="server" Text="Mobile Source" TextKey="MOBILE_SOURCE"></tra:label>:
												</td>
												<td><asp:textbox id="MobileSrc" runat="server" maxlength="150" Columns="30" width="390" cssclass="NormalTextBox"></asp:textbox></td>
												<td>&nbsp;
												</td>
											</tr>
											<TR>
												<td class="SubHead" width="105"><tra:label id="Label5" runat="server" Text="Guid" TextKey="GUID"></tra:label>:</td>
												<td><asp:textbox id="ModuleGuid" runat="server" maxlength="36" Columns="1" width="390" cssclass="NormalTextBox"></asp:textbox></td>
												<td></td>
											</TR>
											<TR>
												<td class="SubHead" vAlign="top" width="116"></td>
												<td><tra:linkbutton id="selectAllButton" runat="server" Text="Select all" TextKey="SELECT_ALL" cssclass="CommandButton"></tra:linkbutton>&nbsp;&nbsp;
													<tra:linkbutton id="selectNoneButton" runat="server" Text="Select none" TextKey="SELECT_NONE" cssclass="CommandButton"></tra:linkbutton></td>
												<td></td>
											</TR>
											<tr>
												<td class="SubHead" vAlign="top" width="116"><tra:label id="Label6" runat="server" Text="Portals" TextKey="PORTALS"></tra:label>:
												</td>
												<td>
													<div style="OVERFLOW: auto; HEIGHT: 200px"><asp:checkboxlist id="PortalsName" runat="server" CssClass="Normal" RepeatColumns="1" RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0"></asp:checkboxlist></div>
												</td>
												<td></td>
											</tr>
										</table>
										<p><tra:linkbutton class="CommandButton" id="updateButton" runat="server" Text="Update" TextKey="UPDATE">Update</tra:linkbutton>&nbsp;
											<tra:linkbutton class="CommandButton" id="cancelButton" runat="server" Text="Cancel" CausesValidation="False" TextKey="CANCEL">Cancel</tra:linkbutton>&nbsp;
											<tra:linkbutton class="CommandButton" id="deleteButton" runat="server" Text="Delete this module type" CausesValidation="False" TextKey="DELETE_THIS_MODULE_TYPE">Delete this module type</tra:linkbutton></p>
										<P>
											<asp:Label id="lblErrorDetail" runat="server" cssclass="Error"></asp:Label></P>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td class="rb_AlternatePortalFooter"><div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div></foot:Footer></td>
					</tr>
				</table>
			</div>
		</form>
	</body>
</html>
