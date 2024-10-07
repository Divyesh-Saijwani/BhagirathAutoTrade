CREATE PROCEDURE dbo.usp_AddOrUpdateUserBrokerAndConfigurations
    @UserId INT,
    @BrokerId INT,
    @Configurations dbo.ConfigurationsType READONLY
AS
BEGIN
    DECLARE @UserBrokerId INT;

    -- Check if UserBroker entry already exists
    IF EXISTS (SELECT 1 FROM UserBroker WHERE UserId = @UserId AND BrokerId = @BrokerId)
    BEGIN
        -- Update existing UserBroker entry
        UPDATE UserBroker
        SET IsActive = 1
        WHERE UserId = @UserId AND BrokerId = @BrokerId;
        
        -- Get the UserBrokerId of the existing entry
        SET @UserBrokerId = (SELECT UserBrokerId FROM UserBroker WHERE UserId = @UserId AND BrokerId = @BrokerId);
    END
    ELSE
    BEGIN
        -- Insert new UserBroker entry and get the new UserBrokerId
        INSERT INTO UserBroker (UserId, BrokerId, IsActive)
        OUTPUT INSERTED.UserBrokerId
        VALUES (@UserId, @BrokerId, 1);
        
        -- Set the new UserBrokerId
        SET @UserBrokerId = SCOPE_IDENTITY();
    END

    -- Merge the new configurations with existing ones
    MERGE INTO UserBrokerConfiguration AS target
    USING @Configurations AS source
    ON target.UserBrokerId = @UserBrokerId AND target.ConfigKey = source.ConfigKey
    WHEN MATCHED THEN
        UPDATE SET target.ConfigValue = source.ConfigValue
    WHEN NOT MATCHED THEN
        INSERT (UserBrokerId, ConfigKey, ConfigValue)
        VALUES (@UserBrokerId, source.ConfigKey, source.ConfigValue);
END;
