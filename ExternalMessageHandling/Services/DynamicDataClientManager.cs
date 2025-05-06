using ExternalMessageHandling.Interfaces;
using ExternalMessaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace ExternalMessageHandling.Services
{
    /// <summary>
    /// DynamicDataClientManager service is responsible for initializing IDynamicDataClientService and
    /// providing access to DataDynamicService and MessageSenderService to other services.
    /// </summary>
    public class DynamicDataClientManager : IDynamicDataClientManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DynamicDataClientManager> logger;

        /// <summary>
        /// The Dynamic Data client service task, uses Lazy Task to ensure single async initialization.
        /// </summary>
        private readonly Lazy<Task> dynamicDataClientServiceTask;

        /// <summary>
        /// The Dynamic Data client service.
        /// </summary>
        private readonly IDynamicDataClientService dynamicDataClientService;

        /// <summary>
        /// Initializes a new instance of the DynamicDataClientManager class.
        /// </summary>
        public DynamicDataClientManager(ILogger<DynamicDataClientManager> logger, IDynamicDataClientService dynamicDataClientService)
        {
            this.logger = logger;
            this.dynamicDataClientService = dynamicDataClientService;
            this.dynamicDataClientServiceTask = new Lazy<Task>(InitializeClientServiceTaskAsync);
        }

        /// <summary>
        /// Initializes and resolves the Dynamic Data client service instance.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            await this.dynamicDataClientServiceTask.Value.ConfigureAwait(false);
        }

        /// <summary>
        /// Initializes the Dynamic Data client service task.
        /// </summary>
        private async Task InitializeClientServiceTaskAsync()
        {
            try
            {
                // initializes the client service
                var connectionResult = await this.dynamicDataClientService.InitAsync("WebClient").ConfigureAwait(false);
                if (!connectionResult)
                {
                    logger.LogError("Failed to connect Dynamic Data Client Service");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize Dynamic Data Client Service");
                logger.LogError("The messaging to the scheduler failed. Jobs will be submitted but not communicated to the scheduler");
            }
        }

        /// <summary>
        /// Gets the Entry service.
        /// </summary>
        /// <returns>The Entry Dynamic service.</returns>
        public IDataDynamicService? GetDataDynamicService()
        {
            try
            {
                return this.dynamicDataClientService.GetDataDynamicService();
            }
            catch (Exception)
            {
                // if there is an issue while getting the Entry service, return null
                return null;
            }
        }

        /// <summary>
        /// Gets the Message Sender service.
        /// </summary>
        /// <returns>The Message Sender Service.</returns>
        public IMessageSenderService? GetMessageSenderService()
        {
            try
            {
                return this.dynamicDataClientService.GetMessageSenderService();
            }
            catch (Exception)
            {
                // if there is an issue while getting the Message Sender service, return null
                return null;
            }
        }
    }
}
