using HarmonyLib;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpostorsHandbook.Patches
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.FixedUpdate))]
    public static class FixedUpdatePatch
    {
        public static void Postfix(GameManager __instance)
        {
            PlayerManager.MyRole?.FixedUpdate();
            foreach (KeyValuePair<byte, BaseRole> role in HostManager.PlayerRoles) role.Value.FixedUpdate();
        }
    }
}
