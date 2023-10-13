using HarmonyLib;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImpostorsHandbook.Patches
{
    internal class MeetingStartPatch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHudStartPatch
        {
            public static void Prefix(MeetingHud __instance)
            {
                foreach (KeyValuePair<byte, BaseRole> pair in HostManager.PlayerRoles)
                {
                    PlayerControl player = GameData.Instance.GetPlayerById(pair.Key).Object;
                    if (pair.Value is Seer seer)
                    {
                        if (seer.targetPlayer == null) return;
                        if (!HostManager.PlayerRoles.ContainsKey(seer.targetPlayer.PlayerId)) return;
                        if (seer.player != PlayerControl.LocalPlayer) RpcManager.SendRoles(player.PlayerId, new Dictionary<byte, Enum.Role> { { seer.targetPlayer.PlayerId, HostManager.PlayerRoles[seer.targetPlayer.PlayerId].Enum } });
                        else if (!PlayerManager.KnownRoles.ContainsKey(seer.targetPlayer.PlayerId)) PlayerManager.KnownRoles.Add(seer.targetPlayer.PlayerId, HostManager.PlayerRoles[seer.targetPlayer.PlayerId].Enum);
                    }
                }
            }
        }
    }
}
