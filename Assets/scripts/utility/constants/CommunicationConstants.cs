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
            SessionTerminated,
            PlayerMessage
        }

        public enum BroadcastType
        {
            LeaveSession
        }
        
        public const int JoinRequestDuration = 10;
    }
}