using ExternalMessageHandling.Interfaces;
using Microsoft.Extensions.Hosting;

namespace ExternalMessageHandling.Services
{
    /// <summary>
    /// The Dynamic Data Entry Service initializer.
    /// </summary>
    /// <param name="clientManager">The client manager.</param>
    public class DynamicDataClientInitializer(IDynamicDataClientManager clientManager) : IHostedService
    {
        /// <summary>
        /// Initializes the Dynamic Data Client.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await clientManager.InitializeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Stops the Dynamic Data Client Initializer.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
