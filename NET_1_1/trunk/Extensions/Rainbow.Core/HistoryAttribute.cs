using System;

namespace Rainbow
{
	/// <summary>
	/// HistoryAttribute can be used to track code authors and modification
	/// </summary>
	[
		AttributeUsage(
			AttributeTargets.All,
			Inherited = false,
			AllowMultiple = true
			)
		]
	public sealed class HistoryAttribute : Attribute
	{
		/// <summary>
		/// Requires all parameters
		/// </summary>
		/// <param name="name">name</param>
		/// <param name="email">email</param>
		/// <param name="version">version</param>
		/// <param name="date">date</param>
		/// <param name="comment">comment</param>
		public HistoryAttribute(string name, string email, string version, string date, string comment)
		{
			this.Name = name;
			this.Email = email;
			this.Version = version;
			this.Date = DateTime.Parse(date);
			this.Comment = comment;
		}

		/// <summary>
		/// Requires name, date and comment
		/// </summary>
		/// <param name="name">name</param>
		/// <param name="date">date</param>
		/// <param name="comment">comment</param>
		public HistoryAttribute(string name, string date, string comment)
		{
			this.Name = name;
			try
			{
				this.Date = DateTime.Parse(date);
			}
			catch (FormatException ex)
			{
				throw new ApplicationException("'" + date + "' is an invalid date", ex);
			}
			this.Comment = comment;
		}

		/// <summary>
		/// Requires name, date and comment
		/// </summary>
		/// <param name="name">name</param>
		/// <param name="date">date</param>
		public HistoryAttribute(string name, string date)
		{
			this.Name = name;
			this.Date = DateTime.Parse(date);
		}

		private string name;

		/// <summary>
		/// The Author name
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}


		private string email;

		/// <summary>
		/// The Email of the Author
		/// </summary>
		public string Email
		{
			get { return email; }
			set { email = value; }
		}


		private string version;

		/// <summary>
		/// Modification Version
		/// </summary>
		public string Version
		{
			get { return version; }
			set { version = value; }
		}


		private DateTime date;

		/// <summary>
		/// Modification date
		/// </summary>
		public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}


		private string comment;

		/// <summary>
		/// Modification description
		/// </summary>
		public string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

	}
}