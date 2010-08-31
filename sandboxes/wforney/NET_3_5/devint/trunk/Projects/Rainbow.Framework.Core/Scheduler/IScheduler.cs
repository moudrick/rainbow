namespace Rainbow.Framework.Scheduler
{
    // Author: Federico Dal Maso
    // e-mail: ifof@libero.it
    // date: 2003-06-17

    /// <summary>
    /// Standard interface for a scheduler
    /// </summary>
    public interface IScheduler
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the scheduler timer period
        /// </summary>
        long Period { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an array of tasks of the specified module target
        /// </summary>
        /// <param name="moduleOwnerId">
        /// The module Owner Id.
        /// </param>
        /// <returns>An array of scheduler tasks.</returns>
        SchedulerTask[] GetTasksByOwner(int moduleOwnerId);

        /// <summary>
        /// Gets an array of tasks of the specified module owner
        /// </summary>
        /// <param name="moduleTargetId">The module Target Id.</param>
        /// <returns>An array of scheduler tasks.</returns>
        SchedulerTask[] GetTasksByTarget(int moduleTargetId);

        /// <summary>
        /// Inserts a new task
        /// </summary>
        /// <param name="task">The scheduler task.</param>
        /// <returns>A scheduler task.</returns>
        SchedulerTask InsertTask(SchedulerTask task);

        /// <summary>
        /// Removes a task
        /// </summary>
        /// <param name="task">The scheduler task.</param>
        void RemoveTask(SchedulerTask task);

        /// <summary>
        /// Start the scheduler
        /// </summary>
        void Start();

        /// <summary>
        /// Stop scheduler activities
        /// </summary>
        void Stop();

        #endregion
    }
}