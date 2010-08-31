/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * COPYRIGHT (c) 2003 by DUEMETRI and contributors.
 * Coder: Emmanuele De Andreis
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 15/APR/2003
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

namespace Rainbow.Framework.Helpers
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Nice HTMLText helper. Contains both the HTML string
    ///     and a tag-free version of the same string.
    ///     by Manu
    /// </summary>
    public struct HTMLText
    {
        #region Constants and Fields

        /// <summary>
        /// The inner html.
        /// </summary>
        private string innerHtml;

        /// <summary>
        /// The inner text.
        /// </summary>
        private string innerText; // Same as _InnerHTML but completely tag-free 

        /// <summary>
        /// The inner xhtml.
        /// </summary>
        private string innerXhtml; // Same as _InnerHTML but converted to XHTML

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HTMLText"/> struct. 
        /// HTMLText
        /// </summary>
        /// <param name="myText">
        /// My text.
        /// </param>
        public HTMLText(string myText)
        {
            this.innerHtml = myText;
            this.innerText = string.Empty;
            this.innerXhtml = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     InnerHTML
        /// </summary>
        /// <value>The inner HTML.</value>
        public string InnerHTML
        {
            get
            {
                return this.innerHtml;
            }

            set
            {
                this.innerHtml = value;
            }
        }

        /// <summary>
        ///     InnerText
        ///     Same as InnerHTML but completely tag-free
        /// </summary>
        /// <value>The inner text.</value>
        public string InnerText
        {
            get
            {
                this.innerText = CleanHTML(this.innerHtml);
                return this.innerText;
            }
        }

        /// <summary>
        ///     InnerXHTML
        /// </summary>
        /// <value>The inner XHTML.</value>
        public string InnerXHTML
        {
            get
            {
                this.innerXhtml = this.GetXHtml(this.innerHtml);
                return this.innerXhtml;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Converts the struct to a string value
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        public static implicit operator string(HTMLText value)
        {
            return value.InnerHTML;
        }

        /// <summary>
        ///     Converts the struct from a string value
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        public static implicit operator HTMLText(string value)
        {
            var h = new HTMLText(value);
            return h;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an abstract HTML of maxLenght characters maximum
        /// </summary>
        /// <param name="maxLenght">
        /// The max lenght.
        /// </param>
        /// <returns>
        /// The get abstract html.
        /// </returns>
        public string GetAbstractHTML(int maxLenght)
        {
            if (maxLenght >= this.InnerHTML.Length || this.InnerHTML == string.Empty)
            {
                return this.InnerHTML;
            }

            var abstr = this.InnerHTML.Substring(0, maxLenght);
            abstr = abstr.Substring(0, abstr.LastIndexOf(' ')) + "...";
            return abstr;
        }

        /// <summary>
        /// Gets an abstract text (no HTML tags!) of maxLenght characters maximum
        /// </summary>
        /// <param name="maxLenght">
        /// The max lenght.
        /// </param>
        /// <returns>
        /// The get abstract text.
        /// </returns>
        public string GetAbstractText(int maxLenght)
        {
            var abstr = this.InnerText;
            if (maxLenght >= abstr.Length || abstr == string.Empty)
            {
                return this.InnerText;
            }

            abstr = abstr.Substring(0, maxLenght);
            var l = abstr.LastIndexOf(' ');
            l = (l > 0) ? l : abstr.Length;
            abstr = abstr.Substring(0, l) + "...";
            return abstr.Trim();
        }

        /// <summary>
        /// Break the text in rows of rowLenght characters maximum
        ///     using HTML content
        /// </summary>
        /// <param name="rowLenght">
        /// The row lenght.
        /// </param>
        /// <returns>
        /// The get breaked html.
        /// </returns>
        public string GetBreakedHTML(int rowLenght)
        {
            return this.breakThis(this.InnerHTML, rowLenght);
        }

        /// <summary>
        /// Break the text in rows of rowLenght characters maximum,
        ///     using text content, useful for emails
        /// </summary>
        /// <param name="rowLenght">
        /// </param>
        /// <returns>
        /// The get breaked text.
        /// </returns>
        public string GetBreakedText(int rowLenght)
        {
            return this.breakThis(this.InnerText, rowLenght);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Transforms the match to lowercase
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The match to lower.
        /// </returns>
        private static string MatchToLower(Match m)
        {
            return m.ToString().ToLower();
        }

        /// <summary>
        /// Processes the attribute.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The process attribute.
        /// </returns>
        private static string ProcessAttribute(Match m)
        {
            // Attribute value (no quote) to Quoted Attribute Value
            string output = "=([^\",^\\s,.]*)[\\s]";
            string regex = Regex.Replace(m.ToString(), output, new MatchEvaluator(QuoteAttibute));

            // Attribute to lowercase
            output = "\\s[^=\"]*=";
            regex = Regex.Replace(regex, output, new MatchEvaluator(MatchToLower));

            // Attribute value (no quote) to Quoted Attribute Value (end of tag)
            output = "=([^\",^\\s,.]*)[>]";
            return Regex.Replace(regex, output, new MatchEvaluator(QuoteAttibuteEnd));
        }

        /// <summary>
        /// Quote the result
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The quote attibute.
        /// </returns>
        private static string QuoteAttibute(Match m)
        {
            var str = m.ToString().Remove(0, 1).Trim();
            return "=\"" + str + "\" ";
        }

        /// <summary>
        /// Quote the result (end tag)
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The quote attibute end.
        /// </returns>
        private static string QuoteAttibuteEnd(Match m)
        {
            var str = m.ToString().Remove(0, 1);
            str = str.Remove(str.Length - 1, 1);
            return string.Format("=\"{0}\">", str);
        }

        /// <summary>
        /// Removes any HTML tags
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The clean html.
        /// </returns>
        private static string CleanHTML(string input)
        {
            string output = Regex.Replace(input, @"(\<[^\<]*\>)", string.Empty);
            output = output.Replace("<", "&lt;");
            output = output.Replace(">", "&gt;");
            return output;
        }

        /// <summary>
        /// Returns XHTML
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The get x html.
        /// </returns>
        private string GetXHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Open tags (no attributes)
            string regex = "<[A-Za-z0-9:]*[>,\\s]";
            string output = Regex.Replace(input, regex, new MatchEvaluator(MatchToLower));

            // Closed tags
            regex = "</[A-Za-z0-9]*>";
            output = Regex.Replace(output, regex, new MatchEvaluator(MatchToLower));

            // Open tags (with attributes)
            regex =
                "<[a-zA-Z]+[a-zA-Z0-9:]*(\\s+[a-zA-Z]+[a-zA-Z0-9:]*((=[^\\s\"'<>]+)|(=\"[^\"]*\")|(='[^']*')|()))*\\s*\\/?\\s*>";
            output = Regex.Replace(output, regex, new MatchEvaluator(ProcessAttribute));

            // HR
            output = output.Replace("<hr>", "<hr />");

            // BR
            output = output.Replace("<br>", "<br />");
            return output;
        }

        /// <summary>
        /// Breaks the this.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="rowLenght">
        /// The row lenght.
        /// </param>
        /// <returns>
        /// The break this.
        /// </returns>
        private string breakThis(string input, int rowLenght)
        {
            // Clean up input
            char[] removeChar = { ' ', '\n' };
            input = input.Trim(removeChar);

            var s = new StringBuilder();

            string row;
            var index = 0;
            var len = input.Length;
            var count = 0;

            while (index <= len)
            {
                if ((index + rowLenght) < len)
                {
                    row = input.Substring(index, rowLenght);
                }
                else
                {
                    // Last row
                    s.Append(input.Substring(index));
                    s.Append(Environment.NewLine);
                    break;
                }

                // Seach for end of line
                var last = row.IndexOf('\n');
                if (last == 0)
                {
                    row = Environment.NewLine;
                }
                else if (last > 0)
                {
                    row = row.Substring(0, last);
                }
                else
                {
                    last = row.LastIndexOf(' ');
                    if (last > 0)
                    {
                        row = row.Substring(0, last) + Environment.NewLine;
                    }
                }

                s.Append(row);
                index += row.Length;

                count++;

                // Avoid loop
                if (row == string.Empty || index == len || count > len)
                {
                    break;
                }
            }

            // Clean up output
            return s.ToString().Trim(removeChar);
        }

        #endregion
    }
}