using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ActiveUp.WebControls.HtmlTextBox.Tools;
using Rainbow.BusinessRules;
using Rainbow.Configuration;
using Rainbow.Core;
using Rainbow.Settings;
using Image = ActiveUp.WebControls.HtmlTextBox.Tools.Image;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// List of available HTML editors
	/// </summary>
	public class HtmlEditorDataType : ListDataType
	{
		public HtmlEditorDataType()
		{
			InnerDataType = PropertiesDataType.List;
			InitializeComponents();
		}

		public static void HtmlEditorSettings(Hashtable editorSettings, Setting.SettingGroup group)
		{
			Hashtable pS = Active.Portal.CustomSettings;
			//PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

			Setting Editor = new Setting(new HtmlEditorDataType());
			Editor.Order = 1;
			Editor.Group = group;
			Editor.EnglishName = "Editor";
			Editor.Description = "Select the Html Editor for Module";

			Setting ControlWidth = new Setting(new IntegerDataType());
			ControlWidth.Value = "700";
			ControlWidth.Order = 2;
			ControlWidth.Group = group;
			ControlWidth.EnglishName = "Editor Width";
			ControlWidth.Description = "The width of editor control";

			Setting ControlHeight = new Setting(new IntegerDataType());
			ControlHeight.Value = "400";
			ControlHeight.Order = 3;
			ControlHeight.Group = group;
			ControlHeight.EnglishName = "Editor Height";
			ControlHeight.Description = "The height of editor control";

			Setting ShowUpload = new Setting(new BooleanDataType());
			ShowUpload.Value = "true";
			ShowUpload.Order = 4;
			ShowUpload.Group = group;
			ShowUpload.EnglishName = "Upload?";
			ShowUpload.Description = "Only used if Editor is ActiveUp HtmlTextBox";

			Setting ModuleImageFolder = null;
			if (pS != null)
			{
				if (Active.Portal.FullPath != null)
				{
					ModuleImageFolder = new Setting(new FolderDataType(HttpContext.Current.Server.MapPath(pS.FullPath + "/images"), "default"));
					ModuleImageFolder.Value = "default";
					ModuleImageFolder.Order = 5;
					ModuleImageFolder.Group = group;
					ModuleImageFolder.EnglishName = "Default Image Folder";
					ModuleImageFolder.Description = "This folder is used for editor in this module to take and upload images";
				}

				// Set the portal default values
				if (pS != null)
				{
					if (pS["SITESETTINGS_DEFAULT_EDITOR"] != null)
						Editor.Value = pS["SITESETTINGS_DEFAULT_EDITOR"].ToString();
					if (pS["SITESETTINGS_EDITOR_WIDTH"] != null)
						ControlWidth.Value = pS["SITESETTINGS_EDITOR_WIDTH"].ToString();
					if (pS["SITESETTINGS_EDITOR_HEIGHT"] != null)
						ControlHeight.Value = pS["SITESETTINGS_EDITOR_HEIGHT"].ToString();
					if (pS["SITESETTINGS_EDITOR_HEIGHT"] != null)
						ControlHeight.Value = pS["SITESETTINGS_EDITOR_HEIGHT"].ToString();
					if (pS["SITESETTINGS_SHOWUPLOAD"] != null)
						ShowUpload.Value = pS["SITESETTINGS_SHOWUPLOAD"].ToString();
					if (pS["SITESETTINGS_DEFAULT_IMAGE_FOLDER"] != null)
						ModuleImageFolder.Value = pS["SITESETTINGS_DEFAULT_IMAGE_FOLDER"].ToString();
				}
			}

			editorSettings.Add("Editor", Editor);
			editorSettings.Add("Width", ControlWidth);
			editorSettings.Add("Height", ControlHeight);
			editorSettings.Add("ShowUpload", ShowUpload);
			if (ModuleImageFolder != null)
				editorSettings.Add("MODULE_IMAGE_FOLDER", ModuleImageFolder);
		}

		protected override void InitializeComponents()
		{
			base.InitializeComponents();
			// Default
			Value = "FreeTextBox";
			// Change the default value to Portal Default Editor Value by jviladiu@portalServices.net 13/07/2004

			if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
			{
				PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				if (pS != null)
				{
					if (pS["SITESETTINGS_DEFAULT_EDITOR"] != null)
						Value = pS["SITESETTINGS_DEFAULT_EDITOR"].ToString();
				}
			}
		}

		public override object DataSource
		{
			get { return "Plain Text;FCKEditor;FCKEditor V2;FreeTextBox;ActiveUp HtmlTextBox".Split(';'); }
		}

		public override string Description
		{
			get { return "HtmlEditor List"; }
		}

		public override string Value
		{
			get { return (innerValue); }
			set
			{
				innerValue = value;

				DropDownList dd = (DropDownList) innerControl;
				if (dd.Items.FindByValue(value) != null)
				{
					dd.ClearSelection();
					dd.Items.FindByValue(value).Selected = true;
					innerValue = value;
				}
				else
				{
					//Invalid value
				}
			}
		}

		private string getFtbLanguage(string rainbowLanguage)
		{
			switch (rainbowLanguage.Substring(rainbowLanguage.Length - 2).ToLower())
			{
				case "en":
					return "en-US";
				case "us":
					return "en-US";
				case "es":
					return "es-ES";
				case "cn":
					return "zh-cn";
				case "cz":
					return "cz-CZ";
				case "fi":
					return "fi-fi";
				case "nl":
					return "nl-NL";
				case "de":
					return "de-de";
				case "il":
					return "he-IL";
				case "it":
					return "it-IT";
				case "jp":
					return "ja-JP";
				case "kr":
					return "ko-kr";
				case "no":
					return "nb-NO";
				case "pt":
					return "pt-pt";
				case "ro":
					return "ro-RO";
				case "ru":
					return "ru-ru";
				case "se":
					return "sv-se";
				case "tw":
					return "zh-TW";
				default:
					return "en-US";
			}
		}

		public IHtmlEditor GetEditor(Control placeHolderHTMLEditor, int moduleID, bool showUpload, PortalSettings portalSettings)
		{
			IHtmlEditor DesktopText;
			string moduleImageFolder = ModuleSettings.MergeModuleSettings(moduleID)["MODULE_IMAGE_FOLDER"].ToString();

			// Grabs ID from the place holder so that a unique editor is on the page if more than one
			// But keeps same ID so that the information can be submitted to be saved. [CDT]
			string uniqueID = placeHolderHTMLEditor.ID;

			switch (Value)
			{
				case "FCKEditor": // jviladiu@portalservices.net 2004/07/30.
					using (FCKTextBox fck = new FCKTextBox())
					{
						fck.ImageFolder = moduleImageFolder;
						fck.BasePath = Path.WebPathCombine(Path.ApplicationRoot, "aspnet_client/FCK/");
						fck.Config["EditorAreaCSS"] = portalSettings.GetCurrentTheme().CssFile;
						fck.Config["AutoDetectLanguage"] = "false";
						fck.Config["DefaultLanguage"] = portalSettings.PortalUILanguage.Name.Substring(0, 2);
						fck.Config["ToolbarImagesPath"] = Path.WebPathCombine(fck.BasePath, "images/toolbar/office2003/");
						fck.Config["ImageUploadURL"] = Path.WebPathCombine(Path.ApplicationRoot, "app_support/FCK.filemanager/upload.aspx");
						fck.Config["ImageBrowserURL"] = Path.WebPathCombine(Path.ApplicationRoot, "app_support/FCK.filemanager/imagegallery.aspx");
						fck.Config["ImageBrowserWindowWidth"] = "450";
						fck.Config["ImageBrowserWindowHeight"] = "300";
						fck.ID = String.Concat("FCKTextBox", uniqueID);

						DesktopText = ((IHtmlEditor) fck);
					}
					placeHolderHTMLEditor.Controls.Add(((Control) DesktopText));
					break;

				case "FCKEditor V2": // jviladiu@portalservices.net 2004/11/09.
					using (FCKTextBoxV2 fckv2 = new FCKTextBoxV2())
					{
						fckv2.ImageFolder = moduleImageFolder;
						fckv2.BasePath = Path.WebPathCombine(Path.ApplicationRoot, "aspnet_client/FCKEditorV2/");
						fckv2.Config["AutoDetectLanguage"] = "false";
						fckv2.Config["DefaultLanguage"] = portalSettings.PortalUILanguage.Name.Substring(0, 2);
						fckv2.Config["EditorAreaCSS"] = portalSettings.GetCurrentTheme().CssFile;
						fckv2.ID = String.Concat("FCKTextBox", uniqueID);

						DesktopText = ((IHtmlEditor) fckv2);
					}
					placeHolderHTMLEditor.Controls.Add(((Control) DesktopText));
					break;

				case "FreeTextBox":
					using (FreeTextBox freeText = new FreeTextBox())
					{
						freeText.ToolbarLayout = "ParagraphMenu,FontFacesMenu,FontSizesMenu,FontForeColorPicker,FontBackColorPicker,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat;CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;InsertRule|Delete,Cut,Copy,Paste;Undo,Redo,Print;InsertTable,InsertTableColumnAfter,InsertTableColumnBefore,InsertTableRowAfter,InsertTableRowBefore,DeleteTableColumn,DeleteTableRow,InsertImageFromGallery";
						freeText.ImageGalleryUrl = Path.WebPathCombine(Path.ApplicationRoot, "app_support/ftb.imagegallery.aspx?rif={0}&cif={0}");
						freeText.ImageFolder = moduleImageFolder;
						freeText.ImageGalleryPath = Path.WebPathCombine(portalSettings.FullPath, freeText.ImageFolder);
						freeText.ID = String.Concat("FreeText", uniqueID);
						freeText.Language = getFtbLanguage(portalSettings.PortalUILanguage.Name);

						DesktopText = ((IHtmlEditor) freeText);
					}
					placeHolderHTMLEditor.Controls.Add(((Control) DesktopText));
					break;

				case "ActiveUp HtmlTextBox":
					using (HtmlTextBox h = new HtmlTextBox())
					{
						h.ImageFolder = moduleImageFolder;
						DesktopText = (IHtmlEditor) h;
						placeHolderHTMLEditor.Controls.Add(((Control) DesktopText));

						// Allow content editors to see the content with the same style that it is displayed in
						h.ContentCssFile = portalSettings.GetCurrentTheme().CssFile;

						// Custom Properties must come after control is added to placeholder
						h.EnsureToolsCreated();

						// Set the icons folder
						h.IconsDir = Path.WebPathCombine(Path.ApplicationRoot, "aspnet_client/ActiveUp/icons/");

						// Add the Help icon
						StringBuilder sbHelp = new StringBuilder();
						sbHelp.Append("var Help=window.open('");
						sbHelp.Append(Path.ApplicationRoot);
						sbHelp.Append("/aspnet_client/ActiveUp/");
						sbHelp.Append("htmltextbox_help.html', 'HTML_TextBox_Help', 'height=520, width=520, resizable=yes, scrollbars=yes, menubar=no, toolbar=no, directories=no, location=no, status=no');Help.moveTo('20', '20');");
						Custom openPage = new Custom();
						openPage.IconOff = "help_off.gif";
						openPage.IconOver = "help_off.gif";
						openPage.ClientSideOnClick = sbHelp.ToString();
						h.Toolbars[0].Tools.Add(openPage);

						// Add the image library			
						Image imageLibrary = (Image) h.Toolbars[0].Tools["Image"];
						imageLibrary.AutoLoad = true;

						// Clear the directories collection because it is stored in ViewState and must be cleared or upload will result in display of multiple directories of the same name
						imageLibrary.Directories.Clear();
						imageLibrary.Directories.Add("images", HttpContext.Current.Server.MapPath(portalSettings.FullPath + h.ImageFolder), portalSettings.FullPath + h.ImageFolder + "/");

						if (!showUpload)
							imageLibrary.UploadDisabled = true;
					}
					break;

				case "Plain Text":
				default:
					DesktopText = (new TextEditor());
					placeHolderHTMLEditor.Controls.Add(((Control) DesktopText));
					break;
			}
			return DesktopText;
		}
	}
}