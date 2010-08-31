namespace Rainbow.Framework.Setup
{
    using System.Collections;

    /// <summary>
    /// IInstaller inteface is used by installable modules
    /// </summary>
    public interface IInstaller
    {
        #region Public Methods

        /// <summary>
        /// Commits the specified state saver.
        /// </summary>
        /// <param name="stateSaver">
        /// The state saver.
        /// </param>
        void Commit(IDictionary stateSaver);

        /// <summary>
        /// Installs the specified state saver.
        /// </summary>
        /// <param name="stateSaver">
        /// The state saver.
        /// </param>
        void Install(IDictionary stateSaver);

        /// <summary>
        /// Rollbacks the specified state saver.
        /// </summary>
        /// <param name="stateSaver">
        /// The state saver.
        /// </param>
        void Rollback(IDictionary stateSaver);

        /// <summary>
        /// Uninstalls the specified state saver.
        /// </summary>
        /// <param name="stateSaver">
        /// The state saver.
        /// </param>
        void Uninstall(IDictionary stateSaver);

        #endregion
    }
}