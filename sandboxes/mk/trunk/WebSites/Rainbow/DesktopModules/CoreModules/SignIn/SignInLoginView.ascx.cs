using System;
using System.Web.UI.WebControls;
using Rainbow.Framework;
using Rainbow.Framework.Security;
using Rainbow.Framework.Web.UI.WebControls;

public partial class SignInLoginView : PortalModuleControl 
{
    #region Module properties 

    public override Guid GuidID {
        get {
            return new Guid( "{B9D6DBC3-72CF-49d3-B170-036D07CB33DD}" );
        }
    }

    public override bool Searchable {
        get {
            return false;
        }
    }

    /// <summary>
    /// Overrides ModuleSetting to render this module type un-cacheable
    /// </summary>
    /// <value></value>
    public override bool Cacheable {
        get { return false; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            loginControl.UserNameLabelText = General.GetString("EMAIL", "EMail");
            loginControl.PasswordLabelText = General.GetString("PASSWORD", "Password");
            loginControl.LoginButtonText = General.GetString("SIGNIN", "Sign in");
            loginControl.PasswordRecoveryText = General.GetString("SIGNIN_SEND_PWD", "Forgotten Password?");
            loginControl.RememberMeText = General.GetString("REMEMBER_LOGIN", "Remember Login");
            loginControl.TextLayout = LoginTextLayout.TextOnTop;
            loginControl.TitleText = string.Empty;
            //loginControl.PasswordRecoveryUrl = 
        }
    }

    protected void loginControl_Authenticate(object sender, AuthenticateEventArgs e)
    {
        e.Authenticated =
            (!string.IsNullOrEmpty(
                  SignOnController.SignOn(loginControl.UserName,
                                          loginControl.Password,
                                          loginControl.RememberMeSet)));
    }
}
