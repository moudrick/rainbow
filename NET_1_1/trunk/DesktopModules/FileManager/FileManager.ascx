<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FileManager.ascx.cs" Inherits="Rainbow.DesktopModules.FileManager" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<!-- *** File Directory Tree *** --><asp:placeholder id="myPlaceHolder" Runat="server"></asp:placeholder>
<SCRIPT>
function btnDelete_Click() {
	a = window.confirm("Are you sure want to delete this Files ?");
	if(a) {
		return true;
	}
	else {
		return false;
	}
}
function imgACL_Click(filename) {
	alert(filename);
}
</SCRIPT>
<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
	<TR>
		<TD><ASP:IMAGEBUTTON id="btnGoUp" ALTERNATETEXT="Up One Level" IMAGEURL="images/btnUp.jpg" RUNAT="server"
				ImageAlign="AbsMiddle"></ASP:IMAGEBUTTON>&nbsp;&nbsp;&nbsp;<ASP:LABEL id="lblError" RUNAT="server" FORECOLOR="red"></ASP:LABEL></TD>
		<td><ASP:IMAGEBUTTON id="btnDelete" ALTERNATETEXT="Delete selected files" IMAGEURL="images/btnDelete.jpg"
				RUNAT="server" ImageAlign="AbsMiddle"></ASP:IMAGEBUTTON></td>
	</TR>
	<TR>
		<TD colspan="2"><ASP:DATAGRID id="dgFile" RUNAT="server" CELLSPACING="0" CELLPADDING="0" ALLOWSORTING="True" AUTOGENERATECOLUMNS="False"
				WIDTH="100%" BORDERWIDTH="0" AlternatingItemStyle-CssClass="Grid_AlternatingItem Normal" ItemStyle-CssClass="Grid_Item Normal"
				HeaderStyle-CssClass="Grid_Header NormalBold" AlternatingItemStyle-Wrap="false" ItemStyle-Wrap="false">
				<COLUMNS>
					<ASP:TEMPLATECOLUMN>
						<ITEMTEMPLATE>
							<ASP:CHECKBOX ID="chkChecked" RUNAT="server"></ASP:CHECKBOX>
						</ITEMTEMPLATE>
					</ASP:TEMPLATECOLUMN>
					<tra:TemplateColumn HeaderText="FileName" SortExpression="fileName" TextKey="FILEMAN_FILENAME">
						<HeaderStyle HORIZONTALALIGN="left"></HeaderStyle>
						<ItemStyle></ItemStyle>
						<ITEMTEMPLATE><span style="white-space:nowrap;">
							&nbsp;
							<ASP:PLACEHOLDER ID="plhImgEdit" RUNAT="server"></ASP:PLACEHOLDER>
							<ASP:IMAGE ID="imgType" RUNAT="server" BORDERWIDTH="0" BORDERSTYLE="None" ImageAlign="absmiddle"></ASP:IMAGE>
							<ASP:LINKBUTTON ID="lnkName" CSSCLASS ="FileManager" RUNAT="server" TEXT='<%# DataBinder.Eval(Container.DataItem,"filename") %>' COMMANDNAME="ItemClicked" CAUSESVALIDATION="false">
							</ASP:LINKBUTTON>
							</span>
						</ITEMTEMPLATE>
						<EDITITEMTEMPLATE><span style="white-space:nowrap;">
							&nbsp;
							<ASP:PLACEHOLDER ID="plhImgEdit" RUNAT="server"></ASP:PLACEHOLDER>
							<ASP:IMAGE ID="imgType" RUNAT="server" BORDERWIDTH="0" BORDERSTYLE="None" ImageAlign="absmiddle"></ASP:IMAGE>
							<ASP:TEXTBOX ID="txtEditName" RUNAT="server" TEXT='<%# DataBinder.Eval(Container.DataItem,"filename") %>'>
							</ASP:TEXTBOX></span>
						</EDITITEMTEMPLATE>
					</tra:TemplateColumn>
					<TRA:BOUNDCOLUMN DATAFIELD="size" SORTEXPRESSION="size" READONLY="True" HEADERTEXT="Size (KB)" TextKey="FILEMAN_SIZE">
						<HEADERSTYLE HORIZONTALALIGN="Right"></HEADERSTYLE>
						<ITEMSTYLE HORIZONTALALIGN="Right"></ITEMSTYLE>
					</TRA:BOUNDCOLUMN>
					<tra:BOUNDCOLUMN DATAFIELD="modified" SORTEXPRESSION="modified" READONLY="True" HEADERTEXT="Modified"
						TextKey="FILEMAN_MODIFIED">
						<HEADERSTYLE HORIZONTALALIGN="Right"></HEADERSTYLE>
						<ITEMSTYLE HORIZONTALALIGN="Right"></ITEMSTYLE>
					</tra:BOUNDCOLUMN>
					<ASP:TEMPLATECOLUMN ITEMSTYLE-HORIZONTALALIGN="Right">
						<ITEMTEMPLATE><span style="white-space:nowrap;">&nbsp;
							<ASP:LINKBUTTON ID="LinkButton1" RUNAT="server" COMMANDNAME="Edit" CAUSESVALIDATION="false" TEXT=" [Rename]"></ASP:LINKBUTTON>
							&nbsp;</span>
						</ITEMTEMPLATE>
						<EDITITEMTEMPLATE><span style="white-space:nowrap;">&nbsp;
							<ASP:LINKBUTTON ID="LinkButton3" RUNAT="server" COMMANDNAME="Update" TEXT=" [Update]"></ASP:LINKBUTTON>&nbsp;
							<ASP:LINKBUTTON ID="LinkButton2" RUNAT="server" COMMANDNAME="Cancel" CAUSESVALIDATION="false" TEXT=" [Cancel]"></ASP:LINKBUTTON>
							&nbsp;</span>
						</EDITITEMTEMPLATE>
					</ASP:TEMPLATECOLUMN>
				</COLUMNS>
			</ASP:DATAGRID></TD>
	</TR>
	<TR class="Grid_Header NormalBold">
		<TD align="left" colspan="2"><ASP:LABEL id="lblCounter" RUNAT="server"></ASP:LABEL></TD>
	</TR>
	<TR>
		<TD colspan="2">&nbsp;</TD>
	</TR>
