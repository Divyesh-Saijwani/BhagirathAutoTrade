CREATE PROCEDURE [usp_GetTodayStockCalculations]
AS
BEGIN
    -- Set the date to today's date at midnight
    DECLARE @TodayStart DATETIME = CAST(GETDATE() AS DATE);
    -- Set the date to the end of today
    DECLARE @TodayEnd DATETIME = DATEADD(DAY, 1, @TodayStart);

    SELECT 
        [AlgoCalculationId],
        [Symbol],
        [Exchange],
        [Type],
        [Instrument],
        [OptionType],
        [StrikePrice],
        [Expiry],
        [LowPoint],
        [AveragePoint],
        [MaxPoint],
        [Direction],
        [EntryTime],
        [StopLoss],
        [Target],
        [CalculationDateTime],
        [IsActive]
    FROM 
        [dbo].[StockCalculation]
    WHERE 
        [CalculationDateTime] >= @TodayStart
        AND [CalculationDateTime] < @TodayEnd
END
