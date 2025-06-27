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
            JoinTimeout,
            Kicked,
            PlayerMessage
        }

        public enum BroadcastType
        {
            LeaveSession
        }
        
        public const int JoinRequestDuration = 10;
    }
}