namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// Structure used for list settings
	/// </summary>
	public struct SettingOption
	{
		private int val;
		private string name;

		public int Val
		{
			get { return this.val; }
			set { this.val = value; }
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public SettingOption(int aVal, string aName)
		{
			val = aVal;
			name = aName;
		}
	}
}