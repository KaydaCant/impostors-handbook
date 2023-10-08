using HarmonyLib;
using UnityEngine;

namespace ImpostorsHandbook.Patches
{
    internal class MenuMusicPatch
    {
        [HarmonyPatch(typeof(SoundStarter), nameof(SoundStarter.Awake))]
        public static class SoundStarterPatch
        {
            public static void Prefix(SoundStarter __instance)
            {
                if (__instance.name != "MainMenuBgMusic") return;
                AudioClip? audioClip = AssetManager.menuMusic;
                __instance.SoundToPlay = audioClip;
            }
        }
    }
}
