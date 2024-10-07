CREATE TYPE dbo.ConfigurationsType AS TABLE
(
    BrokerId UNIQUEIDENTIFIER,
    ConfigurationId UNIQUEIDENTIFIER,
    ConfigKey VARCHAR(50),
    ConfigValue VARCHAR(50),
    NeedsToUpdateDaily BIT DEFAULT 0
);