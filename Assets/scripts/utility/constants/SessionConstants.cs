using System;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Constant
{        
    public static class SessionConstants
    {
        public enum SessionOwnership
        {
            Host,
            Client
        }
        
        public enum SessionPrivacy
        {
            Public,
            Public_friend,
            AskToJoin,
            AskToJoin_friend,
            Private,
            SinglePlayer
        }

        public enum PropertyKeys
        {
            IsAskToJoin,
            IsFriendOnly,
        }
        
        public static Dictionary<SessionPrivacy, string> PrivacyStateToString { get; } = new()
        {
            { SessionPrivacy.Public, "Public" },
            { SessionPrivacy.Public_friend, "Public (friend)" },
            { SessionPrivacy.AskToJoin, "Ask to join" },
            { SessionPrivacy.AskToJoin_friend, "Ask to join (friend)" },
            { SessionPrivacy.Private, "Private" },
            { SessionPrivacy.SinglePlayer, "Single Player Mode" }
        };
    }
}
