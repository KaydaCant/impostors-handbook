using HarmonyLib;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ImpostorsHandbook.Patches
{
    internal class PlayerNamePatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerUpdate
        {
            public static void Postfix(PlayerControl __instance)
            {
                /*
                 *  NAME ICONS:
                 *  0: IMPOSTOR
                 *  1: LOVER
                 *  2: DOUSED (ARSONIST)
                 *  3: WINNER LAST ROUND
                 */

                BaseRole? playerRole = null;
                if (__instance == PlayerControl.LocalPlayer) playerRole = PlayerManager.MyRole;
                else if (PlayerManager.KnownRoles.ContainsKey(__instance.PlayerId)) playerRole = Managers.RoleManager.GetRole(PlayerManager.KnownRoles[__instance.PlayerId]);

                string nameIcons = "";
                if (PlayerManager.ImpostorPartners.Contains(__instance.PlayerId)) nameIcons += $"<sprite=0>";

                string roleString = "";
                if (playerRole != null) roleString = $"\n<color=#{UnityExtensions.ToHtmlStringRGBA(playerRole.Color)}>{playerRole.Name}</color>";

                __instance.cosmetics.nameText.color = new Color32(255, 255, 255, 255);
                __instance.cosmetics.nameText.text = $"{__instance.name}{nameIcons}{roleString}";

                // TODO: Update this with positions instead of awkward text size
                //__instance.cosmetics.colorBlindText.text = $"<size=18> </size>\n{__instance.cosmetics.GetColorBlindText()}"; // Waist
                __instance.cosmetics.colorBlindText.text = $"<size=3> </size>\n{__instance.cosmetics.GetColorBlindText()}"; // Below Name
                
                __instance.cosmetics.nameText.spriteAsset = AssetManager.nameIcons;
            }
        }
    }
}
