/* Install script Ecommerce, by Manu [manu-dea@hotmail dot it] */
/* Modified by Thierry [thierry@tiptopweb.com.au] to include options for products */
/* Modified by Thierry [thierry@tiptopweb.com.au] exception : rb_CartRemoveAllItems used in rb_AddOrderDetails and has to be included before 
 */

IF NOT EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_Cart]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_Cart] (
	[RecordID] [int] IDENTITY (1, 1) NOT NULL ,
	[CartID] [nvarchar] (50) NULL ,
	[Quantity] [int] NOT NULL CONSTRAINT [DF_Cart_Quantity] DEFAULT (1),
	[ProductID] [int] NOT NULL ,
	[DateCreated] [datetime] NOT NULL CONSTRAINT [DF_Cart_DateCreated] DEFAULT (getdate()),
	[ModuleID] [int] NOT NULL ,
	[MetadataXml] [nvarchar] (3000) NULL,
	CONSTRAINT [PK_Cart] PRIMARY KEY  NONCLUSTERED 
	(
		[RecordID]
	) WITH  FILLFACTOR = 90
)
END
GO

/* Thierry : add options to the product module */
/* modification of the initial script by Joerg Szepan : fix error on updating tables if existing */

IF NOT EXISTS (SELECT * FROM sysobjects SO INNER JOIN syscolumns SC on SO.id=SC.id where SO.id = object_id(N'[rb_Cart]') and OBJECTPROPERTY(SO.id, N'IsUserTable') = 1 and SC.name=N'MetadataXml')
BEGIN
ALTER TABLE [rb_Cart] WITH NOCHECK ADD 
	[MetadataXml] [nvarchar] (3000) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_Orders]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_Orders] (
	[OrderID] [char] (24) NOT NULL ,
	[UserID] [int] NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[TotalGoods] [money] NULL CONSTRAINT [DF_rb_Orders_TotalGoods] DEFAULT (0),
	[TotalShipping] [money] NULL CONSTRAINT [DF_rb_Orders_TotalShipping] DEFAULT (0),
	[TotalTaxes] [money] NULL CONSTRAINT [DF_rb_Orders_TotalTaxes] DEFAULT (0),
	[TotalExpenses] [money] NULL CONSTRAINT [DF_rb_Orders_TotalExpenses] DEFAULT (0),
	[TotalWeight] [real] NULL CONSTRAINT [DF_rb_Orders_TotalWeight] DEFAULT (0),
	[DateCreated] [datetime] NULL ,
	[DateModified] [datetime] NULL ,
	[Status] [int] NULL ,
	[PaymentMethod] [nvarchar] (50) NULL ,
	[ShippingMethod] [nvarchar] (50) NULL ,
	[ShippingData] [ntext] NULL ,
	[BillingData] [ntext] NULL ,
	[TransactionID] [nvarchar] (50) NULL ,
	[AuthCode] [nvarchar] (50) NULL ,
	[ISOCurrencySymbol] [char] (3) NULL ,
	[WeightUnit] [nvarchar] (15) NULL ,
	CONSTRAINT [PK_Orders] PRIMARY KEY  CLUSTERED 
	(
		[OrderID]
	) WITH  FILLFACTOR = 90,
	CONSTRAINT [FK_Orders_Users] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [rb_Users] (
		[UserID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_OrderDetails]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_OrderDetails] (
	[OrderID] [char] (24) NOT NULL ,
	[ProductID] [int] NOT NULL ,
	[Quantity] [int] NOT NULL ,
	[ModelName] [nvarchar] (256) NOT NULL ,
	[ModelNumber] [nvarchar] (256) NOT NULL ,
	[UnitPrice] [money] NOT NULL ,
	[Weight] [real] NOT NULL CONSTRAINT [DF__rb_OrderD__Weight] DEFAULT (0),
	[MetadataXml] [nvarchar] (3000) NULL,
	CONSTRAINT [FK_rb_OrderDetails_rb_Orders] FOREIGN KEY 
	(
		[OrderID]
	) REFERENCES [rb_Orders] (
		[OrderID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
)
END
GO

/* Thierry : add options to the product module */
/* modification of the initial script by Joerg Szepan : fix error on updating tables if existing */

IF NOT EXISTS (SELECT * FROM sysobjects SO INNER JOIN syscolumns SC on SO.id=SC.id where SO.id = object_id(N'[rb_OrderDetails]') and OBJECTPROPERTY(SO.id, N'IsUserTable') = 1 and SC.name=N'MetadataXml')
BEGIN
ALTER TABLE [rb_OrderDetails] WITH NOCHECK ADD 
	[MetadataXml] [nvarchar] (3000) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_Products]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Products] (
	[ProductID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CategoryID] [int] NOT NULL ,
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Products_DisplayOrder] DEFAULT (0),
	[ModelNumber] [nvarchar] (256) NULL ,
	[ModelName] [nvarchar] (256) NULL ,
	[UnitPrice] [money] NOT NULL CONSTRAINT [DF_Products_SalePrice] DEFAULT (0),
	[FeaturedItem] [bit] NOT NULL CONSTRAINT [DF_Products_FeaturedItem] DEFAULT (0),
	[LongDescription] [ntext] NULL ,
	[ShortDescription] [ntext] NULL ,
	[MetadataXml] [ntext] NULL ,
	[TaxRate] [float] NULL ,
	[Weight] [float] NULL ,
	CONSTRAINT [PK_Products] PRIMARY KEY  CLUSTERED 
	(
		[ProductID]
	) WITH  FILLFACTOR = 90,
	CONSTRAINT [FK_Products_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_Products_st]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Products_st] (
	[ProductID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CategoryID] [int] NOT NULL ,
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Products_st_DisplayOrder] DEFAULT (0),
	[ModelNumber] [nvarchar] (256) NULL ,
	[ModelName] [nvarchar] (256) NULL ,
	[UnitPrice] [money] NOT NULL CONSTRAINT [DF_Products_st_SalePrice] DEFAULT (0),
	[FeaturedItem] [bit] NOT NULL CONSTRAINT [DF_Products_st_FeaturedItem] DEFAULT (0),
	[LongDescription] [ntext] NULL ,
	[ShortDescription] [ntext] NULL ,
	[MetadataXml] [ntext] NULL ,
	[TaxRate] [float] NULL CONSTRAINT [DF_rb_Products_st_TaxRate] DEFAULT (10),
	[Weight] [float] NULL CONSTRAINT [DF_rb_Products_st_Weight] DEFAULT (0),
	CONSTRAINT [PK_Products_st] PRIMARY KEY  CLUSTERED 
	(
		[ProductID]
	) WITH  FILLFACTOR = 90,
	CONSTRAINT [FK_Products_st_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_ShipPrices]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_ShipPrices] (
	[ShipPriceId] [int] IDENTITY (1, 1) NOT NULL ,
	[Weight] [float] NOT NULL ,
	[Price] [money] NOT NULL ,
	CONSTRAINT [PK_ShipPrices] PRIMARY KEY  CLUSTERED 
	(
		[ShipPriceId]
	)
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_ShipZones]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_ShipZones] (
	[CountryID] [nchar] (2) NOT NULL ,
	[ShipPriceId] [int] NOT NULL ,
	CONSTRAINT [FK_rb_ShipZones_rb_Countries] FOREIGN KEY 
	(
		[CountryID]
	) REFERENCES [rb_Countries] (
		[CountryID]
	),
	CONSTRAINT [FK_rb_ShipZones_rb_ShipPrices] FOREIGN KEY 
	(
		[ShipPriceId]
	) REFERENCES [rb_ShipPrices] (
		[ShipPriceId]
	)
)
END
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_Products_stModified]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_Products_stModified]
GO

