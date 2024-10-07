CREATE PROCEDURE dbo.usp_GetBrokerAndConfigurations
@BrokerId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.BrokerId,
        b.Name,
        b.Description,
        b.APIDocumentationUrl,
        b.AuthenticationUrl,
        b.IsActive,
        bc.BrokerConfigurationId,
        bc.ConfigKey,
        bc.NeedsToUpdateDaily
    FROM
        Broker b
    LEFT JOIN
        BrokerConfiguration bc ON b.BrokerId = bc.BrokerId
    WHERE
        b.BrokerId = @BrokerId;
END;
