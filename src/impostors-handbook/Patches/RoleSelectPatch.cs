using HarmonyLib;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using InnerNet;
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

                PlayerManager.RoleDictionary.Clear();

                Il2CppSystem.Collections.Generic.List<ClientData> unsortedPlayerList = new();
                AmongUsClient.Instance.GetAllClients(unsortedPlayerList);
                List<GameData.PlayerInfo> playerList = (from c in unsortedPlayerList.ToArray().ToList()
                                                        where c.Character != null
                                                        where c.Character.Data != null
                                                        where !c.Character.Data.Disconnected && !c.Character.Data.IsDead
                                                        orderby c.Id
                                                        select c.Character.Data).ToList<GameData.PlayerInfo>();
                foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers)
                {
                    if (playerInfo.Object != null && playerInfo.Object.isDummy)
                    {
                        playerList.Add(playerInfo);
                    }
                }

                var gameSettings = new RandomizationSettings(3, 1, 1);
                var roleOpportunities = new List<RoleOpportunity>
                {
                    new RoleOpportunity(Enum.Role.Jester, Enum.Team.Neutral, 0.5)
                };

                var roles = Managers.RoleManager.RandomizeRoles(gameSettings, roleOpportunities);
                for (int i = 0; i < playerList.Count; i++)
                {
                    //Logger<Plugin>.Info(role.ToString());
                    PlayerControl player = playerList[i].Object;
                    Role role = Managers.RoleManager.EnumToRole(roles[i]);
                    PlayerManager.RoleDictionary.Add(player.PlayerId, role);
                    //player.RpcSetRole(role.GetBaseRole());
                    player.SetRole(role.GetBaseRole());
                    player.RpcSetRole(role.GetBaseRole());
                }

                List<string> entries = new();
                foreach (KeyValuePair<byte, Role> playerRole in PlayerManager.RoleDictionary)
                {
                    string roleEntry = "";
                    roleEntry += playerRole.Key.ToString(); // Player
                    roleEntry += ":"; // Seperator
                    roleEntry += ((int) playerRole.Value.GetRole()).ToString(); // Role
                    entries.Add(roleEntry);
                }
                
                string message = string.Join(";", entries);
                RpcManager.RpcSendRoles(PlayerControl.LocalPlayer, message);
                /*Dictionary<byte, Enum.Role> rolePacket = new();
                foreach(KeyValuePair<byte, Role> rolePair in PlayerManager.RoleDictionary)
                {
                    rolePacket[rolePair.Key] = rolePair.Value.GetRole();
                }
                RpcManager.RpcSendRoles(PlayerControl.LocalPlayer, rolePacket);*/

                return false;
            }
        }
    }
}