CREATE TRIGGER [rb_Products_stModified]
ON [rb_Products_st]
FOR DELETE, INSERT, UPDATE 
AS 
BEGIN
DECLARE ChangedModules CURSOR FOR
SELECT ModuleID
FROM inserted
UNION
SELECT ModuleID
FROM deleted
 
DECLARE @ModID     int
OPEN ChangedModules     
FETCH NEXT FROM ChangedModules
INTO @ModID
WHILE @@FETCH_STATUS = 0
BEGIN
EXEC rb_ModuleEdited @ModID
FETCH NEXT FROM ChangedModules
INTO @ModID
END
CLOSE ChangedModules
DEALLOCATE ChangedModules
END
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_AddOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddOrder]
GO

CREATE PROCEDURE rb_AddOrder
(
	@OrderID 		char(24),
	@ModuleID	 	int,
	@UserID	 		int,
	@TotalGoods	 	money,
	@TotalShipping	 	money,
	@TotalTaxes	 	money,
	@TotalExpenses	 	money,
	@ISOCurrencySymbol	char(3),
	@Status	 		int,
	@DateCreated	 	datetime,
	@DateModified		datetime,
	@PaymentMethod 		nvarchar(50),
	@ShippingMethod 	nvarchar(50),
	@TotalWeight	 	real,
	@WeightUnit		nvarchar(15),
	@ShippingData 		ntext,
	@BillingData	 	ntext
)
AS INSERT INTO rb_Orders 
(
	OrderID,
	ModuleID,
	UserID,
	TotalGoods,
	TotalShipping,
	TotalTaxes,
	TotalExpenses,
	ISOCurrencySymbol,
	Status,
	DateCreated,
	DateModified,
	PaymentMethod,
	ShippingMethod,
	TotalWeight,
	WeightUnit,
	ShippingData,
	BillingData
) 
 
VALUES 
(
	 @OrderID,
	 @ModuleID,
	 @UserID,
	 @TotalGoods,
	 @TotalShipping,
	 @TotalTaxes,
	 @TotalExpenses,
	 @ISOCurrencySymbol,
	 @Status,
	 @DateCreated,
	 @DateModified,
	 @PaymentMethod,
	 @ShippingMethod,
	 @TotalWeight,
	 @WeightUnit,
	 @ShippingData,
	 @BillingData
)
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartRemoveAllItems]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartRemoveAllItems]
GO

CREATE Procedure rb_CartRemoveAllItems
(
    @CartID nvarchar(50),
    @ModuleID int
)
AS
DELETE FROM rb_Cart
WHERE 
    CartID = @CartID
AND
    ModuleID = @ModuleID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_AddOrderDetails]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddOrderDetails]
GO

CREATE PROCEDURE rb_AddOrderDetails 
	@OrderID char(24),
	@CartID nvarchar(50),
	@ModuleID int
AS
-- Copy items from given shopping cart to OrdersDetail table for given OrderID
INSERT INTO rb_OrderDetails
(
    OrderID, 
    ProductID, 
    MetadataXml,
    Quantity, 
    ModelName,
    ModelNumber,
    UnitPrice,
    Weight
)
SELECT 
    @OrderID, 
    rb_Cart.ProductID, 
    rb_Cart.MetadataXml,
    Quantity, 
    rb_Products_st.ModelName,
    rb_Products_st.ModelNumber,
    rb_Products_st.UnitPrice,
    rb_Products_st.Weight
