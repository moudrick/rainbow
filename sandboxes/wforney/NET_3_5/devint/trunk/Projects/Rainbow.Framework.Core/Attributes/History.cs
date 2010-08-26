using System;

namespace Rainbow.Framework
{
    /// <summary>
    /// History can be used to track code authors and modification
    /// </summary>
    [AttributeUsage(
        AttributeTargets.All,
        Inherited = false,
        AllowMultiple = true
        )]
    public class History : Attribute
    {
        /// <summary>
        /// Requires all parameters
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="email">email</param>
        /// <param name="version">version</param>
        /// <param name="date">date</param>
        /// <param name="comment">comment</param>
        public History(string name, string email, string version, string date, string comment)
        {
            this.Name = name;
            this.Email = email;
            this.Version = version;
            this.Date = DateTime.Parse(date);
            this.Comment = comment;
        }

        /// <summary>
        /// Requires name, date and comment
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="date">date</param>
        /// <param name="comment">comment</param>
        public History(string name, string date, string comment)
        {
            this.Name = name;
            try
            {
                this.Date = DateTime.Parse(date);
            }
            catch (FormatException ex)
            {
                throw new Exception("'" + date + "' is an invalid date", ex);
            }
            this.Comment = comment;
        }

        /// <summary>
        /// Requires name, date and comment
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="date">date</param>
        public History(string name, string date)
        {
            this.Name = name;
            this.Date = DateTime.Parse(date);
        }

        /// <summary>
        /// The Author name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The Email of the Author
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Modification Version
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// Modification date
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Modification description
        /// </summary>
        public virtual string Comment { get; set; }
    }
}