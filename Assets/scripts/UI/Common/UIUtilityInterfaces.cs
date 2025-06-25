using System.Threading.Tasks;
using Unity.Services.CloudCode.Subscriptions;

public interface IGeneric
{
    public void RegisterUI(); //register the UI to UImanager
    public void UnregisterUI(); //unregister the UI to UImanager
}

public interface IRefreshable : IGeneric
{
    public Task Refresh(); //refresh function
}

public interface IResponse : IGeneric
{
    public void Init(IMessageReceivedEvent evt); //init based on server response
}