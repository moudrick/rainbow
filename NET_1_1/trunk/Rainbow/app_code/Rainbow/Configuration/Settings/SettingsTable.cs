using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ELB;
using Esperantus;
using Rainbow.Helpers;
using Rainbow.Settings;
using Rainbow.UI.DataTypes;
using Page = Rainbow.UI.Page;

namespace Rainbow.Configuration
{

	#region Event argument class

	/// <summary>
	/// Class that defines data for the event
	/// </summary>
	public class SettingsTableEventArgs : EventArgs 
	{

		private SettingItem _currentItem;

		/// <summary>
		///     
		/// </summary>
		/// <param name="item" type="Rainbow.Configuration.SettingItem">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public SettingsTableEventArgs(SettingItem item)
		{
			_currentItem = item;
		}

		/// <summary>
		/// CurrentItem
		/// </summary>
		public SettingItem CurrentItem
		{  
			get
			{
				return(_currentItem);
			}
			set
			{
				_currentItem = value;
			}
		}
	}

	#endregion

	#region Delegate

	/// <summary>
	/// UpdateControlEventHandler delegate
	/// </summary>
	public delegate void UpdateControlEventHandler(object sender, SettingsTableEventArgs e);
	
	#endregion

	#region SettingsTable control

	/// <summary>
	/// A databound control that takes in custom settings list in a SortedList
	/// object and creates the hierarchy of the settings controls in two different
	/// ways. One shows the grouped settings flat and the other shows the grouped 
	/// settings in selectable tabs.  
	/// </summary>
	public class SettingsTable : WebControl, INamingContainer
	{
		// Notes and Credits:
		// Motive: 
		//    In the property page of rainbow modules, there are groups of settings.
		//    Some people like the old way of look and feel of the settings (befoer 
		//    svn version 313, some like the new way of grouping the settings into 
		//    tabs. This modification handles over the power to make choice to the end 
		//    user by providing an attribute "UseGrouingTabs" which in turn will get 
		//    value from Rainbow.Settings (an entry is added over there) that is set 
		//    by user.
		//
		// What is changed: 
		//    Many changes in order to implement the functionality and make the control
		//    an nice databound control. However, the child control creating logic is
		//    NOT changed. Basically, these logic was in the DataBind() function of the 
		//    previous implementation. Event processing logic is NOT changed.
		//
		// Credits:
		//    Most credit should go to the developers who created and modifed the previous 
		//    class because they created the logic for doing business. I keep their names 
		//    and comments in the new code and I also keep a copy of the whole old code in
		//    the region "Previous SettingsTable control" to honor their contributions.
		//    
		// Special Credit:
		//    This modification is done per Manu's request and he also gave many good 
		//    suggestions.
		//
		//    Hongwei Shen (hongwei.shen@gmail.com) Oct. 15, 2005


		#region Member variables

		private SortedList Settings;

		// when grouping tabs created, it is set to true to inject javascripts
		private bool _groupingTabsCreated = false;
		private Hashtable _editControls;
		private bool _useGroupingTabs = false;

		/// <summary>
		/// Used to store reference to base object it, 
		/// can be ModuleID or Portal ID
		/// </summary>
		public int ObjectID = -1;

		#endregion

		#region properties

		/// <summary>
		/// DataSource, it is limited to SortedList type
		/// </summary>
		public object DataSource
		{
			get
			{
				return Settings;
			}
			set
			{
				if(value == null || value is SortedList)
				{
					Settings = (SortedList) value;
				}
				else
				{
					throw new ArgumentException("DataSource must be SortedList type", "DataSource");
				}
			}    
		}
		
		/// <summary>
		/// Settings control collection, it is initialized only 
		/// when referenced.
		/// </summary>
		protected virtual Hashtable EditControls
		{
			get
			{
				if(_editControls == null)
				{
					_editControls = new Hashtable();
				}
				return _editControls;
			}
		}
		
		/// <summary>
		/// If set to true, create the control hirarchy grouping property 
		/// settings into selected tabs. Otherwise, create the control
		/// hirarchy as flat fieldsets. Default is false.
		/// </summary>
		public bool UseGroupingTabs
		{
			get 
			{
				//return _useGroupingTabs;
				return Rainbow.Settings.Config.UseSettingsGroupingTabs;
			}
			set
			{
				_useGroupingTabs = value;
			}
		}
	
