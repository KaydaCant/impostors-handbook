using AmongUs.Data.Player;
using HarmonyLib;
using ImpostorsHandbook.Managers;
using Reactor.Utilities;

namespace ImpostorsHandbook.Patches
{
    internal class EndGamePatch
    {
        // TODO: Override win stinger and make custom win stinger sounds.

        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        public static class EndGameStartPatch
        {
            public static void Prefix(EndGameManager __instance)
            {
                Logger<Plugin>.Info($"EndGamePatch Here!!");
                TempData.winners.Clear();
                foreach (byte winnerId in PlayerManager.RoundWinners)
                {
                    Logger<Plugin>.Info($"Adding winner to list: {GameData.Instance.GetPlayerById(winnerId).PlayerName}");
                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(winnerId);
                    WinningPlayerData winnerData = new(playerInfo);
                    winnerData.IsImpostor = Managers.RoleManager.GetRole(PlayerManager.KnownRoles[winnerId]).Team == Enum.Team.Impostor;

                    TempData.winners.Add(winnerData);
                }
            }
        }

        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
        public static class EndGameSetEverythingUpPatch
        {
            public static void Postfix(EndGameManager __instance)
            {
                PlayerManager.MyRole = null;
                PlayerManager.KnownRoles.Clear();
                PlayerManager.ImpostorPartners.Clear();
                HostManager.PlayerRoles.Clear();
                __instance.WinText.text = PlayerManager.WinTitle;
                __instance.WinText.color = PlayerManager.WinColor;
                __instance.BackgroundBar.material.SetColor("_Color", PlayerManager.WinColor);
            }
        }
    }
}
