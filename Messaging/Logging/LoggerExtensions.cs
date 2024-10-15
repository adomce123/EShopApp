using Microsoft.Extensions.Logging;

namespace Messaging.Logging
{
    public static class LoggerExtensions
    {
        public static void LogWithOrderAndCorrelationIds(this ILogger logger, string message, object orderId, object correlationId)
        {
            logger.LogInformation("{Message} OrderId: {OrderId}, Corr Id: {CorrelationId}", message, orderId, correlationId);
        }
    }
}
