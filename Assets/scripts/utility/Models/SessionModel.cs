using System;
using UnityEngine;

namespace Models
{
    static class SessionModel
    {
        public struct SessionHostInfo //for host
        {
            public Int64 UserID;
            public byte PlayerLevel;
            public string PlayerName;

            public SessionHostInfo(Int64 userID, byte playerLevel, string playerName)
            {
                UserID = userID;
                PlayerLevel = playerLevel;
                PlayerName = playerName;
            }
        }

        public struct SessionPlayerInfo //for client
        {
            public Int64 UserID;
            public byte PlayerLevel;
            public string PlayerName;
            
            public SessionPlayerInfo(Int64 userID, byte playerLevel, string playerName)
            {
                UserID = userID;
                PlayerLevel = playerLevel;
                PlayerName = playerName;
            }
        }
    }
}

