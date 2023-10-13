using HarmonyLib;
using Il2CppSystem.Data;
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
                __instance.cosmetics.nameText.text = GetName(__instance);
                __instance.cosmetics.nameText.color = new Color32(255, 255, 255, 255);
                __instance.cosmetics.nameText.spriteAsset = AssetManager.nameIcons;

                // TODO: Update this with positions instead of awkward text size
                //__instance.cosmetics.colorBlindText.text = $"<size=18> </size>\n{__instance.cosmetics.GetColorBlindText()}"; // Waist
                __instance.cosmetics.colorBlindText.text = $"<size=3> </size>\n{__instance.cosmetics.GetColorBlindText()}"; // Below Name
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (PlayerVoteArea playerState in  __instance.playerStates)
                {
                    PlayerControl player = GameData.Instance.GetPlayerById(playerState.TargetPlayerId).Object;
                    playerState.NameText.text = GetName(player);
                    playerState.NameText.color = new Color32(255, 255, 255, 255);
                    playerState.NameText.spriteAsset = AssetManager.nameIcons;
                }
            }
        }

        public static string GetName(PlayerControl player)
        {
            /*
             *  NAME ICONS:
             *  0: IMPOSTOR
             *  1: LOVER
             *  2: DOUSED (ARSONIST)
             *  3: WINNER LAST ROUND
             *  4: SEER ORB
             */

            BaseRole? playerRole = null;
            if (player == PlayerControl.LocalPlayer) playerRole = PlayerManager.MyRole;
            else if (PlayerManager.KnownRoles.ContainsKey(player.PlayerId)) playerRole = Managers.RoleManager.GetRole(PlayerManager.KnownRoles[player.PlayerId]);

            string nameIcons = "";
            if (PlayerManager.ImpostorPartners.Contains(player.PlayerId)) nameIcons += $"<sprite=0>";
            if (PlayerManager.MyRole is Seer seer && seer.targetPlayer == player) nameIcons += $"<sprite=4>";

            string roleString = "";
            if (playerRole != null) roleString = $"\n<color=#{UnityExtensions.ToHtmlStringRGBA(playerRole.Color)}>{playerRole.Name}</color>";

            return $"{player.name}{nameIcons}{roleString}";
        }
    }
}
