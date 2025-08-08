using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constant;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Newtonsoft.Json;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.Subscriptions;
using UnityEngine;

namespace Communications
{
    public static class Messaging
    {
        public static async UniTask SendMsgToPlayer(this CommunicationManager origin, string message, CommunicationConstants.MessageType type, string playerID)
        {
            await CloudCodeService.Instance.CallModuleEndpointAsync("Communications",
                "SendMsgToPlayer",
                new Dictionary<string, object>
                {
                    { "message", message }, { "messageType", type },
                    { "playerId", playerID }
                });
        }
        
        public static async UniTask SendMsgToPlayer(this CommunicationManager origin, CommunicationConstants.MessageType type, string playerID)
        {
            await CloudCodeService.Instance.CallModuleEndpointAsync("Communications",
                "SendMsgToPlayer",
                new Dictionary<string, object>
                {
                    { "message", "" }, { "messageType", type },
                    { "playerId", playerID }
                });
        }
        
        //GM only function
        public static async UniTask SendMsgToAllPlayers(this CommunicationManager origin, string message, CommunicationConstants.BroadcastType type)
        {
            await CloudCodeService.Instance.CallModuleEndpointAsync("Communications",
                "SendMsgToAllPlayers",
                new Dictionary<string, object>
                {
                    { "message", message }, { "messageType", type }
                });
        }

        //must call at initialization
        public static UniTask SubscribeToMessage(this CommunicationManager origin)
        {
            SubscriptionEventCallbacks callback = new();

            callback.MessageReceived += async @event =>
            {
                IMessageReceivedEvent evt = @event;
                Enum.TryParse(evt.MessageType, out CommunicationConstants.MessageType type);

                await origin.OnMessageReceived(type, evt);
            };
            callback.Error += @event =>
            {
                Debug.Log($"Got player subscription Error: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
            };
            
            return CloudCodeService.Instance.SubscribeToPlayerMessagesAsync(callback).AsUniTask();
        }
        
        public static UniTask<ISubscriptionEvents> SubscribeToBroadCast(this CommunicationManager origin)
        {
            SubscriptionEventCallbacks callback = new();

            callback.MessageReceived += async @event =>
            {
                IMessageReceivedEvent evt = @event;
                Enum.TryParse(evt.MessageType, out CommunicationConstants.BroadcastType type);

                await origin.OnBroadCastReceived(type, evt);
            };
            callback.Error += @event =>
            {
                Debug.Log($"Got player subscription Error: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
            };

            return CloudCodeService.Instance.SubscribeToProjectMessagesAsync(callback).AsUniTask();
        }
    }
}
