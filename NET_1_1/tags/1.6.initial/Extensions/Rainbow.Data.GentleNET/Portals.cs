using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using Gentle.Framework;
using Rainbow.Core;

namespace Rainbow.Data.GentleNET
{
	/// <summary>
	/// Summary description for Portals.
	/// </summary>
	public class Portals : PortalsProvider
	{
		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     by Manu 16/10/2003
		///     Added 2 mods:
		///     1) Rbversion is created if it is missed.
		///        This is expecially good for empty databases.
		///        Be aware that this can break compatibility with 1613 version
		///     2) Connection problems are thown immediately as errors.
		/// </remarks>
		[Category("Data"),
			Description("Retrieves the database version"),
			DefaultValue(1110)]
		public int DatabaseVersion
		{
			get
			{
				try
				{
					//Create rbversion if it is missing
					string createRbVersions =
						"IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rb_Versions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)" +
							"CREATE TABLE [rb_Versions] (" +
							"[Release] [int] NOT NULL , " +
							"[Version] [nvarchar] (50) NULL , " +
							"[ReleaseDate] [datetime] NULL " +
							") ON [PRIMARY]"
						;
					Broker.Execute(createRbVersions);
				}
				catch (SqlException ex)
				{
					ErrorHandler.HandleException("If this fails most likely cannot connect to db or no permission", ex);
					//If this fails most likely cannot connect to db or no permission
					throw;
				}

				SqlResult sr = Broker.Execute("SELECT TOP 1 Release FROM rb_Versions ORDER BY Release DESC");

				return sr.ErrorCode == 0 && sr.RowsContained > 0
					? sr.GetInt(0, "Release")
					: 1110;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Rainbow.Core.Portal value...
		/// </returns>
		public Portal GetPortal(int portalId)
		{
			rb_Portals p = rb_Portals.Retrieve(portalId);
			if (p == null)
				return new Portal();
			else
			{
				using (Portal cp = new Portal())
				{
					cp.Id = p.PortalID;
					cp.Name = p.PortalName;
					cp.Alias = p.PortalAlias;
					cp.CustomSettings = Settings(p.PortalID);
					cp.Path = p.PortalPath;

					return cp;
				}
			}
		}

		public override Hashtable Settings(int portalId)
		{
			Hashtable ht = new Hashtable();
			rb_Portals p = rb_Portals.Retrieve(portalId);
			GentleList ps = (GentleList) p.referencedrb_PortalSettings();
			foreach (rb_PortalSettings ips in ps)
			{
				ht.Add(ips.SettingName, ips.SettingValue);
			}
			return ht;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="mySettingName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public override string SettingString(string mySettingName)
		{
			rb_Portals p = rb_Portals.Retrieve(Active.Portal.Id);
			GentleList ps = (GentleList) p.referencedrb_PortalSettings();
			rb_PortalSettings ll = (rb_PortalSettings) ps.Find(new Key(typeof (rb_PortalSettings), true, "SettingName", mySettingName));
			return ll.SettingValue;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="cp" type="Rainbow.Core.Portal">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="mySettingName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="mySettingValue" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public override void UpdateSettingString(Portal cp, string mySettingName, string mySettingValue)
		{
			rb_Portals p = rb_Portals.Retrieve(cp.Id);
			GentleList ps = (GentleList) p.referencedrb_PortalSettings();
			rb_PortalSettings ll = (rb_PortalSettings) ps.Find(new Key(typeof (rb_PortalSettings), true, "SettingName", mySettingName));
			if (ll == null) ll = new rb_PortalSettings(cp.Id, mySettingName, mySettingValue);
			else ll.SettingValue = mySettingValue;
			ll.Persist();
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public string LanguageList
		{
			get { return SettingString("SITESETTINGS_LANGLIST"); }
		}

		#region Component Module

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Portals(IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public Portals()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Rainbow.Core.Portal value...
		/// </returns>
		public override Portal Portal(int portalId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Get a portal by its alias
		/// </summary>
		/// <param name="portalAlias" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Portal value...
		/// </returns>
		public override Portal Portal(string portalAlias)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A int value...
		/// </returns>
		public override int GetDatabaseVersion()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public override string GetLanguageList()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="strLangList" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public override string SetLanguageList(string strLangList)
		{
			throw new NotImplementedException();
		}
	}
}