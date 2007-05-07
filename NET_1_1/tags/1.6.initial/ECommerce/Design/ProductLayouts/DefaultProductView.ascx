<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ import Namespace="Rainbow.Configuration" %>
<%@ Register TagPrefix="product" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow.ECommerce" %>
<%@ Register TagPrefix="myOptions" TagName="Options" Src="options.ascx" %>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.Design.ProductItem" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table borderColor="#ffffff" align="center" cellSpacing="0" cellPadding="0" width="95%" border="0">
	<tr>
		<td>
			<table cellSpacing="0" cellPadding="4" width="100%" border="0">
				<tr>
					<td align="left" valign="top" class="SubHead">
						<asp:Label runat="server" Text='<%#GetMetadata("ModelName")%>'>
						</asp:Label>
					</td>
				</tr>
				<tr>
					<td align="left" valign="top" class="text">
						<tra:Literal TextKey="PRODUCT_REFERENCE" Text="Reference" runat="server" id="Literal1" />:&nbsp;<%#GetMetadata("ModelNumber")%>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td class="textwhite">
			<img align="left" hspace="10" width='<%#GetModifiedImageWidth()%>' height='<%#GetModifiedImageHeight()%>' src='<%#GetModifiedImageUrl(true)%>' border='1'>
			<table align="left" cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<td vAlign="top">
						<table width="100%" cellpadding="4" cellspacing="0">
							<tr id="TrPrice" align="Left" Visible='<%# (UnitPrice != 0)%>' runat="server">
								<td>
									<tra:Label TextKey="PRODUCT_PRICE" Text="Price" runat="server" />:
									<%# DisplayPrice(UnitPrice) %>
								</td>
							</tr>
							<tr Visible='<%# (TaxRate != 0)%>' align="Left" runat="server">
								<td>
									<tra:Label TextKey="PRODUCT_INCLUDING_TAXES" Text="(inc. tax)" runat="server" />:
									<%# DisplayPrice(UnitPrice, TaxRate) %>
								</td>
							</tr>
							<tr Visible='<%# (UnitPrice == 0)%>' align="Left" runat="server" >
								<td class="smalltext">
									<br>
									<tra:Label TextKey="PRODUCT_PRICE_NOT_AVAILABLE" Text="Please contact us to purchase this item." runat="server" />
								</td>
							</tr>
							<tr Visible='<%# (Weight != 0)%>' align="Left" runat="server">
								<td>
									<tra:Label TextKey="WEIGHT" Text="Weight" runat="server"/>:
									<%# Weight %>g
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<br>
			<%#GetMetadata("LongDescription")%>
		</td>
	</tr>
</table>
