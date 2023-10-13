using ImpostorsHandbook.Enum;
using ImpostorsHandbook.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpostorsHandbook.Managers
{
    internal static class HostManager
    {
        public static Dictionary<byte, BaseRole> PlayerRoles;

        static HostManager()
        {
            PlayerRoles = new();
        }

        public static void SendRoles()
        {
            foreach (KeyValuePair<byte, BaseRole> pair in PlayerRoles)
            {
                PlayerControl player = GameData.Instance.GetPlayerById(pair.Key).Object;

                Dictionary<byte, Role> knownRoles = new();

                if (pair.Value.Team == Team.Impostor) {
                    List<byte> impostorPartners = new();
                    foreach(KeyValuePair<byte, BaseRole> partnerPair in PlayerRoles)
                    {
                        byte partnerId = partnerPair.Key;
                        if (partnerPair.Value.Team != Team.Impostor) continue;

                        impostorPartners.Add(partnerId);
                        knownRoles.Add(partnerId, partnerPair.Value.Enum); // TODO: Only if game setting "Impostors Know Partner Roles" is true.
                    }

                    if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerManager.ImpostorPartners = impostorPartners;
                    else RpcManager.SendImpostorPartners(player.PlayerId, impostorPartners);
                }
                
                if (!knownRoles.ContainsKey(player.PlayerId)) knownRoles.Add(player.PlayerId, pair.Value.Enum);

                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) {
                    foreach (KeyValuePair<byte, Role> knownRole in knownRoles)
                    {
                        if (knownRole.Key == PlayerControl.LocalPlayer.PlayerId) PlayerManager.MyRole = RoleManager.GetRole(knownRole.Value, PlayerControl.LocalPlayer);
                        if (!PlayerManager.KnownRoles.ContainsKey(knownRole.Key)) PlayerManager.KnownRoles.Add(knownRole.Key, knownRole.Value);
                    }
                }
                else RpcManager.SendRoles(player.PlayerId, knownRoles);

                player.SetRole(pair.Value.Type);
                player.RpcSetRole(pair.Value.Type);
            }
        }
    }
}
