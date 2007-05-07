<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Products.ascx.cs" Inherits="Rainbow.DesktopModules.Products" %>
<TABLE class="Normal" CssClass="rb_DefaultLayoutDiv" id="ControlTable" cellSpacing="0"
	cellPadding="0" width="100%" border="0">
	<TR id="DisplayTitlePanel" runat="server">
		<TD>
			<tra:label id="lblTitle" CssClass="SubHead" runat="server" EnableViewState="True" Font-Bold="true"></tra:label>
		</TD>
	</TR>
	<TR id="DisplayMessagePanel" runat="server">
		<TD>
			<tra:label id="lblMessage" CssClass="Error" runat="server"></tra:label>
		</TD>
	</TR>
	<TR id="DisplayHeaderPanel" runat="server">
		<TD align="right" nowrap>
			<table cellSpacing="10" cellPadding="0" align="right" border="0">
				<tr>
					<td class="normal">
						<tra:LinkButton id="AccountBtn" class="CommandButton" TextKey="PRODUCT_VIEW_ACCOUNT" runat="server"
							Visible="False" Text="View account"></tra:LinkButton>
					</td>
					<td class="normal">
						<tra:LinkButton id="ViewCartBtn" class="CommandButton" TextKey="PRODUCT_VIEW_CART" runat="server"
							Visible="False" Text="View cart"></tra:LinkButton>
					</td>
				</tr>
			</table>
		</TD>
	</TR>
	<TR id="DisplayProductListPanel" runat="server">
		<TD valign="top" class="normal">
			<!-- List of Products -->
			<asp:datalist id="ProductList" runat="server" ItemStyle-Width="1%" EnableViewState="False">
				<ItemStyle VerticalAlign="Top" Width="1%"></ItemStyle>
			</asp:datalist>
			<div align="center"><cc1:paging id="pgProducts" runat="server"></cc1:paging></div>
		</TD>
	</TR>
	<tr id="DisplayProductPanel" runat="server">
		<td class="normal">
			<!-- Details of Selected Product -->
			<asp:placeholder id="ProductDetails" runat="server" EnableViewState="False"></asp:placeholder>
		</td>
	</tr>
	<tr id="DisplayCartPanel" runat="server">
		<td class="normal" align="center">
			<!-- Shopping Cart, set ProductID invisible (needed to update cart) --><br>
			<TABLE cellSpacing="0" cellPadding="0" border="0">
				<TR vAlign="top">
					<TD>
						<asp:datagrid id="CartList" runat="server" BorderColor="Black" GridLines="Vertical" cellpadding="4"
							Font-Size="8pt" Font-Names="Verdana" HeaderStyle-CssClass="CartListHead" FooterStyle-CssClass="CartListFooter"
							ItemStyle-CssClass="CartListItem" AlternatingItemStyle-CssClass="CartListItemAlt" DataKeyField="ProductID"
							AutoGenerateColumns="False">
							<AlternatingItemStyle CssClass="CartListItemAlt"></AlternatingItemStyle>
							<ItemStyle CssClass="CartListItem"></ItemStyle>
							<HeaderStyle CssClass="CartListHead"></HeaderStyle>
							<FooterStyle CssClass="CartListFooter"></FooterStyle>
							<Columns>
								<tra:TemplateColumn Visible="False" HeaderText="ProductID" TextKey="PRODUCT_ID">
									<ItemTemplate>
										<asp:Label id="ProductID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ProductID") %>' />
										<asp:Label id="SavedMetadataXml" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MetadataXml") %>' />
									</ItemTemplate>
								</tra:TemplateColumn>
								<tra:TemplateColumn HeaderText="Name" TextKey="PRODUCT_NAME">
									<ItemTemplate>
										<asp:LinkButton CommandName="CartItemInfo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ModelName") %>' CausesValidation="False">
										</asp:LinkButton>
										<br>
										<asp:Label id="MetadataXml" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MetadataXml") %>' />
									</ItemTemplate>
								</tra:TemplateColumn>
								<tra:BoundColumn TextKey="PRODUCT_REFERENCE" DataField="ModelNumber" HeaderText="Reference"></tra:BoundColumn>
								<tra:TemplateColumn HeaderText="Quantity" TextKey="PRODUCT_QUANTITY">
									<ItemTemplate>
										<asp:TextBox id="Quantity" runat="server" Columns="4" MaxLength="3" Text='<%# DataBinder.Eval(Container.DataItem, "Quantity") %>' width="40px" CssClass="NormalTextBox"/>
									</ItemTemplate>
								</tra:TemplateColumn>
								<tra:BoundColumn TextKey="PRODUCT_PRICE" DataField="UnitCost" HeaderText="Price"></tra:BoundColumn>
								<tra:BoundColumn TextKey="PRODUCT_PRICE_WITH_TAX" DataField="UnitCostWithTaxes" HeaderText="Price (inc. taxes)"></tra:BoundColumn>
								<tra:BoundColumn TextKey="PRODUCT_SUBTOTAL" DataField="ExtendedAmount" HeaderText="Subtotal"></tra:BoundColumn>
								<tra:TemplateColumn HeaderText="Remove" TextKey="PRODUCT_REMOVE">
									<ItemTemplate>
										<center>
											<asp:CheckBox id="Remove" runat="server" />
										</center>
									</ItemTemplate>
								</tra:TemplateColumn>
							</Columns>
						</asp:datagrid></TD>
				</TR>
				<tr>
					<td align="right">
						<P>
							<tra:Label id="txtTotal" runat="server" CssClass="NormalBold" Text="Total" TextKey="PRODUCT_TOTAL">Total</tra:Label>:
							<asp:label id="lblTotal" runat="server" EnableViewState="false" CssClass="NormalBold"></asp:label><BR>
							<tra:Label id="txtTotalTaxes" runat="server" CssClass="NormalBold" Text="Total" TextKey="PRODUCT_TOTAL_TAXES">Total Taxes</tra:Label>:
							<asp:label id="lblTotalTaxes" runat="server" EnableViewState="false" CssClass="NormalBold"></asp:label><BR>
							<tra:Label id="txtTotalWithTaxes" runat="server" CssClass="NormalBold" Text="Total" TextKey="PRODUCT_TOTAL_WITH_TAXES">Total with Taxes</tra:Label>:
							<asp:label id="lblTotalWithTaxes" runat="server" EnableViewState="false" CssClass="NormalBold"></asp:label></P>
					</td>
				</tr>
				<TR>
					<TD class="Error" align="right"><BR>
						<tra:Literal TextKey="PRODUCT_MUST_LOG_IN_FOR_CHECKOUT" Text="You must first login or register to check out!"
							runat="server" id="pleaseLogon" /><br>
						<br>
						<tra:LinkButton class="CommandButton" id="UpdateBtn" runat="server" Text="Update" TextKey="PRODUCT_UPDATE"
							CausesValidation="False"></tra:LinkButton>&nbsp;&nbsp;
						<tra:LinkButton class="CommandButton" id="CheckoutBtn" runat="server" Text="Check out" TextKey="PRODUCT_CHECKOUT"
							CausesValidation="False"></tra:LinkButton></TD>
				</TR>
			</TABLE>
			<!-- End Shopping Cart -->
		</td>
	</tr>
	<tr id="DisplayOrderListPanel" runat="server">
		<td align="center">
			<table height="100%" cellspacing="0" cellpadding="0" width="550" border="0">
				<tr valign="top">
					<TD align="center"><br>
						<asp:datagrid id="OrderList" runat="server" BorderColor="Black" GridLines="Vertical" cellpadding="4"
							Font-Name="Verdana" Font-Size="8pt" HeaderStyle-CssClass="CartListHead" FooterStyle-CssClass="CartListFooter"
							ItemStyle-CssClass="CartListItem" AlternatingItemStyle-CssClass="CartListItemAlt" DataKeyField="OrderID"
							AutoGenerateColumns="False" Font-Names="Verdana">
							<AlternatingItemStyle CssClass="CartListItemAlt"></AlternatingItemStyle>
							<ItemStyle CssClass="CartListItem"></ItemStyle>
							<HeaderStyle CssClass="CartListHead"></HeaderStyle>
							<FooterStyle CssClass="CartListFooter"></FooterStyle>
							<Columns>
								<tra:TemplateColumn HeaderText="Order Reference" TextKey="PRODUCT_ORDER_REFERENCE">
									<ItemTemplate>
										<asp:LinkButton id="OrderID" CommandName="OrderItemInfo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderID") %>' CausesValidation="False">
										</asp:LinkButton>
									</ItemTemplate>
								</tra:TemplateColumn>
								<tra:BoundColumn TextKey="PRODUCT_ORDER_DATE_CREATED" DataField="DateCreated" HeaderText="Date Created"
									DataFormatString="{0:d}"></tra:BoundColumn>
								<tra:BoundColumn TextKey="PRODUCT_ORDER_DATE_MODIFIED" DataField="DateModified" HeaderText="Last Modified"
									DataFormatString="{0:d}"></tra:BoundColumn>
								<tra:BoundColumn TextKey="PRODUCT_ORDER_TOTAL" DataField="TotalGoods" HeaderText="Total"></tra:BoundColumn>
							</Columns>
						</asp:datagrid>
					</TD>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="DisplayOrderPanel" runat="server">
		<td class="normal" align="center">
			<!-- Details of Selected Order -->
			<asp:placeholder id="OrderDetails" runat="server"></asp:placeholder>
		</td>
	</tr>
	<tr id="DisplayFooterPanel" runat="server">
		<td><br>
			<table cellSpacing="0" cellPadding="0" width="400" align="center" border="0">
				<tr>
					<td class="normal">
						<tra:LinkButton class="CommandButton" id="ContinueBtn" runat="server" TextKey="PRODUCT_CONTINUE_SHOPPING"
							Text="Continue shopping"></tra:LinkButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</TABLE>
