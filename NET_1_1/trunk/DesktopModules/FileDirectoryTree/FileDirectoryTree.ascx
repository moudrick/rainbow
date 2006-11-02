<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FileDirectoryTree.ascx.cs" Inherits="Rainbow.DesktopModules.FileDirectoryTree" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<!-- *** File Directory Tree *** -->

<asp:PlaceHolder ID="myPlaceHolder" Runat="server"></asp:PlaceHolder>

<script language="javascript">
	function Toggle(myObject) 
	{
		object = document.getElementById(myObject);
		mySpan = document.getElementById(myObject + "_img");
		if (object.style.display == 'inline'){
			object.style.display = 'none';
			mySpan.src = baseImg+'dir.gif';
		} else {
			object.style.display = 'inline';
			mySpan.src = baseImg+'dir_open.gif';
		}
	} 
</script>

<!-- *** End of File Directory Tree *** -->