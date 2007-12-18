<%@ Page Language="c#" codebehind="ProductsCheckOut.aspx.cs" autoeventwireup="false" Inherits="Rainbow.ECommerce.ProductsCheckOut" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="secure" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="secure" TagName="Footer" Src="Footer.ascx" %>
<html>
<head>
</head>
<body>
<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
        
            <tbody><form id="ProductsCheckOut" runat="server" class="rb_AlternateLayoutTable">
					<tr valign="top">
						<td>
							<secure:header id="Header1" runat="server"></secure:header>
						</td>
					</tr>
					<tr>
                    <td valign="top" align="middle">
							<table cellspacing="0" cellpadding="0" width="650" align="center" border="0">
								<tbody>
									<tr valign="top">
										<td align="left" colspan="3">
											<p>
                                            <TRA:LABEL id="MyError" runat="Server" EnableViewState="false" cssclass="Error"></TRA:LABEL>
											</p>
											<p>
                                            <TRA:LABEL id="Message01" runat="server" Text="Please verify the information below to make sure it is correct." TextKey="CHECKOUT_VERIFY_INFORMATION" CssClass="Normal"></TRA:LABEL>
                                            <br />
												<TRA:LABEL id="Message02" runat="server" Text="Click on the back button to go to the previous step, click on the continue button to proceed to the next step." TextKey="CHECKOUT_VERIFY_INFORMATION_CLICK_BACK" CssClass="Normal"></TRA:LABEL>
											</p>
										</td>
									</tr>
                                <asp:panel id="PanelStep1" runat="server">
                                    <!-- STEP 1 : CHECK ACCOUNT INFORMATION -->
                                    <tr>
                                        <td class="head" align="left" colspan="3">
                                            <br />
                                            <TRA:LABEL id="Panel1Label" runat="server" Text="STEP 1/4: Profile Information" TextKey="CHECKOUT_STEP1"></TRA:LABEL>
                                            <br />
                                            
                                            <hr noshade="noshade" size="1" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <TRA:LABEL class="TitleHead" id="NameLabel" runat="server" Text="Name" TextKey="NAME" Height="22" Width="83px"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="NameField" runat="server" Width="350px" size="25"></asp:TextBox>
                                        </td>
                                        <td class="normal">
                                            <TRA:REQUIREDFIELDVALIDATOR id="RequiredName" runat="server" TextKey="VALID_NAME" ControlToValidate="NameField" ErrorMessage="'Name' must not be left blank"></TRA:REQUIREDFIELDVALIDATOR>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="CompanyLabel" runat="server" EnableViewState="False" Text="Company" TextKey="COMPANY"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="CompanyField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="AddressLabel" runat="server" EnableViewState="False" Text="Address" TextKey="ADDRESS"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="AddressField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="CityLabel" runat="server" EnableViewState="False" Text="City" TextKey="CITY"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="CityField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ZipLabel" runat="server" EnableViewState="False" Text="Postal Code/Zip" TextKey="ZIP"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ZipField" runat="server" cssclass="NormalTextBox" maxlength="10" Columns="28" width="60px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="CountryLabel" runat="server" Text="Country" TextKey="COUNTRY"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:DropDownList id="CountryField" runat="server" Width="350px" DataTextField="DisplayName" DataValueField="Name" AutoPostBack="True"></asp:DropDownList>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr id="StateRow" runat="server">
                                        <td class="TitleHead">
                                            <TRA:LABEL id="StateLabel" runat="server" Text="Province/State" TextKey="PROV_STATE"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:DropDownList id="StateField" runat="server" Width="170px" DataTextField="DisplayName" DataValueField="CountryCode"></asp:DropDownList>
                                            <TRA:LABEL id="InLabel" runat="server" Text="in" TextKey="IN"></TRA:LABEL>
                                            &nbsp; 
                                            <TRA:LABEL id="ThisCountryLabel" runat="server" Font-Bold="True" Font-Italic="True"></TRA:LABEL>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="PhoneLabel" runat="server" EnableViewState="False" Text="Telephone" TextKey="PHONE"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="PhoneField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="FaxLabel" runat="server" EnableViewState="False" Text="Fax" TextKey="FAX"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="FaxField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr id="PIvaRow" runat="server">
                                        <td class="TitleHead">
                                            <TRA:LABEL id="PIvaLabel" runat="server" EnableViewState="False" Text="Company Number" TextKey="PIVA"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="PIvaField" runat="server" cssclass="NormalTextBox" maxlength="11" Columns="28" width="200px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr id="CFiscaleRow" runat="server">
                                        <td class="TitleHead">
                                            <TRA:LABEL id="CFiscaleLabel" runat="server" EnableViewState="False" Text="Fiscal Code" TextKey="CFISCALE"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="CFiscaleField" runat="server" cssclass="NormalTextBox" maxlength="16" Columns="28" width="200px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="SendNewsletterLabel" runat="server" EnableViewState="False" Text="Send Newsletter" TextKey="SEND_NEWSLETTER"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:CheckBox id="SendNewsletter" runat="server"></asp:CheckBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="EmailLabel" runat="server" Text="Email Address" TextKey="EMAIL"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="EmailField" runat="server" Width="350px" size="25"></asp:TextBox>
                                        </td>
                                        <td class="normal">
                                            <TRA:REGULAREXPRESSIONVALIDATOR id="ValidEmail" runat="server" TextKey="VALID_EMAIL" ControlToValidate="EmailField" ErrorMessage="You must use a valid email address" ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" Display="Dynamic"></TRA:REGULAREXPRESSIONVALIDATOR>
                                            <TRA:REQUIREDFIELDVALIDATOR id="RequiredEmail" runat="server" Text="'Email' must not be left blank" TextKey="INSERT_EMAIL" ControlToValidate="EmailField" ErrorMessage="'Email' must not be left blank"></TRA:REQUIREDFIELDVALIDATOR>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            &nbsp; 
                                        </td>
                                        <td class="normal">
                                            <TRA:LITERAL id="addressCorrect" runat="server" Text="Please make sure that the above email address is correct as it will be used to send a copy of your order." TextKey="CHECKOUT_MAKE_SURE_ADDRESS_IS_CORRECT"></TRA:LITERAL>
                                            <br />
                                            <TRA:LITERAL id="addressCorrect2" runat="server" Text="If it is not the case, please." TextKey="CHECKOUT_MAKE_SURE_ADDRESS_IS_CORRECT_2"></TRA:LITERAL>
                                            <br />
                                            <br />
                                            <TRA:LINKBUTTON class="CommandButton" id="UpdateProfileBtn" runat="server" Text="Update your profile" TextKey="UPDATE_YOUR_PROFILE" DESIGNTIMEDRAGDROP="1929" CausesValidation="False">Update your 
            profile</TRA:LINKBUTTON>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="normal" colspan="3">
                                            <br />
                                            <br />
                                            <TRA:LINKBUTTON class="CommandButton" id="Panel1BackBtn" runat="server" Text="Back" TextKey="BACK" CausesValidation="False"></TRA:LINKBUTTON>
                                            &nbsp;&nbsp; 
                                            <TRA:LINKBUTTON class="CommandButton" id="Panel1CancelBtn" runat="server" Text="Cancel" TextKey="CANCEL" CausesValidation="False"></TRA:LINKBUTTON>
                                            &nbsp;&nbsp; 
                                            <TRA:LINKBUTTON class="CommandButton" id="Panel1ContinueBtn" runat="server" Text="Continue" TextKey="CONTINUE"></TRA:LINKBUTTON>
                                            <br />
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
									</asp:panel>
                                <asp:panel id="PanelStep2" runat="server">
                                    <!-- STEP 2 : CHECK SHIPPING INFO, SELECT SHIPPING METHOD -->
                                    <tr>
                                        <td class="head" align="left" colspan="3">
                                            <br />
                                            <TRA:LABEL id="Panel2Label" runat="server" Text="STEP 2/4: Shipping Information" TextKey="CHECKOUT_STEP2"></TRA:LABEL>
                                            <br />
                                            
                                            <hr noshade="noshade" size="1" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="normal" colspan="3">
                                            <TRA:LINKBUTTON class="CommandButton" id="CopyShippingBtn" runat="server" Text="Reset my shipping information with my account information" TextKey="CHECKOUT_COPY_TO_SHIPPING_INFO"></TRA:LINKBUTTON>
                                            &nbsp; 
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ShipNameLabel" runat="server" Text="Name" TextKey="NAME" Height="22" Width="83px"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ShipNameField" runat="server" Width="350px" size="25"></asp:TextBox>
                                        </td>
                                        <td class="normal">
                                            <TRA:REQUIREDFIELDVALIDATOR id="ShipRequiredName" runat="server" ControlToValidate="NameField" ErrorMessage="'Name' must not be left blank" TTextKey="INSERT_NAME"></TRA:REQUIREDFIELDVALIDATOR>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ShipCompanyLabel" runat="server" EnableViewState="False" Text="Company" TextKey="COMPANY"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ShipCompanyField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ShipAddressLabel" runat="server" EnableViewState="False" Text="Address" TextKey="ADDRESS"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ShipAddressField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ShipCityLabel" runat="server" EnableViewState="False" Text="City" TextKey="CITY"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ShipCityField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ShipZipLabel" runat="server" EnableViewState="False" Text="Postal Code/Zip" TextKey="ZIP"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ShipZipField" runat="server" cssclass="NormalTextBox" maxlength="10" Columns="28" width="60px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ShipCountryLabel" runat="server" Text="Country" TextKey="COUNTRY"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:DropDownList id="ShipCountryField" runat="server" Width="350px" DataTextField="DisplayName" DataValueField="Name" AutoPostBack="True"></asp:DropDownList>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr id="ShipStateRow" runat="server">
                                        <td class="TitleHead">
                                            <TRA:LABEL id="ShipStateLabel" runat="server" Text="Province/State" TextKey="PROV_STATE"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:DropDownList id="ShipStateField" runat="server" Width="170px" DataTextField="DisplayName" DataValueField="CountryCode"></asp:DropDownList>
                                            <TRA:LABEL id="ShipInLabel" runat="server" Text="in" TextKey="IN"></TRA:LABEL>
                                            &nbsp; 
                                            <TRA:LABEL id="ShipThisCountryLabel" runat="server" Font-Bold="True" Font-Italic="True"></TRA:LABEL>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="SPhoneLabel" runat="server" EnableViewState="False" Text="Telephone" TextKey="PHONE"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ShipPhoneField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="TitleHead">
                                            <TRA:LABEL id="SFaxLabel" runat="server" EnableViewState="False" Text="Fax" TextKey="FAX"></TRA:LABEL>
                                        </td>
                                        <td class="normal">
                                            &nbsp; 
                                            <asp:TextBox id="ShipFaxField" runat="server" cssclass="NormalTextBox" maxlength="50" Columns="28" width="350px"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="normal" colspan="3">
                                            <br />
                                            <br />
                                            <TRA:LINKBUTTON class="CommandButton" id="Panel2BackBtn" runat="server" Text="Back" TextKey="BACK" CausesValidation="False"></TRA:LINKBUTTON>
                                            &nbsp;&nbsp; 
                                            <TRA:LINKBUTTON class="CommandButton" id="Panel2CancelBtn" runat="server" Text="Cancel" TextKey="CANCEL" CausesValidation="False"></TRA:LINKBUTTON>
                                            &nbsp;&nbsp; 
                                            <TRA:LINKBUTTON class="CommandButton" id="Panel2ContinueBtn" runat="server" Text="Continue" TextKey="CONTINUE"></TRA:LINKBUTTON>
                                        <br><br><br></td>
                                    </tr>
									</asp:panel>
                                <asp:panel id="PanelStep3" runat="server">
                                    <!-- STEP 3 : VIEW CART, CALCULATE FINAL PRICE, SELECT PAYMENT METHOD -->
                                    <tr>
                                        <td class="head" align="left" colspan="3">
                                            <br />
                                            <TRA:LABEL id="Panel3Label" runat="server" Text="STEP 3/4: Calculate Final Price" TextKey="CHECKOUT_STEP3"></TRA:LABEL>
                                            <br />
                                           
                                            <hr noshade="noshade" size="1" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="middle">
                                            <!-- Shopping Cart, set ProductID invisible (needed to update cart) -->
                                            <br />
                                            <table cellspacing="0" cellpadding="0" border="0">
                                                <tbody>
                                                    <tr valign="top">
                                                        <td>
                                                            <asp:datagrid id="MyCartList" runat="server" BorderColor="Black" GridLines="Vertical" cellpadding="4" ShowFooter="True" DataKeyField="Quantity" AutoGenerateColumns="False">
                                                                <AlternatingItemStyle cssclass="Normal"></AlternatingItemStyle>
                                                                <ItemStyle cssclass="Normal"></ItemStyle>
                                                                <HeaderStyle cssclass="NormalBold"></HeaderStyle>
                                                                <FooterStyle cssclass="Normal"></FooterStyle>
																<Columns>
																	<tra:TemplateColumn HeaderText="Name" TextKey="PRODUCT_NAME">
																		<ItemTemplate>
																			<tra:Label id="ModelNameCart" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ModelName") %>' />
																			<br />
																			<asp:Label id="MetadataXml" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MetadataXml") %>' />
																		</ItemTemplate>
																	</tra:TemplateColumn>
																	<tra:BoundColumn TextKey="PRODUCT_REFERENCE" DataField="ModelNumber" HeaderText="Reference"></tra:BoundColumn>
																	<tra:TemplateColumn HeaderText="Quantity" TextKey="PRODUCT_QUANTITY">
																		<ItemTemplate>
																			<tra:Label id="Quantity" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Quantity") %>' />
																		</ItemTemplate>
																	</tra:TemplateColumn>
																	<tra:BoundColumn TextKey="PRODUCT_PRICE" DataField="UnitCost" HeaderText="Price"></tra:BoundColumn>
																	<tra:BoundColumn TextKey="PRODUCT_PRICE_WITH_TAX" DataField="UnitCostWithTaxes" HeaderText="Price (with Tax)"></tra:BoundColumn>
																	<tra:BoundColumn TextKey="PRODUCT_SUBTOTAL" DataField="ExtendedAmount" HeaderText="Subtotal"></tra:BoundColumn>
																</Columns>
                                                            </asp:datagrid>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="normal" align="right">
                                                            <br />
                                                            <TRA:LABEL id="Label1" runat="server" Text="Total" TextKey="PRODUCT_TOTAL" CssClass="NormalBold"></TRA:LABEL>
                                                            <asp:label id="lblTotal" runat="server" EnableViewState="true" CssClass="NormalBold"></asp:label>
                                                            <br />
                                                            <TRA:LABEL id="Label2" runat="server" Text="Total" TextKey="PRODUCT_TOTAL_TAXES" CssClass="NormalBold">Total 
                  Taxes</TRA:LABEL>
                                                            <asp:label id="lblTotalTaxes" runat="server" EnableViewState="true" CssClass="NormalBold"></asp:label>
                                                            <br />
                                                            <TRA:LABEL id="TotalLabel" runat="server" Text="Total" TextKey="PRODUCT_TOTAL_WITH_TAXES" CssClass="NormalBold">Total 
                  with Taxes</TRA:LABEL>
                                                            <asp:label id="lblTotalWithTaxes" runat="server" EnableViewState="true" CssClass="NormalBold"></asp:label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="normal" align="right">
                                                           
                                                                <TRA:LABEL id="lblTotalShippingLabel" runat="server" TextKey="CHECKOUT_TOTAL_SHIPPING" text="Total Shipping:"></TRA:LABEL>
                                                                <br />
                                                                <TRA:LABEL class="NormalBold" id="lblTotalShippingField" runat="server" EnableViewState="true"></TRA:LABEL>
                                                                <br />
                                                                <asp:RadioButtonList id="ShippingList" runat="server" CssClass="Normal"></asp:RadioButtonList>
                                                                </td>
                                                    </tr>
								</tbody>
							</table>
						</td>
					</tr>
                                    <tr id="ChooseCheckoutType" runat="server">
                                        <td class="normal" colspan="3">
                                            <TRA:LABEL id="lblSelectPayment" runat="server" TextKey="CHECKOUT_SELECT_PAYMENT" text="Total Products:">Select 
            payment method</TRA:LABEL>
                                                : 
                                                <asp:DropDownList id="ddSelectPayment" runat="server"></asp:DropDownList>
                                                <TRA:LINKBUTTON class="CommandButton" id="Panel3ContinueBtn" runat="server" Text="Continue" TextKey="CONTINUE"></TRA:LINKBUTTON><br><br></td>
                                    </tr>
                                    <tr>
                                        <td class="normal" colspan="3">
<TRA:LINKBUTTON class="CommandButton" id="Panel3BackBtn" runat="server" Text="Back" TextKey="BACK" CausesValidation="False"></TRA:LINKBUTTON>
                                            &nbsp;&nbsp; 
                                            <TRA:LINKBUTTON class="CommandButton" id="Panel3CancelBtn" runat="server" Text="Cancel" TextKey="CANCEL" CausesValidation="False"></TRA:LINKBUTTON>
                                        <br><br></td>
                                    </tr>
                                </asp:panel>
                            </table></tbody>
                        </form></td>
                </tr>
				</tbody>
			</table>
<table width="100%" class="rb_AlternateLayoutTable"><tr><td><br>
			<!-- Form, not running server side -->
                    <asp:panel id="GatewayForm" BackColor="white" HorizontalAlign="Center" runat="server" EnableViewState="False" Visible="False"></asp:panel>
            </td></tr></table>
<table cellspacing="0" cellpadding="0" width="100%" border="0">
        <tbody>
			<tr valign="top">
				<td>
					<secure:footer id="Footer1" runat="server"></secure:footer>
				</td>
			</tr>
        </tbody>
		</table>
	</body>
</html>