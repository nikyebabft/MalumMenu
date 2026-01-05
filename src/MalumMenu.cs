using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;

namespace MalumMenu;

[BepInPlugin(Id, "MalumMenu", malumVersion)]
public class MalumMenu : BasePlugin
{
    public const string Id = "org.purp.malummenu";
    public const string malumVersion = "4.2.0";

    public static MalumMenu Instance { get; private set; }

    // Config entries
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<bool> useHorizontalUI;
    public static ConfigEntry<bool> disableModStamp;
    public static ConfigEntry<string> userId;
    public static ConfigEntry<string> userName;
    public static ConfigEntry<bool> isGuest;
    public static ConfigEntry<string> platform;

    // Harmony instance
    private Harmony harmony;

    public override void Load()
    {
        Instance = this;

        // Configuration
        menuKeybind = Config.Bind("UI", "Menu Keybind", "Delete", "Keybind to open/close the menu");
        menuHtmlColor = Config.Bind("UI", "Menu Color", "#2d2d30", "HTML color code for the menu background");
        useHorizontalUI = Config.Bind("UI", "Use Horizontal Layout", false, "Use horizontal tabbed layout instead of vertical");
        disableModStamp = Config.Bind("UI", "Disable Mod Stamp", false, "Disable the mod stamp in the main menu");
        userId = Config.Bind("Account", "User ID", "", "Custom User ID (leave empty for random)");
        userName = Config.Bind("Account", "Username", "", "Custom username (leave empty for random)");
        isGuest = Config.Bind("Account", "Is Guest", false, "Appear as guest account");
        platform = Config.Bind("Account", "Platform", "StandaloneSteamPC", "Platform to spoof: StandaloneSteamPC, StandaloneEpicPC, StandaloneWin10, IPhone, Android, Switch, Xbox, Playstation");

        // Initialize cheat toggles
        var _ = CheatToggles.Keybinds;

        // Apply patches
        harmony = new Harmony(Id);
        try
        {
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Log.LogError($"Failed to patch: {ex}");
        }

        // Add GameObject for MenuUI
        var menuObject = new GameObject("MalumMenuUI");
        menuObject.hideFlags = HideFlags.HideAndDontSave;
        UnityEngine.Object.DontDestroyOnLoad(menuObject);
        menuObject.AddComponent<MenuUI>();

        // Add GameObject for KeybindListener
        var keybindObject = new GameObject("MalumKeybindListener");
        keybindObject.hideFlags = HideFlags.HideAndDontSave;
        UnityEngine.Object.DontDestroyOnLoad(keybindObject);
        var listener = keybindObject.AddComponent<CheatToggles.KeybindListener>();
        listener.Plugin = this;

        Log.LogInfo($"MalumMenu v{malumVersion} loaded successfully!");
    }

    public override bool Unload()
    {
        harmony?.UnpatchSelf();
        return base.Unload();
    }

    private void Update()
    {
        try
        {
            // Call all cheat functions that need to run every frame
            if (PlayerControl.LocalPlayer != null)
            {
                MalumCheats.noKillCdCheat(PlayerControl.LocalPlayer);
                MalumCheats.teleportCursorCheat();
                MalumCheats.noClipCheat();
                MalumCheats.speedBoostCheat();
                MalumCheats.walkInVentCheat();
                MalumCheats.saveWaypointCheat();
                MalumCheats.clearWaypointsCheat();

                // Role-specific cheats
                var role = PlayerControl.LocalPlayer.Data?.Role;
                if (role != null)
                {
                    switch (role.Role)
                    {
                        case RoleTypes.Engineer:
                            MalumCheats.engineerCheats(role.TryCast<EngineerRole>());
                            break;
                        case RoleTypes.Shapeshifter:
                            MalumCheats.shapeshifterCheats(role.TryCast<ShapeshifterRole>());
                            break;
                        case RoleTypes.Scientist:
                            MalumCheats.scientistCheats(role.TryCast<ScientistRole>());
                            break;
                        case RoleTypes.Tracker:
                            MalumCheats.trackerCheats(role.TryCast<TrackerRole>());
                            break;
                        case RoleTypes.Phantom:
                            MalumCheats.phantomCheats(role.TryCast<PhantomRole>());
                            break;
                    }
                }
            }

            // HudManager cheats
            if (HudManager.Instance != null)
            {
                MalumCheats.useVentCheat(HudManager.Instance);
            }

            // ShipStatus cheats
            if (ShipStatus.Instance != null)
            {
                MalumCheats.sabotageCheat(ShipStatus.Instance);
                MalumCheats.kickVentsCheat();
                MalumCheats.killAllCheat();
                MalumCheats.killAllCrewCheat();
                MalumCheats.killAllImpsCheat();
                
                // Fungle-specific sabotage
                if (ShipStatus.Instance.TryCast<FungleShipStatus>() != null)
                {
                    MalumCheats.fungleSabotageCheat(ShipStatus.Instance.TryCast<FungleShipStatus>());
                }
            }

            // Animation cheats
            MalumCheats.ScanCheat();
            MalumCheats.AnimationCheat();

            // Meeting cheats
            MalumCheats.closeMeetingCheat();
            MalumCheats.skipMeetingCheat();
            MalumCheats.callMeetingCheat();

            // Host-only cheats
            MalumCheats.forceStartGameCheat();
            MalumCheats.completeMyTasksCheat();
            MalumCheats.ReviveCheat();

            // Update waypoint visual markers
            WaypointSystem.UpdateVisualMarkers();
        }
        catch (Exception ex)
        {
            // Silent catch to prevent spam in console
        }
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }
}
