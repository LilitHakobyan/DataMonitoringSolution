
namespace ExternalMessaging.Interfaces
{
    public interface IDataDynamicService
    {
        Task<IDataDynamic> GetEntryAsync(int entryId);
        Task SubscribeAsync(Func<DynamicServiceActivity, int, Task> dynamicEntryActionCallback);
        Task UnsubscribeAsync();
    }
}
