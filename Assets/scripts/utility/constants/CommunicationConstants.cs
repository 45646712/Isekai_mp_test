using UnityEngine;

namespace Constant
{
    public class CommunicationConstants
    {
        public enum MessageType
        {
            SystemMessage,
            DebugMessage,
            JoinRequest,
            JoinRequestTimeout,
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