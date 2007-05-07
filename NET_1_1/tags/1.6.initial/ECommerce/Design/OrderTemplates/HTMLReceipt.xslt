<?xml version='1.0' encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ms="urn:schemas-microsoft-com:xslt"
	xmlns:rainbow="urn:rainbow">
	<xsl:template match="BillingData">
		<u>
			<xsl:value-of select="rainbow:Localize('PRODUCT_ACCOUNT_INFORMATION', 'Account information')"></xsl:value-of>
		</u><br /><br />
		<xsl:value-of select="rainbow:Localize('NAME', 'Name')"></xsl:value-of>:&#160;<xsl:value-of select="FullName"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('COMPANY', 'Company')"></xsl:value-of>:&#160;<xsl:value-of select="Company"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('ADDRESS', 'Address')"></xsl:value-of>:&#160;<xsl:value-of select="Address"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('CITY', 'City')"></xsl:value-of>:&#160;<xsl:value-of select="Zip"></xsl:value-of>
		 - <xsl:value-of select="City"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('COUNTRY', 'Country')"></xsl:value-of>:&#160;<xsl:value-of select="Country"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('PROV_STATE', 'State')"></xsl:value-of>:&#160;<xsl:value-of select="State"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('PHONE', 'Phone')"></xsl:value-of>:&#160;<xsl:value-of select="Phone"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('FAX', 'Fax')"></xsl:value-of>:&#160;<xsl:value-of select="Fax"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('EMAIL', 'Email')"></xsl:value-of>:&#160;<xsl:value-of select="EMail"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('NOTE', 'Note')"></xsl:value-of>:&#160;<xsl:value-of select="Note"></xsl:value-of><br />
	</xsl:template>
	<xsl:template match="ShippingData">
		<u>
			<xsl:value-of select="rainbow:Localize('PRODUCT_SHIPPING_INFORMATION', 'Shipping information')"></xsl:value-of>
		</u>
		<br />
		<br />
		<xsl:value-of select="rainbow:Localize('NAME', 'Name')"></xsl:value-of>:&#160;<xsl:value-of select="FullName"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('COMPANY', 'Company')"></xsl:value-of>:&#160;<xsl:value-of select="Company"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('ADDRESS', 'Address')"></xsl:value-of>:&#160;<xsl:value-of select="Address"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('CITY', 'City')"></xsl:value-of>:&#160;<xsl:value-of select="ZipCode"></xsl:value-of>
		 - <xsl:value-of select="City"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('COUNTRY', 'Country')"></xsl:value-of>:&#160;<xsl:value-of select="Country"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('PROV_STATE', 'State')"></xsl:value-of>:&#160;<xsl:value-of select="State"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('PHONE', 'Phone')"></xsl:value-of>:&#160;<xsl:value-of select="Phone"></xsl:value-of><br />
		<xsl:value-of select="rainbow:Localize('FAX', 'Fax')"></xsl:value-of>:&#160;<xsl:value-of select="Fax"></xsl:value-of><br />
	</xsl:template>
	<xsl:template match="Items">
		<xsl:apply-templates></xsl:apply-templates>
	</xsl:template>
	<xsl:template match="OrderDetail">
		<tr>
			<td align="left" class="normal">
				<xsl:value-of select="ModelNumber"></xsl:value-of>
			</td>
			<td align="left" class="normal">
				<xsl:value-of select="ModelName"></xsl:value-of>
				&#160;
				<xsl:value-of select="ModelOptions"></xsl:value-of>
			</td>
			<td align="center" class="normal">
				<xsl:value-of select="Quantity"></xsl:value-of>
			</td>
			<td align="right" class="normal">
				<xsl:value-of select="rainbow:FormatMoney(UnitPrice/Amount, UnitPrice/ISOCurrencySymbol)" />
			</td>
			<td align="right" class="normal">
				<xsl:value-of select="rainbow:FormatMoney(Total/Amount, Total/ISOCurrencySymbol)" />
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="/">
		<table cellspacing="0" cellpadding="6" align="center" border="0" nowrap="nowrap" valign="top">
			<tr class="CartListHead">
				<td colspan="2" nowrap="nowrap" align="left" class="normal"><xsl:value-of select="rainbow:Localize('PRODUCT_ORDER_REFERENCE', 'Order reference')"></xsl:value-of>:
					<xsl:value-of select="//OrderID"></xsl:value-of>
					- <xsl:value-of select="rainbow:Localize('DATE', 'Date')"></xsl:value-of>: 
					<xsl:value-of select="rainbow:FormatDateTime(//DateCreated)"></xsl:value-of>
				</td>
			</tr>
			<tr>
				<td valign="top" width="50%" class="normal">
					<xsl:apply-templates select="//BillingData" />
				</td>
				<td valign="top" width="50%" class="normal">
					<xsl:apply-templates select="//ShippingData" />
				</td>
			</tr>
			<tr>
				<td valign="top" colspan="2">
					<table width="100%" border="0">
						<tr>
							<td class="normal">
								<xsl:value-of select="rainbow:Localize('PRODUCT_PAYMENT_METHOD', 'Payment method')"></xsl:value-of>:&#160;<xsl:value-of select="//PaymentMethod"></xsl:value-of>
							</td>
							<td class="normal">
								<xsl:value-of select="rainbow:Localize('PRODUCT_SHIPPING_METHOD', 'Shipping method')"></xsl:value-of>:&#160;<xsl:value-of select="//ShippingMethod"></xsl:value-of>
							</td>
							<td class="normal">
								<xsl:value-of select="rainbow:Localize('PRODUCT_TOTAL_WEIGHT', 'Total weight')"></xsl:value-of>:&#160;<xsl:value-of select="format-number(number(//TotalWeight), '###,###.##')"></xsl:value-of>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td valign="top" nowrap="nowrap" align="left">
					<tr>
						<td colspan="2">
							<table width="100%" cellspacing="0" cellpadding="4" align="center" border="0" nowrap="nowrap"
								valign="top">
								<tr>
									<th class="CartListHead" nowrap="nowrap" align="center">
										<xsl:value-of select="rainbow:Localize('PRODUCT_REFERENCE', 'Reference')"></xsl:value-of>
									</th>
									<th class="CartListHead" nowrap="nowrap" width="250" align="center">
										<xsl:value-of select="rainbow:Localize('DESCRIPTION', 'Description')"></xsl:value-of>
									</th>
									<th class="CartListHead" nowrap="nowrap">
										<xsl:value-of select="rainbow:Localize('PRODUCT_QUANTITY', 'Quantity')"></xsl:value-of>
									</th>
									<th class="CartListHead" nowrap="nowrap" align="center">
										<xsl:value-of select="rainbow:Localize('PRODUCT_UNIT', 'Unit')"></xsl:value-of>
									</th>
									<th class="CartListHead" nowrap="nowrap" align="center">
										<xsl:value-of select="rainbow:Localize('PRODUCT_PRICE', 'Price')"></xsl:value-of>
									</th>
								</tr>
								<xsl:apply-templates select="//Items" />
								<tr>
									<td colspan="5">
										<hr />
									</td>
								</tr>
								<tr>
									<td colspan="2">&#160;</td>
									<td colspan="2" align="right" class="normal">
                                        <xsl:value-of select="rainbow:Localize('TOTAL', 'Total')"></xsl:value-of>:
                                    </td>
									<th nowrap="nowrap" align="right" class="normal">
										<xsl:value-of select="rainbow:FormatMoney(//TotalGoods/Amount, //TotalGoods/ISOCurrencySymbol)" />
									</th>
								</tr>
								<tr>
									<td colspan="2">&#160;</td>
									<td colspan="2" align="right" class="normal">
                                        <xsl:value-of select="rainbow:Localize('PRODUCT_SHIPPING', 'Shipping')"></xsl:value-of>:
                                    </td>
									<th nowrap="nowrap" align="right" class="normal">
										<xsl:value-of select="rainbow:FormatMoney(//TotalShipping/Amount, //TotalShipping/ISOCurrencySymbol)" />
									</th>
								</tr>
								<tr>
									<td colspan="2">&#160;</td>
									<td colspan="2" align="right" class="normal">
                                        <xsl:value-of select="rainbow:Localize('PRODUCT_TOTAL_TAXES', 'Total Taxes')"></xsl:value-of>:
                                    </td>
									<th nowrap="nowrap" align="right" class="normal">
										<xsl:value-of select="rainbow:FormatMoney(//TotalTaxes/Amount, //TotalTaxes/ISOCurrencySymbol)" />
									</th>
								</tr>
								<tr>
									<td colspan="2">&#160;</td>
									<td colspan="2" align="right" class="normal">
                                        <xsl:value-of select="rainbow:Localize('PRODUCT_TOTAL_EXPENSES', 'Total Expenses')"></xsl:value-of>:
                                    </td>
									<th nowrap="nowrap" align="right" class="normal">
										<xsl:value-of select="rainbow:FormatMoney(//TotalExpenses/Amount, //TotalExpenses/ISOCurrencySymbol)" />
									</th>
								</tr>
								<tr>
									<td colspan="2">&#160;</td>
									<td colspan="2" align="right" class="normal">
                                        <xsl:value-of select="rainbow:Localize('PRODUCT_TOTAL_PRICE_(INC._TAXES)', 'Total price (inc. Taxes)')"></xsl:value-of>:
                                    </td>
									<th nowrap="nowrap" align="right" class="normal">
										<b>
											<xsl:value-of select="rainbow:FormatMoney(//TotalOrder/Amount, //TotalOrder/ISOCurrencySymbol)" />
										</b>
									</th>
								</tr>
							</table>
						</td>
					</tr>
				</td>
			</tr>
		</table>
	</xsl:template>
</xsl:stylesheet>