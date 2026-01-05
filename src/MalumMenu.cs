using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "2.6.2";  // CHANGED TO 2.6.2
    public static List<string> supportedAU = ["2025.3.25", "2025.3.31", "2025.6.10", "2025.9.9", "2025.10.14", "2025.11.18"];
    public static MenuUI menuUI;
    // public static ConsoleUI consoleUI;
    public static DoorsUI doorsUI;
    public static TasksUI tasksUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<bool> useHorizontalUI;
    public static ConfigEntry<string> spoofLevel;
    public static ConfigEntry<string> spoofPlatform;
    public static ConfigEntry<bool> spoofDeviceId;
    public static ConfigEntry<string> guestFriendCode;
    public static ConfigEntry<bool> guestMode;
    public static ConfigEntry<bool> noTelemetry;
    public static ConfigEntry<bool> freeCosmetics;
    public static ConfigEntry<bool> avoidBans;
    public static ConfigEntry<bool> unlockFeatures;

    public override void Load()
    {
        //Load config settings
        menuKeybind = Config.Bind("MalumMenu.GUI",
                                "Keybind",
                                "Z",  // KEPT AS Z
                                "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");

        menuHtmlColor = Config.Bind("MalumMenu.GUI",
                                "Color",
                                "",
                                "A custom color for your MalumMenu GUI. Supports html color codes");

        useHorizontalUI = Config.Bind("MalumMenu.GUI",
                                "UseHorizontalUI",
                                false,
                                "When enabled, use the (new) horizontal tab-based UI instead of the vertical one.");

        guestMode = Config.Bind("MalumMenu.GuestMode",
                                "GuestMode",
                                false,
                                "When enabled, a new guest account will generate every time you start the game, allowing you to bypass account bans and PUID detection");

        guestFriendCode = Config.Bind("MalumMenu.GuestMode",
                                "FriendName",
                                "",
                                "The username that will be used when setting a friend code for your guest account. IMPORTANT: Can only be used with GuestMode, needs to be ≤ 10 characters, and cannot include special characters/discriminator (#1234)");

        spoofLevel = Config.Bind("MalumMenu.Spoofing",
                                "Level",
                                "",
                                "A custom player level to display to others in online games to hide your actual platform. IMPORTANT: Custom levels can only be within 0 and 4294967295. Decimal numbers will not work");

        spoofPlatform = Config.Bind("MalumMenu.Spoofing",
                                "Platform",
                                "",
                                "A custom gaming platform to display to others in online lobbies to hide your actual platform. List of supported platforms: https://skeld.js.org/enums/_skeldjs_constant.Platform.html");

        spoofDeviceId = Config.Bind("MalumMenu.Privacy",
                                "HideDeviceId",
                                true,
                                "When enabled it will hide your unique deviceId from Among Us, which could potentially help bypass hardware bans in the future");

        noTelemetry = Config.Bind("MalumMenu.Privacy",
                                "NoTelemetry",
                                true,
                                "When enabled it will stop Among Us from collecting analytics of your games and sending them to Innersloth using Unity Analytics");

        // Add new config entries for waypoints if they don't exist
        try
        {
            // These are used by WaypointSystem but not exposed in GUI config
            Config.Bind("MalumMenu.Waypoints", "SaveKey", "None", "Key to save waypoint (not used in GUI)");
            Config.Bind("MalumMenu.Waypoints", "ClearKey", "None", "Key to clear waypoints (not used in GUI)");
        }
        catch { }

        // Passives are enabled by default
        CheatToggles.unlockFeatures = CheatToggles.freeCosmetics = CheatToggles.avoidBans = true;

        Harmony.PatchAll();

        menuUI = AddComponent<MenuUI>();
        // consoleUI = AddComponent<ConsoleUI>();
        doorsUI = AddComponent<DoorsUI>();
        tasksUI = AddComponent<TasksUI>();
        AddComponent<CheatToggles.KeybindListener>().Plugin = this;

        // Initialize WaypointSystem (triggers static constructor)
        try
        {
            var _ = WaypointSystem.Waypoints; // Access static property to trigger constructor
        }
        catch { }

        // Disable Telemetry (haven't fully tested if it works, but according to Unity docs it should)
        if (noTelemetry.Value)
        {
            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            PerformanceReporting.enabled = false;
        }
    }

    // Update method to run cheats every frame
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

                // Role-specific cheats - FIXED: Compare role byte directly without RoleTypes enum
                var role = PlayerControl.LocalPlayer.Data?.Role;
                if (role != null)
                {
                    var roleType = role.Role;
                    
                    // Engineer role byte value
                    if (roleType == 2) // Engineer
                        MalumCheats.engineerCheats(role.TryCast<EngineerRole>());
                    else if (roleType == 3) // Shapeshifter
                        MalumCheats.shapeshifterCheats(role.TryCast<ShapeshifterRole>());
                    else if (roleType == 4) // Scientist
                        MalumCheats.scientistCheats(role.TryCast<ScientistRole>());
                    else if (roleType == 5) // Tracker
                        MalumCheats.trackerCheats(role.TryCast<TrackerRole>());
                    else if (roleType == 6) // Phantom
                        MalumCheats.phantomCheats(role.TryCast<PhantomRole>());
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
}
