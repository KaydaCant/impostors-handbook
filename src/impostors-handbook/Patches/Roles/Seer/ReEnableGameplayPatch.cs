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
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
    internal class SeerReEnableGameplayPatch
    {
        public static void Postfix(ExileController __instance)
        {
            if (PlayerManager.MyRole is not Seer seer) return;
            seer.hasOrb = true;
            seer.targetPlayer = null;
        }
    }
}
