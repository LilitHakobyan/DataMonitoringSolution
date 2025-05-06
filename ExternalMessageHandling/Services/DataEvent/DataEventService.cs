using ExternalMessageHandling.Models;
using ExternalMessaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ExternalMessageHandling.Services.DataEvent
{
    /// <summary>
    /// The  Event Service.
    /// </summary>
    /// <param name="hubContext">The  event hub context instance.</param>
    public class DataEventService(IHubContext<DataEventHub, IDataEventClient> hubContext, ILogger<DataEventService> logger)
    {
        /// <summary>
        /// Sends an  event to hub context clients.
        /// </summary>
        /// <param name="">The .</param>
        /// <param name="action">The action.</param>
        public async Task SendEvent(DataDynamicModel data, string action)
        {
            await hubContext.Clients.All.ReceiveDataEvent(data, action);
        }

        /// <summary>
        /// Sends an update to the hub context clients.
        /// </summary>
        /// <param name="activity">The dynamic activity.</param>
        /// <param name="Data">The  data.</param> 
        public async Task SendUpdate(DynamicServiceActivity activity, IDataDynamic? data)
        {
            // sends the subscribed activity to the client without data
            if (activity == DynamicServiceActivity.Subscribed)
            {
                // send to client
                await SendEvent(new DataDynamicModel(), activity.ToString());
                return;
            }

            // nothing to send if  data is null
            if (data == null)
            {
                return;
            }

            // gets client  data
            var clientModel = this.ResolveClientModel(data);

            logger.LogInformation($" info: Status {clientModel.Status} for : {clientModel.DataId}");

            // send to client
            await SendEvent(clientModel, activity.ToString());
        }

        /// <summary>
        /// Maps a scheduler  dynamic to a client monitor  dynamic model.
        /// </summary>
        /// <param name="dynamic">The dynamic  representation.</param>
        /// <returns>Returns Monitor  Dynamic model.</returns>
        public DataDynamicModel ResolveClientModel(IDataDynamic dynamic)
        {
            return new DataDynamicModel
            {
                Status =dynamic.Status,
                DataId = dynamic.DataId,
                StartTime = dynamic.StartTimeUtc.Equals(DateTime.MinValue) ? null : dynamic.StartTimeUtc,
                CompletionTime = dynamic.CompletionTimeUtc.Equals(DateTime.MinValue) ? null : dynamic.CompletionTimeUtc,
            };
        }
    }
}