FROM 
    rb_Cart 
  INNER JOIN rb_Products_st ON rb_Cart.ProductID = rb_Products_st.ProductID
  
WHERE 
    CartID = @CartID AND rb_Cart.ModuleID = @ModuleID
/* Removal of  items from user's shopping cart */
exec rb_CartRemoveAllItems @CartID, @ModuleID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartAddItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartAddItem]
GO

CREATE Procedure rb_CartAddItem
(
    @CartID nvarchar(50),
    @ProductID int,
    @Quantity int,
    @ModuleID int,
    @MetadataXml nvarchar(3000)
)
As
DECLARE @CountItems int
SELECT
    @CountItems = Count(ProductID)
FROM
    rb_Cart
WHERE
    ProductID = @ProductID
  AND
    CartID = @CartID
  AND
    ModuleID = @ModuleID
  AND 
    MetadataXml = @MetadataXml

IF @CountItems > 0  /* There are items - update the current quantity - check the options */
    UPDATE
        rb_Cart
    SET
        Quantity = (@Quantity + rb_Cart.Quantity)
    WHERE
        ProductID = @ProductID
      AND
        CartID = @CartID
      AND
        ModuleID = @ModuleID
      AND 
        MetadataXml = @MetadataXml
ELSE  /* New entry for this Cart.  Add a new record */
    INSERT INTO rb_Cart
    (
        CartID,
        Quantity,
        ProductID,
        ModuleID,
        DateCreated,
        MetadataXml
    )
    VALUES
    (
        @CartID,
        @Quantity,
        @ProductID,
        @ModuleID,
        GetDate(),
        @MetadataXml
    )
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartEmpty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartEmpty]
GO

CREATE Procedure rb_CartEmpty
(
    @CartID nvarchar(50),
    @ModuleID int
)
AS
DELETE FROM rb_Cart
WHERE 
    CartID = @CartID 
AND 
    ModuleID = @ModuleID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartItemCount]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartItemCount]
GO

CREATE Procedure rb_CartItemCount
(
    @CartID    nvarchar(50),
    @ModuleID int,
    @ItemCount int OUTPUT
)
AS
SELECT 
    @ItemCount = COUNT(ProductID)
    
FROM 
    rb_Cart
    
WHERE 
    CartID = @CartID
AND
    ModuleID = @ModuleID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartList]
GO

/* Update script Ecommerce - 1732 fixes, by Manu [manu-dea@hotmail dot it] */
CREATE Procedure rb_CartList
(
    @CartID nvarchar(50),
    @ModuleID int,
    @WorkflowVersion int
)
AS
IF ( @WorkflowVersion = 1 )
BEGIN
	SELECT 
	    rb_Products.ProductID, 
	    rb_Products.ModelName,
	    rb_Products.ModelNumber,
	    rb_Cart.Quantity,
	    rb_Products.UnitPrice as UnitCost,
	    (rb_Products.UnitPrice + (rb_Products.UnitPrice * rb_Products.TaxRate / 100)) as UnitCostWithTaxes,	    
	    Cast(((rb_Products.UnitPrice + (rb_Products.UnitPrice * rb_Products.TaxRate / 100)) * rb_Cart.Quantity) as money) as ExtendedAmount,
	    (SELECT SettingValue FROM rb_ModuleSettings WHERE ModuleID = @ModuleID AND SettingName = 'Currency') as ISOCurrencySymbol,
	    rb_Products.Weight,
        rb_Cart.MetadataXml
	FROM 
	    rb_Products,
	    rb_Cart
	WHERE 
	    rb_Products.ProductID = rb_Cart.ProductID
	  AND 
	    rb_Cart.CartID = @CartID
	  AND
	    rb_Cart.ModuleID = @ModuleID
	ORDER BY 
	    rb_Products.ModelName, 
	    rb_Products.ModelNumber
END
ELSE
BEGIN
	SELECT 
	    rb_Products_st.ProductID, 
	    rb_Products_st.ModelName,
	    rb_Products_st.ModelNumber,
	    rb_Cart.Quantity,
	    rb_Products_st.UnitPrice as UnitCost,
	    (rb_Products_st.UnitPrice + (rb_Products_st.UnitPrice * rb_Products_st.TaxRate / 100)) as UnitCostWithTaxes,	    
	    Cast(((rb_Products_st.UnitPrice + (rb_Products_st.UnitPrice * rb_Products_st.TaxRate / 100)) * rb_Cart.Quantity) as money) as ExtendedAmount,
	    (SELECT SettingValue FROM rb_ModuleSettings WHERE ModuleID = @ModuleID AND SettingName = 'Currency') as ISOCurrencySymbol,
	    rb_Products_st.Weight,
        rb_Cart.MetadataXml
	FROM 
	    rb_Products_st,
	    rb_Cart
	WHERE 
	    rb_Products_st.ProductID = rb_Cart.ProductID
	  AND 
	    rb_Cart.CartID = @CartID
	  AND
	    rb_Cart.ModuleID = @ModuleID
	ORDER BY 
	    rb_Products_st.ModelName, 
	    rb_Products_st.ModelNumber
 END
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartMigrate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartMigrate]
GO

CREATE Procedure rb_CartMigrate
(
    @OriginalCartID nvarchar(50),
    @NewCartID      nvarchar(50)
)
AS
DECLARE @ItemCount int
/* check if items (for any module) in original cart */
SELECT 
    @ItemCount = COUNT(ProductID)
