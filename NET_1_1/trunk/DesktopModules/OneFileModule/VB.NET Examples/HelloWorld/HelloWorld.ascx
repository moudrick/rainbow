<%@ Control language="vb" Inherits="Rainbow.DesktopModules.OneFileModule" CodeBehind="OneFileModule.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>

<script language="vb" runat="server">
	Sub Page_Load() 
		InitSettings(SettingsType.Str)

		if SettingsExists
			lblFirstName.Text = GetSetting("FirstName")
			lblLastName.Text = GetSetting("LastName")
		end if
	End Sub
</script>

<cc1:DesktopModuleTitle EditText="Edit" EditUrl="~/DesktopModules/Admin/PropertyPage.aspx" PropertiesText="PROPERTIES" PropertiesUrl="~/DesktopModules/Admin/PropertyPage.aspx" runat="server" ID="ModuleTitle" />

Hello Rainbow World from: <asp:label id="lblFirstName" runat="server" /> <asp:label id="lblLastName" runat="server" />
