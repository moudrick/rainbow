using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Web;

namespace Rainbow.BusinessRules
{
	/// <summary>
	/// Summary description for Globals.
	/// </summary>
	public class Globals : Component
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
		///     belongs in globals (still in business layer) but will leave it here until later
		/// </remarks>
		public static string ProductVersion
		{
			// TODO: Maybe make an Active class for business layer too and stick this in there
			get
			{
				//TODO: Find a way to still cache it, but not access the httpcontext since it's bad form...
				if (HttpContext.Current.Application["ProductVersion"] == null)
				{
					FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
					HttpContext.Current.Application.Lock();
					HttpContext.Current.Application["ProductVersion"] = f.ProductVersion;
					HttpContext.Current.Application.UnLock();
				}
				return HttpContext.Current.Application["ProductVersion"].ToString();
			}
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
		///     also belongs in globals but need to take a look at how this and above are used.  possibly get rid of one or the other.
		/// </remarks>
		public static int CodeVersion
		{
			get
			{
				if (HttpContext.Current.Application["CodeVersion"] == null)
				{
					FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
					HttpContext.Current.Application.Lock();
					HttpContext.Current.Application["CodeVersion"] = f.FilePrivatePart;
					HttpContext.Current.Application.UnLock();
				}
				return (int) HttpContext.Current.Application["CodeVersion"];
			}
		}

		#region Component Model

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Globals(IContainer container)
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

		public Globals()
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
	}
}