FROM 
    rb_Cart
WHERE 
    CartID = @OriginalCartID
/* if the original cart is not empty, clear the items already present in the destination cart */
IF (@ItemCount > 0)
BEGIN
DELETE FROM rb_Cart
   WHERE 
   CartID = @NewCartID
END
/* migrate the cart */
UPDATE 
    rb_Cart
SET 
    CartID = @NewCartID 
WHERE 
    CartID = @OriginalCartID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartRemoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartRemoveItem]
GO

CREATE Procedure rb_CartRemoveItem
(
    @CartID nvarchar(50),
    @ProductID int,
    @ModuleID int,
    @MetadataXml nvarchar(3000)
)
AS
DELETE FROM rb_Cart
WHERE 
    CartID = @CartID
  AND
    ProductID = @ProductID
  AND
    ModuleID = @ModuleID
  AND
    MetadataXml = @MetadataXml
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartTotal]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartTotal]
GO

CREATE Procedure rb_CartTotal
(
    @CartID    nvarchar(50),
    @IncludeTaxes bit = false,
    @ModuleID int,
    @WorkflowVersion int,
    @TotalCost money OUTPUT,
    @ISOCurrencySymbol	char(3) OUTPUT
)
AS
IF (@IncludeTaxes = 0)
BEGIN
	IF ( @WorkflowVersion = 1 )
	BEGIN
		SELECT 
		    @TotalCost = SUM(rb_Products.UnitPrice * rb_Cart.Quantity)
		FROM 
		    rb_Cart,
		    rb_Products
		WHERE
		    rb_Cart.CartID = @CartID
		  AND
		    rb_Products.ProductID = rb_Cart.ProductID
		  AND
		    rb_Cart.ModuleID = @ModuleID
	END
	ELSE
	BEGIN
		SELECT 
		    @TotalCost = SUM(rb_Products_st.UnitPrice * rb_Cart.Quantity)
		FROM 
		    rb_Cart,
		    rb_Products_st
		WHERE
		    rb_Cart.CartID = @CartID
		  AND
		    rb_Products_st.ProductID = rb_Cart.ProductID
		  AND
		    rb_Cart.ModuleID = @ModuleID
	END
END

ELSE

BEGIN
	IF ( @WorkflowVersion = 1 )
	BEGIN
		SELECT 
		    @TotalCost = SUM((rb_Products.UnitPrice + (rb_Products.UnitPrice * rb_Products.TaxRate / 100)) * rb_Cart.Quantity)
		FROM 
		    rb_Cart,
		    rb_Products
		WHERE
		    rb_Cart.CartID = @CartID
		  AND
		    rb_Products.ProductID = rb_Cart.ProductID
		  AND
		    rb_Cart.ModuleID = @ModuleID
	END
	ELSE
	BEGIN
		SELECT 
		    @TotalCost = SUM((rb_Products_st.UnitPrice + (rb_Products_st.UnitPrice * rb_Products_st.TaxRate / 100)) * rb_Cart.Quantity)
		FROM 
		    rb_Cart,
		    rb_Products_st
		WHERE
		    rb_Cart.CartID = @CartID
		  AND
		    rb_Products_st.ProductID = rb_Cart.ProductID
		  AND
		    rb_Cart.ModuleID = @ModuleID
	END
END

SELECT @ISOCurrencySymbol = (SELECT    SettingValue
                            FROM      rb_ModuleSettings
                            WHERE     (ModuleID = @ModuleID) AND (SettingName = N'Currency') )
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartTotalShipping]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartTotalShipping]
GO

CREATE Procedure rb_CartTotalShipping
    (
        @CartID         nvarchar(50),
        @CountryID      nchar(2),
        @ModuleID int,
        @WorkflowVersion int,
        @TotalShipping  money OUTPUT
    )
    AS

SELECT @TotalShipping = 
        rb_ShipPrices.Price
        *
        CAST((
        	SELECT SUM(rb_Products_st.Weight * rb_Cart.Quantity) AS TotalWeight 
             FROM rb_Cart INNER JOIN rb_Products_st ON rb_Cart.ProductID = rb_Products_st.ProductID 
	WHERE (rb_Cart.CartID = @CartID AND rb_Cart.ModuleID = @ModuleID)
	) *100 AS int)
        /
       CAST(SUM(rb_ShipPrices.Weight)*100 AS int)
FROM         
	rb_ShipPrices INNER JOIN
             rb_ShipZones ON rb_ShipPrices.ShipPriceID = rb_ShipZones.ShipPriceId
WHERE     
	rb_ShipZones.CountryID = @CountryID

GROUP BY rb_ShipPrices.Weight, rb_ShipPrices.Price
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartUpdate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartUpdate]
GO

CREATE Procedure rb_CartUpdate
(
    @CartID    nvarchar(50),
    @ProductID int,
    @Quantity  int,
    @ModuleID int,
    @MetadataXml nvarchar(3000)
)
AS
UPDATE rb_Cart
SET 
    Quantity = @Quantity
WHERE 
    CartID = @CartID 
  AND 
    ProductID = @ProductID
  AND
    ModuleID = @ModuleID
  AND 
    MetadataXml = @MetadataXml
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_DeleteProduct]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteProduct]
GO

CREATE PROCEDURE rb_DeleteProduct
(
    @ProductID int
)
AS
DELETE FROM
    rb_Products_st
WHERE
    ProductID = @ProductID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetProducts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetProducts]
GO

