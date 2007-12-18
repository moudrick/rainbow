<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Page Language="c#" codebehind="BooksEdit.aspx.cs" autoeventwireup="false" Inherits="AmazonFull.BooksEdit" %>
<html>
	<head runat="server"></head>
	<body runat="server">
		<form runat="server" enctype="multipart/form-data" ID="Form1">
		<div class="zen-main" id="zenpanes">
			<div class="rb_DefaultPortalHeader">
				<portal:banner id="SiteHeader" runat="server"></portal:banner>
			</div>
			<div class="div_ev_Table">
				<table cellSpacing="0" cellPadding="4" width="98%" border="0">
					<tbody>
							<TR>
								<TD noWrap colSpan="3">
									<div class="Head">Book Details</div>
									<HR noShade SIZE="1">
									<P>this Amazon Module <B>(bulid #2004-3)<br />
										</B>created by Charles Carroll of <A href="http://www.learnasp.com" target="learnasp">
											learnAsp.com</A> and Rahul Xing of <A href="http://www.KonoTree.com" target="kono">
											KonoTree.com</A></P>
									<P>
										<tra:LinkButton id="topUpdateButton" runat="server" CssClass="CommandButton" Text="Update">Update</tra:LinkButton>&nbsp;
										<tra:LinkButton id="topCancelButton" runat="server" CssClass="CommandButton" Text="Cancel" CausesValidation="False">Cancel</tra:LinkButton>&nbsp;
										<tra:LinkButton id="topDeleteButton" runat="server" CssClass="CommandButton" Text="Delete this item"
											CausesValidation="False">Delete this item</tra:LinkButton>
										<br />
									</P>
								</TD>
							</TR>
							<tr valign="top">
								<td class="SubHead" nowrap>
									ISBN:
								</td>
								<td>
									<asp:TextBox id="ISBNField" runat="server" maxlength="100" Columns="30" width="390" cssclass="NormalTextBox"></asp:TextBox>
								</td>
								<td class="Normal" width="250">
									<P>
										<asp:RequiredFieldValidator id="ISBNRequiredValidator" runat="server" ControlToValidate="ISBNField" ErrorMessage="You Must Enter a Valid ISBN"
											Display="Static"></asp:RequiredFieldValidator></P>
								</td>
							</tr>
							<TR>
								<TD class="SubHead" vAlign="top" noWrap>Add a caption:</TD>
								<TD>
									<asp:TextBox id="CaptionTextBox" runat="server" Width="390px" TextMode="MultiLine" Rows="16"></asp:TextBox></TD>
								<TD class="Normal" width="250"></TD>
							</TR>
							<TR>
								<TD class="SubHead" vAlign="top" noWrap colSpan="3">
									<P>
										<tra:LinkButton id="bottomUpdateButton" runat="server" CssClass="CommandButton" Text="Update">Update</tra:LinkButton>&nbsp;
										<tra:LinkButton id="bottomCancelButton" runat="server" CssClass="CommandButton" Text="Cancel" CausesValidation="False">Cancel</tra:LinkButton>&nbsp;
										<tra:LinkButton id="bottomDeleteButton" runat="server" CssClass="CommandButton" Text="Delete this item"
											CausesValidation="False">Delete this item</tra:LinkButton>
										<HR align="left" width="100%" noShade SIZE="1">
									<P></P>
									<P>
										<SPAN class="Normal">
										<tra:Literal id="CreatedLabelLiteral" runat="server" Text="Created By: " TextKey="CREATED_BY"></tra:Literal>&nbsp; 
										<asp:label id="CreatedBy" runat="server"></asp:label>&nbsp; 
														<tra:Literal id="CreatedDateLiteral" runat="server" Text="On: " TextKey="CREATED_ON"></tra:Literal>&nbsp; 
										<asp:label id="CreatedDate" runat="server"></asp:label></SPAN>
									</P>
								</TD>
							</TR>
						</tbody>
					</table>
				</div>
			<div class="rb_AlternatePortalFooter">
				<div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div></foot:Footer>
			</div>
			</div>
		</form>
	</body>
</html>
