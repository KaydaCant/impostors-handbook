using HarmonyLib;
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

                string nameIcons = "";
                Role? role = null;
                if (Managers.PlayerManager.RoleDictionary.ContainsKey(__instance.PlayerId)) role = Managers.PlayerManager.RoleDictionary[__instance.PlayerId];
                if (role != null && role.GetTeam() == Enum.Team.Impostor) nameIcons += $"<sprite=0>";

                string roleString = "";
                if (role != null) roleString = $"\n<color=#{UnityExtensions.ToHtmlStringRGBA(role.GetColor())}>{role.GetName()}</color>";

                __instance.cosmetics.nameText.color = new Color32(255, 255, 255, 255);
                __instance.cosmetics.nameText.text = $"{__instance.name}{nameIcons}{roleString}";
                //__instance.cosmetics.colorBlindText.text = $"<size=18> </size>\n{__instance.cosmetics.GetColorBlindText()}"; // Waist
                __instance.cosmetics.colorBlindText.text = $"<size=3> </size>\n{__instance.cosmetics.GetColorBlindText()}"; // Below Name

                __instance.cosmetics.nameText.spriteAsset = AssetManager.nameIcons;
            }
        }
    }
}
