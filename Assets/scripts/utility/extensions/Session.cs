using System;
using Constant;
using Unity.Services.Multiplayer;

namespace Extensions
{
    static class Session
    {
        //get property
        public static string GetProperty(this ISessionInfo info, SessionConstants.PropertyKeys key)
        { 
            info.Properties.TryGetValue(key.ToString(), out SessionProperty value);

            return value?.Value;
        }
        
        public static string GetProperty(this ISession session, SessionConstants.PropertyKeys key)
        { 
            session.Properties.TryGetValue(key.ToString(), out SessionProperty value);

            return value?.Value;
        }
        
        //set property(session/host only)
        private static void UpdateFriendOnlyStatus(this ISession session, bool isFriendOnly) => session.AsHost().SetProperty(SessionConstants.PropertyKeys.IsFriendOnly.ToString(), new SessionProperty(isFriendOnly.ToString()));

        private static void UpdateAskToJoinStatus(this ISession session, bool isAskToJoin) => session.AsHost().SetProperty(SessionConstants.PropertyKeys.IsAskToJoin.ToString(), new SessionProperty(isAskToJoin.ToString()));
        
        //privacy state
        public static SessionConstants.SessionPrivacy GetPrivacyState(this ISessionInfo info)
        {
            bool.TryParse(info.GetProperty(SessionConstants.PropertyKeys.IsFriendOnly), out bool isFriendOnly);
            bool.TryParse(info.GetProperty(SessionConstants.PropertyKeys.IsAskToJoin), out bool isAskToJoin);

            if (info.IsLocked)
            {
                return SessionConstants.SessionPrivacy.SinglePlayer;
            }

            if (info.HasPassword)
            {
                return SessionConstants.SessionPrivacy.Private;
            }

            if (isAskToJoin)
            {
                return isFriendOnly
                    ? SessionConstants.SessionPrivacy.AskToJoin_friend
                    : SessionConstants.SessionPrivacy.AskToJoin;
            }
            
            return isFriendOnly
                ? SessionConstants.SessionPrivacy.Public_friend
                : SessionConstants.SessionPrivacy.Public;
        }

        public static SessionConstants.SessionPrivacy GetPrivacyState(this ISession session)
        {
            bool.TryParse(session.GetProperty(SessionConstants.PropertyKeys.IsFriendOnly), out bool isFriendOnly);
            bool.TryParse(session.GetProperty(SessionConstants.PropertyKeys.IsAskToJoin), out bool isAskToJoin);
            
            if (session.IsLocked)
            {
                return SessionConstants.SessionPrivacy.SinglePlayer;
            }

            if (session.HasPassword)
            {
                return SessionConstants.SessionPrivacy.Private;
            }

            if (isAskToJoin)
            {
                return isFriendOnly
                    ? SessionConstants.SessionPrivacy.AskToJoin_friend
                    : SessionConstants.SessionPrivacy.AskToJoin;
            }
            
            return isFriendOnly
                ? SessionConstants.SessionPrivacy.Public_friend
                : SessionConstants.SessionPrivacy.Public;
        }

        public static void SetPrivacyState(this ISession session, SessionConstants.SessionPrivacy privacy, string password = null)
        {
            IHostSession sessionConfig = session.AsHost();

            sessionConfig.IsLocked = privacy is SessionConstants.SessionPrivacy.SinglePlayer;
            sessionConfig.IsPrivate = privacy is SessionConstants.SessionPrivacy.SinglePlayer;
            sessionConfig.UpdateFriendOnlyStatus(privacy is SessionConstants.SessionPrivacy.Public_friend or SessionConstants.SessionPrivacy.AskToJoin_friend);
            sessionConfig.UpdateAskToJoinStatus(privacy is SessionConstants.SessionPrivacy.AskToJoin or SessionConstants.SessionPrivacy.AskToJoin_friend);

            if (privacy is SessionConstants.SessionPrivacy.Private)
            {
                sessionConfig.Password = password;
            }
            else
            {
                sessionConfig.Password = String.Empty;
            }
        }
    }
}