		#endregion

		#region overriden properties

			protected override HtmlTextWriterTag TagKey
		{
			get
			{
				// render the out tag as div
				return HtmlTextWriterTag.Div;
			}
		}

		public override Unit Width
		{
			get
			{
				//return base.Width;
				return Rainbow.Settings.Config.SettingsGroupingWidth;
			}
			set
			{
				base.Width = value;
			}
		}

		public override Unit Height
		{
			get
			{
				//return base.Height;
				return Rainbow.Settings.Config.SettingsGroupingHeight;
			}
			set
			{
				base.Height = value;
			}
		}


		#endregion

		#region Events
		/// <summary>
		/// The UpdateControl event is defined using the event keyword.
		/// The type of UpdateControl is UpdateControlEventHandler.
		/// </summary>
		public event UpdateControlEventHandler UpdateControl;

		#endregion

		#region Events processing

		/// <summary>
		/// This method provide a way to trigger UpdateControl event
		/// for the child controls of this control from outside.
		/// </summary>
		public void UpdateControls()
		{
			foreach(string key in EditControls.Keys)
			{
				Control c = (Control) EditControls[key];
				SettingItem currentItem = (SettingItem) Settings[c.ID];
				currentItem.EditControl = c;
				OnUpdateControl(new SettingsTableEventArgs(currentItem));
			}
		}

		/// <summary>
		/// Raises UpdateControl Event 
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

		#endregion

		#region overriden methods

		public override void DataBind()
		{
			// raise databinding event in case there are binding scripts
			base.OnDataBinding(EventArgs.Empty);

			// clear existing control hierarchy
			Controls.Clear();
			ClearChildViewState();

			// start tracking changes during databinding
			TrackViewState();

			// create control hierarchy from the data source
			CreateControlHierarchy(true);
			ChildControlsCreated = true;
		}
 
		protected override void CreateChildControls()
		{
			Controls.Clear();

			// recover control hierarchy from viewstate is not implemented 
			// at this time. If somebody wants to do it, turn the following 
			// line on and do the logic in "createGroupFlat" and 
			// "createGroupingTabs"
			
			// CreateControlHierarchy(false);
		}
	
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender (e);

