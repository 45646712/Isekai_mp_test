using System;
using System.Collections.Generic;
using Constant;
using Cysharp.Threading.Tasks;
using Models;
using Extensions;
using Newtonsoft.Json;
using Unity.Services.Multiplayer;

using Access = Unity.Services.CloudCode.GeneratedBindings.Data.DataConstants_DataAccessibility;
using PublicData = Constant.PlayerDataConstants.PublicDataType;

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
            SessionConstants.SessionPrivacy currentPrivacy = SessionManager.Instance.CurrentSession.GetPrivacyState();
            
            if (privacy == SessionConstants.SessionPrivacy.Private && password is { Length: < 4 })
            {
                LogManager.instance.LogErrorAndShowUI("Password Must Be Longer Than 4 Characters!");
                return;
            }

            password = privacy != SessionConstants.SessionPrivacy.Private
                ? null
                : password + AccountConstant.BypassSessionPwRestriction;
            
            IHostSession sessionConfig = session.AsHost();

            sessionConfig.IsLocked = privacy is SessionConstants.SessionPrivacy.SinglePlayer;
            sessionConfig.IsPrivate = privacy is SessionConstants.SessionPrivacy.SinglePlayer;
            sessionConfig.Password = password;
            sessionConfig.UpdateFriendOnlyStatus(privacy is SessionConstants.SessionPrivacy.Public_friend or SessionConstants.SessionPrivacy.AskToJoin_friend);
            sessionConfig.UpdateAskToJoinStatus(privacy is SessionConstants.SessionPrivacy.AskToJoin or SessionConstants.SessionPrivacy.AskToJoin_friend);

            session.AsHost().SavePropertiesAsync();
        }
    }
    
    static class SessionData
    {
        public static async UniTask UpdateSessionHostInfo(this SessionManager manager)
        {
            List<string> targetKeys = new()
            {
                nameof(PublicData.UserID),
                nameof(PublicData.Lv),
                nameof(PublicData.Name)
            };

            Dictionary<string, object> data = await CloudCodeManager.Instance.LoadMultiPlayerData(Access.Public, targetKeys);

            long userID = (long)data[nameof(PublicData.UserID)];
            byte playerLevel = (byte)data[nameof(PublicData.Lv)];
            string playerName = (string)data[nameof(PublicData.Name)];

            SessionModel.SessionHostInfo hostInfo = new SessionModel.SessionHostInfo(userID, playerLevel, playerName);
            string jsonSource = JsonConvert.SerializeObject(hostInfo);

            manager.hostOption.SessionProperties[SessionConstants.PropertyKeys.SessionHostInfo.ToString()] = new SessionProperty(jsonSource);

            if (manager.CurrentSession is { IsHost: true })
            {
                manager.CurrentSession.AsHost().SetProperty(SessionConstants.PropertyKeys.SessionHostInfo.ToString(), new SessionProperty(jsonSource));
                await manager.CurrentSession.AsHost().SavePropertiesAsync();
            }
        }
        
        public static async UniTask UpdateSessionPlayerInfo(this SessionManager manager)
        {
            List<string> targetKeys = new()
            {
                nameof(PublicData.UserID),
                nameof(PublicData.Lv),
                nameof(PublicData.Name)
            };

            Dictionary<string, object> data = await CloudCodeManager.Instance.LoadMultiPlayerData(Access.Public, targetKeys);

            long userID = (long)data[nameof(PublicData.UserID)];
            byte playerLevel = (byte)data[nameof(PublicData.Lv)];
            string playerName = (string)data[nameof(PublicData.Name)];
            
            SessionModel.SessionPlayerInfo clientInfo = new SessionModel.SessionPlayerInfo(userID, playerLevel, playerName);
            string jsonSource = JsonConvert.SerializeObject(clientInfo);

            manager.CurrentSession.CurrentPlayer.SetProperty(SessionConstants.PropertyKeys.SessionPlayerInfo.ToString(), new PlayerProperty(jsonSource));
        }
    }
}