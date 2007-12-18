using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Core;
using Rainbow.Helpers;
using Image = Esperantus.WebControls.Image;
using Literal = Esperantus.WebControls.Literal;
using Page = Rainbow.BusinessRules.Page;

namespace Rainbow.Configuration
{
	/// <summary>
	/// Class that defines data for the event
	/// </summary>
	public class SettingsTableEventArgs : EventArgs
	{
		private Setting _currentItem;

		/// <summary>
		///     
		/// </summary>
		/// <param name="item" type="Rainbow.Configuration.Setting">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public SettingsTableEventArgs(Setting item)
		{
			_currentItem = item;
		}

		/// <summary>
		/// CurrentItem
		/// </summary>
		public Setting CurrentItem
		{
			get { return (_currentItem); }
			set { _currentItem = value; }
		}
	}

	/// <summary>
	/// UpdateControlEventHandler delegate
	/// </summary>
	public delegate void UpdateControlEventHandler(object sender, SettingsTableEventArgs e);

	/// <summary>
	/// A table control used to manage custom settings list
	/// </summary>
	public class SettingsTable : Panel //Table
	{
		private int objectID = -1;

		/// <summary>
		/// Used to store reference to base object it, 
		/// can be ModuleID or Portal ID
		/// </summary>
		public int ObjectID
		{
			get { return objectID; }
			set { objectID = value; }
		}


		/// <summary>
		/// Used to store controls of settings
		/// </summary>
		protected Hashtable EditControls = new Hashtable();

		private SortedList Settings;

		/// <summary>
		/// The UpdateControl event is defined using the event keyword.
		/// The type of UpdateControl is UpdateControlEventHandler.
		/// </summary>
		public event UpdateControlEventHandler UpdateControl;