CREATE  Procedure rb_GetProducts
(
	@CategoryID int,
	@ModuleID int,
	@WorkflowVersion int
)
AS
IF ( @WorkflowVersion = 1 )
BEGIN
	
	IF @CategoryID = -1
		SELECT
	        ProductID,
		    DisplayOrder,
		    ModelNumber,
		    ModelName,
		    UnitPrice,
		    FeaturedItem,
		    LongDescription,
		    ShortDescription,
		    MetadataXml,
		    Weight,
		    TaxRate
		FROM 
		    rb_Products
		WHERE 
		    FeaturedItem = 1 AND ModuleID = @ModuleID
		ORDER BY 
		    CategoryID, DisplayOrder, ModelName, ModelNumber
	ELSE
		SELECT 
		    ProductID,
		    DisplayOrder,
		    ModelNumber,
		    ModelName,
		    UnitPrice, 
		    FeaturedItem,
		    LongDescription,
		    ShortDescription,
		    MetadataXml,
		    Weight,
		    TaxRate
		FROM 
		    rb_Products
		WHERE 
		    CategoryID = @CategoryID AND ModuleID = @ModuleID
		ORDER BY 
		    DisplayOrder, ModelName, ModelNumber
END 
ELSE
BEGIN
	IF @CategoryID = -1
		SELECT
	        ProductID,
		    DisplayOrder,
		    ModelNumber,
		    ModelName,
		    UnitPrice,
		    FeaturedItem,
		    LongDescription,
		    ShortDescription,
		    MetadataXml,
		    Weight,
		    TaxRate
		FROM 
		    rb_Products_st
		WHERE 
		    FeaturedItem = 1 AND ModuleID = @ModuleID
		ORDER BY 
		    CategoryID, DisplayOrder, ModelName, ModelNumber
	ELSE
		SELECT 
		    ProductID,
		    DisplayOrder,
		    ModelNumber,
		    ModelName,
		    UnitPrice,
		    FeaturedItem,
		    LongDescription,
		    ShortDescription,
		    MetadataXml,
		    Weight,
		    TaxRate
		FROM 
		    rb_Products_st
		WHERE 
		    CategoryID = @CategoryID AND ModuleID = @ModuleID
		ORDER BY 
		    DisplayOrder, ModelName, ModelNumber
END
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetProductsCategoryList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetProductsCategoryList]
GO

CREATE   PROCEDURE rb_GetProductsCategoryList
(
@ModuleID int
)
AS
--Create a Temporary table to hold the tabs for this query
CREATE TABLE #TabTree
(
        [TabID] [int],
        [TabName] [nvarchar] (50),
        [ParentTabID] [int],
        [TabOrder] [int],
        [NestLevel] [int],
        [TreeOrder] [varchar] (1000)
)
SET NOCOUNT ON  -- Turn off echo of "... row(s) affected"
DECLARE @LastLevel smallint
SET @LastLevel = 0
-- First, the parent levels: just the TabID for the Shop Module
INSERT INTO     #TabTree
SELECT  rb_Tabs.TabID,
        rb_Tabs.TabName,
        rb_Tabs.ParentTabID,
        rb_Tabs.TabOrder,
        0,
        cast(100000000 + rb_Tabs.taborder as varchar)
FROM         rb_Modules INNER JOIN rb_Tabs ON rb_Modules.TabID = rb_Tabs.TabID
WHERE   ModuleID = @ModuleID
-- Next, the children levels
WHILE (@@rowcount > 0)
BEGIN
  SET @LastLevel = @LastLevel + 1
  INSERT        #TabTree (TabID, TabName, ParentTabID, TabOrder, NestLevel, TreeOrder) 
                SELECT  rb_Tabs.TabID,
                        Replicate('-', (@LastLevel-1) *2) + rb_Tabs.TabName,
                        rb_Tabs.ParentTabID,
                        rb_Tabs.TabOrder,
                        @LastLevel,
                        cast(#TabTree.TreeOrder as varchar) + '.' + cast(100000000 + rb_Tabs.TabOrder as varchar)
                FROM    rb_Tabs join #TabTree on rb_Tabs.ParentTabID= #TabTree.TabID
                WHERE   EXISTS (SELECT 'X' FROM #TabTree WHERE TabID = rb_Tabs.ParentTabID AND NestLevel = @LastLevel - 1)
                ORDER BY #TabTree.TabOrder
END
-- Reorder the tabs by using a 2nd Temp table and an identity field to keep them straight.
-- Tiptopweb: Remove the first level as it is the TabID for the Shop
select IDENTITY(int,1,2) as ord , cast(TabID as varchar) as TabID into #tabs
from #TabTree
order by nestlevel, Treeorder
-- Change the taborder in the sirt temp table so that tabs are ordered in sequence
update #TabTree set taborder=(select ord from #tabs where cast(#tabs.TabID as int)=#TabTree.TabID) 
-- Return Temporary Table
SELECT TabID as categoryID, tabname as categoryName
FROM #TabTree 
WHERE nestlevel <> 0
order by TreeOrder
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetProductsPaged]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetProductsPaged]
GO

