CREATE PROCEDURE [usp_InsertStockCalculation]
    @Symbol VARCHAR(50),
    @Exchange VARCHAR(10),
    @Type VARCHAR(10),
    @Instrument VARCHAR(10),
    @OptionType VARCHAR(5),
    @StrikePrice DECIMAL(18, 2),
    @Expiry DATETIME,
    @LowPoint DECIMAL(18, 2),
    @AveragePoint DECIMAL(18, 2),
    @MaxPoint DECIMAL(18, 2),
    @Direction VARCHAR(10),
    @EntryTime DATETIME,
    @StopLoss DECIMAL(18, 2),
    @Target DECIMAL(18, 2),
    @IsActive BIT
AS
BEGIN
    BEGIN TRY
        DECLARE @NewAlgoCalculationId UNIQUEIDENTIFIER = NEWID();

        INSERT INTO [dbo].[StockCalculation] 
            ([AlgoCalculationId], [Symbol], [Exchange], [Type], [Instrument], [OptionType], [StrikePrice], [Expiry], 
             [LowPoint], [AveragePoint], [MaxPoint], [Direction], [EntryTime], [StopLoss], [Target], 
             [CalculationDateTime], [IsActive])
        OUTPUT INSERTED.[AlgoCalculationId]
        VALUES 
            (@NewAlgoCalculationId, @Symbol, @Exchange, @Type, @Instrument, @OptionType, @StrikePrice, @Expiry, 
             @LowPoint, @AveragePoint, @MaxPoint, @Direction, @EntryTime, @StopLoss, @Target, 
             GETDATE(), @IsActive);
    END TRY
    BEGIN CATCH
        -- Handle error
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        -- Log the error or handle it as needed
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END