CREATE PROCEDURE dbo.usp_AddOrUpdateBrokerAndConfigurations
    @BrokerId UNIQUEIDENTIFIER,
    @Name VARCHAR(100),
    @Description VARCHAR(MAX),
    @APIDocumentationUrl VARCHAR(MAX),
    @AuthenticationUrl VARCHAR(MAX),
    @IsActive BIT,
    @Configurations dbo.ConfigurationsType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if broker name already exists
        IF EXISTS (SELECT 1 FROM Broker WHERE Name = @Name)
        BEGIN
            RAISERROR ('Broker name already exists.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Upsert Broker
        IF EXISTS (SELECT 1 FROM Broker WHERE BrokerId = @BrokerId)
        BEGIN
            UPDATE Broker
            SET
                Name = @Name,
                Description = @Description,
                APIDocumentationUrl = @APIDocumentationUrl,
                AuthenticationUrl = @AuthenticationUrl,
                IsActive = @IsActive
            WHERE BrokerId = @BrokerId;
        END
        ELSE
        BEGIN
            INSERT INTO Broker (BrokerId, Name, Description, APIDocumentationUrl, AuthenticationUrl, IsActive)
            VALUES (@BrokerId, @Name, @Description, @APIDocumentationUrl, @AuthenticationUrl, @IsActive);
        END

        -- Upsert BrokerConfigurations
        DECLARE @ConfigKey VARCHAR(50), @ConfigValue VARCHAR(50), @NeedsToUpdateDaily BIT, @BrokerConfigurationId UNIQUEIDENTIFIER;

        DECLARE ConfigCursor CURSOR FOR
        SELECT ConfigKey, ConfigValue, NeedsToUpdateDaily, ConfigurationId as BrokerConfigurationId
        FROM @Configurations;

        OPEN ConfigCursor;
        FETCH NEXT FROM ConfigCursor INTO @ConfigKey, @ConfigValue, @NeedsToUpdateDaily, @BrokerConfigurationId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @BrokerConfigurationId IS NOT NULL
            BEGIN
                -- Update existing BrokerConfiguration
                UPDATE BrokerConfiguration
                SET
                    ConfigKey = @ConfigKey,
                    NeedsToUpdateDaily = @NeedsToUpdateDaily
                WHERE BrokerConfigurationId = @BrokerConfigurationId;
            END
            ELSE
            BEGIN
                -- Insert new BrokerConfiguration
                INSERT INTO BrokerConfiguration (BrokerConfigurationId, BrokerId, ConfigKey, NeedsToUpdateDaily)
                VALUES (NEWID(), @BrokerId, @ConfigKey, @NeedsToUpdateDaily);
            END

            FETCH NEXT FROM ConfigCursor INTO @ConfigKey, @ConfigValue, @NeedsToUpdateDaily, @BrokerConfigurationId;
        END;

        CLOSE ConfigCursor;
        DEALLOCATE ConfigCursor;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Return the error message
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
