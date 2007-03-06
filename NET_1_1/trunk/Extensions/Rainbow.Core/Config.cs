using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using Microsoft.ApplicationBlocks.ConfigurationManagement;

namespace Rainbow.Core
{
	/// <summary>
	/// Summary description for Configuration.
	/// </summary>
	[Serializable]
	public class Config : System.ComponentModel.Component
	{
		/// <summary>
		///     ConfigData Hashtable
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public Hashtable ConfigData
		{
			get
			{
				return _configData;
			}
			set
			{
				_configData = value; 
			}
		}

		private Hashtable _configData;

		#region Component Model / Instance stuff
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		///     
		/// </summary>
		/// <param name="container" type="System.ComponentModel.IContainer">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="configSection" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public Config(System.ComponentModel.IContainer container,string configSection)
		{
			container.Add(this);
			InitializeComponent();
			_configData = ConfigHelper.Read(configSection);
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="container" type="System.ComponentModel.IContainer">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public Config(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_configData = ConfigHelper.Read();
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="configSection" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public Config(string configSection)
		{
			InitializeComponent();
			_configData = ConfigHelper.Read(configSection);
		}
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public Config()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_configData = ConfigHelper.Read();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			_configData = null;

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
	}
	
	sealed public class ConfigHelper
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private ConfigHelper() {}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public static Hashtable Read()
		{
			return Read(string.Empty);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="sectionName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public static Hashtable Read(string sectionName) 
		{
			Hashtable configData = null;

			try
			{
				if(sectionName != null && sectionName.Length > 0) 
				{
					// Read the configuration section named RainbowConfig
					configData = (Hashtable)ConfigurationManager.Read(sectionName);
				} 
				else 
				{
					// Read the default configuration section (must be Hashtable-based)
					configData = (Hashtable)ConfigurationManager.Read();
				}

				if( configData == null )
				{
					configData = new Hashtable();
				}

				return configData;
			}
			catch( Exception ex )
			{
				string message = string.Empty;
				for( Exception tempException = ex; tempException != null; tempException = tempException.InnerException )
				{
					message += tempException.Message + Environment.NewLine + "----------" + Environment.NewLine;
				}

				throw new Exception(message);
			}
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="section" type="System.Collections.Hashtable">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void Write(Hashtable section)
		{
			Write(section,string.Empty);
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="section" type="System.Collections.Hashtable">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="sectionName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void Write(Hashtable section,string sectionName)
		{
			try 
			{
				if(section != null) 
				{
					if(sectionName != null && sectionName.Length > 0)
					{
						// Write the hashtable to configuration section named section
						ConfigurationManager.Write(sectionName,section);
					} 
					else 
					{
						// Write the default configuration section (must be Hashtable-based)
						ConfigurationManager.Write(section);
					}
				}
			}
			catch( Exception ex )
			{
				string message = string.Empty;
				for( Exception tempException = ex; tempException != null; tempException = tempException.InnerException )
				{
					message += tempException.Message + Environment.NewLine + "----------" + Environment.NewLine;
				}

				throw new Exception(message);
			}
		}
	}
}
