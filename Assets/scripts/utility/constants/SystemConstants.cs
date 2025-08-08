using System.Collections.Generic;
using UnityEngine;

namespace Constant
{
    public static class ErrorKeyConstants
    {
        public static Dictionary<string, string> KeyToResponse { get; } = new()
        {
            {MultipleOwnership,""},
            {InvalidPassword, "Invalid password!"},
        };
        
        public const string MultipleOwnership = "Failed to start NetworkManager component as client";
        public const string InvalidPassword = "password does not match";
    }
}