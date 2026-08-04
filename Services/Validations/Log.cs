namespace RealEstate.Services.Validations;

#pragma warning disable CA1515
public static partial class LogMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "An unexpected error occurred.")]
    public static partial void UnexpectedError(
        ILogger logger,
        Exception ex);
}
