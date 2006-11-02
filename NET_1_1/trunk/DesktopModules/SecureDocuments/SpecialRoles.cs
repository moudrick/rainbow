using System;
using System.ComponentModel;        
using System.Reflection;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// Summary description for SpecialRoles.
	/// </summary>
	public class SpecialRoles
	{
		/// <summary>
		/// Special roles used by the Rainbow system with arbitrary
		/// value for their ID by Brian Kierstead 4/15/2005
		/// </summary>
		public enum SpecialPortalRoles
		{
			[Description("All Users")] AllUsers = -1,
			[Description("Authenticated Users")] AuthenticatedUsers = -2,
			[Description("Unauthenticated Users")] UnauthenticatedUsers = -3
		}

		/// <summary>
		/// Add the special roles found in SpecialPortalRoles
		/// </summary>
		/// <param name="listRoles"></param>
		public static void populateSpecialRoles(ref System.Web.UI.WebControls.CheckBoxList listRoles)
		{
			foreach(string s in Enum.GetNames(typeof(SpecialPortalRoles)))
			{
				SpecialPortalRoles desc = (SpecialPortalRoles)Enum.Parse(typeof(SpecialPortalRoles), s);
				string stringDesc = GetDescription(desc);
				listRoles.Items.Add(new System.Web.UI.WebControls.ListItem
					(stringDesc, ((int)Enum.Parse(typeof(SpecialPortalRoles), s)).ToString() ));
			}
		}

		/// <summary>
		/// Retrieve the description tag from the enum
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetDescription(Enum value)
		{
			FieldInfo fi= value.GetType().GetField(value.ToString()); 
			DescriptionAttribute[] attributes = 
				(DescriptionAttribute[])fi.GetCustomAttributes(
				typeof(DescriptionAttribute), false);
			return (attributes.Length>0)?attributes[0].Description:value.ToString();
		}

		/// <summary>
		/// Return the description for the enum entry with value index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string GetRoleName(int index)
		{
			string s = Enum.GetName(typeof(SpecialPortalRoles), index);
			SpecialPortalRoles desc = (SpecialPortalRoles)Enum.Parse(typeof(SpecialPortalRoles), s);
			return GetDescription(desc);
		}
	}
}