			// the scripts needed for using grouping tabs
			if (_groupingTabsCreated)
			{
				// Jonathan - tabsupport
				if(((UI.Page)this.Page).IsClientScriptRegistered("x_core") == false)
				{
					((UI.Page)this.Page).RegisterClientScript("x_core", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_core.js"));
				}
				if(((UI.Page)this.Page).IsClientScriptRegistered("x_event") == false)
				{
					((UI.Page)this.Page).RegisterClientScript("x_event", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_event.js"));
				}
				if(((UI.Page)this.Page).IsClientScriptRegistered("x_dom") == false)
				{
					((UI.Page)this.Page).RegisterClientScript("x_dom", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_dom.js"));
				}
				if(((UI.Page)this.Page).IsClientScriptRegistered("tabs_js") == false)
				{
					((UI.Page)this.Page).RegisterClientScript("tabs_js", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_tpg.js"));
				}
				// End tab support

				// this piece of script was previously in PropertyPage.aspx, but it should be
				// part of the SettingsTable control when using tabs. So, I moved it to here.
				// It is startup script
				if(!((UI.Page)this.Page).IsStartupScriptRegistered("tab_startup_js"))
				{ 
					string script = 
						"<script language=\"javascript\" type=\"text/javascript\">" + 
						" var tabW = " + Width.Value + "; " + 
						" var tabH = " + Height.Value + "; " +
						" var tpg1 = new xTabPanelGroup('tpg1', tabW, tabH, 50, 'tabPanel', 'tabGroup', 'tabDefault', 'tabSelected'); " + 
						"</script>";

					((UI.Page)this.Page).RegisterStartupScript("tab_startup_js", script);
				}
			}
		}


		#endregion

		#region Control Methods

		/// <summary>
		/// Creating control hierarchy of this control. Depending on the
		/// value of UseGroupingTabs, two exclusively different hierarchies
		/// may be created. One uses grouping tabs and another uses flat
		/// fieldsets.  
		/// </summary>
		/// <param name="useDataSource">
		/// If true, create controlhierarchy from data source, otherwise
		/// create control hierachy from view state
		/// </param>
		protected virtual void CreateControlHierarchy(bool useDataSource)
		{
			// re-order settings items, the re-ordered items
			// is put in SettingsOrder
			SortedList orderedSettings = processDataSource();

			if (UseGroupingTabs)
			{
				createGroupingTabs(useDataSource, orderedSettings);
			}
			else
			{
				createGroupFlat(useDataSource, orderedSettings);
			}
		}

		/// <summary>
		/// Re-order the settings items. The reason why this processing is
		/// necessary is that two settings items may have same order. 
		/// </summary>
		protected virtual SortedList processDataSource()
		{
			// Jes1111 -- force the list to obey SettingItem.Order property and divide it into groups
			// Manu -- a better order system avoiding try and catch.
			//         Now settings with no order have a progressive order number 
			//         based on their position on list
			SortedList SettingsOrder = new SortedList();
			int order = 0;

			foreach(string key in Settings.GetKeyList())
			{
				if(Settings[key]!= null)
				{
					if(Settings[key] is SettingItem)
					{
						order = ((SettingItem)Settings[key]).Order;

						while (SettingsOrder.ContainsKey(order))
						{
							// be sure do not have duplicate order key or 
							// we get an error
							order++; 
						}
						SettingsOrder.Add(order, key);
					}
					else
					{
						ErrorHandler.Publish(LogLevel.Debug, "Unexpected '" + Settings[key].GetType().FullName + "' in settings table.");
					}
				}
			}
			return SettingsOrder;
		}

		#endregion

		#region private help functions

		/// <summary>
		/// Create the flat settings groups control hirarchy
		/// </summary>
		private void createGroupFlat(bool useDataSource, SortedList SettingsOrder)
		{
			if(SettingsOrder.GetKeyList() == null)
			{
				return;
			}

			if(!useDataSource)
			{
				// recover control hierarchy from view state is not implemented
				return;
			}

			HtmlGenericControl _fieldset = new HtmlGenericControl("dummy");
	
			Table _tbl = new Table();

			Control editControl = new Control();
				
			// Initialize controls
			SettingItemGroup currentGroup = SettingItemGroup.NONE;

			foreach(string currentSetting in SettingsOrder.GetValueList())
			{
				SettingItem currentItem = (SettingItem) Settings[currentSetting];

				if (currentItem.Group != currentGroup)
				{

					if ( _fieldset.Attributes.Count > 0 ) // add built fieldset
					{
						_fieldset.Controls.Add(_tbl);
						this.Controls.Add(_fieldset);
					}

					// start a new fieldset
					_fieldset = createNewFieldSet(currentItem);

					// start a new table
					_tbl = new Table();
					_tbl.Attributes.Add("class","SettingsTableGroup");
					_tbl.Attributes.Add("width","100%");
					currentGroup = currentItem.Group;
				}
				_tbl.Rows.Add(createOneSettingRow(currentSetting, currentItem));
			}

			_fieldset.Controls.Add(_tbl);
			this.Controls.Add(_fieldset);
		}

		/// <summary>
		/// Create the grouping tabs control hirarchy
		/// </summary>
		private void createGroupingTabs(bool useDataSource, SortedList SettingsOrder)
		{
			if(Settings.GetKeyList() == null)
			{
				return;
			}

			if(!useDataSource)
			{
				// recover control hierarchy from view state is not implemented
				return;
			}

			HtmlGenericControl tabPanelGroup = new HtmlGenericControl("div");
			tabPanelGroup.Attributes.Add("id", "tpg1");
			tabPanelGroup.Attributes.Add("class", "tabPanelGroup");

			HtmlGenericControl tabGroup = new HtmlGenericControl("div");
			tabGroup.Attributes.Add("class", "tabGroup");
			tabPanelGroup.Controls.Add(tabGroup);
					
			HtmlGenericControl tabPanel = new HtmlGenericControl("div");
			tabPanel.Attributes.Add("class", "tabPanel");

			HtmlGenericControl tabDefault = new HtmlGenericControl("div");
			tabDefault.Attributes.Add("class", "tabDefault");

			HtmlGenericControl _fieldset = new HtmlGenericControl("dummy");
			
			Table _tbl = new Table();

			// Initialize controls
			SettingItemGroup currentGroup = SettingItemGroup.NONE;

			foreach(string currentSetting in SettingsOrder.GetValueList())
			{
				SettingItem currentItem = (SettingItem) Settings[currentSetting];

				if(tabDefault.InnerText.Length == 0)
				{
					tabDefault = new HtmlGenericControl("div");
					tabDefault.Attributes.Add("class", "tabDefault");
					tabDefault.InnerText = Localize.GetString(currentItem.Group.ToString());
				}

				if (currentItem.Group != currentGroup)
				{

					if ( _fieldset.Attributes.Count > 0 ) // add built fieldset
					{
						_fieldset.Controls.Add(_tbl);
								
						tabPanel.Controls.Add(_fieldset);
						tabPanelGroup.Controls.Add(tabPanel);
						tabGroup.Controls.Add(tabDefault);
					}
					// start a new fieldset
					_fieldset = createNewFieldSet(currentItem);

					tabPanel = new HtmlGenericControl("div");
					tabPanel.Attributes.Add("class", "tabPanel");
							
					tabDefault = new HtmlGenericControl("div");
					tabDefault.Attributes.Add("class", "tabDefault");
					tabDefault.InnerText = Localize.GetString(currentItem.Group.ToString());
							
					// start a new table
					_tbl = new Table();
					_tbl.Attributes.Add("class","SettingsTableGroup");
					_tbl.Attributes.Add("width","100%");
					currentGroup = currentItem.Group;
				}

				_tbl.Rows.Add(createOneSettingRow(currentSetting, currentItem));
			}

			tabGroup.Controls.Add(tabDefault);

			_fieldset.Controls.Add(_tbl);
								
			tabPanel.Controls.Add(_fieldset);
			tabPanelGroup.Controls.Add(tabPanel);

			this.Controls.AddAt(0, tabPanelGroup);
		
			this._groupingTabsCreated = true;
		}

		/// <summary>
		/// Returns a new field set with legend for a new settings group
		/// </summary>
		/// <param name="currentItem">
		/// The settings item
		/// </param>
		/// <returns>
		/// Fieldset control
		/// </returns>
		private HtmlGenericControl createNewFieldSet(SettingItem currentItem)
		{
			// start a new fieldset
			HtmlGenericControl fieldset = new HtmlGenericControl("fieldset");
			fieldset.Attributes.Add("class", string.Concat("SettingsTableGroup ", currentItem.Group.ToString().ToLower()));

			// create group legend
			HtmlGenericControl legend = new HtmlGenericControl("legend");
			legend.Attributes.Add("class","SubSubHead");
			Esperantus.WebControls.Literal legendText = new Esperantus.WebControls.Literal();
			legendText.TextKey = currentItem.Group.ToString();
			legendText.Text = currentItem.GroupDescription;
			legend.Controls.Add(legendText);
			fieldset.Controls.Add(legend);

			return fieldset;
		}

		/// <summary>
		/// Returns one settings row that contains a cell for help, a cell for setting item
		/// name and a cell for setting item and validators.
		/// </summary>
		/// <param name="currentItem"></param>
		/// <returns></returns>
		private TableRow createOneSettingRow(string currentSetting, SettingItem currentItem)
		{
			// the table row is going to have three cells 
            TableRow row = new TableRow();

			// cell for help icon and description
			TableCell helpCell = new TableCell();
			Esperantus.WebControls.Image img = new Esperantus.WebControls.Image();

			if(currentItem.Description.Length > 0)
			{
				Image _myImg = ((UI.Page)this.Page).CurrentTheme.GetImage("Buttons_Help","Help.gif");
				img = new Esperantus.WebControls.Image();
				img.ImageUrl = _myImg.ImageUrl;
				img.Height = _myImg.Height;
				img.Width = _myImg.Width;
				// Jminond: added netscape tooltip support
				img.AlternateText = currentItem.Description;
				img.Attributes.Add("title", Localize.GetString(currentSetting + "_DESCRIPTION"));
				img.TextKey = currentSetting + "_DESCRIPTION"; //Fixed key for simplicity
			}
			else
			{
				// Jes1111 - 17/12/2004
				img = new Esperantus.WebControls.Image();
				img.Width = Unit.Pixel(25);
				img.ImageUrl =  ((Page)this.Page).CurrentTheme.GetImage("Spacer","Spacer.gif").ImageUrl;
			}

			helpCell.Controls.Add(img);

			// add help cell to the row
			row.Cells.Add(helpCell);

			// Setting Name cell
			TableCell nameCell = new TableCell();
			nameCell.Attributes.Add("width","20%");
			nameCell.CssClass = "SubHead";
			Esperantus.WebControls.Literal nameText = new Esperantus.WebControls.Literal();
			nameText.TextKey = currentSetting;
			nameText.Text = currentItem.EnglishName;
			nameCell.Controls.Add(nameText);
			
			// add name cell to the row
			row.Cells.Add(nameCell);
			
			// Setting Control cell
			TableCell settingCell = new TableCell();
			settingCell.Attributes.Add("width","80%");
			settingCell.CssClass = "st-control";

			Control editControl;
			try
			{
				editControl = currentItem.EditControl;
				editControl.ID = currentSetting; // Jes1111
				editControl.EnableViewState = true;
			}
			catch(Exception ex)
			{
				editControl = new LiteralControl("There was an error loading this control");
				LogHelper.Logger.Log(LogLevel.Warn, "There was an error loading '" + currentItem.EnglishName + "'", ex);
			}
			settingCell.Controls.Add(editControl);

			nameText.LabelForControl = editControl.ClientID;

			//Add control to edit controls collection
			EditControls.Add(currentSetting, editControl);

			//Validators
			settingCell.Controls.Add(new LiteralControl("<br />"));

			//Required
			if (currentItem.Required && !(editControl is EasyListBox))
			{
				RequiredFieldValidator req = new RequiredFieldValidator();
				req.ErrorMessage = Localize.GetString("SETTING_REQUIRED", "%1% is required!", req).Replace("%1%", currentSetting);
				req.ControlToValidate = currentSetting;
				req.CssClass = "Error";
				req.Display = ValidatorDisplay.Dynamic;
				req.EnableClientScript = true;
				settingCell.Controls.Add(req);
			}

			//Range Validator
			if (currentItem.MinValue != 0 || currentItem.MaxValue != 0)
			{
				RangeValidator rang = new RangeValidator();

				switch(currentItem.DataType)
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

					if(currentItem.MaxValue == 0)
					{
						rang.ErrorMessage = Localize.GetString("SETTING_EQUAL_OR_GREATER", "%1% must be equal or greater than %2%!", rang).Replace("%1%", currentSetting).Replace("%2%", currentItem.MinValue.ToString());
					}
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
				settingCell.Controls.Add(rang);
			}
			// add setting cell into the row
			row.Cells.Add(settingCell);

			// all done send it back
			return row;
		}

		#endregion

		#region Previous SettingsTable control
		/*
			/// <summary>
			/// A table control used to manage custom settings list
			/// </summary>
			public class SettingsTable : Panel //Table
			{

				/// <summary>
				/// Used to store reference to base object it, 
				/// can be ModuleID or Portal ID
				/// </summary>
				public int ObjectID = -1;

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
					// Jonathan - tabsupport
					if(((UI.Page)this.Page).IsClientScriptRegistered("x_core") == false)
					{
						((UI.Page)this.Page).RegisterClientScript("x_core", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_core.js"));
					}
					if(((UI.Page)this.Page).IsClientScriptRegistered("x_event") == false)
					{
						((UI.Page)this.Page).RegisterClientScript("x_event", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_event.js"));
					}
					if(((UI.Page)this.Page).IsClientScriptRegistered("x_dom") == false)
					{
						((UI.Page)this.Page).RegisterClientScript("x_dom", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_dom.js"));
					}
					if(((UI.Page)this.Page).IsClientScriptRegistered("tabs_js") == false)
					{
						((UI.Page)this.Page).RegisterClientScript("tabs_js", Path.WebPathCombine (Path.ApplicationRoot,  "/aspnet_client/x/x_tpg.js"));
					}
					// End tab support
			
					//this.Rows.Clear();
					//temp
					//			this.Attributes.Add("border","1");
					//			this.Width = new Unit("100%");
					this.CssClass = "settings-table";

					if(Settings != null)
					{

						HtmlGenericControl tabPanelGroup = new HtmlGenericControl("div");
						tabPanelGroup.Attributes.Add("id", "tpg1");
						tabPanelGroup.Attributes.Add("class", "tabPanelGroup");

						HtmlGenericControl tabGroup = new HtmlGenericControl("div");
						tabGroup.Attributes.Add("class", "tabGroup");
					
						//tabPanelGroup.Attributes.Add("class", "tabDefault");

						HtmlGenericControl tabPanel = new HtmlGenericControl("div");
						tabPanel.Attributes.Add("class", "tabPanel");

						HtmlGenericControl tabDefault = new HtmlGenericControl("div");
						tabDefault.Attributes.Add("class", "tabDefault");

						if(Settings.GetKeyList() != null)
						{
							// Jes1111 -- force the list to obey SettingItem.Order property and divide it into groups
							// Manu -- a better order system avoiding try and catch.
							//         Now settings with no order have a progressive order number 
							//         based on their position on list
							SortedList SettingsOrder = new SortedList();
							int order = 0;

							foreach(string key in Settings.GetKeyList())
							{

								if(Settings[key]!= null)
								{

									if(Settings[key] is SettingItem)
									{
										order = ((SettingItem)Settings[key]).Order;

										while (SettingsOrder.ContainsKey(order))
											order++; //be sure do not have duplicate order key or we get an error
										SettingsOrder.Add(order, key);
									}

									else
									{
										//LogHelper.Logger.Log(LogLevel.Debug, "Unexpected '" + Settings[key].GetType().FullName + "' in settings table.");
										ErrorHandler.Publish(LogLevel.Debug, "Unexpected '" + Settings[key].GetType().FullName + "' in settings table.");
									}
								}
							}
							HtmlGenericControl _fieldset = new HtmlGenericControl("dummy");
							HtmlGenericControl _legend = new HtmlGenericControl("dummy");
							//HtmlGenericControl h4  = new HtmlGenericControl("h4");
	
							Table _tbl = new Table();
							TableRow tr = new TableRow();
							TableCell cHelp = new TableCell();
							TableCell c1 = new TableCell();
							TableCell c2 = new TableCell();
							Esperantus.WebControls.Literal myGroupControl = new Esperantus.WebControls.Literal();
							Esperantus.WebControls.Literal myControl = new Esperantus.WebControls.Literal();
							Esperantus.WebControls.Image img = new Esperantus.WebControls.Image();
							//Image img = new Image();
							Control editControl = new Control();
							//string _groupClass = string.Empty;
							// Initialize controls
							SettingItemGroup currentGroup = SettingItemGroup.NONE;

							tabPanelGroup.Controls.Add(tabGroup);

							foreach(string currentSetting in SettingsOrder.GetValueList())
							{
								SettingItem currentItem = (SettingItem) Settings[currentSetting];

								if(tabDefault.InnerText.Length == 0)
								{
									tabDefault = new HtmlGenericControl("div");
									tabDefault.Attributes.Add("class", "tabDefault");
									tabDefault.InnerText = Localize.GetString(currentItem.Group.ToString());
								}

								if (currentItem.Group != currentGroup)
								{

									if ( _fieldset.Attributes.Count > 0 ) // add built fieldset
									{
										_fieldset.Controls.Add(_tbl);
										//this.Controls.Add(_fieldset);
								
										tabPanel.Controls.Add(_fieldset);
										tabPanelGroup.Controls.Add(tabPanel);
										tabGroup.Controls.Add(tabDefault);
								
									}
									// start a new fieldset
									_fieldset = new HtmlGenericControl("fieldset");
									_fieldset.Attributes.Add("class", string.Concat("SettingsTableGroup ", currentItem.Group.ToString().ToLower()));
									// get group legend text
									myGroupControl = new Esperantus.WebControls.Literal();
									myGroupControl.TextKey = currentItem.Group.ToString();
									myGroupControl.Text = currentItem.GroupDescription;

									tabPanel = new HtmlGenericControl("div");
									tabPanel.Attributes.Add("class", "tabPanel");
							
									tabDefault = new HtmlGenericControl("div");
									tabDefault.Attributes.Add("class", "tabDefault");
									tabDefault.InnerText = Localize.GetString(currentItem.Group.ToString());
							
									//h4 = new HtmlGenericControl("h4");
									//h4.InnerText = currentItem.GroupDescription;
									//tabPanel.Controls.Add(h4);

							
									// add group legend
									_legend = new HtmlGenericControl("legend");
									_legend.Attributes.Add("class","SubSubHead");
									_legend.Controls.Add(myGroupControl);
									_fieldset.Controls.Add(_legend);
									// start a new table
									_tbl = new Table();
									_tbl.Attributes.Add("class","SettingsTableGroup");
									_tbl.Attributes.Add("width","100%");
									currentGroup = currentItem.Group;
								}
								// start new row
								tr = new TableRow();
								// Help cell
								cHelp = new TableCell();

								if(currentItem.Description.Length > 0)
								{
									Image _myImg = ((UI.Page)this.Page).CurrentTheme.GetImage("Buttons_Help","Help.gif");
									img = new Esperantus.WebControls.Image();
									img.ImageUrl = _myImg.ImageUrl;
									img.Height = _myImg.Height;
									img.Width = _myImg.Width;
									// Jminond: added netscape tooltip support
									img.AlternateText = currentItem.Description;
									img.Attributes.Add("title", Localize.GetString(currentSetting + "_DESCRIPTION"));
									img.TextKey = currentSetting + "_DESCRIPTION"; //Fixed key for simplicity
								}

								else
								{
									// Jes1111 - 17/12/2004
									img = new Esperantus.WebControls.Image();
									img.Width = Unit.Pixel(25);
									img.ImageUrl =  ((Page)this.Page).CurrentTheme.GetImage("Spacer","Spacer.gif").ImageUrl;
								}

								cHelp.Controls.Add(img);
								// add to row
								tr.Controls.Add(cHelp);
								// Setting Name cell
								c1 = new TableCell();
								c1.Attributes.Add("width","20%");
								c1.CssClass = "SubHead";
								//c1.Wrap = false; //?
								myControl = new Esperantus.WebControls.Literal();
								myControl.TextKey = currentSetting;
								myControl.Text = currentItem.EnglishName;
								c1.Controls.Add(myControl);
								// add to row
								tr.Controls.Add(c1);
								// Setting Control cell
								c2 = new TableCell();
								c2.Attributes.Add("width","80%");
								c2.CssClass = "st-control";

								try
								{
									editControl = currentItem.EditControl;
									editControl.ID = currentSetting; // Jes1111
									editControl.EnableViewState = true;
								}

								catch(Exception ex)
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
								if (currentItem.Required && !(editControl is EasyListBox))
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

									switch(currentItem.DataType)
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

										if(currentItem.MaxValue == 0)
										{
											rang.ErrorMessage = Localize.GetString("SETTING_EQUAL_OR_GREATER", "%1% must be equal or greater than %2%!", rang).Replace("%1%", currentSetting).Replace("%2%", currentItem.MinValue.ToString());
										}

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
							//_fieldset.Controls.Add(_tbl);
							//this.Controls.Add(_fieldset);
							tabGroup.Controls.Add(tabDefault);

							_fieldset.Controls.Add(_tbl);
							//this.Controls.Add(_fieldset);
								
							tabPanel.Controls.Add(_fieldset);
							tabPanelGroup.Controls.Add(tabPanel);

							this.Controls.AddAt(0, tabPanelGroup);
						}
					}
				}

				/// <summary>
				/// UpdateControls
				/// </summary>
				public void UpdateControls()
				{

					foreach(string key in EditControls.Keys)
					{
						Control c = (Control) EditControls[key];
						SettingItem currentItem = (SettingItem) Settings[c.ID];
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
					get
					{
						return Settings;
					}
					set
					{
						Settings = (SortedList) value;
					}        
				}
			}
		*/
		#endregion
	}

	#endregion
}