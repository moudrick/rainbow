<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EcommSelectPayment.ascx.cs" Inherits="Rainbow.ECommerce.DesktopModules.EcommSelectPayment" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<tra:label id="lblSelectPayment" text="Total Products:" TextKey="BOOKING.CHECKOUT_SELECT_PAYMENT" runat="server">Select payment method</tra:label>:<asp:dropdownlist id="ddSelectPayment" runat="server" DataTextField="MerchantName" DataValueField="Key"></asp:dropdownlist>
<tra:linkbutton class="CommandButton" id="Panel3ContinueBtn" TextKey="BOOKING.CONTINUE" runat="server" Text="Continue"></tra:linkbutton>
