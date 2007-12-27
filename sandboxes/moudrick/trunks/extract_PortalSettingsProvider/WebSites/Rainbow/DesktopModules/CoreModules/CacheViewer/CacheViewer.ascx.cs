using System;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Rainbow.Framework.Core;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Content.Web.Modules
{
    public partial class CacheViewer : PortalModuleControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            int rb_cacheCount = 0;
            int rb_cacheSize;
            int rb_cacheTotal = 0;
            int asp_cacheTotal = 0;

            //TODO: [moudrick] move it to RainbowContext
            SortedList cacheContents = new SortedList();
            foreach (DictionaryEntry cacheItem in HttpContext.Current.Cache)
            {
                asp_cacheTotal = asp_cacheTotal + cacheItem.Value.ToString().Length;

                if (cacheItem.Key.ToString().StartsWith(RainbowContext.Current.UniqueID))
                {
                    cacheContents.Add(cacheItem.Key.ToString(), cacheItem.Value.ToString());
                }
            }

            Table table = new Table();
            table.CellSpacing = 0;
            table.CellPadding = 4;
            TableRow row;
            TableCell cell;
            table.CssClass = "Normal";

            foreach (DictionaryEntry contentsItem in cacheContents)
            {
                rb_cacheSize = contentsItem.Value.ToString().Length;
                rb_cacheCount = rb_cacheCount + 1;
                rb_cacheTotal = rb_cacheTotal + rb_cacheSize;

                row = new TableRow();
                row.BackColor = Color.Gray;
                row.ForeColor = Color.White;
                cell = new TableCell();
                cell.Text = contentsItem.Key.ToString();
                row.Cells.Add(cell);
                cell = new TableCell();
                cell.Text = string.Concat("approx. ", rb_cacheSize.ToString("n0"), " characters");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                table.Rows.Add(row);

                row = new TableRow();
                cell = new TableCell();
                cell.ColumnSpan = 2;
                cell.Text = contentsItem.Value.ToString();
                row.Cells.Add(cell);
                table.Rows.Add(row);
            }

            CachePanel.Controls.Add(table);

            table = new Table();
            table.CellSpacing = 0;
            table.CellPadding = 4;
            table.CssClass = "Normal";

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Rainbow Cache Count:";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = rb_cacheCount.ToString("n0") + " items";
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Total Cache Count:";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = HttpContext.Current.Cache.Count.ToString("n0") + " items";
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Rainbow Cache Size:";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = rb_cacheTotal.ToString("n0") + " characters";
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Total Cache Size:";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = asp_cacheTotal.ToString("n0") + " characters";
            row.Cells.Add(cell);
            table.Rows.Add(row);

            CachePanel.Controls.Add(table);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);

//			ModuleTitle = new DesktopModuleTitle();

//			Controls.AddAt(0, ModuleTitle);

            base.OnInit(e);
        }

        #endregion

        // Jes1111
        /// <summary>
        /// Overrides ModuleSetting to render this module type un-cacheable
        /// </summary>
        /// <value><c>true</c> if cacheable; otherwise, <c>false</c>.</value>
        public override bool Cacheable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the GUID ID.
        /// </summary>
        /// <value>The GUID ID.</value>
        public override Guid GuidID
        {
            get { return new Guid("{33F254F8-2537-4486-A91D-E8544D407200}"); }
        }

        /// <summary>
        /// Admin Module
        /// </summary>
        /// <value><c>true</c> if [admin module]; otherwise, <c>false</c>.</value>
        public override bool AdminModule
        {
            get { return true; }
        }
    }
}