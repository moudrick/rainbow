/* Uninstall script Ecommerce, by Manu [manu-dea@hotmail dot it] */

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_AddOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddOrder]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_AddOrderDetails]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddOrderDetails]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartAddItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartAddItem]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartEmpty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartEmpty]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartItemCount]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartItemCount]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartList]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartMigrate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartMigrate]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartRemoveAllItems]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartRemoveAllItems]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartRemoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartRemoveItem]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartTotal]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartTotal]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartTotalShipping]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartTotalShipping]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_CartUpdate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_CartUpdate]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_DeleteProduct]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteProduct]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetProducts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetProducts]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetProductsCategoryList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetProductsCategoryList]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetProductsPaged]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetProductsPaged]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetSingleProduct]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleProduct]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_UpdateProduct]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateProduct]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetOrderDetails]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetOrderDetails]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetOrders]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetOrders]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetOrdersByUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetOrdersByUser]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetSingleOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleOrder]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_GetUserForOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetUserForOrder]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_ProductsChangeCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_ProductsChangeCategory]
GO

IF EXISTS (SELECT * FROM sysobjects where id = object_id(N'[rb_UpdateOrder]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateOrder]
GO
