<%@ Page language="c#" CodeBehind="EditPortalModule.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.Installer.EditPortalModules" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>

<HTML>
  <HEAD runat="server">
</HEAD>
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
												<td class="Head" align="left"><tra:label id="Label1" runat="server" Text="Module type definition" TextKey="MODULE_TYPE_DEFINITION"></tra:label></td>
											</tr>
											<tr>
												<td colSpan="2">
													<hr noShade SIZE="1">
												</td>
											</tr>
										</table>
										<P>&nbsp;</P>
										<table cellSpacing="0" cellPadding="3" width="750" border="0" runat="server" id="tableManual">
											<tr>
												<td class="SubHead" width="105">
													<tra:label id="Label2" runat="server" TextKey="FRIENDLY_NAME" Text="Friendly Name"></tra:label>:
												</td>
												<td width="3" rowSpan="6">&nbsp;
												</td>
												<td>
<asp:label id=FriendlyName runat="server" CssClass="Normal" ForeColor="Silver" Font-Bold="True"></asp:label></td>
												<td width="10" rowSpan="6">&nbsp;
												</td>
												<td class="Normal" width="250"></td>
											</tr>
											<TR>
												<td class="SubHead" noWrap width="105"><tra:label id="Label3" runat="server" Text="Desktop Source" TextKey="DESKTOP_SOURCE"></tra:label>:
												</td>
												<td>
<asp:label id=DesktopSrc runat="server" CssClass="Normal" ForeColor="Silver" Font-Bold="True"></asp:label></td>
												<td class="Normal"></td>
											</TR>
											<tr>
												<td class="SubHead" width="105"><tra:label id="Label4" runat="server" Text="Mobile Source" TextKey="MOBILE_SOURCE"></tra:label>:
												</td>
												<td>
<asp:label id=MobileSrc runat="server" CssClass="Normal" ForeColor="Silver" Font-Bold="True"></asp:label></td>
												<td>&nbsp;
												</td>
											</tr>
											<TR>
												<td class="SubHead" width="116"><tra:label id="Label5" runat="server" Text="Guid" TextKey="GUID"></tra:label>:</td>
												<td><asp:label id="lblGUID" runat="server" CssClass="Normal" Font-Bold="True" ForeColor="Silver"></asp:label></td>
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
												<td><br><br>
													<div style="OVERFLOW: auto; HEIGHT: 200px"><asp:checkboxlist id="PortalsName" runat="server" CssClass="Normal" RepeatColumns="1" RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0"></asp:checkboxlist></div>
												</td>
												<td></td>
											</tr>
										</table>
										<p><tra:linkbutton class="CommandButton" id="updateButton" runat="server" Text="Update" TextKey="UPDATE">Update</tra:linkbutton>&nbsp;
											<tra:linkbutton class="CommandButton" id="cancelButton" runat="server" Text="Cancel" CausesValidation="False" TextKey="CANCEL">Cancel</tra:linkbutton>&nbsp;</p>
										<P>
											<asp:Label id="lblErrorDetail" runat="server" cssclass="Error"></asp:Label></P>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></td>
					</tr>
				</table>
			</div>
		</form>
	</body>
</HTML>
