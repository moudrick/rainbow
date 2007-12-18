<%@ Control language="c#" Inherits="Rainbow.DesktopModules.OneFileModule" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Import Namespace="System.Web.Mail" %>
<%@ Import Namespace="Rainbow.UI" %>
<%@ Import Namespace="Rainbow.UI.WebControls" %>
<%@ Import Namespace="Esperantus" %>
<%@ Import Namespace="Rainbow.Configuration" %>
<script language="C#" runat="server">


	string yahooGroupName;
	string yahooServerName="yahoogroups.com";


	void Page_Load(Object sender, EventArgs e)
	{
		InitSettings(SettingsType.Str);
		// Note you have these variables available everywhere in your code:
		// string SettingsStr  -- The content of setting "Settings string"
		// bool DebugMode      -- true if setting "Debug Mode" is clicked
		// bool SettingsExists -- false if settings are missing
		// string GetStrSetting(settingName) -- Returns the setting from SettingsStr
		// string GetXmlSetting(settingName) -- Returns the setting from XML file
		// string GetSetting(settingName)    -- Returns the setting in search order: 
		//                                      1)SettingsStr, 2)XML file

		if (SettingsExists)
		{
			yahooGroupName= GetSetting("YahooGroupName");

			if (DebugMode)
				Message.Text = "Debug info: " + yahooGroupName+ " - " + yahooServerName;
		}
		
		email.Text = PortalSettings.CurrentUser.Identity.Email;
	}

    void SubscribeBtn_Click(Object sender, EventArgs e) {
		JoinList(email.Text, yahooGroupName);
		Message.Text = email.Text + " subscribed!";
    }

    void LeaveBtn_Click(Object sender, EventArgs e) {
		LeaveList(email.Text,yahooGroupName);
		Message.Text = email.Text + " unsubscribed!";
    }
    
    void JoinList(string email, string listname) {
		MailMessage Mailer = new MailMessage();   
		Mailer.From = email; 
		Mailer.To =  listname + "-subscribe "+ yahooServerName;
		Mailer.Subject = "subscribe";
		Mailer.Body = "subscribe";
		SmtpMail.SmtpServer = PortalSettings.SmtpServer;
		SmtpMail.Send(Mailer);    
    }
    
    void LeaveList(string email, string listname) {
		MailMessage Mailer = new MailMessage();   
		Mailer.From = email; 
		Mailer.To =  listname + "-unsubscribe "+ yahooServerName;
		Mailer.Subject = "unubscribe";
		Mailer.Body = "unsubscribe";
		SmtpMail.SmtpServer = PortalSettings.SmtpServer;
		SmtpMail.Send(Mailer);    
    }
</script>
<cc1:DesktopModuleTitle EditText="Edit" EditUrl="~/DesktopModules/Admin/PropertyPage.aspx" PropertiesText="PROPERTIES" PropertiesUrl="~/DesktopModules/Admin/PropertyPage.aspx" runat="server" ID="ModuleTitle" />
<hr noshade="noshade" size="1pt" width="98%" />
<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td><span class="SubSubHead" style="height:20">YahooGroups Signup:</span></td>
	</tr>
	<tr>
		<td>
			<span class="Normal">Email:</span></td>
		</td>
	</tr>
	<tr>
		<td>
			<asp:TextBox id="email" columns="9" width="130" cssclass="NormalTextBox" runat="server" />
			<div class="SubHead">
				<asp:RegularExpressionValidator runat="server" id="validEMailRegExp" ControlToValidate="email" Display="Dynamic" ErrorMessage="Please enter a valid email address." ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" />
				<asp:RequiredFieldValidator runat="server" id="rfvEMail" ControlToValidate="email" Display="Dynamic" ErrorMessage="'Email' must not be left blank." />
			</div>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Button id="SubscribeBtn" runat="server" OnClick="SubscribeBtn_Click" Text="Join" />
			<asp:Button id="LeaveBtn" runat="server" OnClick="LeaveBtn_Click" Text="Leave" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:label id="Message" class="NormalRed" runat="server" />
		</td>
	</tr>
</table>