CREATE     PROCEDURE rb_GetProductsPaged
(
	@ModuleID int,
        	@CategoryID int,
	@Page int = 1,
	@RecordsPerPage int = 10,
	@WorkflowVersion int
)
AS
-- We don't want to return the # of rows inserted
-- into our temporary table, so turn NOCOUNT ON
SET NOCOUNT ON
--Create a temporary table
CREATE TABLE #TempItems
(
	ID		int IDENTITY,
        	ProductID	int,
	DisplayOrder	int,
	ModelNumber	nvarchar(256),
	ModelName	nvarchar(256),
	UnitPrice	money,
	FeaturedItem	bit,
	LongDescription	ntext,
	ShortDescription ntext,
	MetadataXml	ntext,
              Weight		float,
              TaxRate            float
)
IF ( @WorkflowVersion = 1 )
BEGIN
	-- We don't want to return the # of rows inserted
	-- into our temporary table, so turn NOCOUNT ON
	SET NOCOUNT ON
	-- Insert the rows from tblItems into the temp. table
	
	IF @CategoryID = -1
  	BEGIN
		INSERT INTO
		#TempItems
		(
			ProductID,
			DisplayOrder,
			ModelNumber,
			ModelName,
			UnitPrice,
			FeaturedItem,
			LongDescription,
			ShortDescription,
			MetadataXml,
                                        Weight,
			TaxRate
		)
		SELECT
		    	rb_Products.ProductID,
			rb_Products.DisplayOrder,
			rb_Products.ModelNumber,
			rb_Products.ModelName,
			rb_Products.UnitPrice,
			rb_Products.FeaturedItem,
			rb_Products.LongDescription,
			rb_Products.ShortDescription,
			rb_Products.MetadataXml,
			rb_Products.Weight,
			rb_Products.TaxRate
		FROM
			rb_Products
		WHERE 
		    rb_Products.FeaturedItem = 1 AND rb_Products.ModuleID = @ModuleID
		ORDER BY 
		    CategoryID, DisplayOrder, ModelName, ModelNumber
	END
	ELSE
	BEGIN
		INSERT INTO
		#TempItems
		(
			ProductID,
			DisplayOrder,
			ModelNumber,
			ModelName,
			UnitPrice,
			FeaturedItem,
			LongDescription,
			ShortDescription,
			MetadataXml,
                                        Weight,
			TaxRate
		)
		SELECT
		    	rb_Products.ProductID,
			rb_Products.DisplayOrder,
			rb_Products.ModelNumber,
			rb_Products.ModelName,
			rb_Products.UnitPrice,
			rb_Products.FeaturedItem,
			rb_Products.LongDescription,
			rb_Products.ShortDescription,
			rb_Products.MetadataXml,
			rb_Products.Weight,
			rb_Products.TaxRate
	FROM
			rb_Products
		WHERE 
		    rb_Products.CategoryID = @CategoryID AND rb_Products.ModuleID = @ModuleID
		ORDER BY 
		    DisplayOrder, ModelName, ModelNumber
	END
END
ELSE
BEGIN
	-- We don't want to return the # of rows inserted
	-- into our temporary table, so turn NOCOUNT ON
	SET NOCOUNT ON
	-- Insert the rows from tblItems into the temp. table
	
	IF @CategoryID = -1
  	BEGIN
		INSERT INTO
		#TempItems
		(
			ProductID,
			DisplayOrder,
			ModelNumber,
			ModelName,
			UnitPrice,
			FeaturedItem,
			LongDescription,
			ShortDescription,
			MetadataXml,
                                        Weight,
			TaxRate
		)
		SELECT
		    	rb_Products_st.ProductID,
			rb_Products_st.DisplayOrder,
			rb_Products_st.ModelNumber,
			rb_Products_st.ModelName,
			rb_Products_st.UnitPrice,
			rb_Products_st.FeaturedItem,
			rb_Products_st.LongDescription,
			rb_Products_st.ShortDescription,
			rb_Products_st.MetadataXml,
			rb_Products_st.Weight,
			rb_Products_st.TaxRate
		FROM
			rb_Products_st
		WHERE 
		    rb_Products_st.FeaturedItem = 1 AND rb_Products_st.ModuleID = @ModuleID
		ORDER BY 
		    CategoryID, DisplayOrder, ModelName, ModelNumber
	END
	ELSE
	BEGIN
		INSERT INTO
		#TempItems
		(
			ProductID,
			DisplayOrder,
			ModelNumber,
			ModelName,
			UnitPrice,
			FeaturedItem,
			LongDescription,
			ShortDescription,
			MetadataXml,
                                        Weight,
			TaxRate
		)
		SELECT
		    	rb_Products_st.ProductID,
			rb_Products_st.DisplayOrder,
			rb_Products_st.ModelNumber,
			rb_Products_st.ModelName,
			rb_Products_st.UnitPrice,
			rb_Products_st.FeaturedItem,
			rb_Products_st.LongDescription,
			rb_Products_st.ShortDescription,
			rb_Products_st.MetadataXml,
			rb_Products_st.Weight,
			rb_Products_st.TaxRate
		FROM
			rb_Products_st
		WHERE 
		    rb_Products_st.CategoryID = @CategoryID AND rb_Products_st.ModuleID = @ModuleID
		ORDER BY 
		    DisplayOrder, ModelName, ModelNumber
	END
END
-- Find out the first and last record we want
DECLARE @FirstRec int, @LastRec int
SELECT @FirstRec = (@Page - 1) * @RecordsPerPage
SELECT @LastRec = (@Page * @RecordsPerPage + 1)
-- Now, return the set of paged records, plus, an indiciation of we
-- have more records or not!
SELECT *, (SELECT COUNT(*) FROM #TempItems) RecordCount
FROM #TempItems
WHERE ID > @FirstRec AND ID < @LastRec
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetSingleProduct]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleProduct]
GO

