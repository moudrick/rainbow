<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MagicUrls.ascx.cs" Inherits="Rainbow.DesktopModules.MagicUrls" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:PlaceHolder id="PlaceHolder1" runat="server"></asp:PlaceHolder>
<cc1:XmlEditGrid id="XmlEditGrid1" runat="server" CssClass="Grid">
	<SelectedItemStyle CssClass="SelItem"></SelectedItemStyle>
	<EditItemStyle CssClass="EditItem"></EditItemStyle>
	<AlternatingItemStyle CssClass="AltItem"></AlternatingItemStyle>
	<ItemStyle CssClass="Item" VerticalAlign="Top"></ItemStyle>
	<HeaderStyle CssClass="Header"></HeaderStyle>
	<FooterStyle CssClass="Footer"></FooterStyle>
	<PagerStyle CssClass="Pager"></PagerStyle>
</cc1:XmlEditGrid><br>
<asp:PlaceHolder id="PlaceHolder2" runat="server"></asp:PlaceHolder>
