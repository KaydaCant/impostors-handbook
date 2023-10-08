using Epic.OnlineServices.Reports;
using HarmonyLib;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using InnerNet;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostorsHandbook.Patches
{
    internal class RoleSelectPatch
    {
        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class SelectRolesPatch
        {
            public static bool Prefix(RoleManager __instance)
            {

                HostManager.PlayerRoles.Clear();

                Il2CppSystem.Collections.Generic.List<ClientData> unsortedPlayerList = new();
                AmongUsClient.Instance.GetAllClients(unsortedPlayerList);
                List<GameData.PlayerInfo> playerList = (from c in unsortedPlayerList.ToArray().ToList()
                                                        where c.Character != null
                                                        where c.Character.Data != null
                                                        where !c.Character.Data.Disconnected && !c.Character.Data.IsDead
                                                        orderby c.Id
                                                        select c.Character.Data).ToList();

                foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers)
                {
                    if (playerInfo.Object != null && playerInfo.Object.isDummy)
                    {
                        playerList.Add(playerInfo);
                    }
                }

                var gameSettings = new RandomizationSettings(3, 2, 1);
                var roleOpportunities = new List<RoleOpportunity>
                {
                    new RoleOpportunity(Enum.Role.Jester, Enum.Team.Neutral, 0.5)
                };

                RpcManager.SendRpc(RpcManager.RpcType.ResetRoles, Array.Empty<byte>());

                var roles = Managers.RoleManager.RandomizeRoles(gameSettings, roleOpportunities);
                for (int i = 0; i < playerList.Count; i++)
                {
                    PlayerControl player = playerList[i].Object;
                    BaseRole targetRole = Managers.RoleManager.GetRole(roles[i]);

                    Logger<Plugin>.Info($"Assigning role '{targetRole.Name}' to '{player.name}'.");
                    HostManager.PlayerRoles.Add(player.PlayerId, targetRole.Enum);
                }

                HostManager.SendRoles();

                return false;
            }
        }
    }
}
