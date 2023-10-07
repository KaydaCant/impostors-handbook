using ImpostorsHandbook.Roles;
using System.Collections.Generic;

namespace ImpostorsHandbook.Managers
{
    internal static class PlayerManager
    {
        public static Dictionary<byte, Role> RoleDictionary;
        public static Dictionary<byte, bool> KnowPlayerRole;

        static PlayerManager()
        {
            RoleDictionary = new();
            KnowPlayerRole = new();
        }
    }
}
