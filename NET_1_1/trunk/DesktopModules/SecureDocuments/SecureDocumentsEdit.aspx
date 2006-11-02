<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Page Language="c#" CodeBehind="SecureDocumentsEdit.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SecureDocumentsEdit" %>
<HTML>
	<HEAD runat="server" />
	<body runat="server">
		<form encType="multipart/form-data" runat="server">
			<div class="rb_AlternateLayoutDiv">
				<table class="rb_AlternateLayoutTable">
					<tr vAlign="top">
						<td class="rb_AlternatePortalHeader" vAlign="top"><portal:banner id="SiteHeader" runat="server"></portal:banner></td>
					</tr>
					<tr>
						<td><br>
							<table cellSpacing="0" cellPadding="4" width="98%" border="0">
								<tr vAlign="top">
									<td width="150">&nbsp;
									</td>
									<td>
										<table cellSpacing="0" cellPadding="0" width="500">
											<tr>
												<td class="Head" align="left"><tra:label id="PageTitleLabel" runat="server" Height="22"></tra:label></td>
											</tr>
											<tr>
												<td colSpan="2">
													<hr noShade SIZE="1">
												</td>
											</tr>
										</table>
										<table cellSpacing="0" cellPadding="0" width="726" border="0">
											<tr vAlign="top">
												<td class="SubHead" width="100"><tra:label id="FileNameLabel" runat="server" Height="22" TextKey="FILE_NAME">File name</tra:label></td>
												<td>&nbsp;
												</td>
												<td><asp:textbox id="NameField" runat="server" cssclass="NormalTextBox" width="353" Columns="28"
														maxlength="150"></asp:textbox></td>
												<td class="Normal" width="250"><tra:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" TextKey="ERROR_VALID_NAME" CssClass="Error"
														Display="Static" ErrorMessage="You Must Enter a Valid Name" ControlToValidate="NameField"></tra:requiredfieldvalidator></td>
											</tr>
											<tr vAlign="top">
												<td class="SubHead"><tra:label id="CategoryLabel" runat="server" Height="22" TextKey="FILE_CATEGORY">Category</tra:label></td>
												<td>&nbsp;
												</td>
												<td><asp:textbox id="CategoryField" runat="server" cssclass="NormalTextBox" width="353" Columns="28"
														maxlength="50"></asp:textbox></td>
											</tr>
											<tr>
												<td class="SubHead"><tra:label id="Label1" runat="server" Height="22" TextKey="FILE_PERMISSIONS">File Permissions</tra:label></td>
												<td colSpan="2">
													<hr width="100%" noShade SIZE="1">
													<asp:checkboxlist id="rolesList" Runat="server" RepeatDirection="Horizontal" RepeatColumns="2" /></td>
											</tr>
											<TR>
												<td class="SubHead"><tra:label id="Label2" runat="server" Height="22" TextKey="FILES">Files</tra:label></td>
												<TD></TD>
												<TD></TD>
											</TR>
											<TR>
												<td></td>
												<TD colSpan="2"><asp:datagrid id="DataGridFiles" Runat="server" OnDeleteCommand="DataGridFiles_OnDeleteCommand"
														AllowPaging="False" AllowSorting="False" AllowCustomPaging="False" AutoGenerateColumns="False" BorderWidth="0"
														CellPadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False" OnItemDataBound="DataGridFiles_OnItemDataBound" >
														<Columns>
															<asp:TemplateColumn>
																<ItemTemplate>
																	<tra:LinkButton id="DeleteFile" runat="server" Text="Delete" CausesValidation="False" 
																		CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ItemID")%>' CommandName="Delete" />
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn >
																<ItemTemplate>
																	<asp:Label ID="dash" Runat="server">&nbsp;&nbsp;-&nbsp;&nbsp;</asp:Label>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:BoundColumn DataField="FileName" />
															<asp:TemplateColumn >
																<ItemTemplate>
																	<asp:Label ID="extraColumnName" Runat="server" />
																</ItemTemplate>
															</asp:TemplateColumn>
														</Columns>
													</asp:datagrid></TD>
											</TR>
											<TR>
												<TD colspan="3">&nbsp;</TD>
											</TR>
											<tr vAlign="top">
												<td class="SubHead" width="100"><tra:label id="UrlLabel" runat="server" Height="22" TextKey="URL">Url</tra:label></td>
												<td>&nbsp;
												</td>
												<td><asp:textbox id="PathField" runat="server" cssclass="NormalTextBox" width="353" Columns="28"
														maxlength="250"></asp:textbox></td>
											</tr>
											<tr>
												<td class="SubHead" height="36"><tra:label id="OrLabel" runat="server" Height="22" TextKey="OR">or</tra:label></td>
												<td colSpan="2" height="36">&nbsp;
													<br>
												</td>
											</tr>
											<tr vAlign="top">
												<td class="SubHead" vAlign="middle" noWrap><tra:label id="UploadLabel" runat="server" Height="22" TextKey="FILE_UPLOAD">Upload File</tra:label>&nbsp;
												</td>
												<td>&nbsp;
												</td>
												<td><input id="FileUpload" style="WIDTH: 353px; FONT-FAMILY: verdana" type="file" name="FileUpload"
														runat="server" width="300">
												</td>
											</tr>
											<TR>
												<TD class="SubHead" vAlign="middle" noWrap></TD>
												<TD></TD>
												<TD align="center"></TD>
											</TR>
											<TR>
												<TD class="SubHead" vAlign="middle" noWrap></TD>
												<TD></TD>
												<TD></TD>
											</TR>
											<TR>
												<TD class="SubHead" vAlign="middle" noWrap></TD>
												<TD></TD>
												<TD></TD>
											</TR>
										</table>
										<p><tra:linkbutton class="CommandButton" id="updateButton" runat="server" Text="Update"></tra:linkbutton>&nbsp;
											<tra:linkbutton class="CommandButton" id="cancelButton" runat="server" Text="Cancel" CausesValidation="False" />&nbsp;
											<tra:linkbutton class="CommandButton" id="deleteButton" runat="server" Text="Delete this item" CausesValidation="False" />&nbsp;
											<hr noshade size="1">
											<span class="Normal">
											<tra:Literal TextKey="CREATED_BY" Text="Created by" id="CreatedLabel" runat="server"></tra:Literal>&nbsp;
											<asp:label id="CreatedBy" runat="server"></asp:label>&nbsp;
											<tra:Literal TextKey="ON" Text="on" id="OnLabel" runat="server"></tra:Literal>&nbsp;
											<asp:label id="CreatedDate" runat="server"></asp:label>
										</span>
										<P>
											<tra:Label id="Message" runat="server" ForeColor="Red" CssClass="NormalRed"></tra:Label>
										</P>
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
</HTML>
