<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Page language="c#" Inherits="Rainbow.DesktopModules.DiscussionEdit" CodeBehind="DiscussionEdit.aspx.cs" AutoEventWireup="false" %>
<html>
  <head runat="server" ID="Head1"></head>
	<body runat="server">
		<form name="form1" runat="server">
			<div class="zen-main" id="zenpanes">
				<div class="rb_DefaultPortalHeader">
					<portal:banner id="Banner1" runat="server" ShowTabs="false"></portal:banner>
				</div>
				<div class="div_ev_Table">
					<table cellSpacing="0" cellPadding="0">
						<tr>
							<td align="left">
								<tra:Label class="Head" TextKey="DS_NEWTHREAD" Text="Add a new thread" id="DiscussionEditInstructions" runat="Server" />
								<%-- <asp:Label class="Head" TextKey="DS_NEWTHREAD" Text="Add a new thread" id="DiscussionEditInstructions" runat="Server" /> --%>
							</td>
							<td align="right">
							</td>
						</tr>
						<tr>
							<td colSpan="2">
								<P>
									<hr noshade size="1">
								<P></P>
								<P>&nbsp;</P>
							</td>
						</tr>
					</table>
					<asp:panel id="EditPanel" runat="server" Visible="true">
						<TABLE cellSpacing="0" cellPadding="4" width="600" border="0">
							<TR vAlign="top">
								<TD class="SubHead" width="150">
									<tra:Label id="title_label" runat="Server" Text="Subject" TextKey="SUBJECT"></tra:Label></TD>
								<TD rowSpan="4">&nbsp;
								</TD>
								<TD width="*">
									<asp:TextBox id="TitleField" runat="server" maxlength="100" columns="40" width="500" cssclass="NormalTextBox"></asp:TextBox><%-- translation needed --%>
									<asp:RequiredFieldValidator id="Requiredfieldvalidator1" runat="server" ControlToValidate="TitleField" Display="Dynamic"
										ErrorMessage="Please enter a summary of your reply."></asp:RequiredFieldValidator></TD>
							</TR>
							<TR vAlign="top">
								<TD class="SubHead"><tra:Label id="body_label" runat="Server" Text="Body" TextKey="DS_BODY"></tra:Label></TD>
								<TD width="*"><asp:placeholder id="DescriptionField" runat="server" /></TD>
							</TR>
							<TR vAlign="top">
								<TD>&nbsp;
								</TD>
								<TD>
									<asp:LinkButton class="CommandButton" id="submitButton" runat="server" Text="Submit"></asp:LinkButton>&nbsp;
									<asp:LinkButton class="CommandButton" id="cancelButton" runat="server" Text="Cancel" CausesValidation="False"></asp:LinkButton>&nbsp;
								</TD>
							</TR>
							<TR vAlign="top">
								<TD class="SubHead"></TD>
								<TD>&nbsp;
								</TD>
							</TR>
						</TABLE>
					</asp:panel>
					<table cellSpacing="0" cellPadding="4" width="600" border="0">
						<ItemTemplate>
							<TBODY>
								<tr vAlign="top">
									<td align="left">
										<asp:Panel id="OriginalMessagePanel" runat="server">
											<TABLE id="Table1" cellSpacing="0" cellPadding="0" border="0">
												<TR>
													<TD colSpan="2">
														<tra:Label class="SubHead" id="Label1" runat="server" Text="Original Message" TextKey="DS_ORIGINALMSG"></tra:Label><BR>
														<BR>
													</TD>
												</TR>
												<TR>
													<TD>
														<tra:Label class="SubHead" id="Label2" runat="server" Text="Subject" TextKey="SUBJECT"></tra:Label></TD>
													<TD>&nbsp;
														<asp:label id="Title" runat="server"></asp:label></TD>
												</TR>
												<TR>
													<TD>
														<tra:Label class="SubHead" id="Label3" runat="server" Text="Author" TextKey="AUTHOR"></tra:Label></TD>
													<TD>&nbsp;
														<asp:label id="CreatedByUser" runat="server"></asp:label></TD>
												</TR>
												<TR>
													<TD>
														<tra:Label class="SubHead" id="Label4" runat="server" Text="Created Date" TextKey="DATE"></tra:Label></TD>
													<TD>&nbsp;
														<asp:label id="CreatedDate" runat="server"></asp:label></TD>
												</TR>
											</TABLE>
											<BR>
											<asp:label id="Body" runat="server"></asp:label>
											<P></P>
										</asp:Panel>
									</td>
								</tr>
						</ItemTemplate></TBODY></table>
				</div>
				<div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div></foot:Footer>
			</div>
		</form>
	</body>
</html>
