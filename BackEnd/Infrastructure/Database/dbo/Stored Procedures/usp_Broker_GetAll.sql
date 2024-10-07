CREATE PROCEDURE [dbo].[usp_Broker_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BrokerId, Name, Description, APIDocumentationUrl, AuthenticationUrl, IsActive
    FROM [dbo].[Broker];
END
GO
