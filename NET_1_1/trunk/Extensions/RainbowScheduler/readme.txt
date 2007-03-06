Asynchronous scheduler
by Federico Dal Maso

It's an asynchronous scheduler, that works in background (like a daemon) to execute generic tasks scheduled by module at specified datetime.

See here for idea and what about you can do with it: 
http://www.rainbowportal.net/AspNetForums/ShowPost.aspx?PostID=3334

It's implemented in separated dll, you can find source in /RainbowScheduler.

A module can insert new task in scheduler simple by create a SchedulerTask object and adding it by PortalSettings.Scheduler.InsertTask method. A module can add a task for a different module by setting a different ModuleTargetID in ScehdulerTask object.

Module that use scheduler must implement RAinbow.Scheduler.ISchedulable interface.

When a task dues, scheduler will instance the target module and call ScheduleDo method (see ISchedulable) passing a task object. task object include an argument field of object type. You can use it to store information for task (it must be serializable, because it's stored in binary field of a db table)

If ScheduleDo will work without throwing exception ScheduleCommit method
(ISchedulable) will call else ScheduleRollback (ISchedulable) will be call.

You MUST update web.config and your .sln file.
in web.config you can find 3 new parameters. SchedulerEnable, SchedulerCacheSize, SchedulerPeriod.

ScheduleEnable now is set no "no", because it's an alfa version ;). Set to "yes" if you want to start scheduler. 

SchedulerCacheSize set how many task should be stored in an in-memory cache by scheduler. Scheduler performs a SELECT in db to refill cache only when cache is empty, else use internal cache to check if a task dues when internal timer tick. So, if you have only a daily task, only a SELECT in 3 months is sent to db (i suppose a 100 cache size setting) Cache is sync with db when a task is inserted or removed.

SchedulerPeriod is a period use to internal timer to check if any task should be executed.

Scheduler is pluggable with new kind of scheduler implementation. In fact only IScheduler interface is used in rainbow. So if you want to create a scheduler that accept tick via telnet and don't use internal timer and cache, you can do it.

I suggest you to use CachedScheduler implementation of IScheduler interface. It use db to store tasks and a smart cache.

The script for updating db is available in this directory. [sqlsched.sql]

Fede_
