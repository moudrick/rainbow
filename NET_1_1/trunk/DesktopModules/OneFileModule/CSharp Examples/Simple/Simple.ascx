<%@ Control language="c#" Inherits="Rainbow.DesktopModules.OneFileModule" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<script language="C#" runat="server">

	void Page_Load(Object sender, EventArgs e)
	{
		InitSettings(SettingsType.Str);

		if (SettingsExists)
		{
			lblFirstName.Text = GetSetting("FirstName");
			lblLastName.Text = GetSetting("LastName");
			lblSettingsStr.Text = SettingsStr;
		}
	}

</script>
<cc1:DesktopModuleTitle EditText="Edit" EditUrl="~/DesktopModules/Admin/PropertyPage.aspx" PropertiesText="PROPERTIES" PropertiesUrl="~/DesktopModules/Admin/PropertyPage.aspx" runat="server" ID="ModuleTitle" />
<b>Setting FirstName:</b>
<asp:label id="lblFirstName" runat="server" /><br>
<b>Setting LastName:</b>
<asp:label id="lblLastName" runat="server" /><br>
<b>SettingsStr:</b>
<asp:label id="lblSettingsStr" runat="server" />
