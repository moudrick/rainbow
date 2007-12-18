<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.Design.ProductItem" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ import Namespace="Rainbow.Configuration" %>
<%@ Register TagPrefix="myOptions" TagName="Options" Src="options.ascx" %>
<%@ Register TagPrefix="product" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow.ECommerce" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<table width="100%" cellSpacing="0" cellPadding="4" border="0">
	<TBODY>
		<tr>
			<td colspan="2" align="left" valign="top" class="SubHead">
				<product:ProductLinkGoToDetails ProductID='<%# ProductID %>' runat="server" Text='<%#GetMetadata("ModelName")%>'></product:ProductLinkGoToDetails>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="left" valign="top" class="text">
				<tra:Literal TextKey="PRODUCT_REFERENCE" Text="Reference" runat="server" id="Literal1" />:&nbsp;<%#GetMetadata("ModelNumber")%>
			</td>
		</tr>
		<tr>
			<td>
				<product:ProductImageGoToDetails ProductID='<%# ProductID %>' ImageUrl='<%# GetThumbnailImageUrl(true)%>' runat="server" Text='<%#GetMetadata("ModelName")%>' width='<%#GetThumbnailImageWidth()%>' height='<%#GetThumbnailImageHeight()%>'></product:ProductImageGoToDetails>
			</td>
			<td width="100%" valign="top">
				<table width="100%" align="left" cellSpacing="0" cellPadding="0" border="0">
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
												<tra:Label TextKey="PRODUCT_INCLUDING_TAXES" Text="(inc. tax)" runat="server" id="Label2" />:
												<%# DisplayPrice(UnitPrice, TaxRate) %>
											</td>
										</tr>
										<tr Visible='<%# (UnitPrice == 0)%>' align="left" runat="server">
											<td class="smalltext">
												<br>
												<tra:Label TextKey="PRODUCT_PRICE_NOT_AVAILABLE" Text="Please contact us to purchase this item." runat="server" id="Label3" />
											</td>
										</tr>
										<tr Visible = '<%# (ProductHasOptions == true)%>' runat="Server">
											<td><myoptions:options id="ctlOptions" SetOptions='<%# OptionString %>' Runat="Server"></myoptions:options></td>
										</tr>
										<tr id="TrAddToCart" Visible='<%# (UnitPrice != 0)%>' runat="server" class="Normal">
											<td>
												<BR>
												<product:ProductAddToCart ProductID='<%# ProductID %>' class="CommandButton" TextKey="PRODUCT_ADD_TO_CART" Text="Add to Cart" runat="server"></product:ProductAddToCart>
											</td>
										</tr>
										<tr id="TrShortDescription" runat="server">
											<td class="textwhite">
												<tra:HyperLink id="Hyperlink3" Text="Edit" TextKey="EDIT" ImageUrl='<%# GetCurrentImageFromTheme("Buttons_Edit", "Edit.gif") %>' NavigateUrl='<%# "~/ECommerce/DesktopModules/ProductsEdit.aspx?" + GetMetadata("CmdEditProduct")%>' Visible='<%# GetMetadata("IsEditable") == "True"%>' runat="server" />
												<%# GetMetadata("ShortDescription") %>
												<BR>
											</td>
										</tr>
										<tr Visible='<%# (Weight != 0)%>' align="Left" runat="server">
											<td>
												<tra:Label TextKey="WEIGHT" Text="Weight" runat="server" />:
												<%# Weight %>g
											</td>
										</tr>

									</TBODY>
								</table>
							</td>
						</tr>
					</TBODY>
				</table>
			</td>
		</tr>
	</TBODY>
</table>
<HR noShade SIZE="1">

