using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;
using Rainbow.DesktopModules;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// Status of order
	/// </summary>   
	public enum OrderStatus 
	{
		ToBeCompleted = -1,
		SuccessfullyCompleted = 0,
		Failed = 10
	}
}