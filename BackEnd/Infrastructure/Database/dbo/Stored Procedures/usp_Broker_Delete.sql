CREATE PROCEDURE [dbo].[usp_Broker_Delete]
    @BrokerId VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

        -- Attempt to update the IsActive flag
        UPDATE [dbo].[Broker]
        SET IsActive = 0
        WHERE BrokerId = @BrokerId;

END;
