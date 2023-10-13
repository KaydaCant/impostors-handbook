using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.IO;
using ImpostorsHandbook.Managers;
using Reactor;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace ImpostorsHandbook;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
//[ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
public partial class Plugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public static AssetBundle? MusicBundle { get; private set; }
    public static AssetBundle? AssetBundle { get; private set; }

    public override void Load()
    {
        MusicBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "ImpostorsHandbook", "assets", "impostorshandbook.music"));
        AssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "ImpostorsHandbook", "assets", "impostorshandbook.assets"));

        Logger<Plugin>.Info("Loading assets...");
        AssetManager.LoadAssets();
        Logger<Plugin>.Info("All assets loaded!");

        Harmony.PatchAll();
    }
}
