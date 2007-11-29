<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register assembly="Rainbow.Framework" namespace="Rainbow.Framework.Web.UI.WebControls"
    tagprefix="rbfwebui" %>

<%@ page autoeventwireup="false" codefile="LinksEdit.aspx.cs" inherits="Rainbow.Content.Web.Modules.LinksEdit"
    language="c#" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr>
                        <td align="left" class="Head">
                            <rbfwebui:localize id="Literal1" runat="server" text="Link detail" textkey="LINKDETAILS">
                            </rbfwebui:localize>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr>
                        <td class="SubHead" width="100">
                            <rbfwebui:localize id="Literal2" runat="server" text="Title" textkey="TITLE">
                            </rbfwebui:localize>:
                        </td>
                        <td height="295" rowspan="6">
                            &nbsp;
                        </td>
                        <td>
                            <asp:textbox id="TitleField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="150" width="390">
                            </asp:textbox>
                        </td>
                        <td height="295" rowspan="6" width="25">
                            &nbsp;
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="Req1" runat="server" controltovalidate="TitleField"
                                display="Static" errormessage="You Must Enter a Valid Title" textkey="ERROR_VALID_TITLE">
                            </asp:requiredfieldvalidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal3" runat="server" text="Url" textkey="URL">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="UrlField" runat="server" columns="30" cssclass="NormalTextBox" maxlength="150"
                                width="390">
                            </asp:textbox>
                        </td>
                        <td class="Normal">
                            <asp:requiredfieldvalidator id="Req2" runat="server" controltovalidate="UrlField"
                                display="Dynamic" errormessage="You Must Enter a Valid URL" textkey="ERROR_VALID_URL">
                            </asp:requiredfieldvalidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal4" runat="server" text="Mobile Url" textkey="MOBILEURL">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="MobileUrlField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="150" width="390">
                            </asp:textbox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal5" runat="server" text="Description" textkey="DESCRIPTION">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="DescriptionField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="150" width="390">
                            </asp:textbox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal6" runat="server" text="Target" textkey="TARGET">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:dropdownlist id="TargetField" runat="server" cssclass="NormalTextBox" width="390px">
                            </asp:dropdownlist>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal7" runat="server" text="View Order" textkey="VIEWORDER">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="ViewOrderField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="3" width="390">
                            </asp:textbox>
                        </td>
                        <td class="Normal">
                            <asp:requiredfieldvalidator id="RequiredViewOrder" runat="server" controltovalidate="ViewOrderField"
                                display="Static" errormessage="You Must Enter a Valid View Order" textkey="ERROR_VALID_VIEWORDER">
                            </asp:requiredfieldvalidator>
                            <asp:comparevalidator id="VerifyViewOrder" runat="server" controltovalidate="ViewOrderField"
                                display="Static" errormessage="You Must Enter a Valid View Order" operator="DataTypeCheck"
                                textkey="ERROR_VALID_VIEWORDER" type="Integer">
                            </asp:comparevalidator>
                        </td>
                    </tr>
                </table>
                <rbfwebui:linkbutton id="updateButton" runat="server" cssclass="CommandButton" text="Update">
                </rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="cancelButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                    text="Cancel">
                </rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="deleteButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                    text="Delete this item">
                </rbfwebui:linkbutton>
                <hr noshade="noshade" size="1" width="500" />
                <span class="Normal">
                    <rbfwebui:localize id="Literal8" runat="server" text="Created by" textkey="CREATEDBY">
                    </rbfwebui:localize>
                    &nbsp;
                    <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>
                    &nbsp;
                    <rbfwebui:localize id="Literal9" runat="server" text="on" textkey="ON">
                    </rbfwebui:localize>
                    &nbsp;
                    <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                    <br />
                </span>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
