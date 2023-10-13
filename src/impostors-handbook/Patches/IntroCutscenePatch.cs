using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using Reactor.Utilities.Extensions;
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

            if (PlayerManager.MyRole.Team == Enum.Team.Crewmate) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => true));
            if (PlayerManager.MyRole.Team == Enum.Team.Impostor) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => PlayerManager.KnownRoles.ContainsKey(playerInfo.PlayerId) && Managers.RoleManager.GetRole(PlayerManager.KnownRoles[playerInfo.PlayerId]).Team == Enum.Team.Impostor));
            if (PlayerManager.MyRole.Team == Enum.Team.Neutral) teamToDisplay = IntroCutscene.SelectTeamToShow((Func<GameData.PlayerInfo, bool>)((GameData.PlayerInfo playerInfo) => playerInfo.PlayerId == player.PlayerId));

            Vector3 position = __instance.BackgroundBar.transform.position;
            position.y -= 1.25f; //0.25f
            __instance.BackgroundBar.transform.position = position;
            __instance.BackgroundBar.material.SetColor("_Color", PlayerManager.MyRole.Color);

            string teamText = "";
            if (PlayerManager.MyRole.Team == Enum.Team.Crewmate) teamText = $"<color=#{UnityExtensions.ToHtmlStringRGBA(Crewmate.Instance.Color)}>Crewmate</color>";
            if (PlayerManager.MyRole.Team == Enum.Team.Impostor) teamText = $"<color=#{UnityExtensions.ToHtmlStringRGBA(Impostor.Instance.Color)}>Impostor</color>";
            if (PlayerManager.MyRole.Team == Enum.Team.Neutral) teamText = $"<color=#{UnityExtensions.ToHtmlStringRGBA(Jester.Instance.Color)}>Neutral</color>";

            __instance.TeamTitle.text = PlayerManager.MyRole.Name;
            if (teamText != "") __instance.TeamTitle.text = $"{PlayerManager.MyRole.Name}\n<size=8>{teamText}</size>";

            __instance.TeamTitle.color = PlayerManager.MyRole.Color;
            __instance.TeamTitle.fontSizeMax = 11;

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
