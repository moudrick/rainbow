<%@ Register TagPrefix="myOptions" TagName="Options" Src="optionsEdit.ascx" %>
<%@ Page Language="c#" AutoEventWireup="false" Codebehind="ProductsEdit.aspx.cs" Inherits="Rainbow.ECommerce.ProductsEdit" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<HTML>
	<HEAD runat="server">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" runat="server">
		<form id="MainForm" encType="multipart/form-data" runat="server">
			<div class="rb_DefaultLayoutDiv">
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
									<td width="*">
										<table cellSpacing="0" cellPadding="0" width="500">
											<tr>
												<td class="Head" align="left"><asp:label id="PageTitleLabel" runat="server" Height="22"></asp:label></td>
											</tr>
											<tr>
												<td colSpan="2">
													<hr noShade SIZE="1">
												</td>
											</tr>
										</table>
										<table cellSpacing="0" cellPadding="0" width="726" border="0">
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal1" runat="server" Text="Display Order" TextKey="PICTURES_DISPLAY_ORDER"></tra:literal></td>
												<td><asp:textbox id="DisplayOrder" runat="server" CssClass="NormalTextBox" MaxLength="10" Width="100px">0</asp:textbox></td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal2" runat="server" Text="Flip" TextKey="PICTURES_FLIP"></tra:literal></td>
												<td><asp:dropdownlist id="selFlip" CssClass="NormalTextBox" Width="100px" Runat="server">
														<asp:ListItem Value="None" Selected="True">None</asp:ListItem>
														<asp:ListItem Value="X">X</asp:ListItem>
														<asp:ListItem Value="Y">Y</asp:ListItem>
														<asp:ListItem Value="XY">XY</asp:ListItem>
													</asp:dropdownlist></td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal3" runat="server" Text="Rotate" TextKey="PICTURES_ROTATE"></tra:literal></td>
												<td><asp:dropdownlist id="selRotate" CssClass="NormalTextBox" Width="100px" Runat="server">
														<asp:ListItem Value="None" Selected="True">None</asp:ListItem>
														<asp:ListItem Value="90">90</asp:ListItem>
														<asp:ListItem Value="180">180</asp:ListItem>
														<asp:ListItem Value="270">270</asp:ListItem>
													</asp:dropdownlist></td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal4" runat="server" Text="Model Name" TextKey="PRODUCT_MODELNAME"></tra:literal></td>
												<td><asp:textbox id="ModelName" runat="server" CssClass="NormalTextBox" MaxLength="255" Width="401px"></asp:textbox></td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal5" runat="server" Text="Model Number" TextKey="PRODUCT_MODELNUMBER"></tra:literal></td>
												<td><asp:textbox id="ModelNumber" runat="server" CssClass="NormalTextBox" MaxLength="255" Width="401px"></asp:textbox></td>
											</tr>
											<TR>
												<TD class="SubHead" noWrap><tra:literal id="Literal6" runat="server" Text="Unit Price" TextKey="PRODUCT_UNITPRICE"></tra:literal></TD>
												<TD noWrap><asp:textbox id="UnitPrice" runat="server" CssClass="NormalTextBox" MaxLength="10" Width="100px"></asp:textbox><asp:label id="lblCurrentCurrency" runat="server"></asp:label></TD>
											</TR>
											<TR>
												<TD class="SubHead" noWrap><tra:literal id="Literal7" runat="server" Text="Tax Rate (%)" TextKey="PRODUCT_TAXRATE"></tra:literal></TD>
												<TD noWrap><asp:textbox id="TaxRate" runat="server" CssClass="NormalTextBox" MaxLength="10" Width="100px"></asp:textbox></TD>
											</TR>
											<TR>
												<TD class="SubHead" noWrap><tra:literal id="Literal8" runat="server" Text="Weight (g)" TextKey="PRODUCT_WEIGHT"></tra:literal></TD>
												<TD noWrap><asp:textbox id="Weight" runat="server" CssClass="NormalTextBox" MaxLength="10" Width="100px"></asp:textbox></TD>
											</TR>
											<tr>
												<td class="SubHead" noWrap><tra:literal id="Literal9" runat="server" Text="Options" TextKey="PRODUCT_OPTIONS"></tra:literal></td>
												<td><myoptions:options id="ctlOptions1" Runat="Server" OptionName="option1"></myoptions:options><myoptions:options id="ctlOptions2" Runat="Server" OptionName="option2"></myoptions:options><myoptions:options id="ctlOptions3" Runat="Server" OptionName="option3"></myoptions:options></td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal10" runat="server" Text="Short Description" TextKey="PICTURES_SHORT_DESCRIPTION"></tra:literal></td>
												<td><asp:textbox id="ShortDescription" runat="server" Height="120px" CssClass="NormalTextBox" MaxLength="255"
														Width="401px" TextMode="MultiLine"></asp:textbox></td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal11" runat="server" Text="Long Description" TextKey="PICTURES_LONG_DESCRIPTION"></tra:literal></td>
												<td><asp:textbox id="LongDescription" runat="server" Height="120px" CssClass="NormalTextBox" MaxLength="255"
														Width="401px" TextMode="MultiLine"></asp:textbox></td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal12" runat="server" Text="Image File" TextKey="PICTURES_FILE"></tra:literal></td>
												<td><input id="flPicture" type="file" name="flPicture" runat="server">
												</td>
											</tr>
											<tr>
												<td class="SubHead" width="200"><tra:literal id="Literal13" runat="server" Text="Include EXIF" TextKey="PICTURES_INCLUDE_EXIF"></tra:literal></td>
												<td><asp:checkbox id="chkIncludeExif" Runat="server" Checked="True"></asp:checkbox></td>
											</tr>
											<TR>
												<td class="SubHead" width="200"><tra:literal id="Literal14" runat="server" Text="Featured Item?" TextKey="PRODUCT_FEATURED_ITEM"></tra:literal></td>
												<TD>
													<TABLE id="Table2" cellSpacing="0" cellPadding="1" align="left" border="0">
														<TR>
															<TD class="Normal" noWrap><asp:radiobuttonlist id="rblFeaturedItem" runat="server" CssClass="Normal" RepeatLayout="Flow" RepeatDirection="Horizontal"
																	RepeatColumns="2">
																	<asp:ListItem Value="1">Yes</asp:ListItem>
																	<asp:ListItem Value="0" Selected="True">No</asp:ListItem>
																</asp:radiobuttonlist></TD>
														</TR>
													</TABLE>
													<tra:literal id="Literal15" runat="server" Text="Featured items will be displayed on the root page"
														TextKey="PRODUCT_FEATURED_ITEM_DESCRIPTION"></tra:literal></TD>
											</TR>
											<TR>
												<td valign="top" class="SubHead" width="200"><tra:literal id="Literal16" runat="server" Text="Category" TextKey="PRODUCT_CATEGORY"></tra:literal></td>
												<TD><asp:dropdownlist id="ddlCategory" runat="server" CssClass="NormalTextBox" DataTextField="CategoryName"
														DataValueField="CategoryID"></asp:dropdownlist><BR>
													<tra:Literal TextKey="PRODUCT_CATEGORY_DESCRIPTION" runat="server" ID="Literal17">
												Before adding products, 
                  create categories! A category is created as a child page of 
                  the tab holding the E-Commerce module (simply use the create 
                  tab admin module!). <BR>Only featured items are displayed on the page where the module has been created.
											</tra:Literal>
												</TD>
											</TR>
										</table>
										<p><asp:linkbutton id="updateButton" runat="server" CssClass="CommandButton">Update</asp:linkbutton>&nbsp;&nbsp;
											<asp:linkbutton id="cancelButton" runat="server" CssClass="CommandButton" CausesValidation="False">Cancel</asp:linkbutton>&nbsp;&nbsp;
											<asp:linkbutton id="deleteButton" runat="server" CssClass="CommandButton" CausesValidation="False">Delete this product</asp:linkbutton><br>
											<hr width="500" noShade SIZE="1">
										<P><asp:label id="Message" runat="server" CssClass="NormalRed" ForeColor="Red"></asp:label></P>
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
