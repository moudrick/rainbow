 // Created by Manu

namespace Rainbow.Framework
{
    using System;

    /// <summary>
    /// Defines the level of the event to log
    /// </summary>
    [Flags]
    public enum LogLevels
    {
        /// <summary>
        ///     None
        /// </summary>
        None = 0, 

        /// <summary>
        ///     Debug
        /// </summary>
        Debug = 2, 

        /// <summary>
        ///     Info
        /// </summary>
        Info = 4, 

        /// <summary>
        ///     Warn
        /// </summary>
        Warn = 8, 

        /// <summary>
        ///     Error
        /// </summary>
        Error = 16, 

        /// <summary>
        ///     Fatal
        /// </summary>
        Fatal = 32
    }
}