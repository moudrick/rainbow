namespace Rainbow.Framework.Scheduler
{
    using System;

    /// <summary>
    /// Summary description for SchedulerException.
    /// </summary>
    /// <remarks>
    /// Author: Federico Dal Maso
    ///     e-mail: ifof@libero.it
    ///     date: 2003-06-17
    /// </remarks>
    [Serializable]
    public class SchedulerException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerException"/> class. 
        /// The scheduler exception.
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="innerException">
        /// </param>
        public SchedulerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerException"/> class. 
        /// The scheduler exception.
        /// </summary>
        /// <param name="message">
        /// </param>
        public SchedulerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerException"/> class. 
        /// The scheduler exception.
        /// </summary>
        public SchedulerException()
        {
        }

        #endregion
    }
}