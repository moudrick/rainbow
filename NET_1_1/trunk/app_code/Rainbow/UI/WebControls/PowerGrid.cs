using System.Drawing;
using System.Web.UI.WebControls;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// Power Grid
	/// </summary>
	public class PowerGrid : DataGrid
	{
		/// <summary>
		/// 
		/// </summary>
		protected Label lblFooter;

		private string m_PagerCurrentPageCssClass = string.Empty;

		private string m_PagerOtherPageCssClass = string.Empty;

		#region Public Properties
		/// <summary>
		/// 
		/// </summary>
		public string SortExpression
		{
			get
			{
				return base.Attributes["SortExpression"];
			}

			set
			{
				base.Attributes["SortExpression"] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsSortedAscending
		{
			get
			{
				return base.Attributes["SortedAscending"].Equals("yes");
			}

			set
			{
				if (!value)
				{
					base.Attributes["SortedAscending"] = "no";
					return;
				}
				base.Attributes["SortedAscending"] = "yes";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string PagerCurrentPageCssClass
		{
			get
			{
				return m_PagerCurrentPageCssClass;
			}

			set
			{
				m_PagerCurrentPageCssClass = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PagerOtherPageCssClass
		{
			get
			{
				return m_PagerOtherPageCssClass;
			}

			set
			{
				m_PagerOtherPageCssClass = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string FooterText
		{
			get
			{
				return this.lblFooter.Text;
			}

			set
			{
				this.lblFooter.Text = value;
			}
		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		public PowerGrid()
		{
			lblFooter = new Label();
			base.PagerStyle.Mode = PagerMode.NumericPages;
			base.PagerStyle.BackColor = Color.Gainsboro;
			base.PagerStyle.PageButtonCount = 10;
			base.PagerStyle.HorizontalAlign = HorizontalAlign.Center;
			base.FooterStyle.BackColor = Color.Gainsboro;
			base.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
			base.ShowFooter = true;
			base.AutoGenerateColumns = false;
			base.AllowPaging = true;
			base.PageSize = 7;
			base.CellSpacing = 2;
			base.CellPadding = 2;
			base.GridLines = GridLines.None;
			base.BorderColor = Color.Black;
			base.BorderStyle = BorderStyle.Solid;
			base.BorderWidth = 1;
			base.ForeColor = Color.Black;
			base.Font.Size = FontUnit.XXSmall;
			base.Font.Name = "Verdana";
			base.ItemStyle.BackColor = Color.Beige;
			base.AlternatingItemStyle.BackColor = Color.PaleGoldenrod;
			base.HeaderStyle.Font.Bold = true;
			base.HeaderStyle.BackColor = Color.Brown;
			base.HeaderStyle.ForeColor = Color.White;
			base.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
			base.AllowSorting = true;
			base.Attributes["SortedAscending"] = "yes";
			base.ItemCreated += new DataGridItemEventHandler(this.OnItemCreated);
			base.SortCommand += new DataGridSortCommandEventHandler(this.OnSortCommand);
			base.PageIndexChanged += new DataGridPageChangedEventHandler(this.OnPageIndexChanged);
		}
		/// <summary>
		/// 
		/// </summary>
		public void OnItemCreated(object sender, DataGridItemEventArgs e)
		{
			ListItemType listItemType = e.Item.ItemType;
			if (listItemType == ListItemType.Footer)
			{
				int i1 = e.Item.Cells.Count;
				for (int j = i1 - 1; j > 0; j--)
				{
					e.Item.Cells.RemoveAt(j);
				}
				e.Item.Cells[0].ColumnSpan = i1;
				e.Item.Cells[0].Controls.Add(lblFooter);
			}
			if (listItemType == ListItemType.Pager)
			{
				TableCell tableCell1 = (TableCell)e.Item.Controls[0];
				for (int k = 0; k < tableCell1.Controls.Count; k += 2)
				{
					try
					{
						LinkButton linkButton = (LinkButton)tableCell1.Controls[k];
						linkButton.Text = string.Concat("[ ", linkButton.Text, " ]");
						linkButton.CssClass = m_PagerOtherPageCssClass;
					}
					catch
					{
						Label label1 = (Label)tableCell1.Controls[k];
						label1.Text = string.Concat("Page ", label1.Text);
						if (m_PagerCurrentPageCssClass.Equals(string.Empty))
						{
							label1.ForeColor = Color.Blue;
							label1.Font.Bold = true;
						}
						else
						{
							label1.CssClass = m_PagerCurrentPageCssClass;
						}
					}
				}
			}
			//if (listItemType == ListItemType.Item)
			if (listItemType == ListItemType.Header)
			{
				string str1 = base.Attributes["SortExpression"];
				string str2 = !base.Attributes["SortedAscending"].Equals("yes") ? " 6" : " 5";
				for (int i2 = 0; i2 < base.Columns.Count; i2++)
				{
					if (str1 == base.Columns[i2].SortExpression)
					{
						TableCell tableCell2 = e.Item.Cells[i2];
						Label label2 = new Label();
						label2.Font.Name = "webdings";
						label2.Font.Size = FontUnit.XXSmall;
						label2.Text = str2;
						tableCell2.Controls.Add(label2);
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void OnSortCommand(object sender, DataGridSortCommandEventArgs e)
		{
			string _SortExpression;
			string _SortedAscending;

			_SortExpression = this.Attributes["SortExpression"];
			_SortedAscending = this.Attributes["SortedAscending"];

			this.Attributes["SortExpression"] = e.SortExpression;
			this.Attributes["SortedAscending"] = "yes";

			if (e.SortExpression == _SortExpression) 
			{
				if (_SortedAscending == "yes")
					this.Attributes["SortedAscending"] = "no";
			}		
		}
		/// <summary>
		/// 
		/// </summary>
		public void OnPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
		{
			this.CurrentPageIndex = e.NewPageIndex;
		}
		/// <summary>
		/// 
		/// </summary>
		public void SetGfwStyles()
		{
			this.CellPadding = 3;
			this.CellSpacing = 0;
			this.BorderColor = Color.Black;
			this.BackColor = Color.WhiteSmoke;
			this.ForeColor = Color.Black;
			this.GridLines = GridLines.Both;
			this.ItemStyle.BackColor = Color.WhiteSmoke;
			this.ItemStyle.VerticalAlign = VerticalAlign.Top;
			this.AlternatingItemStyle.BackColor = Color.LightGray;
			this.AlternatingItemStyle.VerticalAlign = VerticalAlign.Top;
			this.HeaderStyle.ForeColor = Color.Black;
			this.HeaderStyle.Font.Bold = true;
			this.HeaderStyle.BackColor = Color.LightGray;
		}	
	}

}