CREATE PROC rb_GetSingleProduct
(
    @ProductID    int,
    @WorkflowVersion int
)
AS
IF ( @WorkflowVersion = 1 )
BEGIN
	SELECT 
		OriginalProducts.ProductID, 
		(
			SELECT TOP 1
				ProductID
			FROM 
				rb_Products
			WHERE 
				ModuleID = (SELECT ModuleID FROM rb_Products WHERE ProductID = OriginalProducts.ProductID)
				AND ProductID <> OriginalProducts.ProductID
				AND (DisplayOrder < OriginalProducts.DisplayOrder OR (DisplayOrder = OriginalProducts.DisplayOrder AND ProductID < OriginalProducts.ProductID))
			ORDER BY
				OriginalProducts.DisplayOrder - DisplayOrder, OriginalProducts.ProductID - ProductID
		) AS PreviousProductID,
		(
			SELECT TOP 1
				ProductID
			FROM 
				rb_Products
			WHERE 
				ModuleID = (SELECT ModuleID FROM rb_Products WHERE ProductID = OriginalProducts.ProductID)
				AND ProductID <> OriginalProducts.ProductID
				AND (DisplayOrder > OriginalProducts.DisplayOrder OR (DisplayOrder = OriginalProducts.DisplayOrder AND ProductID > OriginalProducts.ProductID))
			ORDER BY
				DisplayOrder - OriginalProducts.DisplayOrder , ProductID - OriginalProducts.ProductID 
		) AS NextProductID,
		OriginalProducts.ModuleID,
		OriginalProducts.CategoryID,
		OriginalProducts.DisplayOrder,
		OriginalProducts.ModelNumber,
		OriginalProducts.ModelName,
		OriginalProducts.UnitPrice,
		OriginalProducts.FeaturedItem,
		OriginalProducts.LongDescription,
		OriginalProducts.ShortDescription,
		OriginalProducts.MetadataXml,
		OriginalProducts.Weight,
		OriginalProducts.TaxRate
	FROM 
		rb_Products As OriginalProducts
	WHERE 
		ProductID = @ProductID
END
ELSE
BEGIN
	SELECT 
		OriginalProducts.ProductID, 
		(
			SELECT TOP 1
				ProductID
			FROM 
				rb_Products_st
			WHERE 
				ModuleID = (SELECT ModuleID FROM rb_Products_st WHERE ProductID = OriginalProducts.ProductID)
				AND ProductID <> OriginalProducts.ProductID
				AND (DisplayOrder < OriginalProducts.DisplayOrder OR (DisplayOrder = OriginalProducts.DisplayOrder AND ProductID < OriginalProducts.ProductID))
			ORDER BY
				OriginalProducts.DisplayOrder - DisplayOrder, OriginalProducts.ProductID - ProductID
		) AS PreviousProductID,
		(
			SELECT TOP 1
				ProductID
			FROM 
				rb_Products_st
			WHERE 
				ModuleID = (SELECT ModuleID FROM rb_Products_st WHERE ProductID = OriginalProducts.ProductID)
				AND ProductID <> OriginalProducts.ProductID
				AND (DisplayOrder > OriginalProducts.DisplayOrder OR (DisplayOrder = OriginalProducts.DisplayOrder AND ProductID > OriginalProducts.ProductID))
			ORDER BY
				DisplayOrder - OriginalProducts.DisplayOrder , ProductID - OriginalProducts.ProductID 
		) AS NextProductID,
		OriginalProducts.ModuleID,
		OriginalProducts.CategoryID,
		OriginalProducts.DisplayOrder,
		OriginalProducts.ModelNumber,
		OriginalProducts.ModelName,
		OriginalProducts.UnitPrice,
		OriginalProducts.FeaturedItem,
		OriginalProducts.LongDescription,
		OriginalProducts.ShortDescription,
		OriginalProducts.MetadataXml,
		OriginalProducts.Weight,
		OriginalProducts.TaxRate
	FROM 
		rb_Products_st As OriginalProducts
	WHERE 
		ProductID = @ProductID
END
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_UpdateProduct]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateProduct]
GO

CREATE PROC rb_UpdateProduct
(
    @ProductID           int,
    @ModuleID         int,
    @CategoryID		int,
    @DisplayOrder	int,
    @ModelNumber         nvarchar(256),
    @ModelName         nvarchar(256),
    @UnitPrice	money,
    @FeaturedItem	bit,
    @LongDescription	ntext,
    @ShortDescription	ntext,
    @MetadataXml	ntext,
    @Weight		float,
    @TaxRate                  float
)
AS
IF (@ProductID=0) OR NOT EXISTS (SELECT * FROM rb_Products_st WHERE ProductID = @ProductID)
INSERT INTO rb_Products_st
(
    ModuleID,
    CategoryID,
    DisplayOrder,
    ModelNumber,
    ModelName,
    UnitPrice,
    FeaturedItem,
    LongDescription,
    ShortDescription,
    MetadataXml,
    Weight,
    TaxRate
)
VALUES
(
    @ModuleID,
    @CategoryID,
    @DisplayOrder,
    @ModelNumber,
    @ModelName,
    @UnitPrice,
    @FeaturedItem,
    @LongDescription,
    @ShortDescription,
    @MetadataXml,
    @Weight,
    @TaxRate
)
ELSE
BEGIN
UPDATE
    rb_Products_st
