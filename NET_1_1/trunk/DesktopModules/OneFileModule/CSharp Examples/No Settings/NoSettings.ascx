<%@ Control language="c#" Inherits="Rainbow.DesktopModules.OneFileModule" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>

<script language="C#" runat="server">

	void Page_Load(Object sender, EventArgs e)
	{
		if (IsPostBack == false)
		{
		}
	}

</script>

<cc1:DesktopModuleTitle EditText="Edit" EditUrl="~/DesktopModules/Admin/PropertyPage.aspx" PropertiesText="PROPERTIES" PropertiesUrl="~/DesktopModules/Admin/PropertyPage.aspx" runat="server" ID="ModuleTitle" />
This module does not use the settings system provided by the OneFileModuleKit!<P>
Note: You can delete the tag &lt;cc1:DesktopModuleTitle ... /&gt; and the &lt;%@ Register ... %&gt;
if you do not need the title (which includes the edit settings and properties link).
