namespace Rainbow.Framework.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Paging class, Rainbow special edition
    /// </summary>
    [DefaultProperty("PageNumber")]
    [ToolboxData("<{0}:Paging TextKey='' runat=server></{0}:Paging>")]
    [Designer("Rainbow.Framework.UI.Design.PagingDesigner")]
    public class Paging : WebControl, IPaging
    {
        #region Constants and Fields

        /// <summary>
        ///     Button 'First'
        /// </summary>
        protected Button btnFirst;

        /// <summary>
        ///     Button 'Last'
        /// </summary>
        protected Button btnLast;

        /// <summary>
        ///     Button 'Next'
        /// </summary>
        protected Button btnNext;

        /// <summary>
        ///     Button 'Previous'
        /// </summary>
        protected Button btnPrev;

        /// <summary>
        ///     Total page count
        /// </summary>
        protected Label lblPageCount;

        /// <summary>
        ///     Label containg text 'of'
        /// </summary>
        protected Label lblof;

        /// <summary>
        ///     Page number
        /// </summary>
        protected TextBox tbPageNumber;

        #endregion

        // variables we use to manage state
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "Paging" /> class. 
        ///     Main class manging pages
        /// </summary>
        public Paging()
        {
            this.RecordsPerPage = 10;

            // Construct controls
            this.btnFirst = new Button
                {
                   CommandArgument = "first", Width = new Unit("25px"), EnableViewState = false, Text = @" |< " 
                };
            this.btnFirst.Font.Bold = true;
            this.Controls.Add(this.btnFirst);

            this.btnPrev = new Button
                {
                   EnableViewState = false, Width = new Unit("25px"), Text = @" < ", CommandArgument = "prev" 
                };
            this.btnPrev.Font.Bold = true;
            this.Controls.Add(this.btnPrev);

            this.Controls.Add(new LiteralControl("&#160;"));

            this.tbPageNumber = new TextBox { AutoPostBack = true, Width = new Unit("30px"), EnableViewState = true };
            this.Controls.Add(this.tbPageNumber);

            this.Controls.Add(new LiteralControl("&#160;"));

            this.btnNext = new Button
                {
                   Width = new Unit("25px"), EnableViewState = false, CommandArgument = "next", Text = @" > " 
                };
            this.btnNext.Font.Bold = true;
            this.Controls.Add(this.btnNext);

            this.btnLast = new Button
                {
                   Width = new Unit("25px"), Text = @" >| ", CommandArgument = "last", EnableViewState = false 
                };
            this.btnLast.Font.Bold = true;
            this.Controls.Add(this.btnLast);

            this.lblof = new Label { EnableViewState = false, Text = @"&#160;/&#160;" };
            this.Controls.Add(this.lblof);

            this.lblPageCount = new Label { EnableViewState = false };
            this.Controls.Add(this.lblPageCount);

            // Set defaults
            if (this.ViewState["PageNumber"] == null)
            {
                this.PageNumber = 1;
            }

            if (this.ViewState["RecordCount"] == null)
            {
                this.RecordCount = 1;
            }

            if (this.ViewState["HideOnSinglePage"] == null)
            {
                this.HideOnSinglePage = true;
            }

            // Add handlers
            this.tbPageNumber.TextChanged += this.NavigationTbClick;
            this.btnFirst.Click += this.NavigationButtonClick;
            this.btnPrev.Click += this.NavigationButtonClick;
            this.btnNext.Click += this.NavigationButtonClick;
            this.btnLast.Click += this.NavigationButtonClick;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Move event raised when a move is performed
        /// </summary>
        public event EventHandler OnMove;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether validation is performed when the control buttons are clicked.
        /// </summary>
        /// <value><c>true</c> if [causes validation]; otherwise, <c>false</c>.</value>
        [Description(
            "Gets or sets a value indicating whether validation is performed when the control buttons are clicked.")]
        public bool CausesValidation
        {
            get
            {
                var causesValidation = this.ViewState["CausesValidation"];
                return causesValidation == null || (bool)causesValidation;
            }

            set
            {
                this.ViewState["CausesValidation"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to hide when on single page hides controls when
        ///     there is only one page
        /// </summary>
        /// <value><c>true</c> if [hide on single page]; otherwise, <c>false</c>.</value>
        public bool HideOnSinglePage
        {
            get
            {
                return Convert.ToBoolean(this.ViewState["HideOnSinglePage"]);
            }

            set
            {
                this.ViewState["HideOnSinglePage"] = value.ToString();
            }
        }

        /// <summary>
        ///     Gets the page count.
        /// </summary>
        /// <value>The page count.</value>
        public int PageCount
        {
            get
            {
                // Calculate page count
                var pageCount = this.RecordCount / this.RecordsPerPage;

                // adjust for spillover
                if (this.RecordCount % this.RecordsPerPage > 0)
                {
                    pageCount++;
                }

                if (pageCount < 1)
                {
                    pageCount = 1;
                }

                return pageCount;
            }
        }

        /// <summary>
        ///     Gets or sets the page number.
        /// </summary>
        /// <value>The page number.</value>
        public int PageNumber
        {
            get
            {
                return Convert.ToInt32(this.tbPageNumber.Text);
            }

            set
            {
                this.tbPageNumber.Text = value.ToString();
            }
        }

        /// <summary>
        ///     Gets or sets the record count.
        /// </summary>
        /// <value>The record count.</value>
        public int RecordCount
        {
            get
            {
                return Convert.ToInt32(this.ViewState["RecordCount"]);
            }

            set
            {
                this.ViewState["RecordCount"] = value.ToString();
            }
        }

        /// <summary>
        ///     Gets or sets the records per page.
        /// </summary>
        /// <value>The records per page.</value>
        public int RecordsPerPage { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enable/disable the nav controls based on the current context, update labels
        /// </summary>
        public void RefreshButtons()
        {
            // enable/disable the nav controls based on the current context
            // we should only show the first button if we're NOT on the first page already
            this.btnFirst.Enabled = this.PageNumber != 1;
            this.btnPrev.Enabled = this.PageNumber > 1;
            this.btnNext.Enabled = this.PageNumber < this.PageCount;
            this.btnLast.Enabled = this.PageNumber != this.PageCount;

            // Update labels
            this.lblPageCount.Text = this.PageCount.ToString();

            if (this.PageCount <= 1 && this.HideOnSinglePage)
            {
                this.btnFirst.Visible = false;
                this.btnPrev.Visible = false;
                this.btnNext.Visible = false;
                this.btnLast.Visible = false;
                this.lblof.Visible = false;
                this.lblPageCount.Visible = false;
                this.tbPageNumber.Visible = false;
            }
            else
            {
                this.btnFirst.Visible = true;
                this.btnPrev.Visible = true;
                this.btnNext.Visible = true;
                this.btnLast.Visible = true;
                this.lblof.Visible = true;
                this.lblPageCount.Visible = true;
                this.tbPageNumber.Visible = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.RefreshButtons();
        }

        /// <summary>
        /// Used by OnMove event
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnMoveHandler(EventArgs e)
        {
            if (this.OnMove != null)
            {
                this.OnMove(this, e);
            }
        }

        /// <summary>
        /// Navigations the button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void NavigationButtonClick(object sender, EventArgs e)
        {
            // get the command
            var arg = ((Button)sender).CommandArgument;

            // do the command
            switch (arg)
            {
                case "next":
                    if (this.PageNumber < this.PageCount)
                    {
                        this.PageNumber++;
                    }

                    break;
                case "prev":
                    if (this.PageNumber > 1)
                    {
                        this.PageNumber--;
                    }

                    break;
                case "last":
                    this.PageNumber = this.PageCount;
                    break;
                case "first":
                    this.PageNumber = 1;
                    break;
            }

            this.RefreshButtons();

            // Raise the event OnMove
            this.OnMoveHandler(new EventArgs());
        }

        /// <summary>
        /// Navigations the tb click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void NavigationTbClick(object sender, EventArgs e)
        {
            var pageNumber = Convert.ToInt32(this.tbPageNumber.Text);

            if (pageNumber > this.PageCount)
            {
                this.PageNumber = this.PageCount;
            }
            else if (pageNumber < 1)
            {
                this.PageNumber = 1;
            }
            else
            {
                this.PageNumber = pageNumber;
            }

            this.RefreshButtons();

            // Raise the event OnMove
            this.OnMoveHandler(new EventArgs());
        }

        #endregion
    }
}