SET
    ModuleID = @ModuleID,
    CategoryID = @CategoryID,
    DisplayOrder = @DisplayOrder,
    ModelNumber = @ModelNumber,
    ModelName = @ModelName,
    UnitPrice = @UnitPrice,
    FeaturedItem = @FeaturedItem,
    LongDescription = @LongDescription,
    ShortDescription = @ShortDescription,
    MetadataXml = @MetadataXml,
    Weight = @Weight,
    TaxRate = @TaxRate
WHERE
    ProductID = @ProductID
END
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetOrderDetails]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetOrderDetails]
GO

CREATE Procedure rb_GetOrderDetails
(
    @OrderID    char(24)
)
AS
-- Then, return the recordset of info
SELECT     rb_OrderDetails.ProductID, rb_OrderDetails.ModelName, rb_OrderDetails.MetadataXml, rb_OrderDetails.ModelNumber, rb_OrderDetails.UnitPrice, rb_OrderDetails.Quantity, 
                      rb_Orders.ISOCurrencySymbol, rb_OrderDetails.Weight
FROM         rb_OrderDetails INNER JOIN
                      rb_Orders ON rb_OrderDetails.OrderID = rb_Orders.OrderID
WHERE     (rb_OrderDetails.OrderID = @OrderID)
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetOrders]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetOrders]
GO

CREATE PROCEDURE rb_GetOrders
(
    @ModuleID int
)
AS
SELECT  
        rb_Orders.OrderID,
        rb_Orders.ModuleID, 
        rb_Orders.UserID, 
        rb_Orders.TotalGoods, 
        rb_Orders.TotalShipping, 
        rb_Orders.TotalTaxes, 
        rb_Orders.TotalExpenses, 
        rb_Orders.Status, 
        rb_Orders.DateCreated, 
        rb_Orders.DateModified, 
        rb_Orders.PaymentMethod, 
        rb_Orders.ShippingMethod, 
        rb_Orders.TotalWeight, 
        rb_Orders.ShippingData, 
        rb_Orders.BillingData, 
        rb_Orders.TransactionID, 
        rb_Orders.AuthCode, 
        rb_Users.Name, 
        rb_Users.Company
FROM    rb_Orders LEFT JOIN
              rb_Users ON rb_Orders.UserID = rb_Users.UserID
WHERE     (rb_Orders.ModuleID = @ModuleID)
order by rb_Orders.DateModified desc
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetOrdersByUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetOrdersByUser]
GO

CREATE PROCEDURE rb_GetOrdersByUser
(
    @ModuleID Int,
    @UserID int
)
AS
SELECT  
        rb_Orders.OrderID,
        rb_Orders.ModuleID, 
        rb_Orders.UserID, 
        rb_Orders.TotalGoods, 
        rb_Orders.TotalShipping, 
        rb_Orders.TotalTaxes, 
        rb_Orders.TotalExpenses, 
        rb_Orders.Status, 
        rb_Orders.DateCreated, 
        rb_Orders.DateModified, 
        rb_Orders.PaymentMethod, 
        rb_Orders.ShippingMethod, 
        rb_Orders.TotalWeight, 
        rb_Orders.ShippingData, 
        rb_Orders.BillingData, 
        rb_Orders.TransactionID, 
        rb_Orders.AuthCode, 
        rb_Users.Name, 
        rb_Users.Company
FROM    rb_Orders LEFT JOIN rb_Users ON rb_Orders.UserID = rb_Users.UserID
WHERE (rb_Orders.ModuleID = @ModuleID) and (rb_Users.UserID = @UserID)
order by rb_Orders.DateModified desc
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetSingleOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleOrder]
GO

CREATE PROCEDURE rb_GetSingleOrder
(
    @OrderID char(24)
)
AS
SELECT     *
FROM         rb_Orders
WHERE     (OrderID = @OrderID)
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetUserForOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetUserForOrder]
GO

CREATE Procedure rb_GetUserForOrder
(
    @OrderID	    	int,
    @UserID		int OUTPUT
)
AS
SELECT @UserID = UserID 
FROM rb_Orders
WHERE OrderID = @OrderID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_ProductsChangeCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_ProductsChangeCategory]
GO

CREATE PROC rb_ProductsChangeCategory
(
	@CategoryID int,
	@NewCategoryID int
)
AS
UPDATE rb_Products_st
SET
CategoryID = @NewCategoryID
WHERE
CategoryID = @CategoryID
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_UpdateOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateOrder]
GO

CREATE PROCEDURE rb_UpdateOrder
(
	@OrderID 		char(24),
	@UserID	 		int,
	@TotalGoods	 	money,
	@TotalShipping	 	money,
	@TotalTaxes	 	money,
	@TotalExpenses	 	money,
	@ISOCurrencySymbol	char(3),
	@Status	 		int,
	@PaymentMethod 		nvarchar(50),
	@ShippingMethod 	nvarchar(50),
	@TotalWeight	 	real,
	@WeightUnit		nvarchar(15),
	@ShippingData 		ntext,
	@BillingData		ntext
)
AS
UPDATE  rb_Orders
SET     UserID = @UserID,
	TotalGoods = @TotalGoods,
	TotalShipping = @TotalShipping,
	TotalTaxes = @TotalTaxes,
	TotalExpenses = @TotalExpenses,
	ISOCurrencySymbol = @ISOCurrencySymbol,
	Status = @Status, 
        DateModified = GetDate(),
        PaymentMethod = @PaymentMethod, 
        ShippingMethod = @ShippingMethod, 
        TotalWeight = @TotalWeight,
	WeightUnit = @WeightUnit, 
        ShippingData = @ShippingData, 
        BillingData = @BillingData
WHERE   (OrderID = @OrderID)
GO

