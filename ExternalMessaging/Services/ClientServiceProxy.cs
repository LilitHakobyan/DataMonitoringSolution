using ExternalMessaging.Interfaces;

namespace ExternalMessaging.Services
{
    public class ClientServiceProxy : IDynamicDataClientService
    {
        public IDataDynamicService? GetDataDynamicService()
        {
            throw new NotImplementedException();
        }

        public IMessageSenderService? GetMessageSenderService()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InitAsync(string clientName)
        {
            throw new NotImplementedException();
        }
    }
}
