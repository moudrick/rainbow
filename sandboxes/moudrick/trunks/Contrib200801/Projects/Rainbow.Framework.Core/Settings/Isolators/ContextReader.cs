using System.Web;

namespace Rainbow.Context
{
	/// <summary>
	/// Interface for Config Reader Strategy
	/// </summary>
	public interface IRainbowContextStrategy
	{
		/// <summary>
		/// 
		/// </summary>
		HttpContext Current
		{
			get;
		}
	}

	/// <summary>
	/// Concrete Strategy - gets web current context
	/// </summary>
	public class WebContextReader : IRainbowContextStrategy
	{
		/// <summary>
		/// 
		/// </summary>
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
		readonly IRainbowContextStrategy strategy;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strategy">a Concrete Strategy</param>
		public Reader(IRainbowContextStrategy strategy)
		{
			this.strategy = strategy;
		}

		/// <summary>
		/// </summary>
		public HttpContext HttpContext
		{
			get
			{
				return strategy.Current;
			}
		}
	}
}
