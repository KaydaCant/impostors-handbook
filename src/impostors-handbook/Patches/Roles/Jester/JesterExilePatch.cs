using HarmonyLib;
using ImpostorsHandbook.Managers;
using ImpostorsHandbook.Roles;

namespace ImpostorsHandbook.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class JesterExileBeginPatch
    {
        public static void Postfix(ExileController __instance)
        {
            if (__instance.exiled == null) return;
            byte playerId = __instance.exiled.PlayerId;

            Jester role;
            if (HostManager.PlayerRoles.ContainsKey(playerId) && HostManager.PlayerRoles[playerId] is Jester jester) role = jester;
            else if (PlayerControl.LocalPlayer.PlayerId == playerId && PlayerManager.MyRole is Jester jester2) role = jester2;
            else return;

            role.isVotedOut = true;
        }
    }
}
