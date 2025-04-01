-- Script to clean up all data in the SCPM database
-- Disable foreign key constraint checks
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'

-- Delete data from all tables in reverse order of dependencies
DELETE FROM [dbo].[FCM]
DELETE FROM [dbo].[TaskEach]
DELETE FROM [dbo].[PaymentContract]
DELETE FROM [dbo].[ParkingStatusSensor]
DELETE FROM [dbo].[EntryExitLog]
DELETE FROM [dbo].[Contract]
DELETE FROM [dbo].[Car]
DELETE FROM [dbo].[Feedback]
DELETE FROM [dbo].[Customer]
DELETE FROM [dbo].[Staff]
DELETE FROM [dbo].[ParkingSpace]
DELETE FROM [dbo].[Floor]
DELETE FROM [dbo].[Area]
DELETE FROM [dbo].[ParkingLotPriceHistory]
DELETE FROM [dbo].[ParkingLot]
DELETE FROM [dbo].[Owner]

-- Reset all identity columns
EXEC sp_MSforeachtable 'IF OBJECTPROPERTY(OBJECT_ID(''?''), ''TableHasIdentity'') = 1 DBCC CHECKIDENT(''?'', RESEED, 0)'

-- Re-enable foreign key constraint checks
EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'

-- Confirmation message
PRINT 'All data has been deleted from the database and identity columns have been reset.' 