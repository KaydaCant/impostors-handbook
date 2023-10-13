using HarmonyLib;
using Hazel;
using ImpostorsHandbook.Enum;
using ImpostorsHandbook.Roles;
using Reactor.Utilities;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ImpostorsHandbook.Managers.RpcManager;
using static UnityEngine.GraphicsBuffer;

namespace ImpostorsHandbook.Managers
{
    internal static class RpcManager
    {
        public enum RpcType : byte
        {
            SendRoles,
            SendImpostorPartners,
            ResetRoles,
            SendRoundWinners,
            SendSeerTarget
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
             *  SendRoundWinners:
             *  EndReason(byte), Count(int), [Player(byte)], Count(int), Title(byte[]), Color(byte[4])
             *  
             *  
             *  SendSeerTarget:
             *  Target(byte)
             *  
             */

            Logger<Plugin>.Info($"Sending '{type}' packet.");
            Logger<Plugin>.Info($"MESSAGE: '{message}'");
            if (targetId == -1) Logger<Plugin>.Info($"Sending to ALL.");
            else Logger<Plugin>.Info($"Sending to ONE.");

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 254, SendOption.Reliable, targetId);
            writer.Write((byte) type);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(message);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void SendRoles(byte targetPlayer, Dictionary<byte, Role> roles)
        {
            int targetId = GetTargetId(targetPlayer);
            List<byte> message = new() { (byte)roles.Count };
            foreach (KeyValuePair<byte, Role> item in roles)
            {
                message.Add(item.Key);
                message.Add((byte)item.Value);
            }
            SendRpc(RpcType.SendRoles, message.ToArray(), targetId);
        }

        public static void SendRolesToAll(Dictionary<byte, Role> roles)
        {
            List<byte> message = new() { (byte)roles.Count };
            foreach (KeyValuePair<byte, Role> item in roles)
            {
                message.Add(item.Key);
                message.Add((byte)item.Value);
            }
            SendRpc(RpcType.SendRoles, message.ToArray(), -1);
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

        public static void SendRoundWinners(List<byte> winners, string title, Color32 color)
        {
            List<byte> message = new() { (byte) winners.Count };
            foreach (byte winner in winners)
            {
                message.Add(winner);
            }

            message.Add((byte) title.Length);
            byte[] byteTitle = Encoding.ASCII.GetBytes(title);
            message.AddRange(byteTitle);

            message.Add(color.r);
            message.Add(color.g);
            message.Add(color.b);
            message.Add(color.a);

            SendRpc(RpcType.SendRoundWinners, message.ToArray());
        }

        public static int GetTargetId(byte playerId)
        {
            PlayerControl player = GameData.Instance.GetPlayerById(playerId).Object;
            int targetId = player.OwnerId;
            return targetId;
        }

        public static void SendSeerTarget(byte? targetPlayer)
        {
            int hostId = GetTargetId(GameData.Instance.GetHost().PlayerId);
            byte[] message = new byte[1] { targetPlayer != null ? (byte) targetPlayer : (byte) 255 };
            SendRpc(RpcType.SendSeerTarget, message, hostId);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpcPatch
    {
        public static bool Prefix(byte callId, MessageReader reader)
        {
            if (callId != 254) return true;
            RpcType rpcType = (RpcType) reader.ReadByte();
            byte sender = reader.ReadByte();
            if (sender == PlayerControl.LocalPlayer.PlayerId) { Logger<Plugin>.Info("Ignored packet from self."); return false; }

            Logger<Plugin>.Info($"Recieved '{rpcType}' packet from '{GameData.Instance.GetPlayerById(sender).PlayerName}'");

            switch (rpcType)
            {
                case RpcType.SendRoles:
                    int roleCount = reader.ReadByte();
                    for (int i = 0; i < roleCount; i++)
                    {
                        byte roleOwner = reader.ReadByte();
                        Role role = (Role) reader.ReadByte();

                        if (roleOwner == PlayerControl.LocalPlayer.PlayerId) PlayerManager.MyRole = RoleManager.GetRole(role, GameData.Instance.GetPlayerById(roleOwner).Object);
                        if (!PlayerManager.KnownRoles.ContainsKey(roleOwner)) PlayerManager.KnownRoles.Add(roleOwner, role);
                        Logger<Plugin>.Info($"Recieved role '{role}' for '{GameData.Instance.GetPlayerById(roleOwner).PlayerName}'");
                    }
                    return false;

                case RpcType.SendImpostorPartners:
                    int partnerCount = reader.ReadByte();
                    for (int i = 0; i < partnerCount; i++)
                    {
                        byte partner = reader.ReadByte();
                        if (PlayerManager.ImpostorPartners.Contains(partner)) continue;
                        if (!PlayerManager.ImpostorPartners.Contains(partner))  PlayerManager.ImpostorPartners.Add(partner);
                        Logger<Plugin>.Info($"Recieved impostor partner '{GameData.Instance.GetPlayerById(partner).PlayerName}'");
                    }
                    return false;

                case RpcType.ResetRoles:
                    PlayerManager.MyRole = null;
                    PlayerManager.KnownRoles.Clear();
                    PlayerManager.ImpostorPartners.Clear();
                    return false;

                case RpcType.SendRoundWinners:
                    PlayerManager.RoundWinners.Clear();
                    int winnerCount = reader.ReadByte();
                    for (int i = 0; i < winnerCount; i++)
                    {
                        byte winner = reader.ReadByte();
                        PlayerManager.RoundWinners.Add(winner);
                        Logger<Plugin>.Info($"Recieved round winner '{GameData.Instance.GetPlayerById(winner).PlayerName}'");
                    }

                    int titleLength = reader.ReadByte();
                    List<byte> titleBits = new();
                    for (int i = 0; i < titleLength; i++) 
                    {
                        titleBits.Add(reader.ReadByte()); 
                    }
                    string title = Encoding.ASCII.GetString(titleBits.ToArray());
                    PlayerManager.WinTitle = title;
                    Logger<Plugin>.Info($"Recieved win title: '{title}'");

                    byte r = reader.ReadByte();
                    byte g = reader.ReadByte();
                    byte b = reader.ReadByte();
                    byte a = reader.ReadByte();
                    Color32 color = new(r, g, b, a);
                    PlayerManager.WinColor = color;
                    Logger<Plugin>.Info($"Recieved win color: '({r}, {g}, {b}, {a})'");

                    return false;

                case RpcType.SendSeerTarget:
                    byte targetPlayer = reader.ReadByte();
                    Logger<Plugin>.Info($"Recieved seer target: '{targetPlayer}'");
                    if (targetPlayer != 255 && HostManager.PlayerRoles.ContainsKey(sender) && HostManager.PlayerRoles[sender] is Seer seer) seer.targetPlayer = GameData.Instance.GetPlayerById(targetPlayer).Object;
                    else if (HostManager.PlayerRoles.ContainsKey(sender) && HostManager.PlayerRoles[sender] is Seer seer2) seer2.targetPlayer = null;
                    return false;
            }

            return false;
        }
    }
}
