DECLARE @owner VARCHAR(MAX) = 'owner'
DECLARE @instrument VARCHAR(MAX) = 'instrument'
DECLARE @portfolio VARCHAR(MAX) = 'portfolio'

DECLARE @ownerId INT SELECT @ownerId = Id FROM [dbo].[InstrumentOwners] WHERE [Name] = @owner
DECLARE @instrumentId INT SELECT @instrumentId = Id FROM [dbo].[Instruments] WHERE [Name] = @instrument
DECLARE @portfolioId INT SELECT @portfolioId = Id FROM [dbo].[Portfolios] WHERE [Name] = @portfolio

IF @ownerId IS NULL 
	BEGIN
		INSERT INTO [dbo].[InstrumentOwners]([Name]) VALUES(@owner)
		SELECT @ownerId = Id FROM [dbo].[InstrumentOwners] WHERE [Name] = @owner
	END

IF @instrumentId IS NULL 
	BEGIN
		INSERT INTO [dbo].[Instruments]([Name]) VALUES(@instrument)
		SELECT @instrumentId = Id FROM [dbo].[Instruments] WHERE [Name] = @instrument
	END

IF @portfolioId IS NULL 
	BEGIN
		INSERT INTO [dbo].[Portfolios]([Name]) VALUES(@portfolio)
		SELECT @portfolioId = Id FROM [dbo].[Portfolios] WHERE [Name] = @portfolio
	END

INSERT INTO [dbo].[PriceModels]([PortfolioId], [InstrumentOwnerId], [InstrumentId], [Date], [Price])
	VALUES (@portfolioId, @ownerId, @instrumentId, 'date', 'price')