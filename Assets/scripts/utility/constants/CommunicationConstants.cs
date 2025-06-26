using UnityEngine;

namespace Constant
{
    public static class CommunicationConstants
    {
        public enum MessageType
        {
            SystemMessage,
            DebugMessage,
            JoinRequest,
            JoinAcknowledged,
            JoinDenied,
            PlayerMessage
        }

        public enum BroadcastType
        {
            LeaveSession
        }
        
        public const int JoinRequestDuration = 10;
    }
}