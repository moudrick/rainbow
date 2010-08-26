using System.Web;

namespace Rainbow.Framework.Configuration.Web
{
	/// <summary>
	/// Interface for Config Reader Strategy
	/// </summary>
	public interface IStrategy
	{
        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
		HttpContext Current
		{
			get;
		}
	}

	/// <summary>
	/// Concrete Strategy - gets web current context
	/// </summary>
	public class WebContextReader : IStrategy
	{
        /// <summary>
        /// web current context
        /// </summary>
        /// <value></value>
		public HttpContext Current
		{
			get
			{
				return HttpContext.Current;
			}
		}
	}

	/// <summary>
	/// Reader
	/// </summary>
	public class Reader
	{
		private IStrategy strategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
		public Reader(IStrategy strategy)
		{
			this.strategy = strategy;
		}

        /// <summary>
        /// web current context
        /// </summary>
        /// <value>The current.</value>
		public HttpContext Current
		{
			get
			{
				return strategy.Current;
			}
			
		}
	}
}