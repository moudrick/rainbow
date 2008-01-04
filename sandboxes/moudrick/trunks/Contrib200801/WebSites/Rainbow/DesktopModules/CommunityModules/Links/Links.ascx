<%@ control autoeventwireup="false" codefile="Links.ascx.cs" inherits="Rainbow.Content.Web.Modules.Links"
    language="c#" %>
<asp:datalist id="myDataList" runat="server" cellpadding="2" width="100%">
    <itemtemplate>
        <span class="Normal">
            <rbfwebui:hyperlink id="editLink" runat="server" alternatetext="<%# linkAlternateText %>"
                imageurl="<%# linkImage %>" navigateurl='<%# GetLinkUrl(DataBinder.Eval(Container.DataItem,"ItemID"),DataBinder.Eval(Container.DataItem,"Url")) %>'
                textkey="<%# linkTextKey %>">
            </rbfwebui:hyperlink>
            <rbfwebui:hyperlink id="HyperLink1" runat="server" navigateurl='<%# DataBinder.Eval(Container.DataItem,"Url") %>'
                target='<%# DataBinder.Eval(Container.DataItem,"Target") %>' text='<%# DataBinder.Eval(Container.DataItem,"Title") %>'
                tooltip='<%# DataBinder.Eval(Container.DataItem,"Description") %>'>
            </rbfwebui:hyperlink>
        </span>
        <br />
    </itemtemplate>
</asp:datalist>
