<%@ Control Language="c#" autoeventwireup="false" Inherits="Rainbow.Design.ProductItem" targetschema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="myOptions" TagName="Options" Src="MultiImageOptions.ascx" %>
<%@ Register TagPrefix="product" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow.ECommerce" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ import Namespace="Rainbow.Configuration" %>
<HR>
<table cellSpacing="0" cellPadding="4" border="0">
	<TBODY>
		<tr>
			<td colspan="2" align="left" valign="top" class="SubHead">
				<product:ProductLinkGoToDetails ProductID='<%# ProductID %>' runat="server" Text='<%#GetMetadata("ModelName")%>' id=ProductLinkGoToDetails1>
				</product:ProductLinkGoToDetails>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="left" valign="top" class="text">
				<tra:HyperLink id="Hyperlink3" Text="Edit" TextKey="EDIT" ImageUrl='<%# GetCurrentImageFromTheme("Buttons_Edit", "Edit.gif") %>' NavigateUrl='<%# "~/ECommerce/DesktopModules/ProductsEdit.aspx?" + GetMetadata("CmdEditProduct")%>' Visible='<%# GetMetadata("IsEditable") == "True"%>' runat="server" />
				<tra:Literal TextKey="PRODUCT_REFERENCE" Text="Reference" runat="server" id="Literal1" />:&nbsp;<%#GetMetadata("ModelNumber")%>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<table align="left" cellSpacing="0" cellPadding="0" border="0">
					<TBODY>
						<tr>
							<td vAlign="top">
								<table width="100%" cellpadding="4" cellspacing="0">
									<TBODY>
										<tr id="TrPrice" align="left" Visible='<%# (UnitPrice != 0)%>' runat="server">
											<td>
												<tra:Label TextKey="PRODUCT_PRICE" Text="Price" runat="server" id="Label1" />:
												<%# DisplayPrice(UnitPrice) %>
											</td>
										</tr>
										<tr Visible='<%# (TaxRate != 0)%>' align="left" runat="server">
											<td>
												<tra:Label id="Label2" Text="(inc. tax)" runat="server" TextKey="PRODUCT_INCLUDING_TAXES"></tra:Label>:
												<%# DisplayPrice(UnitPrice, TaxRate) %>
											</td>
										</tr>
										<TR align=left runat="server" Visible="<%# (UnitPrice == 0)%>">
											<TD class="smalltext">
												<tra:Label id="Label3" Text="Please contact us to purchase this item." runat="server" TextKey="PRODUCT_PRICE_NOT_AVAILABLE"></tra:Label></TD>
										</TR>
										<TR runat="Server" Visible="<%# (ProductHasOptions == true)%>">
											<td><myoptions:options id="ctlOptions" ImageProduct='<%# ProductImageGoToDetails1.ClientID %>' SetOptions='<%# OptionString %>' Runat="Server"></myoptions:options></td>
										</TR>
										<tr id="TrAddToCart" Visible='<%# (UnitPrice != 0)%>' runat="server" class="Normal">
											<td>
												<product:ProductAddToCart ProductID='<%# ProductID %>' class="CommandButton" TextKey="PRODUCT_ADD_TO_CART" Text="Add to Cart" runat="server" id=ProductAddToCart1>
												</product:ProductAddToCart>&nbsp;						
										</td>
										</tr>
									</TBODY>
								</table>
							</td>
						</tr>
					</TBODY>
				</table>
			</td>
			<td vAlign="top">
				<product:ProductImageGoToDetails ProductID='<%# ProductID %>' ImageUrl='<%# GetMetadata("ShopPath") + "/" + GetMetadata("ThumbnailFilename")%>' runat="server" Text='<%#GetMetadata("ModelName")%>' id=ProductImageGoToDetails1>
				</product:ProductImageGoToDetails>
			</td>
		</tr>
	</TBODY>
</table>
