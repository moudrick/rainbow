namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// Structure used for list settings
	/// </summary>
	public struct SettingOption
	{
		private int val;
		private string name;

		/// <summary>
		/// 
		/// </summary>
		public int Val 
		{
			get { return this.val; }
			set { this.val = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aVal"></param>
		/// <param name="aName"></param>
		public SettingOption(int aVal, string aName)
		{
			val = aVal;
			name = aName;
		}
	}
}