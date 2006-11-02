<%@ Control Language="c#" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.SimpleMenu.SimpleMenuType" %>
<%@ Register TagPrefix="portal" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<%@ Register TagPrefix="XW" Namespace="CrossServices.Web.UI.WebControls" Assembly="XGUIWebControls" %>
<xmlns:XW="urn:http://schemas.mhsl.net/WebControls" />
	<!--simple image menu-->
	<portal:DESKTOPNAVIGATION
						id=NavigationMenu 
						runat="server"  
						visible="True" 
						CssClass="sm_SimpleMenu" 
						ShowHeader="False" 
						ShowFooter="False" 
						RepeatDirection="<%#MenuRepeatDirection%>"
						ParentPageID="<%#ParentPageID%>" 
						Bind="<%#MenuBindOption%>"
						AutoBind="False" 
						EnableViewState="False">

		<SelectedItemStyle CssClass="sm_SelectedTab" />	
		<SelectedItemTemplate>
			&nbsp;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl(((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabID)%>'
			><XW:XDynamicLabelImage   
				id=XDYNAMICLABELIMAGE1 
				runat="server"
				LabelText='<%# ((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabName %>' >
				<LabelDefinition 
				BackgroundImageURL="" 
				LabelFont="Century Gothic, 31px, style=Bold" 
				LabelForeColor="Green"  
				LabelBackColor="Yellow" />
			</XW:XDynamicLabelImage></a>&nbsp;
		</SelectedItemTemplate>

		<AlternatingItemStyle CssClass="sm_OtherSubTabsAlt" />
		<AlternatingItemTemplate>
			&nbsp;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl(((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabID)%>'
			><XW:XDynamicLabelImage 
				id=XDYNAMICLABELIMAGE2 
				runat="server"
				LabelText='<%# ((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabName %>' >
				<LabelDefinition 
				BackgroundImageURL="" 
				LabelFont="Verdana, 35px, style=Bold" 
				LabelForeColor="Green"  
				LabelBackColor="Red" />			
			</XW:XDynamicLabelImage></a>&nbsp;
	   </AlternatingItemTemplate>

		<ItemStyle CssClass="sm_OtherSubTabs" />
		<ItemTemplate>
			&nbsp;<a href='<%#Rainbow.HttpUrlBuilder.BuildUrl(((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabID)%>'
			><XW:XDynamicLabelImage 
				id=XDYNAMICLABELIMAGE3 
				runat="server"
				LabelText='<%# ((Rainbow.Configuration.TabStripDetails) Container.DataItem).TabName %>' >
				<LabelDefinition 
				BackgroundImageURL="" 
				LabelFont="Century Gothic, 31px, style=Bold" 
				LabelForeColor="Green"  
				LabelBackColor="Magenta" />
			</XW:XDynamicLabelImage></a>&nbsp;
		</ItemTemplate>
		
		<FooterStyle CssClass="sm_Footer" />
		<FooterTemplate></FooterTemplate>
		
		<HeaderStyle CssClass="sm_Header" />
		<HeaderTemplate></HeaderTemplate>
	</portal:DESKTOPNAVIGATION>
	<!--END: simple static menu-->