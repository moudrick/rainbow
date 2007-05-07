using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections;

using Rainbow.Configuration;


namespace Rainbow.Modules.Text
{
	/// <summary>
	/// Class that encapsulates all data logic necessary to query
	/// HTML/text within the Portal database.
	/// </summary>
    public class TextMobileDB 
    {
		/// <summary>
		/// The GetHtmlText method returns a SqlDataReader containing details
		/// about a specific item from the HtmlText database table.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		public IDataReader GetHtmlText(int moduleID) 
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = PortalSettings.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetHtmlText", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

			// Change by Geert.Audenaert@Syntegra.Com
			// Date: 6/2/2003
			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = (int)WorkFlowVersion.Production;
			myCommand.Parameters.Add(parameterWorkflowVersion);
			// End Change Geert.Audenaert@Syntegra.Com

            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			
            // Return the datareader 
            return result;
        }

    }
}
