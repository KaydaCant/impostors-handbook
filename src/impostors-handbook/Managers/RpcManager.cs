using ImpostorsHandbook.Roles;
using InnerNet;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Collections.Generic;

namespace ImpostorsHandbook.Managers
{
    public enum CustomRpcCalls : uint
    {
        SendRoles = 0
    }

    internal class RpcManager
    {
        [MethodRpc((uint) CustomRpcCalls.SendRoles)]
        public static void RpcSendRoles(PlayerControl sender, string message)//Dictionary<byte, Enum.Role> rolePacket)
        {
            if (sender.PlayerId != GameData.Instance.GetHost().PlayerId) return;
            if (sender.PlayerId == PlayerControl.LocalPlayer.PlayerId) return;
            //if (AmongUsClient.Instance.GetHost().Id != sender.PlayerId) return;
            // TODO: Check if sender is host

            // Message contains list of player -> role
            // Example message: "[id]:[role];[id]:[role]"
            //Logger<Plugin>.Info();

            Logger<Plugin>.Info("Recieved SendRoles RPC Packet:");
            Logger<Plugin>.Info(message);

            PlayerManager.RoleDictionary.Clear();
            //PlayerManager.RoleDictionary = roleDictionary;

            string[] players = message.Split(';');
            foreach (string player in players)
            {
                string[] playerData = player.Split(":");
                byte playerId = byte.Parse(playerData[0]); //Convert.FromBase64String(playerData[0])[0];
                Enum.Role roleEnum = (Enum.Role) int.Parse(playerData[1]);
                Role role = RoleManager.EnumToRole(roleEnum);
                PlayerManager.RoleDictionary.Add(playerId, role);
            }
            /*
            foreach (KeyValuePair<byte, Enum.Role> rolePair in rolePacket)
            {
                PlayerManager.RoleDictionary.Add(rolePair.Key, RoleManager.EnumToRole(rolePair.Value));
                Logger<Plugin>.Info($"Assigned role '{rolePair.Value}' to '{GameData.Instance.GetPlayerById(rolePair.Key).PlayerName}'.");
            }*/
        }
    }
}
