if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerAddTask]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_SchedulerAddTask]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerGetExpiredTasks]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_SchedulerGetExpiredTasks]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerGetModuleClassName]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_SchedulerGetModuleClassName]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerGetOrderedTasks]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_SchedulerGetOrderedTasks]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerGetTasksByOwner]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_SchedulerGetTasksByOwner]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerGetTasksByTarget]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_SchedulerGetTasksByTarget]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerRemoveTask]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_SchedulerRemoveTask]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_SchedulerTasks]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [rb_SchedulerTasks]
GO

CREATE TABLE [rb_SchedulerTasks] (
	[IDTask] [int] IDENTITY (1, 1) NOT NULL ,
	[IDModuleOwner] [int] NOT NULL ,
	[IDModuleTarget] [int] NOT NULL ,
	[DueTime] [datetime] NOT NULL ,
	[Description] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Argument] [image] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.rb_SchedulerAddTask
(
	@IDOwner int,
	@IDTarget int,
	@DueTime datetime,
	@Description NVARCHAR(150),
	@Argument image,
	@IDTask int output
)
	
AS
	INSERT INTO rb_SchedulerTasks
	                      (DueTime, IDModuleOwner, IDModuleTarget, Description, Argument)
	VALUES     (@DueTime, @IDOwner, @IDTarget, @Description, @Argument)

SELECT @IDTask = @@IDENTITY
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.rb_SchedulerGetExpiredTasks
	
AS
	SELECT     rb_SchedulerTasks.*, DueTime AS Expr1
	FROM         rb_SchedulerTasks
	WHERE     (DueTime < GETDATE())
	ORDER BY DueTime

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.rb_SchedulerGetModuleClassName
	(
		@IDModule int
	)
AS
	SELECT     rb_GeneralModuleDefinitions.AssemblyName, rb_GeneralModuleDefinitions.ClassName
	FROM         rb_Modules INNER JOIN
	                      rb_ModuleDefinitions ON rb_Modules.ModuleDefID = rb_ModuleDefinitions.ModuleDefID INNER JOIN
	                      rb_GeneralModuleDefinitions ON rb_ModuleDefinitions.GeneralModDefID = rb_GeneralModuleDefinitions.GeneralModDefID
	WHERE     (rb_Modules.ModuleID = @IDModule)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.rb_SchedulerGetOrderedTasks
	
AS
	SELECT     rb_SchedulerTasks.*
	FROM         rb_SchedulerTasks
	ORDER BY DueTime

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.rb_SchedulerGetTasksByOwner
(
	@IDOwner int
)
	
AS
	SELECT     rb_SchedulerTasks.*
	FROM         rb_SchedulerTasks
	WHERE     (IDModuleOwner = @IDOwner)
	ORDER BY DueTime

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.rb_SchedulerGetTasksByTarget
(
	@IDTarget int
)
	
AS
	SELECT     rb_SchedulerTasks.*
	FROM         rb_SchedulerTasks
	WHERE     (IDModuleTarget = @IDTarget)
	ORDER BY DueTime

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.rb_SchedulerRemoveTask
(
	@IDTask int
)
	
AS
	DELETE FROM rb_SchedulerTasks
	WHERE     (IDTask = @IDTask)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
