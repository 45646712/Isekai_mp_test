using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace Communications;

public class Communications
{
    [CloudCodeFunction("SendMsgToPlayer")]
    public async Task SendMsgToPlayer(IExecutionContext context, PushClient pushClient, string message, string messageType, string playerId)
    {
        SendMessageReply response = await pushClient.SendPlayerMessageAsync(context, message, messageType, playerId);
    }

    [CloudCodeFunction("SendMsgToAllPlayers")]
    public async Task SendMsgToAllPlayers(IExecutionContext context, PushClient pushClient, string message, string messageType)
    {
        SendMessageReply response = await pushClient.SendProjectMessageAsync(context, message, messageType);
    }
}

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(PushClient.Create());
    }
}