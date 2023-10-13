using ImpostorsHandbook.Roles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImpostorsHandbook.Managers
{
    internal static class PlayerManager
    {
        public static Dictionary<byte, Enum.Role> KnownRoles;
        public static List<byte> ImpostorPartners;
        public static BaseRole? MyRole;

        public static List<byte> RoundWinners;
        public static string WinTitle;
        public static Color32 WinColor;

        static PlayerManager()
        {
            KnownRoles = new();
            ImpostorPartners = new();
            RoundWinners = new();
            WinTitle = string.Empty;
            WinColor = new Color32();
        }

        public static List<PlayerControl> GetPlayersInAbilityRangeSorted(List<PlayerControl> outputList, bool ignoreColliders)
        {
            outputList.Clear();
            float abilityDistance = PlayerControl.LocalPlayer.Data.Role.GetAbilityDistance();
            Vector2 myPos = PlayerControl.LocalPlayer.GetTruePosition();
            List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers.ToArray().ToList();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                GameData.PlayerInfo playerInfo = allPlayers[i];
                if (IsValidTarget(playerInfo))
                {
                    PlayerControl @object = playerInfo.Object;
                    if (@object && @object.Collider.enabled)
                    {
                        Vector2 vector = @object.GetTruePosition() - myPos;
                        float magnitude = vector.magnitude;
                        if (magnitude <= abilityDistance && (ignoreColliders || !PhysicsHelpers.AnyNonTriggersBetween(myPos, vector.normalized, magnitude, Constants.ShipAndObjectsMask)))
                        {
                            outputList.Add(@object);
                        }
                    }
                }
            }
            outputList.Sort(delegate (PlayerControl a, PlayerControl b)
            {
                float magnitude2 = (a.GetTruePosition() - myPos).magnitude;
                float magnitude3 = (b.GetTruePosition() - myPos).magnitude;
                if (magnitude2 > magnitude3)
                {
                    return 1;
                }
                if (magnitude2 < magnitude3)
                {
                    return -1;
                }
                return 0;
            });
            return outputList;
        }

        public static bool IsValidTarget(GameData.PlayerInfo target)
        {
            return target != null && !target.Disconnected && !target.IsDead && target.PlayerId != PlayerControl.LocalPlayer.PlayerId && !(target.Role == null) && !(target.Object == null) && !target.Object.inVent && !target.Object.inMovingPlat;
        }

        public static PlayerControl? FindClosestTarget()
        {
            List<PlayerControl> playersInAbilityRangeSorted = PlayerManager.GetPlayersInAbilityRangeSorted(RoleBehaviour.GetTempPlayerList().ToArray().ToList(), false);
            if (playersInAbilityRangeSorted.Count <= 0)
            {
                return null;
            }
            return playersInAbilityRangeSorted[0];
        }
    }
}
