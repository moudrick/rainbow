<%@ Control Language="c#" autoeventwireup="false" Inherits="Rainbow.Design.ProductItem" targetschema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="product" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow.ECommerce" %>
<%@ Register TagPrefix="myOptions" TagName="Options" Src="options.ascx" %>
<%@ import Namespace="Rainbow.Configuration" %>
<table bordercolor="#ffffff" cellspacing="0" cellpadding="0" width="95%" align="center" border="0">
    <tbody>
        <tr>
            <td>
                <table cellspacing="0" cellpadding="4" width="100%" border="0">
                    <tbody>
                        <tr>
                            <td class="SubHeadShop" valign="top" align="left">
                                <asp:Label id="Label1" Text='<%#GetMetadata("ModelName")%>' runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="text" valign="top" align="left">
                                <TRA:LITERAL id="Literal1" Text="Reference" runat="server" TextKey="PRODUCT_REFERENCE" />
                                :&nbsp;<%#GetMetadata("ModelNumber")%> 
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
        <tr>
            <td class="textwhite">
                <img height='<%#GetMetadata("ModifiedHeight")%>' hspace="10" src='<%#GetMetadata("ShopPath") + "/" + GetMetadata("ModifiedFilename")%>' width='<%#GetMetadata("ModifiedWidth")%>' align="left" border="1" /> 
                <table cellspacing="0" cellpadding="0" align="left" border="0">
                    <tbody>
                        <tr>
                            <td valign="top">
                                <table cellspacing="0" cellpadding="4" width="100%">
                                    <tbody>
                                        <tr id="TrPrice" align="left" runat="server" visible="<%# (UnitPrice != 0)%>">
                                            <td class="price">
                                                <TRA:LABEL Text="Price" runat="server" TextKey="PRODUCT_PRICE" />
                                                : <%# DisplayPrice(UnitPrice) %></td>
                                        </tr>
                                        <tr align="left" runat="server" visible="<%# (TaxRate != 0)%>">
                                            <td class="taxprice">
                                                <TRA:LABEL Text="(inc. tax)" runat="server" TextKey="PRODUCT_INCLUDING_TAXES" />
                                                : <%# DisplayPrice(UnitPrice, TaxRate) %></td>
                                        </tr>
                                        <tr align="left" runat="server" visible="<%# (UnitPrice == 0)%>">
                                            <td class="smalltext">
                                                <br />
                                                <TRA:LABEL Text="Please contact us to purchase this item." runat="server" TextKey="PRODUCT_PRICE_NOT_AVAILABLE" />
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
        <tr>
            <td class="text">
                <br />
                <%#GetMetadata("LongDescription")%></td>
        </tr>
    </tbody>
</table>
