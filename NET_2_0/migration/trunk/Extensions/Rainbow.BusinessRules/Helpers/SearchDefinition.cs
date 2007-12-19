using System;
using System.Collections;
using System.Text;

namespace Rainbow.Helpers
{
	/// <summary>
	/// This struct stores custom parametes needed by
	/// the search helper for do the search string.
	/// This make the search string consistent and easy
	/// to change without modify all the searchable modules
	/// </summary>
	public struct SearchDefinition
	{
		private const string strItm = "itm.";

		/// <summary>
		///     
		/// </summary>
		/// <param name="tableName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="titleField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="abstractField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="searchField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public SearchDefinition(string tableName, string titleField, string abstractField, string searchField)
		{
			TableName = tableName;
			TabIDField = "mod.TabID";
			ItemIDField = "ItemID";
			TitleField = titleField;
			AbstractField = abstractField;
			CreatedByUserField = "''";
			CreatedDateField = "''";
			ArrSearchFields = new ArrayList();

			if (searchField == string.Empty)
			{
				ArrSearchFields.Add(strItm + TitleField);
				ArrSearchFields.Add(strItm + AbstractField);
			}

			else
			{
				if (searchField == "Title")
					ArrSearchFields.Add(strItm + TitleField);

				else
					ArrSearchFields.Add(strItm + searchField);
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="tableName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="titleField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="abstractField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="createdByUserField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="createdDateField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="searchField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public SearchDefinition(string tableName, string titleField, string abstractField, string createdByUserField, string createdDateField, string searchField)
		{
			TableName = tableName;
			TabIDField = "mod.TabID";
			ItemIDField = "ItemID";
			TitleField = titleField;
			AbstractField = abstractField;
			CreatedByUserField = createdByUserField;
			CreatedDateField = createdDateField;
			ArrSearchFields = new ArrayList();

			if (searchField == string.Empty)
			{
				ArrSearchFields.Add(strItm + TitleField);
				ArrSearchFields.Add(strItm + AbstractField);
			}

			else
			{
				if (searchField == "Title")
					ArrSearchFields.Add(strItm + TitleField);

				else
					ArrSearchFields.Add(strItm + searchField);
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="tableName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="tabIDField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="itemIDField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="titleField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="abstractField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="createdByUserField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="createdDateField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="searchField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public SearchDefinition(string tableName, string tabIDField, string itemIDField, string titleField, string abstractField, string createdByUserField, string createdDateField, string searchField)
		{
			TableName = tableName;
			TabIDField = tabIDField;
			ItemIDField = itemIDField;
			TitleField = titleField;
			AbstractField = abstractField;
			CreatedByUserField = createdByUserField;
			CreatedDateField = createdDateField;
			ArrSearchFields = new ArrayList();

			if (searchField == string.Empty)
			{
				ArrSearchFields.Add(strItm + TitleField);
				ArrSearchFields.Add(strItm + AbstractField);
			}

			else
			{
				if (searchField == "Title")
					ArrSearchFields.Add(strItm + TitleField);

				else
					ArrSearchFields.Add(strItm + searchField);
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string TableName;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string TabIDField;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string ItemIDField;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string TitleField;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string AbstractField;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string CreatedByUserField;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string CreatedDateField;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public ArrayList ArrSearchFields;

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="userID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="searchString" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string SearchSqlSelect(int portalID, int userID, string searchString)
		{
			return SearchSqlSelect(portalID, userID, searchString, true);
		}

		/// <summary>
		/// SQL injection prevention
		/// </summary>
		/// <param name="toClean"></param>
		/// <returns></returns>
		private string FilterString(string toClean)
		{
			StringBuilder c = new StringBuilder(toClean);
			string[] knownbad =
				{
					"select", "insert",
					"update", "delete", "drop",
					"--", "'", "char", ";"
				};

			for (int i = 0; i < knownbad.Length; i++)
			{
				c.Replace(knownbad[i], string.Empty);
			}
			return c.ToString();
		}

		/// <summary>
		/// Builds a SELECT query using given parameters
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="userID"></param>
		/// <param name="searchString"></param>
		/// <param name="hasItemID"></param>
		/// <returns></returns>
		public string SearchSqlSelect(int portalID, int userID, string searchString, bool hasItemID)
		{
			if (CreatedByUserField == null || CreatedByUserField.Length == 0)
				CreatedByUserField = "''";

			if (CreatedDateField == null || CreatedDateField.Length == 0)
				CreatedDateField = "''";
			//SQL inection filter
			searchString = FilterString(searchString);

			if (searchString.Length < 3)
				throw new ArgumentException("Please use a word with at least 3 valid chars (invalid chars were removed).");
			// special extended search feature (used by RSS/Community Service). Added by Jakob Hansen
			string ExtraSQL = string.Empty;

			if (searchString.StartsWith("AddExtraSQL:"))
			{
				int posSS = searchString.IndexOf("SearchString:");

				if (posSS > 0)
				{
					// Get the added searchstring
					if (posSS > 12)
						ExtraSQL = searchString.Substring(12, posSS - 12).Trim();

					else
						ExtraSQL = string.Empty; // no SQL - only searchstring
					searchString = searchString.Substring(posSS + 14).Trim();
				}

				else
				{
					// There are no added searchstring
					ExtraSQL = searchString.Substring(12).Trim();
					searchString = string.Empty;
				}

				// Are the required "AND " missing? (then add it!)
				if (ExtraSQL != string.Empty && !ExtraSQL.StartsWith("AND"))
					ExtraSQL = "AND " + ExtraSQL;
			}
			StringBuilder select = new StringBuilder();
			select.Append("SELECT TOP 50 ");
			select.Append("genModDef.FriendlyName AS ModuleName, ");
			select.Append("CAST (itm.");
			select.Append(TitleField);
			select.Append(" AS NVARCHAR(100)) AS Title, ");
			select.Append("CAST (itm.");
			select.Append(AbstractField);
			select.Append(" AS NVARCHAR(100)) AS Abstract, ");
			select.Append("itm.ModuleID AS ModuleID, ");

			if (hasItemID)
				select.Append(strItm + ItemIDField + " AS ItemID, ");

			else
				select.Append("itm.ModuleID AS ItemID, ");

			if (!CreatedByUserField.StartsWith("'"))
				select.Append(strItm); // Add itm only if not a constant value
			select.Append(CreatedByUserField);
			select.Append(" AS CreatedByUser, ");

			if (!CreatedDateField.StartsWith("'"))
				select.Append(strItm); // Add itm only if not a constant value
			select.Append(CreatedDateField);
			select.Append(" AS CreatedDate, ");
			select.Append(TabIDField + " AS TabID, ");
			select.Append("tab.TabName AS TabName, ");
			select.Append("genModDef.GeneralModDefID AS GeneralModDefID, ");
			select.Append("mod.ModuleTitle AS ModuleTitle ");
			select.Append("FROM ");
			select.Append(TableName);
			select.Append(" itm INNER JOIN ");
			select.Append("rb_Modules mod ON itm.ModuleID = mod.ModuleID INNER JOIN ");
			select.Append("rb_ModuleDefinitions modDef ON mod.ModuleDefID = modDef.ModuleDefID INNER JOIN ");
			select.Append("rb_Tabs tab ON mod.TabID = tab.TabID INNER JOIN ");
			select.Append("rb_GeneralModuleDefinitions genModDef ON modDef.GeneralModDefID = genModDef.GeneralModDefID ");
			//			if (topicName != string.Empty)
			//				select.Append("INNER JOIN rb_ModuleSettings modSet ON mod.ModuleID = modSet.ModuleID");
			select.Append("%TOPIC_PLACEHOLDER_JOIN%");
			SearchHelper.AddSharedSQL(portalID, userID, ref select, TitleField);
			//			if (topicName != string.Empty)
			//				select.Append(" AND (modSet.SettingName = 'TopicName' AND modSet.SettingValue='" + topicName + "')");
			select.Append("%TOPIC_PLACEHOLDER%");

			if (searchString != string.Empty)
				select.Append(" AND " + SearchHelper.CreateTestSQL(ArrSearchFields, searchString, true));

			if (ExtraSQL != string.Empty)
				select.Append(ExtraSQL);
			return select.ToString();
		}

		/* Jakob Hansen, 20 may: Before the RSS/Web Service community release

				/// <summary>
				/// Builds a SELECT query using given parameters
				/// </summary>
				/// <param name="portalID"></param>
				/// <param name="userID"></param>
				/// <param name="searchString"></param>
				/// <returns></returns>
				public string SearchSqlSelect(int portalID, int userID, string 
		searchString, bool hasItemID)
				{
					System.Text.StringBuilder select = new System.Text.StringBuilder();
					select.Append("SELECT ");
					select.Append("genModDef.FriendlyName AS ModuleName, ");
					select.Append("CAST (itm.");
					select.Append(TitleField);
					select.Append(" AS NVARCHAR(100)) AS Title, ");
					select.Append("CAST (itm.");
					select.Append(AbstractField);
					select.Append(" AS NVARCHAR(100)) AS Abstract, ");
					select.Append("itm.ModuleID AS ModuleID, ");

					if (hasItemID)
						select.Append("itm.ItemID AS ItemID, ");

					else
						select.Append("itm.ModuleID AS ItemID, ");

					if (!CreatedByUserField.StartsWith("'"))
						select.Append(strItm);   // Add itm only if not a constant value
					select.Append(CreatedByUserField);
					select.Append(" AS CreatedByUser, ");

					if (!CreatedDateField.StartsWith("'"))
						select.Append(strItm);   // Add itm only if not a constant value
					select.Append(CreatedDateField);
					select.Append(" AS CreatedDate, ");
					select.Append("mod.TabID AS TabID, ");
					select.Append("tab.TabName AS TabName, ");
					select.Append("genModDef.GeneralModDefID AS GeneralModDefID, ");
					select.Append("mod.ModuleTitle AS ModuleTitle ");
					select.Append("FROM ");
					select.Append(TableName);
					select.Append(" itm INNER JOIN ");
					select.Append("rb_Modules mod ON itm.ModuleID = mod.ModuleID INNER JOIN ");
					select.Append("rb_ModuleDefinitions modDef ON mod.ModuleDefID = modDef.ModuleDefID INNER JOIN ");
					select.Append("rb_Tabs tab ON mod.TabID = tab.TabID INNER JOIN ");
					select.Append("rb_GeneralModuleDefinitions genModDef ON modDef.GeneralModDefID = genModDef.GeneralModDefID ");
					Helpers.SearchHelper.AddSharedSQL(portalID, userID, ref select, TitleField);
					select.Append(" AND " + Rainbow.Helpers.SearchHelper.CreateTestSQL(ArrSearchFields, searchString, true));
					return select.ToString();
				}
		*/
	}
}