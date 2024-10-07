CREATE PROCEDURE usp_GetUserBrokerDetailsForUpdate
    @BrokerId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        b.[BrokerId],
        b.[Name],
        b.[Description],
        b.[APIDocumentationUrl],
        b.[AuthenticationUrl],
        ub.[UserBrokerId],
        ub.[UserId],
        ubc.[UserBrokerConfigurationId],
        ubc.[ConfigKey] AS UserConfigKey,
        ubc.[ConfigValue] AS UserConfigValue,
        ubc.[NeedsToChangeEveryDay],
        ubc.[LastModifiedDate]
    FROM 
        Broker b
    LEFT JOIN 
        UserBroker ub ON ub.BrokerId = b.BrokerId
    LEFT JOIN 
        UserBrokerConfiguration ubc ON ub.UserBrokerId = ubc.UserBrokerId
    WHERE 
        b.BrokerId = @BrokerId
        AND ub.UserId = @UserId
        --AND ubc.NeedsToChangeEveryDay = 1
        AND CAST(ubc.LastModifiedDate AS DATE) <> CAST(GETDATE() AS DATE);
END
