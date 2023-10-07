using HarmonyLib;

namespace ImpostorsHandbook.Patches
{
    internal class EndCriteraPatch
    {
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
        public static class CheckEndCriteria
        {
            public static bool Prefix(ShipStatus __instance)
            {
                return false;
            }
        }
    }
}
