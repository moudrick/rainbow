<%@ Register TagPrefix=mobile Namespace=System.Web.UI.MobileControls Assembly=System.Web.Mobile %>
<%@ Control Language=c# AutoEventWireup=false Inherits=System.Web.UI.MobileControls.MobileUserControl TargetSchema=http://schemas.microsoft.com/Mobile/WebUserControl %>
<!-- Rainbow Mobile Portal Style: Default-->
<mobile:stylesheet id=MobileStyleSheet runat=server>
	<mobile:Style Name=Style_Global Font-Size=Normal Font-Name=Verdana,Arial Font-Bold=False Font-Italic=False BackColor=White ForeColor=Gray Wrapping=NotSet Alignment=Left></mobile:Style>

	<mobile:Style Name=Style_Form			StyleReference=Style_Global ></mobile:Style>
	<mobile:Style Name=Style_PortalTitle		StyleReference=Style_Global ForeColor=DarkOrange	Font-Size=Large Font-Bold=True></mobile:Style>
	<mobile:Style Name=Style_TabView		StyleReference=Style_Global ></mobile:Style>
	<mobile:Style Name=Style_Tabs			StyleReference=Style_Global ForeColor=Moccasin		BackColor=Gray Font-Size=Large Font-Bold=True></mobile:Style>
	<mobile:Style Name=Style_ActiveTab		StyleReference=Style_Global ForeColor=DarkOrange	BackColor=Gray Font-Size=Large Font-Bold=True></mobile:Style>
	<mobile:Style Name=Style_Pager			StyleReference=Style_Global ForeColor=Gray Font-Size=Small></mobile:Style>
	<mobile:Style Name=Style_Link			StyleReference=Style_Global ForeColor=Gray Font-Size=Small></mobile:Style>
	<mobile:Style Name=Style_Module			StyleReference=Style_Global ForeColor=Black BackColor=Moccasin ></mobile:Style>
	<mobile:Style Name=Style_Title			StyleReference=Style_Global ForeColor=Black Font-Size=Large  Font-Bold=True></mobile:Style>
	<mobile:Style Name=Style_SubTitle		StyleReference=Style_Global ForeColor=Black Font-Bold=True></mobile:Style>
	<mobile:Style Name=Style_Text			StyleReference=Style_Global BackColor=Red ></mobile:Style>
</mobile:stylesheet>