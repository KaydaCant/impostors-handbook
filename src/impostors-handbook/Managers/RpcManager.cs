using HarmonyLib;
using Hazel;
using ImpostorsHandbook.Enum;
using Reactor.Utilities;
using System.Collections.Generic;
using static ImpostorsHandbook.Managers.RpcManager;

namespace ImpostorsHandbook.Managers
{
    internal static class RpcManager
    {
        public enum RpcType : byte
        {
            SendRoles, // Send known roles (including player's own) to the player.
            SendImpostorPartners, // Send impostor partners to an impostor.
            ResetRoles // Tell the client to reset all roles.
        }

        public static void SendRpc(RpcType type, byte[] message, int targetId = -1)
        {
            /*
             * 
             *  RPC Protocol:
             *  RpcType(byte), Sender(byte), Data
             *  
             *  SendRoles:
             *  Count(int), [Player(byte), Role(byte)]
             *  
             *  SendPartners:
             *  Count(int), [Player(byte)]
             *  
             */

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 254, SendOption.Reliable, targetId);
            writer.Write((byte) type);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(message);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void SendRoles(byte targetPlayer, Dictionary<byte, Role> roles)
        {
            int targetId = GetTargetId(targetPlayer);
            List<byte> message = new() { (byte) roles.Count };
            foreach (KeyValuePair<byte, Role> item in roles)
            {
                message.Add(item.Key);
                message.Add((byte) item.Value);
            }
            SendRpc(RpcType.SendRoles, message.ToArray(), targetId);
        }

        public static void SendImpostorPartners(byte targetPlayer, List<byte> partners)
        {
            int targetId = GetTargetId(targetPlayer);
            List<byte> message = new() { (byte) partners.Count };
            foreach (byte partner in partners)
            {
                message.Add(partner);
            }
            SendRpc(RpcType.SendImpostorPartners, message.ToArray(), targetId);
        }

        public static int GetTargetId(byte playerId)
        {
            PlayerControl player = GameData.Instance.GetPlayerById(playerId).Object;
            int targetId = player.OwnerId;
            return targetId;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpcPatch
    {
        public static void Postfix(byte callId, MessageReader reader)
        {
            if (callId != 254) return;
            RpcType rpcType = (RpcType) reader.ReadByte();
            byte sender = reader.ReadByte();

            switch(rpcType)
            {
                case RpcType.SendRoles:
                    int roleCount = reader.ReadByte();
                    for (int i = 0; i < roleCount; i++)
                    {
                        byte roleOwner = reader.ReadByte();
                        Role role = (Role) reader.ReadByte();

                        if (roleOwner == PlayerControl.LocalPlayer.PlayerId) PlayerManager.MyRole = RoleManager.GetRole(role, GameData.Instance.GetPlayerById(roleOwner).Object);
                        else if (!PlayerManager.KnownRoles.ContainsKey(roleOwner)) PlayerManager.KnownRoles.Add(roleOwner, role);
                        Logger<Plugin>.Info($"Recieved role '{role}' for '{GameData.Instance.GetPlayerById(roleOwner).PlayerName}'");
                    }
                    return;

                case RpcType.SendImpostorPartners:
                    int partnerCount = reader.ReadByte();
                    for (int i = 0; i < partnerCount; i++)
                    {
                        byte partner = reader.ReadByte();
                        if (PlayerManager.ImpostorPartners.Contains(partner)) continue;
                        if (!PlayerManager.ImpostorPartners.Contains(partner))  PlayerManager.ImpostorPartners.Add(partner);
                        Logger<Plugin>.Info($"Recieved impostor partner '{GameData.Instance.GetPlayerById(partner).PlayerName}'");
                    }
                    return;

                case RpcType.ResetRoles:
                    PlayerManager.MyRole = null;
                    PlayerManager.KnownRoles.Clear();
                    PlayerManager.ImpostorPartners.Clear();
                    Logger<Plugin>.Info($"Recieved packet to reset roles.");
                    return;
            }
        }
    }
}
