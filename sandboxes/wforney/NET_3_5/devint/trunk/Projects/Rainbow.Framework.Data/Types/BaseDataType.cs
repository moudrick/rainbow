namespace Rainbow.Framework.Data.Types
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Base Data Type
    /// </summary>
    public abstract class BaseDataType
    {
        #region Constants and Fields

        /// <summary>
        /// The inner data source.
        /// </summary>
        protected object InnerDataSource;

        /// <summary>
        ///     Holds the value.
        /// </summary>
        protected PropertiesDataType InnerDataType;

        /// <summary>
        /// The control width.
        /// </summary>
        protected int ControlWidth = 350;

        /// <summary>
        /// The inner control.
        /// </summary>
        protected Control InnerControl;

        /// <summary>
        /// The inner value.
        /// </summary>
        protected string InnerValue = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets DataSource
        ///     Should be overrided from inherited classes
        /// </summary>
        /// <value>The data source.</value>
        public virtual object DataSource
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Gets or sets the data text field.
        /// </summary>
        /// <value>The data text field.</value>
        public virtual string DataTextField
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Gets or sets the data value field.
        /// </summary>
        /// <value>The data value field.</value>
        public virtual string DataValueField
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public virtual string Description
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets EditControl
        /// </summary>
        /// <value>The edit control.</value>
        public virtual Control EditControl
        {
            get
            {
                if (this.InnerControl == null)
                {
                    this.InitializeComponents();
                }

                // Update value in control
                ((TextBox)this.InnerControl).Text = this.Value;

                // Return control
                return this.InnerControl;
            }

            set
            {
                if (value.GetType().Name != "TextBox")
                {
                    throw new ArgumentException(
                        string.Format("A TextBox values is required, a '{0}' is given.", value.GetType().Name),
                        "EditControl");
                }
                
                this.InnerControl = value;

                // Update value from control
                this.Value = ((TextBox)this.InnerControl).Text;
            }
        }

        /// <summary>
        ///     Not Implemented
        /// </summary>
        /// <value>The full path.</value>
        public virtual string FullPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual PropertiesDataType Type
        {
            get
            {
                return this.InnerDataType;
            }
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public virtual string Value
        {
            get
            {
                return this.InnerValue;
            }

            set
            {
                this.InnerValue = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the components.
        /// </summary>
        protected virtual void InitializeComponents()
        {
            // Text box
            using (var tx = new TextBox())
            {
                tx.CssClass = "NormalTextBox";
                tx.Columns = 30;
                tx.Width = new Unit(this.ControlWidth);
                tx.MaxLength = 1500; // changed max value to 1500 since most of settings are string

                this.InnerControl = tx;
            }
        }

        #endregion
    }
}