</TABLE>
<BR>
<TABLE cellSpacing="0" cellPadding="2" border="0">
	<TR class="Normal">
		<TD nowrap><B>Create New Directory&nbsp;:</B>&nbsp;<ASP:TEXTBOX id="txtNewDirectory" RUNAT="server" Width="224px" ENABLEVIEWSTATE="False"></ASP:TEXTBOX>&nbsp;<ASP:IMAGEBUTTON id="btnNewFolder" ALTERNATETEXT="New Folder" IMAGEURL="images/btnNewFolder.jpg"
				RUNAT="server"></ASP:IMAGEBUTTON></TD>
	</TR>
	<TR class="Normal">
		<TD>&nbsp;</TD>
	</TR>
	<TR class="Normal">
		<TD><B>Upload File (You can upload up to 3 files at 
				the same time)</B></TD>
	</TR>
	<TR class="Normal">
		<TD>1.&nbsp;<INPUT id="file1" type="file" size="28" name="file1"
				RUNAT="server"><BR>
			2.&nbsp;<INPUT id="file2" type="file" size="28" name="file2"
				RUNAT="server"><BR>
			3.&nbsp;<INPUT id="file3" type="file" size="28" name="file3"
				RUNAT="server"><BR>
			&nbsp;&nbsp;&nbsp;&nbsp;<ASP:BUTTON id="btnUpload" RUNAT="server" TEXT="Upload"></ASP:BUTTON></TD>
	</TR>
</TABLE>
<!-- *** End of File Directory Tree *** -->
