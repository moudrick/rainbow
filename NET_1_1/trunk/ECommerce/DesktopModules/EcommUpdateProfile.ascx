<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EcommUpdateProfile.ascx.cs" Inherits="Rainbow.ECommerce.DesktopModules.EcommUpdateProfile" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:panel id="PanelStep1" runat="server">
	<table cellSpacing="0" cellPadding="0" width="650" align="center" border="0">
		<!-- STEP 1 : CHECK ACCOUNT INFORMATION -->
		<TR>
			<TD class="head" align="left" colSpan="3">
				<P>
					<tra:label id="MyError" EnableViewState="false" cssclass="ErrorText" runat="Server"></tra:label></P>
			</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="NameLabel" runat="server" Text="Name" TextKey="BOOKING.NAME" Height="22" Width="83px"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="NameField" runat="server" Width="350px" size="25"></asp:TextBox></TD>
			<TD class="normal">
				<tra:RequiredFieldValidator id="RequiredName" runat="server" TextKey="BOOKING.VALID_NAME" ControlToValidate="NameField" ErrorMessage="'Name' must not be left blank"></tra:RequiredFieldValidator></TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="CompanyLabel" EnableViewState="False" runat="server" Text="Company" TextKey="BOOKING.COMPANY"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="CompanyField" cssclass="NormalTextBox" runat="server" maxlength="50" Columns="28" width="350px"></asp:TextBox></TD>
			<TD></TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="AddressLabel" EnableViewState="False" runat="server" Text="Address" TextKey="BOOKING.ADDRESS"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="AddressField" cssclass="NormalTextBox" runat="server" maxlength="50" Columns="28" width="350px"></asp:TextBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="CityLabel" EnableViewState="False" runat="server" Text="City" TextKey="BOOKING.CITY"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="CityField" cssclass="NormalTextBox" runat="server" maxlength="50" Columns="28" width="350px"></asp:TextBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="ZipLabel" EnableViewState="False" runat="server" Text="Postal Code/Zip" TextKey="BOOKING.ZIP"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="ZipField" cssclass="NormalTextBox" runat="server" maxlength="10" Columns="28" width="60px"></asp:TextBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="CountryLabel" runat="server" Text="Country" TextKey="BOOKING.COUNTRY"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:DropDownList id="CountryField" runat="server" Width="350px" DataTextField="DisplayName" DataValueField="Name" AutoPostBack="True"></asp:DropDownList></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR id="StateRow" runat="server">
			<TD class="subhead" noWrap>
				<tra:Label id="StateLabel" runat="server" Text="Province/State" TextKey="BOOKING.PROV_STATE"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:DropDownList id="StateField" runat="server" Width="170px" DataValueField="CountryCode" DataTextField="DisplayName"></asp:DropDownList>
				<tra:Label id="InLabel" runat="server" Text="in" TextKey="IN"></tra:Label>&nbsp;
				<tra:Label id="ThisCountryLabel" runat="server" Font-Italic="True" Font-Bold="True"></tra:Label></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="PhoneLabel" runat="server" EnableViewState="False" Text="Telephone" TextKey="BOOKING.PHONE"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="PhoneField" runat="server" cssclass="NormalTextBox" width="350px" Columns="28" maxlength="50"></asp:TextBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="FaxLabel" runat="server" EnableViewState="False" Text="Fax" TextKey="BOOKING.FAX"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="FaxField" runat="server" cssclass="NormalTextBox" width="350px" Columns="28" maxlength="50"></asp:TextBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR id="PartitaIvaRow" runat="server">
			<TD class="subhead" noWrap>
				<tra:Label id="PartitaIvaLabel" runat="server" EnableViewState="False" Text="Company Number" TextKey="BOOKING.COMPNUM"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="PartitaIvaField" runat="server" cssclass="NormalTextBox" width="200px" Columns="28" maxlength="11"></asp:TextBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR id="CFiscaleRow" runat="server">
			<TD class="subhead" noWrap>
				<tra:Label id="CFiscaleLabel" runat="server" EnableViewState="False" Text="Fiscal Code" TextKey="BOOKING.CFISCALE"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="CFiscaleField" runat="server" cssclass="NormalTextBox" width="200px" Columns="28" maxlength="16"></asp:TextBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="SendNewsletterLabel" runat="server" EnableViewState="False" Text="Send Newsletter" TextKey="BOOKING.SEND_NEWSLETTER"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:CheckBox id="SendNewsletter" runat="server"></asp:CheckBox></TD>
			<TD>&nbsp;</TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>
				<tra:Label id="EmailLabel" runat="server" Text="Email Address" TextKey="BOOKING.EMAIL"></tra:Label></TD>
			<TD class="normal">&nbsp;
				<asp:TextBox id="EmailField" runat="server" Width="350px" size="25"></asp:TextBox></TD>
			<TD class="normal">
				<tra:RegularExpressionValidator id="ValidEmail" runat="server" TextKey="BOOKING.VALID_EMAIL" ErrorMessage="You must use a valid email address" ControlToValidate="EmailField" Display="Dynamic" ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"></tra:RegularExpressionValidator>
				<tra:RequiredFieldValidator id="RequiredEmail" runat="server" Text="'Email' must not be left blank" TextKey="BOOKING.INSERT_EMAIL" ErrorMessage="'Email' must not be left blank" ControlToValidate="EmailField"></tra:RequiredFieldValidator></TD>
		</TR>
		<TR>
			<TD class="subhead" noWrap>&nbsp;
			</TD>
			<TD class="normal">
				<tra:Literal id="addressCorrect" runat="server" Text="Please make sure that the above email address is correct as it will be used to send a copy of your order." TextKey="BOOKING.CHECKOUT_MAKE_SURE_ADDRESS_IS_CORRECT"></tra:Literal><BR>
				<tra:Literal id="addressCorrect2" runat="server" Text="If it is not the case, please." TextKey="BOOKING.CHECKOUT_MAKE_SURE_ADDRESS_IS_CORRECT_2"></tra:Literal><BR>
				<tra:LinkButton class="CommandButton" id="UpdateProfileBtn" runat="server" Text="Update your profile" TextKey="BOOKING.UPDATE_YOUR_PROFILE" CausesValidation="False" DESIGNTIMEDRAGDROP="1929">Update your profile</tra:LinkButton></TD>
			<TD class="normal">
			&nbsp;
		</TR>
	</table>
</asp:panel>
