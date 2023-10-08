using ImpostorsHandbook.Roles;
using System.Collections.Generic;

namespace ImpostorsHandbook.Managers
{
    internal static class PlayerManager
    {
        public static Dictionary<byte, Enum.Role> KnownRoles;
        public static List<byte> ImpostorPartners;
        public static BaseRole? MyRole;

        static PlayerManager()
        {
            KnownRoles = new();
            ImpostorPartners = new();
        }
    }
}
