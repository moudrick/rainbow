namespace Rainbow.Framework.Scheduler
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Threading;

    // Author: Federico Dal Maso
    // e-mail: ifof@libero.it
    // date: 2003-06-17

    /// <summary>
    /// SimpleScheduler, perform a select every call to Scheduler
    ///     Usefull only for long period timer.
    /// </summary>
    public class SimpleScheduler : IScheduler
    {
        #region Constants and Fields

        /// <summary>
        ///     The local sch db.
        /// </summary>
        internal SchedulerDB LocalSchDb;

        /// <summary>
        ///     The local period.
        /// </summary>
        protected long LocalPeriod;

        /// <summary>
        ///     The local timer state.
        /// </summary>
        protected TimerState LocalTimerState;

        /// <summary>
        ///     The scheduler.
        /// </summary>
        private static volatile SimpleScheduler theScheduler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleScheduler"/> class.
        ///     The simple scheduler.
        /// </summary>
        /// <param name="applicationMapPath">
        /// The application map path.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="period">
        /// The period.
        /// </param>
        protected SimpleScheduler(string applicationMapPath, IDbConnection connection, long period)
        {
            this.LocalSchDb = new SchedulerDB(connection, applicationMapPath);
            this.LocalPeriod = period;

            this.LocalTimerState = new TimerState();

            var t = new Timer(this.Schedule, this.LocalTimerState, Timeout.Infinite, Timeout.Infinite);

            this.LocalTimerState.Timer = t;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the timer period.
        /// </summary>
        /// <value>The timer period.</value>
        public virtual long Period
        {
            get
            {
                return this.LocalPeriod;
            }

            set
            {
                this.LocalPeriod = value;
                this.LocalTimerState.Timer.Change(0L, this.LocalPeriod);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get scheduler.
        /// </summary>
        /// <param name="applicationMapPath">
        /// The application map path.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <returns>
        /// A Simple Scheduler.
        /// </returns>
        public static SimpleScheduler GetScheduler(string applicationMapPath, IDbConnection connection, long period)
        {
            // Sigleton
            if (theScheduler == null)
            {
                lock (typeof(CachedScheduler))
                {
                    if (theScheduler == null)
                    {
                        theScheduler = new SimpleScheduler(applicationMapPath, connection, period);
                    }
                }
            }

            return theScheduler;
        }

        #endregion

        #region Implemented Interfaces

        #region IScheduler

        /// <summary>
        /// Get an array of tasks of the specified module owner
        /// </summary>
        /// <param name="idModuleOwner">
        /// The id module owner.
        /// </param>
        /// <returns>
        /// An array of scheduler tasks.
        /// </returns>
        public virtual SchedulerTask[] GetTasksByOwner(int idModuleOwner)
        {
            var dr = this.LocalSchDb.GetTasksByOwner(idModuleOwner);
            var ary = new ArrayList();
            while (dr.Read())
            {
                ary.Add(new SchedulerTask(dr));
            }

            dr.Close();

            return (SchedulerTask[])ary.ToArray(typeof(SchedulerTask));
        }

        /// <summary>
        /// Get an array of tasks of the specified module target
        /// </summary>
        /// <param name="idModuleTarget">
        /// The id module target.
        /// </param>
        /// <returns>
        /// An array of scheduler tasks.
        /// </returns>
        public virtual SchedulerTask[] GetTasksByTarget(int idModuleTarget)
        {
            var dr = this.LocalSchDb.GetTasksByOwner(idModuleTarget);
            var ary = new ArrayList();
            while (dr.Read())
            {
                ary.Add(new SchedulerTask(dr));
            }

            dr.Close();

            return (SchedulerTask[])ary.ToArray(typeof(SchedulerTask));
        }

        /// <summary>
        /// Insert a new task
        /// </summary>
        /// <param name="task">
        /// The scheduler task.
        /// </param>
        /// <returns>
        /// A scheduler task.
        /// </returns>
        public virtual SchedulerTask InsertTask(SchedulerTask task)
        {
            if (task.IDTask != -1)
            {
                throw new SchedulerException("Could not insert an inserted task");
            }

            task.SetIdTask(this.LocalSchDb.InsertTask(task));
            return task;
        }

        /// <summary>
        /// Remove a task
        /// </summary>
        /// <param name="task">
        /// The scheduler task.
        /// </param>
        public virtual void RemoveTask(SchedulerTask task)
        {
            if (task.IDTask == -1)
            {
                return;
            }

            this.LocalSchDb.RemoveTask(task.IDTask);
            return;
        }

        /// <summary>
        /// Start the scheduler timer
        /// </summary>
        public virtual void Start()
        {
            this.LocalTimerState.Timer.Change(this.LocalPeriod, this.LocalPeriod);
        }

        /// <summary>
        /// Stop the scheduler timer
        /// </summary>
        public virtual void Stop()
        {
            this.LocalTimerState.Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Call the correct ISchedulable methods of a target module assigned to the task.
        /// </summary>
        /// <param name="task">
        /// The scheduler task.
        /// </param>
        protected void ExecuteTask(SchedulerTask task)
        {
            ISchedulable module;
            try
            {
                module = this.LocalSchDb.GetModuleInstance(task.IDModuleTarget);
            }
            catch
            {
                // TODO:
                return;
            }

            try
            {
                module.ScheduleDo(task);
            }
            catch (Exception ex)
            {
                try
                {
                    module.ScheduleRollback(task);
                }
                catch (Exception ex2)
                {
                    throw new SchedulerException("ScheduleDo fail. Rollback fails", ex2);
                }

                throw new SchedulerException("ScheduleDo fails. Rollback called successfully", ex);
            }

            try
            {
                module.ScheduleCommit(task);
            }
            catch (Exception ex)
            {
                throw new SchedulerException("ScheduleDo called successfully. Commit fails", ex);
            }
        }

        /// <summary>
        /// The schedule.
        /// </summary>
        /// <param name="timerState">
        /// State of the timer.
        /// </param>
        protected virtual void Schedule(object timerState)
        {
            lock (this)
            {
                this.LocalTimerState.Counter++;

                var tsks = this.LocalSchDb.GetExpiredTask();

                this.Stop(); // Stop the timer while it works

                foreach (var tsk in tsks)
                {
                    try
                    {
                        this.ExecuteTask(tsk);
                    }
                    catch
                    {
                        // TODO: We have to apply some policy here...
                        // i.e. Move failed tasks on a log, call a Module feedback interface,....
                        // now task is removed always
                    }

                    this.RemoveTask(tsk);
                }

                this.Start(); // restart the timer
            }
        }

        #endregion

        /// <summary>
        /// Timer State
        /// </summary>
        protected class TimerState
        {
            #region Properties

            /// <summary>
            ///     Gets or sets the counter.
            /// </summary>
            /// <value>The counter.</value>
            public int Counter { get; set; }

            /// <summary>
            ///     Gets or sets the timer.
            /// </summary>
            /// <value>The timer.</value>
            public Timer Timer { get; set; }

            #endregion
        }
    }
}