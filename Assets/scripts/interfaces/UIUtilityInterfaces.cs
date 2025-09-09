using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudCode.Subscriptions;

public interface IGeneric
{
    public void RegisterUI(); //register the UI to UImanager
    public void UnregisterUI(); //unregister the UI to UImanager
}

public interface IRefreshable : IGeneric
{
    public UniTask Refresh(); //refresh function
}

public interface IResponse : IGeneric
{
    public void Init(IMessageReceivedEvent evt); //init based on server response
}

public interface IMultiResponse : IResponse
{
    public void Init(Queue<IMessageReceivedEvent> evt); //init based on queued server responses
}