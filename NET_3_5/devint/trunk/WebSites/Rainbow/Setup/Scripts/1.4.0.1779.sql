IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_PortalSettings_rb_Portals]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_PortalSettings
	DROP CONSTRAINT FK_rb_PortalSettings_rb_Portals
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_PortalSettings_rb_Portals]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_PortalSettings WITH NOCHECK ADD CONSTRAINT
	FK_rb_PortalSettings_rb_Portals FOREIGN KEY
	(
	PortalID
	) REFERENCES rb_Portals
	(
	PortalID
	) ON DELETE CASCADE
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Modules_rb_Pages1]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Modules
	DROP CONSTRAINT FK_rb_Modules_rb_Pages1
GO

--Dropping 1 at the end
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Modules_rb_Pages]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Modules
	DROP CONSTRAINT FK_rb_Modules_rb_Pages
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Modules_rb_Pages]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Modules WITH NOCHECK ADD CONSTRAINT
	FK_rb_Modules_rb_Pages FOREIGN KEY
	(
	TabID
	) REFERENCES rb_Pages
	(
	PageID
	) ON DELETE CASCADE
	
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Pages_rb_Portals]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Pages
	DROP CONSTRAINT FK_rb_Pages_rb_Portals
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Pages_rb_Portals]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Pages WITH NOCHECK ADD CONSTRAINT
	FK_rb_Pages_rb_Portals FOREIGN KEY
	(
	PortalID
	) REFERENCES rb_Portals
	(
	PortalID
	) ON DELETE CASCADE
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_ModuleSettings_rb_Modules]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_ModuleSettings
	DROP CONSTRAINT FK_rb_ModuleSettings_rb_Modules
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_ModuleSettings_rb_Modules]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_ModuleSettings WITH NOCHECK ADD CONSTRAINT
	FK_rb_ModuleSettings_rb_Modules FOREIGN KEY
	(
	ModuleID
	) REFERENCES rb_Modules
	(
	ModuleID
	) ON DELETE CASCADE
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Modules_rb_ModuleDefinitions1]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Modules
	DROP CONSTRAINT FK_rb_Modules_rb_ModuleDefinitions1
GO

--Dropping 1
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Modules_rb_ModuleDefinitions]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Modules
	DROP CONSTRAINT FK_rb_Modules_rb_ModuleDefinitions
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Modules_rb_ModuleDefinitions]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Modules WITH NOCHECK ADD CONSTRAINT
	FK_rb_Modules_rb_ModuleDefinitions FOREIGN KEY
	(
	ModuleDefID
	) REFERENCES rb_ModuleDefinitions
	(
	ModuleDefID
	) ON DELETE CASCADE
	
GO