using System.ComponentModel;

namespace Rainbow.Core
{
	/// <summary>
	/// Summary description for User.
	/// </summary>
	public class User : Component
	{
		private int id;
		private int portalId;
		private string name;
		private string company;
		private string address;
		private string city;
		private string zip;
		private string countryId;
		private int stateId;
		private string phone;
		private string fax;
		private string password;
		private string email;
		private bool sendNewsletter;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public int PortalId
		{
			get { return portalId; }
			set { portalId = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Company
		{
			get { return company; }
			set { company = value; }
		}

		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		public string City
		{
			get { return city; }
			set { city = value; }
		}

		public string Zip
		{
			get { return zip; }
			set { zip = value; }
		}

		public string CountryId
		{
			get { return countryId; }
			set { countryId = value; }
		}

		public int StateId
		{
			get { return stateId; }
			set { stateId = value; }
		}

		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}

		public string Fax
		{
			get { return fax; }
			set { fax = value; }
		}

		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		public bool SendNewsletter
		{
			get { return sendNewsletter; }
			set { sendNewsletter = value; }
		}

		#region Component Model

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public User(IContainer container)
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

		public User()
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