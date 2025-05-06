namespace ExternalMessageHandling.Services
{
    /// <summary>
    ///  Manages subscription and unsubscribe requests to dynamic data.
    /// </summary>
    public class DynamicDataSubscriptionManager
    {
        /// <summary>
        /// Lock object to ensure thread safety.
        /// </summary>
        private readonly object _lock = new();

        /// <summary>
        /// The task completion source to trigger subscription requests.
        /// </summary>
        private TaskCompletionSource<bool> subscribeTrigger = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>
        /// The task completion source to trigger unsubscribe requests.
        /// </summary>
        private TaskCompletionSource<bool> unsubscribeTrigger = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>
        /// Requests a new subscription by setting the subscription trigger.
        /// </summary>
        public void RequestSubscribe()
        {
            // ensures only one thread modifies subscribeTrigger at a time
            lock (_lock)
            {
                // reset the trigger only if it is already completed to allow re-subscription
                if (subscribeTrigger.Task.IsCompleted)
                {
                    subscribeTrigger = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                }

                // notifies the WaitForSubscribeTriggerAsync method that a subscription request has been made
                subscribeTrigger.TrySetResult(true);
            }
        }

        /// <summary>
        /// Requests an unsubscribe by setting the unsubscribe trigger.
        /// </summary>
        public void RequestUnsubscribe()
        {
            // ensures only one thread modifies subscribeTrigger at a time
            lock (_lock)
            {
                // reset the trigger only if it is already completed to allow re-unsubscribe
                if (unsubscribeTrigger.Task.IsCompleted)
                {
                    unsubscribeTrigger = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                }

                // notifies the WaitForUnsubscribeTriggerAsync method that an unsubscribe request has been made
                unsubscribeTrigger.TrySetResult(true);
            }
        }

        /// <summary>
        /// Waits for a subscription request to be triggered.
        /// </summary>
        /// <param name="stoppingToken">Token to cancel waiting if the service stops.</param>
        public async Task WaitForSubscribeTriggerAsync(CancellationToken stoppingToken)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            // detects when a subscription request is made
            await Task.WhenAny(subscribeTrigger.Task, Task.Delay(Timeout.Infinite, linkedCts.Token));
        }

        /// <summary>
        /// Waits for an unsubscribe request to be triggered.
        /// </summary>
        /// <param name="stoppingToken">Token to cancel waiting if the service stops.</param>
        public async Task WaitForUnsubscribeTriggerAsync(CancellationToken stoppingToken)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            await Task.WhenAny(unsubscribeTrigger.Task, Task.Delay(Timeout.Infinite, linkedCts.Token));
        }

        /// <summary>
        /// Resets both the subscribe and unsubscribe triggers.
        /// This ensures that the next subscription/unsubscribe requests will be properly awaited.
        /// </summary>
        public void ResetTriggers()
        {
            lock (_lock)
            {
                subscribeTrigger = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                unsubscribeTrigger = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }
    }
}
