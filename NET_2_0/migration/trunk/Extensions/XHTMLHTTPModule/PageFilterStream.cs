using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetResources.Web
{
	/// <summary>
	/// PageFilterStream does all the dirty work of tinkering the 
	/// outgoing HTML stream. This is a good place to add keywords and
	/// enforce some compilancy with web standards.
	/// </summary>
	public class PageFilterStream : Stream
	{
		private Stream responseStream;
		private long position;
		private StringBuilder responseHtml;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputStream"></param>
		public PageFilterStream(Stream inputStream)
		{
			responseStream = inputStream;
			responseHtml = new StringBuilder();
		}

		#region Filter overrides

		/// <summary>
		/// 
		/// </summary>
		public override bool CanRead
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool CanSeek
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool CanWrite
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Close()
		{
			responseStream.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Flush()
		{
			responseStream.Flush();
		}

		/// <summary>
		/// 
		/// </summary>
		public override long Length
		{
			get { return 0; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override long Position
		{
			get { return position; }
			set { position = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return responseStream.Seek(offset, origin);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		public override void SetLength(long length)
		{
			responseStream.SetLength(length);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return responseStream.Read(buffer, offset, count);
		}

		#endregion

		#region Dirty work

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			string strBuffer = UTF8Encoding.UTF8.GetString(buffer, offset, count);

			// ---------------------------------
			// Wait for the closing </html> tag
			// ---------------------------------
			Regex eof = new Regex("</html>", RegexOptions.IgnoreCase);

			if (!eof.IsMatch(strBuffer))
				responseHtml.Append(strBuffer);
			else
			{
				responseHtml.Append(strBuffer);

				string finalHtml = responseHtml.ToString();
				Regex re = null;


				// The title has an id="..." which we need to get rid of
				re = new Regex("<title(\\s+id=.+?)>", RegexOptions.IgnoreCase);
				finalHtml = re.Replace(finalHtml, new MatchEvaluator(TitleMatch));

				// Replace language="javascript" with script type="text/javascript"
				re = new Regex("(?<=script\\s*)(language=\"javascript\")", RegexOptions.IgnoreCase);
				finalHtml = re.Replace(finalHtml, new MatchEvaluator(JavaScriptMatch));

				// If there are still any language="javascript" are left, delete them
				finalHtml = Regex.Replace(finalHtml, "language=\"javascript\"", string.Empty, RegexOptions.IgnoreCase);

				// Clean up images. Some images have a border property which is deprecated in XHTML
				re = new Regex("<img.*(border=\".*?\").*?>", RegexOptions.IgnoreCase);
				finalHtml = re.Replace(finalHtml, new MatchEvaluator(ImageBorderMatch));

				// Wrap the __VIEWSTATE tag in a div to pass validation
				re = new Regex("(<input.*?__VIEWSTATE.*?/>)", RegexOptions.IgnoreCase);
				finalHtml = re.Replace(finalHtml, new MatchEvaluator(ViewStateMatch));

				// If __doPostBack is registered, replace the whole function
				if (finalHtml.IndexOf("__doPostBack") > -1)
				{
					try
					{
						int pos1 = finalHtml.IndexOf("var theform;");
						int pos2 = finalHtml.IndexOf("theform.__EVENTTARGET", pos1);
						string methodText = finalHtml.Substring(pos1, pos2 - pos1);
						string formID = Regex.Match(methodText, "document.forms\\[\"(.*?)\"\\];", RegexOptions.IgnoreCase).Groups[1].Value.Replace(":", "_");

						finalHtml = finalHtml.Replace(methodText,
						                              @"var theform = document.getElementById ('" + formID + "');\r\n\t\t");
					}
					catch
					{
					}
				}

				// Remove the "name" attribute from <form> tags because they are invalid
				re = new Regex("<form\\s+(name=.*?\\s)", RegexOptions.IgnoreCase);
				finalHtml = re.Replace(finalHtml, new MatchEvaluator(FormNameMatch));

				// Write the formatted HTML back
				byte[] data = UTF8Encoding.UTF8.GetBytes(finalHtml);


				responseStream.Write(data, 0, data.Length);
			}

		}

		//---------------------------------------------------------------------------
		private static string TitleMatch(Match m)
		{
			return m.ToString().Replace(m.Groups[1].Value, string.Empty);
		}

		//---------------------------------------------------------------------------
		private static string JavaScriptMatch(Match m)
		{
			return m.ToString().Replace(m.Groups[1].Value, "type=\"text/javascript\"");
		}

		//---------------------------------------------------------------------------
		private static string ImageBorderMatch(Match m)
		{
			return m.ToString().Replace(m.Groups[1].Value, string.Empty);
		}

		//---------------------------------------------------------------------------
		private static string ViewStateMatch(Match m)
		{
			return string.Concat("<div>", m.Groups[1].Value, "</div>");
		}

		//---------------------------------------------------------------------------
		private static string FormNameMatch(Match m)
		{
			return m.ToString().Replace(m.Groups[1].Value, string.Empty);
		}

		#endregion
	}
}