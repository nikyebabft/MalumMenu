using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using AmongUs.GameOptions;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "2.6.2";
    public static List<string> supportedAU = ["2025.3.25", "2025.3.31", "2025.6.10", "2025.9.9", "2025.10.14", "2025.11.18"];
    public static MenuUI menuUI;
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
        menuKeybind = Config.Bind("MalumMenu.GUI", "Keybind", "Z", "The keyboard key used to toggle the GUI on and off.");
        menuHtmlColor = Config.Bind("MalumMenu.GUI", "Color", "", "A custom color for your MalumMenu GUI. Supports html color codes");
        useHorizontalUI = Config.Bind("MalumMenu.GUI", "UseHorizontalUI", false, "When enabled, use the (new) horizontal tab-based UI instead of the vertical one.");
        guestMode = Config.Bind("MalumMenu.GuestMode", "GuestMode", false, "When enabled, a new guest account will generate every time you start the game");
        guestFriendCode = Config.Bind("MalumMenu.GuestMode", "FriendName", "", "The username that will be used when setting a friend code for your guest account.");
        spoofLevel = Config.Bind("MalumMenu.Spoofing", "Level", "", "A custom player level to display to others in online games");
        spoofPlatform = Config.Bind("MalumMenu.Spoofing", "Platform", "", "A custom gaming platform to display to others in online lobbies");
        spoofDeviceId = Config.Bind("MalumMenu.Privacy", "HideDeviceId", true, "When enabled it will hide your unique deviceId from Among Us");
        noTelemetry = Config.Bind("MalumMenu.Privacy", "NoTelemetry", true, "When enabled it will stop Among Us from collecting analytics");

        // Passives are enabled by default
        CheatToggles.unlockFeatures = CheatToggles.freeCosmetics = CheatToggles.avoidBans = true;

        Harmony.PatchAll();

        menuUI = AddComponent<MenuUI>();
        doorsUI = AddComponent<DoorsUI>();
        tasksUI = AddComponent<TasksUI>();
        AddComponent<CheatToggles.KeybindListener>().Plugin = this;

        // Initialize WaypointSystem
        try
        {
            var _ = WaypointSystem.Waypoints;
        }
        catch { }

        // Disable Telemetry
        if (noTelemetry.Value)
        {
            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            PerformanceReporting.enabled = false;
        }
    }

    private void Update()
    {
        try
        {
            if (PlayerControl.LocalPlayer != null)
            {
                MalumCheats.noKillCdCheat(PlayerControl.LocalPlayer);
                MalumCheats.teleportCursorCheat();
                MalumCheats.noClipCheat();
                MalumCheats.speedBoostCheat();
                MalumCheats.walkInVentCheat();
                MalumCheats.saveWaypointCheat();
                MalumCheats.clearWaypointsCheat();
                MalumCheats.ShadowCloneCheat();

                // Role-specific cheats
                var role = PlayerControl.LocalPlayer.Data?.Role;
                if (role != null)
                {
                    var roleType = role.Role;
                    
                    if (roleType == RoleTypes.Engineer)
                        MalumCheats.engineerCheats(role.TryCast<EngineerRole>());
                    else if (roleType == RoleTypes.Shapeshifter)
                        MalumCheats.shapeshifterCheats(role.TryCast<ShapeshifterRole>());
                    else if (roleType == RoleTypes.Scientist)
                        MalumCheats.scientistCheats(role.TryCast<ScientistRole>());
                    else if (roleType == RoleTypes.Tracker)
                        MalumCheats.trackerCheats(role.TryCast<TrackerRole>());
                    else if (roleType == RoleTypes.Phantom)
                        MalumCheats.phantomCheats(role.TryCast<PhantomRole>());
                }
            }

            if (HudManager.Instance != null)
                MalumCheats.useVentCheat(HudManager.Instance);

            if (ShipStatus.Instance != null)
            {
                MalumCheats.sabotageCheat(ShipStatus.Instance);
                MalumCheats.kickVentsCheat();
                MalumCheats.killAllCheat();
                MalumCheats.killAllCrewCheat();
                MalumCheats.killAllImpsCheat();
                
                if (ShipStatus.Instance.TryCast<FungleShipStatus>() != null)
                    MalumCheats.fungleSabotageCheat(ShipStatus.Instance.TryCast<FungleShipStatus>());
            }

            MalumCheats.ScanCheat();
            MalumCheats.AnimationCheat();
            MalumCheats.closeMeetingCheat();
            MalumCheats.skipMeetingCheat();
            MalumCheats.callMeetingCheat();
            MalumCheats.forceStartGameCheat();
            MalumCheats.completeMyTasksCheat();
            MalumCheats.ReviveCheat();
            //WaypointSystem.UpdateVisualMarkers();
        }
        catch { }
    }
}
