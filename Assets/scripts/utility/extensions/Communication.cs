using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constant;
using Newtonsoft.Json;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.Subscriptions;
using UnityEngine;

namespace Communications
{
    public static class Messaging
    {
        public static async Task SendMsgToPlayer(this CommunicationManager origin, string message, CommunicationConstants.MessageType type, string playerID)
        {
            await CloudCodeService.Instance.CallModuleEndpointAsync<string>("Communications",
                "SendMsgToPlayer",
                new Dictionary<string, object>
                {
                    { "message", message }, { "messageType", type },
                    { "playerId", playerID }
                });
        }

        //GM only function
        public static async Task SendMsgToAllPlayers(this CommunicationManager origin, string message, CommunicationConstants.BroadcastType type)
        {
            await CloudCodeService.Instance.CallModuleEndpointAsync<string>("Communications",
                "SendMsgToAllPlayers",
                new Dictionary<string, object>
                {
                    { "message", message }, { "messageType", type }
                });
        }

        //must call at initialization
        public static Task SubscribeToMessage(this CommunicationManager origin)
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
            
            return CloudCodeService.Instance.SubscribeToPlayerMessagesAsync(callback);
        }
        
        public static Task SubscribeToBroadCast(this CommunicationManager origin)
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
            
            return CloudCodeService.Instance.SubscribeToProjectMessagesAsync(callback);
        }
    }
}
