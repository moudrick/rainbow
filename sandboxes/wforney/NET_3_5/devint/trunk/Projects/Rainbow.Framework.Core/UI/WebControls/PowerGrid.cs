namespace Rainbow.Framework.Web.UI.WebControls
{
    using System.Drawing;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Power Grid
    /// </summary>
    public class PowerGrid : DataGrid
    {
        #region Constants and Fields

        /// <summary>
        /// The lbl footer.
        /// </summary>
        protected Label FooterLabel;

        /// <summary>
        /// The m_ pager current page css class.
        /// </summary>
        private string pagerCurrentPageCssClass = string.Empty;

        /// <summary>
        /// The m_ pager other page css class.
        /// </summary>
        private string pagerOtherPageCssClass = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "PowerGrid" /> class.
        /// </summary>
        public PowerGrid()
        {
            this.FooterLabel = new Label();
            this.PagerStyle.Mode = PagerMode.NumericPages;
            this.PagerStyle.BackColor = Color.Gainsboro;
            this.PagerStyle.PageButtonCount = 10;
            this.PagerStyle.HorizontalAlign = HorizontalAlign.Center;
            this.FooterStyle.BackColor = Color.Gainsboro;
            this.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
            this.ShowFooter = true;
            this.AutoGenerateColumns = false;
            this.AllowPaging = true;
            this.PageSize = 7;
            this.CellSpacing = 2;
            this.CellPadding = 2;
            this.GridLines = GridLines.None;
            this.BorderColor = Color.Black;
            this.BorderStyle = BorderStyle.Solid;
            this.BorderWidth = 1;
            this.ForeColor = Color.Black;
            this.Font.Size = FontUnit.XXSmall;
            this.Font.Name = "Verdana";
            this.ItemStyle.BackColor = Color.Beige;
            this.AlternatingItemStyle.BackColor = Color.PaleGoldenrod;
            this.HeaderStyle.Font.Bold = true;
            this.HeaderStyle.BackColor = Color.Brown;
            this.HeaderStyle.ForeColor = Color.White;
            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.AllowSorting = true;
            this.Attributes["SortedAscending"] = "yes";
            this.ItemCreated += this.OnItemCreated;
            this.SortCommand += this.OnSortCommand;
            this.PageIndexChanged += this.OnPageIndexChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the footer text.
        /// </summary>
        /// <value>The footer text.</value>
        public string FooterText
        {
            get
            {
                return this.FooterLabel.Text;
            }

            set
            {
                this.FooterLabel.Text = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is sorted ascending.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is sorted ascending; otherwise, <c>false</c>.
        /// </value>
        public bool IsSortedAscending
        {
            get
            {
                return this.Attributes["SortedAscending"].Equals("yes");
            }

            set
            {
                if (!value)
                {
                    this.Attributes["SortedAscending"] = "no";
                    return;
                }

                this.Attributes["SortedAscending"] = "yes";
            }
        }

        /// <summary>
        ///     Gets or sets the pager current page CSS class.
        /// </summary>
        /// <value>The pager current page CSS class.</value>
        public string PagerCurrentPageCssClass
        {
            get
            {
                return this.pagerCurrentPageCssClass;
            }

            set
            {
                this.pagerCurrentPageCssClass = value;
            }
        }

        /// <summary>
        ///     Gets or sets the pager other page CSS class.
        /// </summary>
        /// <value>The pager other page CSS class.</value>
        public string PagerOtherPageCssClass
        {
            get
            {
                return this.pagerOtherPageCssClass;
            }

            set
            {
                this.pagerOtherPageCssClass = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sort expression.
        /// </summary>
        /// <value>The sort expression.</value>
        public string SortExpression
        {
            get
            {
                return this.Attributes["SortExpression"];
            }

            set
            {
                this.Attributes["SortExpression"] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when [item created].
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.
        /// </param>
        public void OnItemCreated(object sender, DataGridItemEventArgs e)
        {
            var listItemType = e.Item.ItemType;
            if (listItemType == ListItemType.Footer)
            {
                var i1 = e.Item.Cells.Count;
                for (var j = i1 - 1; j > 0; j--)
                {
                    e.Item.Cells.RemoveAt(j);
                }

                e.Item.Cells[0].ColumnSpan = i1;
                e.Item.Cells[0].Controls.Add(this.FooterLabel);
            }

            if (listItemType == ListItemType.Pager)
            {
                var tableCell1 = (TableCell)e.Item.Controls[0];
                for (var k = 0; k < tableCell1.Controls.Count; k += 2)
                {
                    try
                    {
                        var linkButton = (LinkButton)tableCell1.Controls[k];
                        linkButton.Text = string.Concat("[ ", linkButton.Text, " ]");
                        linkButton.CssClass = this.pagerOtherPageCssClass;
                    }
                    catch
                    {
                        var label1 = (Label)tableCell1.Controls[k];
                        label1.Text = string.Concat("Page ", label1.Text);
                        if (this.pagerCurrentPageCssClass.Equals(string.Empty))
                        {
                            label1.ForeColor = Color.Blue;
                            label1.Font.Bold = true;
                        }
                        else
                        {
                            label1.CssClass = this.pagerCurrentPageCssClass;
                        }
                    }
                }
            }

            // if (listItemType == ListItemType.Item)
            if (listItemType == ListItemType.Header)
            {
                var str1 = this.Attributes["SortExpression"];
                var str2 = !this.Attributes["SortedAscending"].Equals("yes") ? " 6" : " 5";
                for (var i2 = 0; i2 < this.Columns.Count; i2++)
                {
                    if (str1 == this.Columns[i2].SortExpression)
                    {
                        var tableCell2 = e.Item.Cells[i2];
                        var label2 = new Label();
                        label2.Font.Name = "webdings";
                        label2.Font.Size = FontUnit.XXSmall;
                        label2.Text = str2;
                        tableCell2.Controls.Add(label2);
                    }
                }
            }
        }

        /// <summary>
        /// Called when [page index changed].
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.
        /// </param>
        public void OnPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            this.CurrentPageIndex = e.NewPageIndex;
        }

        /// <summary>
        /// Called when [sort command].
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.DataGridSortCommandEventArgs"/> instance containing the event data.
        /// </param>
        public void OnSortCommand(object sender, DataGridSortCommandEventArgs e)
        {
            string sortExpression = this.Attributes["SortExpression"];
            string sortedAscending = this.Attributes["SortedAscending"];

            this.Attributes["SortExpression"] = e.SortExpression;
            this.Attributes["SortedAscending"] = "yes";

            if (e.SortExpression == sortExpression)
            {
                if (sortedAscending == "yes")
                {
                    this.Attributes["SortedAscending"] = "no";
                }
            }
        }

        /// <summary>
        /// Sets the GFW styles.
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

        #endregion
    }
}