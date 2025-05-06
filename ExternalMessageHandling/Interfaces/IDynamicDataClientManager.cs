using ExternalMessaging.Interfaces;

namespace ExternalMessageHandling.Interfaces
{
    /// <summary>
    /// Defines a manager for dynamic data client.
    /// </summary>
    public interface IDynamicDataClientManager
    {
        /// <summary>
        /// Initializes the dynamic data client.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Gets the Entry service.
        /// </summary>
        IDataDynamicService? GetDataDynamicService();

        /// <summary>
        /// Gets the Message Sender service.
        /// </summary>
        IMessageSenderService? GetMessageSenderService();
    }
}
