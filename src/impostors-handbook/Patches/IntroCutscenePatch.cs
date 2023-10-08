using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using ImpostorsHandbook.Managers;
using UnityEngine;

namespace ImpostorsHandbook.Patches
{
    internal class IntroCutscenePatch
    {
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        public static class BeginCrewmatePatch
        {
            public static bool Prefix(IntroCutscene __instance, ref List<PlayerControl> teamToDisplay)
            {
                return ShowIntroCutscene(__instance, ref teamToDisplay);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        public static class BeginImpostorPatch
        {
            public static bool Prefix(IntroCutscene __instance, ref List<PlayerControl> yourTeam)
            {
                return ShowIntroCutscene(__instance, ref yourTeam);
            }
        }

        static bool ShowIntroCutscene(IntroCutscene __instance, ref List<PlayerControl> teamToDisplay)
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            if (PlayerManager.MyRole == null) return true;

            //if (PlayerManager.MyRole.Team == Enum.Team.Crewmate) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => true));
            //if (PlayerManager.MyRole.Team == Enum.Team.Impostor) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => PlayerManager.KnownRoles.ContainsKey(playerInfo.PlayerId) && RoleManager.GetRole(PlayerManager.KnownRoles[playerInfo.PlayerId]).Team == Enum.Team.Impostor));
            //if (PlayerManager.MyRole.Team == Enum.Team.Neutral) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => playerInfo.PlayerId == player.PlayerId));

            Vector3 position = __instance.BackgroundBar.transform.position;
            position.y -= 0.25f;
            __instance.BackgroundBar.transform.position = position;
            __instance.BackgroundBar.material.SetColor("_Color", PlayerManager.MyRole.Color);

            __instance.TeamTitle.text = PlayerManager.MyRole.Name;
            __instance.TeamTitle.color = PlayerManager.MyRole.Color;

            for (int i = 0; i < teamToDisplay.Count; i++)
            {
                PlayerControl playerControl = teamToDisplay[i];
                if (playerControl == null) continue;

                GameData.PlayerInfo data = playerControl.Data;
                if (data == null) continue;

                PoolablePlayer poolablePlayer = __instance.CreatePlayer(i, 1, data, PlayerManager.MyRole.Team == Enum.Team.Impostor);
                if (i == 0 && data.PlayerId == player.PlayerId)
                {
                    __instance.ourCrewmate = poolablePlayer;
                }
            }

            return false;
        }
    }
}
