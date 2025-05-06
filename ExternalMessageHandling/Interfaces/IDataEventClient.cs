using ExternalMessageHandling.Models;

/// <summary>
/// The Event client interface.
/// </summary>
public interface IDataEventClient
{
    /// <summary>
    /// The Receive event which should be used from client side to receive  events.
    /// </summary>
    /// <param name="data">The model.</param>
    /// <param name="action">The action.</param>
    Task ReceiveDataEvent(DataDynamicModel data, string action);
}
