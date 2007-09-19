using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rainbow.Framework.Security;

public partial class AbtourDefault : Rainbow.Framework.Web.UI.Page {
    
    protected void Page_Load(object sender, EventArgs e) {
        DataBind();
        this.chkRememberMe.Checked = true;
    }

    protected void btnLogin_Click(object sender, EventArgs e) {
        if (this.Page.IsValid) {
            if (PortalSecurity.SignOn(txtEmail.Text, txtPassword.Text, chkRememberMe.Checked) == null) {
                lblError.Visible = true;
            }
        }
    }

    protected void btnHelp_Click(object sender, EventArgs e) {
        //mas que ayuda es recuperar la contraseña
        Response.Redirect("~/DesktopModules/AbtourModules/SignIn/PasswordRecovery.aspx");
    }

    protected void btnRegister_Click(object sender, EventArgs e) {
        Response.Redirect("~/DesktopModules/CoreModules/Register/Register.aspx");
    }

    
    protected void btnEnterAnyway_Click(object sender, EventArgs e) {
 	    Response.Redirect("~/DesktopDefault.aspx");
    }


    protected override void OnPreRender(EventArgs e) {
        base.OnPreRender(e);
        if (Request.IsAuthenticated) {
            Response.Redirect("~/DesktopDefault.aspx");
        }
    }

    protected void valExistsEmail_ServerValidate(object source, ServerValidateEventArgs args) {
        MembershipUser user = Membership.GetUser(txtEmail.Text);
        args.IsValid = user != null;
    }

    protected void valPassword_ServerValidate(object source, ServerValidateEventArgs args) {
        valExistsEmail.Validate();
        if (valExistsEmail.IsValid) {
            args.IsValid = Membership.ValidateUser(txtEmail.Text, txtPassword.Text);
        }
    }


    private string _baseUrl = Rainbow.Framework.HttpUrlBuilder.BuildUrl("~/Portals/_Abtour/login2007/");
    protected string BaseUrl {
        get {
            return _baseUrl;
        }
    }


}
