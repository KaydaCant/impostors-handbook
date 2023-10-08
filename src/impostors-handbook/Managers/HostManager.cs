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
        public static Dictionary<byte, Enum.Role> PlayerRoles;

        static HostManager()
        {
            PlayerRoles = new();
        }

        public static void SendRoles()
        {
            foreach (KeyValuePair<byte, Enum.Role> item in PlayerRoles)
            {
                PlayerControl player = GameData.Instance.GetPlayerById(item.Key).Object;
                BaseRole targetRole = RoleManager.GetRole(item.Value);

                Dictionary<byte, Enum.Role> knownRoles = new();

                if (targetRole.Team == Enum.Team.Impostor) {
                    List<byte> impostorPartners = new();
                    foreach(KeyValuePair<byte, Enum.Role> potentialPartner in PlayerRoles)
                    {
                        byte partnerId = potentialPartner.Key;
                        BaseRole partnerRole = RoleManager.GetRole(potentialPartner.Value);
                        if (partnerRole.Team != Enum.Team.Impostor) continue;

                        impostorPartners.Add(partnerId);
                        knownRoles.Add(partnerId, partnerRole.Enum); // TODO: Only if game setting "Impostors Know Partner Roles" is true.
                    }
                    RpcManager.SendImpostorPartners(player.PlayerId, impostorPartners);
                }
                if (!knownRoles.ContainsKey(player.PlayerId)) knownRoles.Add(player.PlayerId, targetRole.Enum);
                RpcManager.SendRoles(player.PlayerId, knownRoles);
                player.SetRole(targetRole.Type);
                player.RpcSetRole(targetRole.Type);
            }
        }
    }
}
