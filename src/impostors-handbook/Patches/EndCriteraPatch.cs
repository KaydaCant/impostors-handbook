using HarmonyLib;
using ImpostorsHandbook.Enum;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using UnityEngine;

namespace ImpostorsHandbook.Patches
{
    internal class EndCriteraPatch
    {
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
        public static class CheckEndCriteria
        {
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (!GameData.Instance) return false;
                if (ExileController.Instance != null) return false; // Perhaps wait until vote animation end maybe ???
                
                foreach (KeyValuePair<byte, BaseRole> pair in HostManager.PlayerRoles)
                {
                    //if (pair.Value.player.Data.IsDead || pair.Value.player.Data.Disconnected) continue;
                    if (!pair.Value.player.Data.Disconnected && pair.Value is Jester jester && jester.isVotedOut) 
                    {
                        EndGame(__instance, GameOverReason.ImpostorByVote, new List<byte> { pair.Key }, "Jester Wins", Jester.Instance.Color);
                        return false;
                    }
                }
                int numCrewmates = HostManager.PlayerRoles.Where(pair => pair.Value.Team == Team.Crewmate && !pair.Value.player.Data.IsDead && !pair.Value.player.Data.Disconnected).Count();
                int numImpostors = HostManager.PlayerRoles.Where(pair => pair.Value.Team == Team.Impostor && !pair.Value.player.Data.IsDead && !pair.Value.player.Data.Disconnected).Count();
                // __instance.Manager.RpcEndGame(GameOverReason endReason, bool showAd)

                if (numImpostors == 0)
                {
                    Dictionary<byte, BaseRole> winnerDictionary = HostManager.PlayerRoles.Where(pair => pair.Value.Team == Team.Crewmate).ToDictionary(pair => pair.Key, pair => pair.Value);
                    EndGame(__instance, GameOverReason.HumansByVote, winnerDictionary.Keys.ToList(), "Crewmates Win", Crewmate.Instance.Color);
                    return false;
                } 
                else if (numImpostors >= numCrewmates)
                {
                    GameOverReason gameOverReason;
                    switch (TempData.LastDeathReason)
                    {
                        case DeathReason.Exile:
                            gameOverReason = GameOverReason.ImpostorByVote; break;
                        case DeathReason.Kill:
                            gameOverReason = GameOverReason.ImpostorByKill; break;
                        default:
                            gameOverReason = GameOverReason.HumansDisconnect; break;
                    }

                    Dictionary<byte, BaseRole> winnerDictionary = HostManager.PlayerRoles.Where(pair => pair.Value.Team == Team.Impostor).ToDictionary(pair => pair.Key, pair => pair.Value);
                    EndGame(__instance, gameOverReason, winnerDictionary.Keys.ToList(), "Impostors Win", Impostor.Instance.Color);
                    return false;
                }

                return false;
            }
        }

        public static void EndGame(LogicGameFlowNormal __instance, GameOverReason reason, List<byte> winners, string title, Color32 color)
        {
            Dictionary<byte, Role> knownRoles = HostManager.PlayerRoles.ToDictionary(pair => pair.Key, pair => pair.Value.Enum);
            RpcManager.SendRolesToAll(knownRoles);
            //PlayerManager.KnownRoles = knownRoles;
            foreach (KeyValuePair<byte, Role> knownRole in knownRoles)
            {
                if (knownRole.Key == PlayerControl.LocalPlayer.PlayerId) PlayerManager.MyRole = Managers.RoleManager.GetRole(knownRole.Value, PlayerControl.LocalPlayer);
                if (!PlayerManager.KnownRoles.ContainsKey(knownRole.Key)) PlayerManager.KnownRoles.Add(knownRole.Key, knownRole.Value);
            }

            PlayerManager.RoundWinners = winners;
            PlayerManager.WinTitle = title;
            PlayerManager.WinColor = color;

            RpcManager.SendRoundWinners(winners, title, color);
            __instance.Manager.RpcEndGame(reason, false);
        }
    }
}
