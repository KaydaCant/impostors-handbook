using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace ImpostorsHandbook
{
    public static class AssetManager
    {
        public static AudioClip? menuMusic;
        public static TMP_SpriteAsset? nameIcons;

        public static void LoadAssets()
        {
            menuMusic = Plugin.MusicBundle?.LoadAsset<AudioClip>("menusong.ogg")?.DontDestroy();

            nameIcons = Plugin.AssetBundle?.LoadAsset<TMP_SpriteAsset>("nameicons")?.DontDestroy();
        }
    }
}
