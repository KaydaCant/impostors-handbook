using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
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
            if (!PlayerManager.RoleDictionary.ContainsKey(player.PlayerId)) return true;
            Role role = PlayerManager.RoleDictionary[player.PlayerId];

            if (role.GetTeam() == Enum.Team.Crewmate) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => true));
            if (role.GetTeam() == Enum.Team.Impostor) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => PlayerManager.RoleDictionary[playerInfo.PlayerId].GetTeam() == Enum.Team.Impostor));
            if (role.GetTeam() == Enum.Team.Neutral) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => playerInfo.PlayerId == player.PlayerId));

            Vector3 position = __instance.BackgroundBar.transform.position;
            position.y -= 0.25f;
            __instance.BackgroundBar.transform.position = position;
            __instance.BackgroundBar.material.SetColor("_Color", role.GetColor());

            __instance.TeamTitle.text = role.GetName();
            __instance.TeamTitle.color = role.GetColor();

            for (int i = 0; i < teamToDisplay.Count; i++)
            {
                PlayerControl playerControl = teamToDisplay[i];
                if (playerControl == null) continue;

                GameData.PlayerInfo data = playerControl.Data;
                if (data == null) continue;

                PoolablePlayer poolablePlayer = __instance.CreatePlayer(i, 1, data, false);
                if (i == 0 && data.PlayerId == player.PlayerId)
                {
                    __instance.ourCrewmate = poolablePlayer;
                }
            }
            return false;
        }
    }
}
