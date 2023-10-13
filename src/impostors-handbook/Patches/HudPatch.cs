using HarmonyLib;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpostorsHandbook.Patches
{
    static class HudPatch
    {
        public static bool HudActive;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), new Type[] { typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool) })]
        internal class SetHudActivePatch
        {
            public static void Postfix(HudManager __instance, PlayerControl localPlayer, RoleBehaviour role, bool isActive)
            {
                //seer.buttonManager?.Refresh(new AbilityButtonSettings());
                Logger<Plugin>.Info($"Hud Manager Patch. isActive: {isActive}");
                HudActive = isActive;
            }
        }
    }
}
