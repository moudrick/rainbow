<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AbtourDefault.aspx.cs" Inherits="AbtourDefault" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Bienvenido a ABTOUR Viajes</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
      <tr>
        <td valign="top" style='height:138px; background-image: url(<%# BaseUrl %>/fdo-login-2007-tp.jpg); background-repeat:no-repeat' scope="col">&nbsp;</td>
      </tr>
      <tr>
        <td valign="top" style='height:500px; background-image: url(<%# BaseUrl %>/fdo-login-2007-btn.jpg); background-repeat:no-repeat' scope="col"><table width="613" style="height:470px" border="0" cellpadding="0" cellspacing="0">
          <tr>
            <td width="215" height="117" scope="col">&nbsp;</td>
            <td width="183" scope="col">&nbsp;</td>
            <td width="30" scope="col">&nbsp;</td>
            <td width="185" scope="col">&nbsp;</td>
          </tr>
          <tr>
            <td height="32">&nbsp;</td>
            <td colspan="3"><span class="style1">&nbsp;&nbsp;&nbsp;&nbsp;</span>
                <span class="Error">
                    <asp:Label ID="lblError" runat="server" Visible="false" EnableViewState="false">Ha ocurrido un error.Verifique que la direcci&oacute;n de correo y la contrase&ntilde;a hayan sido bien ingresadas.</asp:Label> 
                    <asp:ValidationSummary runat="server" ID="valSummary" />
                </span></td>
            </tr>
          <tr>
            <td height="169">&nbsp;</td>
            <td valign="top" style='background-image: url(<%# BaseUrl %>/login-agt.gif); background-repeat:no-repeat'><table width="100%" style="height:139px" border="0" cellpadding="0" cellspacing="0">
              <tr>
                <td height="18" scope="col">&nbsp;</td>
              </tr>
              <tr>
                <td height="116" valign="top"><table width="100%" border="0" cellspacing="0" cellpadding="10">
                  <tr>
                    <td height="118" valign="top" scope="col"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr>
                        <td height="17" scope="col">Correo electr&oacute;nico:</td>
                      </tr>
                      <tr>
                        <td><asp:TextBox runat="server" ID="txtEmail" Height="12px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valReqEmail" runat="server" ControlToValidate="txtEmail"
                                Display="dynamic" ErrorMessage="Debe ingresar un e-mail." Text="*" ValidationGroup="SignIn" />
                            <asp:CustomValidator ID="valExistsEmail" runat="server" Display="Dynamic" ErrorMessage="Ud. no est&aacute; registrado en nuestro sitio web."
                                Text="*" ValidationGroup="SignIn" OnServerValidate="valExistsEmail_ServerValidate" />
                        </td>
                      </tr>
                      <tr>
                        <td height="15">Contrase&ntilde;a:</td>
                      </tr>
                      <tr>
                        <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                              <td width="62%" scope="col">
                                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" Width="80px" Height="12px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="valReqPassword" runat="server" ControlToValidate="txtPassword"
                                    Display="dynamic" ErrorMessage="Debes ingresar una contrase&ntilde;a" Text="*"
                                    ValidationGroup="SignIn" />
                                <asp:CustomValidator ID="valPassword" runat="server" Display="Dynamic" ErrorMessage="La contrase&ntilde;a ingresada es incorrecta."
                                    Text="*" ValidationGroup="SignIn" OnServerValidate="valPassword_ServerValidate" />
                              </td>
                              <td width="38%" scope="col">
                                <asp:LinkButton runat="server" ID="btnLogin" OnClick="btnLogin_Click">
                                    <img src='<%# BaseUrl %>/arrow-login.gif' alt="Login" width="19" height="19" />
                                </asp:LinkButton>
                              </td>
                            </tr>
                          </table></td>
                      </tr>
                      <tr>
                        <td height="20"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                              <td width="60%" height="18" scope="col">Recordarme: </td>
                              <td width="40%" scope="col"><span class="Normal">
                                <asp:CheckBox runat="Server" ID="chkRememberMe" Height="12px" />
                              </span></td>
                            </tr>
                          </table></td>
                      </tr>
                      <tr>
                        <td height="25"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                          <tr>
                            <td width="55%" height="27" scope="col">
                                <div align="left">
                                    <asp:LinkButton runat="server" ID="btnHelp" OnClick="btnHelp_Click">
                                        <img src='<%# BaseUrl %>/forgot-passwd.gif' alt="olvid&eacute; mi contrase&ntilde;a" width="71" height="22" />
                                    </asp:LinkButton>
                                </div></td>
                            <td width="45%" scope="col">
                                <div align="left">
                                    <asp:LinkButton runat="server" ID="btnRegister" OnClick="btnRegister_Click">
                                        <img src='<%# BaseUrl %>/new-user-Login2007.gif' alt="registrarse" width="59" height="20" />
                                    </asp:LinkButton>
                                </div></td>
                          </tr>
                        </table></td>
                      </tr>
                    </table></td>
                  </tr>
                </table></td>
              </tr>
            </table></td>
            <td valign="top">&nbsp;</td>
            <td valign="top"style="background-image: url(<%# BaseUrl %>/login-other.gif); background-repeat:no-repeat"><table width="100%" style="height:148px" border="0" cellpadding="0" cellspacing="0">
              <tr>
                <td height="19" scope="col">&nbsp;</td>
              </tr>
              <tr>
                <td height="116" valign="top"><table width="100%" border="0" cellspacing="0" cellpadding="12">
                  <tr>
                    <td height="118" valign="top" scope="col"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr>
                        <td height="53" scope="col">
                            <asp:LinkButton runat="Server" ID="btnEnterAnyway" OnClick="btnEnterAnyway_Click">
                                <img src="<%# BaseUrl %>/enter-usr.gif" alt="entrar al sitio" width="123" height="30" />
                            </asp:LinkButton>
                        </td>
                      </tr>
                      <tr>
                        <td><div align="left"><span class="style1" style="color: #666666">Acceda y conozca toda nuestra programaci&oacute;n para sus vacaciones, viaje de negocios &oacute; turismo en Uruguay. </span></div></td>
                      </tr>
                    </table></td>
                  </tr>
                </table></td>
              </tr>
            </table></td>
          </tr>
          <tr>
            <td height="152">&nbsp;</td>
            <td colspan="3" valign="top">&nbsp;</td>
            </tr>
        </table></td>
      </tr>
    </table>
    </div>
    
    </form>
</body>
</html>
