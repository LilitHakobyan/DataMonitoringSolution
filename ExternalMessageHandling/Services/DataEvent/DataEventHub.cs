using ExternalMessageHandling.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ExternalMessageHandling.Services.DataEvent
{
    /// <summary>
    /// The Data Event Hub.
    /// </summary>
    /// <param name="subscriptionManager">The Data Event Service instance.</param>
    public class DataEventHub(DynamicDataSubscriptionManager subscriptionManager, ILogger<DataEventHub> logger) : Hub<IDataEventClient>
    {
        /// <summary>
        /// The Send Data Event method.
        /// </summary>
        /// <param name="Data">The Data model.</param>
        /// <param name="action">The action.</param>
        public async Task SendDataEvent(DataDynamicModel Data, string action)
        {
            await Clients.Caller.ReceiveDataEvent(Data, action);
        }

        /// <summary>
        /// Overrides the OnConnectedAsync method by notifying that client connected.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            logger.LogInformation("Client connected. Sending updates...");
            // request subscription when connected
            subscriptionManager.RequestSubscribe();
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Overrides the OnDisconnectedAsync method by notifying that client disconnected.
        /// </summary>
        /// <param name="exception"></param>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            logger.LogInformation("Client disconnected. Stopping updates.");
            // request unsubscribe when disconnected
            subscriptionManager.RequestUnsubscribe();
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Gets the connection id.
        /// </summary>
        /// <returns></returns>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
