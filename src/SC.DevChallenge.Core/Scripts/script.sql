CREATE TABLE #priceModels (
  [Portfolio] VARCHAR(MAX),
  [Owner] VARCHAR(MAX),  
  [Instrument] VARCHAR(MAX),
  [Date] VARCHAR(MAX),
  Price DECIMAL(5, 2)
)

BULK INSERT #priceModels
FROM '@path'
WITH
(
  FIRSTROW = 2,
  DATAFILETYPE='char',
  FIELDTERMINATOR = ',',
  ROWTERMINATOR = '\n'
)

INSERT INTO [dbo].[InstrumentOwners] SELECT [Owner] FROM #priceModels GROUP BY [Owner]
INSERT INTO [dbo].[Instruments] SELECT [Instrument] FROM #priceModels GROUP BY [Instrument]
INSERT INTO [dbo].[Portfolios] SELECT [Portfolio] FROM #priceModels GROUP BY [Portfolio]

INSERT INTO [dbo].[PriceModels]([PortfolioId], [InstrumentOwnerId], [InstrumentId], [Date], [Price])
	SELECT [PortfolioId] = p.[Id], [InstrumentOwnerId] = o.[Id], [InstrumentId] = i.[Id], [Date] = CONVERT(datetime, m.[Date], 103), [Price] = m.Price
	FROM #priceModels AS m, [dbo].[InstrumentOwners] AS o, [dbo].[Instruments] AS i, [dbo].[Portfolios] AS p
	WHERE m.[Portfolio] = p.[Name] AND m.[Owner] = o.[Name] AND m.[Instrument] = i.[Name]

DROP TABLE #priceModels
