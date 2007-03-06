<%@ Control Language="c#" autoeventwireup="false" Inherits="Rainbow.Design.ProductItem" targetschema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="myOptions" TagName="Options" Src="options.ascx" %>
<%@ Register TagPrefix="product" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow.ECommerce" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ import Namespace="Rainbow.Configuration" %>
<table cellspacing="0" cellpadding="4" width="100%" border="0">
    <tbody>
        <tr>
            <td class="SubHeadShop" valign="top" align="left" colspan="2">
                <PRODUCT:PRODUCTLINKGOTODETAILS Text='<%#GetMetadata("ModelName")%>' runat="server" ProductID="<%# ProductID %>"></PRODUCT:PRODUCTLINKGOTODETAILS>
            </td>
        </tr>
        <tr>
            <td class="text" valign="top" align="left" colspan="2">
                <TRA:LITERAL id="Literal1" Text="Reference" runat="server" TextKey="PRODUCT_REFERENCE" />
                :&nbsp;<%#GetMetadata("ModelNumber")%> 
            </td>
        </tr>
        <tr>
            <td>
                <PRODUCT:PRODUCTIMAGEGOTODETAILS Text='<%#GetMetadata("ModelName")%>' runat="server" ProductID="<%# ProductID %>" height='<%#Unit.Parse("0" + GetMetadata("ThumbnailHeight")+ "px")%>' width='<%#Unit.Parse("0" + GetMetadata("ThumbnailWidth")+ "px")%>' ImageUrl='<%# GetMetadata("ShopPath") + "/" + GetMetadata("ThumbnailFilename")%>'></PRODUCT:PRODUCTIMAGEGOTODETAILS>
            </td>
            <td valign="top" width="100%">
                <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                    <tbody>
                        <tr>
                            <td valign="top">
                                <table cellspacing="0" cellpadding="4" width="100%">
                                    <tbody>
                                        <tr id="TrPrice" align="left" runat="server" visible="<%# (UnitPrice != 0)%>">
                                            <td class="price">
                                                <TRA:LABEL id="Label1" Text="Price" runat="server" TextKey="PRODUCT_PRICE" />
                                                : <%# DisplayPrice(UnitPrice) %></td>
                                        </tr>
                                        <tr align="left" runat="server" visible="<%# (TaxRate != 0)%>">
                                            <td class="taxprice">
                                                <TRA:LABEL id="Label2" Text="(inc. tax)" runat="server" TextKey="PRODUCT_INCLUDING_TAXES" />
                                                : <%# DisplayPrice(UnitPrice, TaxRate) %></td>
                                        </tr>
                                        <tr align="left" runat="server" visible="<%# (UnitPrice == 0)%>">
                                            <td class="smalltext">
                                                <br />
                                                <TRA:LABEL id="Label3" Text="Please contact us to purchase this item." runat="server" TextKey="PRODUCT_PRICE_NOT_AVAILABLE" />
                                            </td>
                                        </tr>
                                        <tr runat="Server" visible="<%# (ProductHasOptions == true)%>">
                                            <td>
                                                <myoptions:options id="ctlOptions" Runat="Server" SetOptions="<%# OptionString %>"></myoptions:options>
                                            </td>
                                        </tr>
                                        <tr class="Normal" id="TrAddToCart" runat="server" visible="<%# (UnitPrice != 0)%>">
                                            <td>
                                                <br />
                                                <img src="/_shop/Themes/shop/img/carello.gif" align="middle" /> 
                                                <PRODUCT:PRODUCTADDTOCART Text="Add to Cart" runat="server" ProductID="<%# ProductID %>" TextKey="PRODUCT_ADD_TO_CART"></PRODUCT:PRODUCTADDTOCART>
                                            </td>
                                        </tr>
                                        <tr id="TrShortDescription" runat="server">
                                            <td class="textwhite">
                                                <TRA:HYPERLINK id="Hyperlink3" Text="Edit" runat="server" TextKey="EDIT" ImageUrl="~/images/edit.gif" Visible='<%# GetMetadata("IsEditable") == "True"%>' NavigateUrl='<%# "~/ECommerce/DesktopModules/ProductsEdit.aspx?" + GetMetadata("CmdEditProduct")%>' />
                                                <%# GetMetadata("ShortDescription") %>
                                                <br />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<hr noshade="noshade" size="1" />
