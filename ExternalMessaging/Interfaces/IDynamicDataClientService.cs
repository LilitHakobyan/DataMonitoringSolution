namespace ExternalMessaging.Interfaces
{
    public interface IDynamicDataClientService
    {
        Task<bool> InitAsync(string clientName);
        IDataDynamicService? GetDataDynamicService();
        IMessageSenderService? GetMessageSenderService();
      
    }
}
