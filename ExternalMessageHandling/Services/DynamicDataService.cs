using ExternalMessageHandling.Interfaces;
using ExternalMessageHandling.Services.DataEvent;
using ExternalMessaging;
using ExternalMessaging.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExternalMessageHandling.Services
{
    /// <summary>
    /// The Dynamic Data Entry Service.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="clientManager">The Dynamic Data Client Manager instance.</param>
    public class DynamicDataService(ILogger<DynamicDataService> logger, IDynamicDataClientManager clientManager, DynamicDataSubscriptionManager subscriptionManager, DataEventService dataEventService) : BackgroundService
    {
        /// <summary>
        /// The Dynamic Service.
        /// </summary>
        private IDataDynamicService? dataService;

        /// <summary>
        /// The active subscription cancellation token source.
        /// </summary>
        private CancellationTokenSource? activeSubscriptionCts;

        /// <summary>
        /// The starting point of the Dynamic Data Entry service.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ensures that client manager is initialized because the initialization happening in another background service this won't make initialization happening twice 
            await clientManager.InitializeAsync().ConfigureAwait(false);

            // the main loop of the service stops when the cancellation token is requested
            while (!stoppingToken.IsCancellationRequested)
            {
                // wait for an external request to start the subscription
                await subscriptionManager.WaitForSubscribeTriggerAsync(stoppingToken);

                // gets entry service from the client manager
                this.dataService = clientManager.GetDataDynamicService();

                // check if cancellation is requested
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                // cancel any existing subscription
                if (activeSubscriptionCts != null)
                {
                    await activeSubscriptionCts.CancelAsync();
                }

                // creates a new cancellation token source based on linked to stopping token
                activeSubscriptionCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                try
                {
                    logger.LogInformation("Starting subscription...");
                    // starts the subscription
                    await this.StartSubscriptionAsync().ConfigureAwait(false);

                    // wait for an external request to stop the subscription
                    await subscriptionManager.WaitForUnsubscribeTriggerAsync(activeSubscriptionCts.Token);

                    logger.LogInformation("Stopping subscription...");
                    // stops the subscription
                    await activeSubscriptionCts.CancelAsync();

                    await StopSubscriptionAsync().ConfigureAwait(false);
                }
                finally
                {
                    // resets subscription state after unsubscribing
                    subscriptionManager.ResetTriggers();
                }
            }
        }

        /// <summary>
        /// Overrides the stop method.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.StopSubscriptionAsync();
        }

        /// <summary>
        /// The start subscription implementation.
        /// </summary>
        private async Task StartSubscriptionAsync()
        {
            // checks for Entry service existence
            if (!this.IsEntryServiceExists())
            {
                return;
            }

            try
            {
                // starts the subscription by passing callback method
                await dataService!.SubscribeAsync(DynamicEntryActionCallback).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Subscription to the entry dynamic service failed.");
            }
        }

        /// <summary>
        /// The stop subscription implementation.
        /// </summary>
        /// <returns></returns>
        private async Task StopSubscriptionAsync()
        {
            // checks for Entry service existence
            if (!this.IsEntryServiceExists())
            {
                return;
            }

            try
            {
                // stops the subscription
                await this.dataService!.UnsubscribeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while unsubscribing from the entry dynamic service.");
            }
        }

        /// <summary>
        /// Handles dynamic entry activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="entryId">The related entry id.</param>
        private async Task DynamicEntryActionCallback(DynamicServiceActivity activity, int entryId)
        {
            // the valid subscribe result when connected
            if (entryId == 0)
            {
                switch (activity)
                {
                    case DynamicServiceActivity.Subscribed:
                        logger.LogInformation("Successfully subscribed to entry updates.");
                        // sending Subscribed activity to the client for reconnecting case
                        await dataEventService.SendUpdate(activity, null);
                        break;
                    case DynamicServiceActivity.Unsubscribed:
                        logger.LogInformation("Successfully unsubscribed from entry updates.");
                        break;
                }

                return;
            }

            // get entry data
            var entryData = await dataService!.GetEntryAsync(entryId);

            logger.LogInformation($"Received activity: {activity} for entry: {entryId}.");

            // send update
            await dataEventService.SendUpdate(activity, entryData);
        }

        /// <summary>
        /// Checks if the Entry service exists.
        /// </summary>
        /// <returns>True if service is successfully resolved, otherwise false.</returns>
        private bool IsEntryServiceExists()
        {
            if (dataService != null)
            {
                return true;
            }

            logger.LogError("Failed to get IEntryDynamicService service.");
            return false;
        }
    }
}
