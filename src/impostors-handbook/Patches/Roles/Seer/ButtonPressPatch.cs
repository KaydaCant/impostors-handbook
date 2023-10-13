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
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    internal class SeerAbilityButtonClickPatch
    {
        public static void Postfix(AbilityButton __instance)
        {
            if (PlayerManager.MyRole is not Seer seer) return;
            if (__instance != seer.abilityButton) return;
            seer.OnAbilityButtonPressed();
        }
    }
}
