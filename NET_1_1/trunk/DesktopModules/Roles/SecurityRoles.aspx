<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Page language="c#" CodeBehind="SecurityRoles.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SecurityRoles" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<html>
	<head runat="server"></head>
	<body runat="server">
		<form runat="server">
			<div class="zen-main" id="zenpanes">
				<div class="rb_DefaultPortalHeader">
							<portal:Banner ShowTabs="false" runat="server" id="Banner1" />
				</div>
				<div class="div_ev_Table">
					<table width="98%" cellspacing="2" cellpadding="2" border="0">
						<tr>
							<td colspan="2">
								<table width="100%" cellspacing="0" cellpadding="0">
									<tr>
										<td align="left">
											<span id="title" class="Head" runat="server">Role Membership</span>
										</td>
									</tr>
									<tr>
										<td>
											<hr noshade size="1">
										</td>
									</tr>
								</table>
								<asp:Label id="Message" CssClass="NormalRed" runat="server" />
							</td>
						</tr>
						<tr>
							<td width="11">
								&nbsp;
							</td>
							<td>
								<table width="100%" cellspacing="0" cellpadding="0">
									<tr>
										<td>
											<asp:TextBox id="windowsUserName" Text="DOMAIN\username" Visible="False" runat="server" CssClass="NormalTextBox"/>
										</td>
										<td class="Normal" noWrap>
											<tra:LinkButton id="addNew" runat="server" CssClass="CommandButton" TextKey="ROLE_ADD_NEW_USER"
												Visible="False">Create new user and add to role</tra:LinkButton>
										</td>
									</tr>
									<tr>
										<td>
											<asp:DropDownList id="allUsers" DataTextField="Email" DataValueField="UserID" runat="server" CssClass="NormalTextBox"/>
										</td>
										<td noWrap>
											<tra:LinkButton id="addExisting" runat="server" CssClass="CommandButton" TextKey="ROLE_ADD_USER">Add existing user to role</tra:LinkButton>
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr valign="top">
							<td width="11">
								&nbsp;
							</td>
							<td noWrap>
								<asp:DataList id="usersInRole" RepeatColumns="2" DataKeyField="UserID" runat="server">
									<ItemStyle Width="225" />
									<ItemTemplate>
										&#160;&#160;
										<tra:ImageButton ImageUrl='<%# CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl %>' CommandName="delete" AlternateText="Remove this user from role" runat="server" />
										<asp:Label Text='<%# DataBinder.Eval(Container.DataItem, "Email") %>' cssclass="Normal" runat="server" />
									</ItemTemplate>
								</asp:DataList>
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<hr noshade size="1">
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<tra:LinkButton id="saveBtn" runat="server" CssClass="CommandButton" TextKey="ROLE_SAVE_CHANGES">Save Role Changes</tra:LinkButton>
							</td>
						</tr>
					</table>
				</div>
				<div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div></foot:Footer>
			</div>
		</form>
	</body>
</html>