		/// <summary>
		/// DataBind
		/// </summary>
		public override void DataBind()
		{
			//this.Rows.Clear();
			//temp
			//			this.Attributes.Add("border","1");
			//			this.Width = new Unit("100%");
			this.CssClass = "settings-table";

			if (Settings != null)
			{
				if (Settings.GetKeyList() != null)
				{
					// Jes1111 -- force the list to obey Setting.Order property and divide it into groups
					// Manu -- a better order system avoiding try and catch.
					//         Now settings with no order have a progressive order number 
					//         based on their position on list
					SortedList SettingsOrder = new SortedList();
					int order = 0;

					foreach (string key in Settings.GetKeyList())
					{
						if (Settings[key] != null)
						{
							if (Settings[key] is Setting)
							{
								order = ((Setting) Settings[key]).Order;

								while (SettingsOrder.ContainsKey(order))
									order++; //be sure do not have duplicate order key or we get an error
								SettingsOrder.Add(order, key);
							}

							else
								LogHelper.Logger.Log(LogLevel.Debug, "Unexpected '" + Settings[key].GetType().FullName + "' in settings table.");
						}
					}
					HtmlGenericControl _fieldset = new HtmlGenericControl("dummy");
					HtmlGenericControl _legend = new HtmlGenericControl("dummy");
					Table _tbl = new Table();
					TableRow tr = new TableRow();
					TableCell cHelp = new TableCell();
					TableCell c1 = new TableCell();
					TableCell c2 = new TableCell();
					Literal myGroupControl = new Literal();
					Literal myControl = new Literal();
					Image img = new Image();
					//Image img = new Image();
					Control editControl = new Control();
					string _groupClass = string.Empty;
					// Initialize controls
					Setting.SettingGroup currentGroup = new Setting.SettingGroup(Setting.SettingGroupIds.NONE);

					foreach (string currentSetting in SettingsOrder.GetValueList())
					{
						Setting currentItem = (Setting) Settings[currentSetting];

						if (currentItem.Group != currentGroup)
						{
							if (_fieldset.Attributes.Count > 0) // add built fieldset
							{
								_fieldset.Controls.Add(_tbl);
								this.Controls.Add(_fieldset);
							}
							// start a new fieldset
							_fieldset = new HtmlGenericControl("fieldset");
							_fieldset.Attributes.Add("class", string.Concat("SettingsTableGroup ", currentItem.Group.ToString().ToLower()));
							// get group legend text
							myGroupControl = new Literal();
							myGroupControl.TextKey = currentItem.Group.ToString();
							myGroupControl.Text = currentItem.Group.Description;
							// add group legend
							_legend = new HtmlGenericControl("legend");
							_legend.Attributes.Add("class", "SubSubHead");
							_legend.Controls.Add(myGroupControl);
							_fieldset.Controls.Add(_legend);
							// start a new table
							_tbl = new Table();
							_tbl.Attributes.Add("class", "SettingsTableGroup");
							_tbl.Attributes.Add("width", "100%");
							currentGroup = currentItem.Group;
						}
						// start new row
						tr = new TableRow();
						// Help cell
						cHelp = new TableCell();

						if (currentItem.Description.Length > 0)
						{
							System.Web.UI.WebControls.Image _myImg = ((Page) this.Page).CurrentTheme.GetImage("Buttons_Help", "Help.gif");
							img = new Image();
							img.ImageUrl = _myImg.ImageUrl;
							img.Height = _myImg.Height;
							img.Width = _myImg.Width;
							img.AlternateText = currentItem.Description;
							img.TextKey = currentSetting + "_DESCRIPTION"; //Fixed key for simplicity
						}

						else
						{
							// Jes1111 - 17/12/2004
							img = new Image();
							img.Width = Unit.Pixel(25);
							img.ImageUrl = ((Page) this.Page).CurrentTheme.GetImage("Spacer", "Spacer.gif").ImageUrl;
						}
						cHelp.Controls.Add(img);
						// add to row
						tr.Controls.Add(cHelp);
						// Setting Name cell
						c1 = new TableCell();
						c1.Attributes.Add("width", "20%");
						c1.CssClass = "SubHead";
						//c1.Wrap = false; //?
						myControl = new Literal();
						myControl.TextKey = currentSetting;
						myControl.Text = currentItem.EnglishName;
						c1.Controls.Add(myControl);
						// add to row
						tr.Controls.Add(c1);
						// Setting Control cell
						c2 = new TableCell();
						c2.Attributes.Add("width", "80%");
						c2.CssClass = "st-control";

						try
						{
							editControl = currentItem.EditControl;
							editControl.ID = currentSetting; // Jes1111
							editControl.EnableViewState = true;
						}

						catch (Exception ex)
						{
							editControl = new LiteralControl("There was an error loading this control");
							LogHelper.Logger.Log(LogLevel.Warn, "There was an error loading '" + currentItem.EnglishName + "'", ex);
						}
						c2.Controls.Add(editControl);
						//508
						myControl.LabelForControl = editControl.ClientID;
						//Add control to edit controls collection
						EditControls.Add(currentSetting, editControl);
						//Validators
						c2.Controls.Add(new LiteralControl("<br />"));

						//Required
						if (currentItem.Required)
						{
							RequiredFieldValidator req = new RequiredFieldValidator();
							req.ErrorMessage = Localize.GetString("SETTING_REQUIRED", "%1% is required!", req).Replace("%1%", currentSetting);
							req.ControlToValidate = currentSetting;
							req.CssClass = "Error";
							req.Display = ValidatorDisplay.Dynamic;
							req.EnableClientScript = true;
							c2.Controls.Add(req);
						}

						//Range Validator
						if (currentItem.MinValue != 0 || currentItem.MaxValue != 0)
						{
							RangeValidator rang = new RangeValidator();

							switch (currentItem.GetType())
							{
								case PropertiesDataType.String:
									rang.Type = ValidationDataType.String;
									break;

								case PropertiesDataType.Integer:
									rang.Type = ValidationDataType.Integer;
									break;

								case PropertiesDataType.Currency:
									rang.Type = ValidationDataType.Currency;
									break;

								case PropertiesDataType.Date:
									rang.Type = ValidationDataType.Date;
									break;

								case PropertiesDataType.Double:
									rang.Type = ValidationDataType.Double;
									break;
							}

							if (currentItem.MinValue >= 0 && currentItem.MaxValue >= currentItem.MinValue)
							{
								rang.MinimumValue = currentItem.MinValue.ToString();

								if (currentItem.MaxValue == 0)
									rang.ErrorMessage = Localize.GetString("SETTING_EQUAL_OR_GREATER", "%1% must be equal or greater than %2%!", rang).Replace("%1%", currentSetting).Replace("%2%", currentItem.MinValue.ToString());

								else
								{
									rang.MaximumValue = currentItem.MaxValue.ToString();
									rang.ErrorMessage = Localize.GetString("SETTING_BETWEEN", "%1% must be between %2% and %3%!", rang).Replace("%1%", currentSetting).Replace("%2%", currentItem.MinValue.ToString()).Replace("%3%", currentItem.MaxValue.ToString());
								}
							}
							rang.ControlToValidate = currentSetting;
							rang.CssClass = "Error";
							rang.Display = ValidatorDisplay.Dynamic;
							rang.EnableClientScript = true;
							c2.Controls.Add(rang);
						}
						tr.Controls.Add(c2);
						_tbl.Rows.Add(tr);
					}
					// add closing here
					_fieldset.Controls.Add(_tbl);
					this.Controls.Add(_fieldset);
				}
			}
		}

		/// <summary>
		/// UpdateControls
		/// </summary>
		public void UpdateControls()
		{
			foreach (string key in EditControls.Keys)
			{
				Control c = (Control) EditControls[key];
				Setting currentItem = (Setting) Settings[c.ID];
				currentItem.EditControl = c;
				OnUpdateControl(new SettingsTableEventArgs(currentItem));
			}
		}

		/// <summary>
		/// The protected OnUpdateControl method raises the event by invoking 
		/// the delegates. The sender is always this, 
		/// the current instance of /the class.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnUpdateControl(SettingsTableEventArgs e)
		{
			if (UpdateControl != null)
			{
				//Invokes the delegates.
				UpdateControl(this, e);
			}
		}

		/// <summary>
		/// DataSource
		/// </summary>
		public object DataSource
		{
			get { return Settings; }
			set { Settings = (SortedList) value; }
		}
	}
}