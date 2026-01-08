using System.Runtime.CompilerServices;
using AmongUs.GameOptions;
using AmongUs.InnerNet.GameDataMessages;
using Sentry.Internal.Extensions;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace MalumMenu;
public static class MalumCheats
{
    public static void closeMeetingCheat()
    {
        if (!CheatToggles.closeMeeting) return;

        if (Utils.isMeeting)
        { 
            // Destroy MeetingHud window gameobject
            MeetingHud.Instance.DespawnOnDestroy = false;
            UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);

            // Gameplay must be reenabled
            DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false));
            PlayerControl.LocalPlayer.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
            ShipStatus.Instance.EmergencyCooldown = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
            Camera.main.GetComponent<FollowerCamera>().Locked = false;
            DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
            ControllerManager.Instance.CloseAndResetAll();
        }
        else if (ExileController.Instance)
        { // Ends exile cutscene if it's playing
            ExileController.Instance.ReEnableGameplay();
            ExileController.Instance.WrapUp();
        }

        CheatToggles.closeMeeting = false; // Button behaviour
    }

    public static void skipMeetingCheat()
    {
        if (!CheatToggles.skipMeeting) return;

        if (Utils.isMeeting)
        {
            MeetingHud.Instance.RpcVotingComplete(new Il2CppStructArray<MeetingHud.VoterState>(0L), null, true);
        }

        CheatToggles.skipMeeting = false;
    }

    public static void callMeetingCheat()
    {
        if (!CheatToggles.callMeeting) return;

        MeetingRoomManager.Instance.AssignSelf(PlayerControl.LocalPlayer, null);
        DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
        PlayerControl.LocalPlayer.RpcStartMeeting(null);

        CheatToggles.callMeeting = false;
    }

    public static void forceStartGameCheat()
    {
        if (!CheatToggles.forceStartGame) return;
        if (Utils.isHost && Utils.isLobby)
        {
            AmongUsClient.Instance.SendStartGame();
        }

        CheatToggles.forceStartGame = false;
    }

    public static void noKillCdCheat(PlayerControl playerControl)
    {
        if (CheatToggles.zeroKillCd && playerControl.killTimer > 0f)
        {
            playerControl.SetKillTimer(0f);
        }
    }

    public static void completeMyTasksCheat()
    {
        if (CheatToggles.completeMyTasks)
        {
            Utils.completeMyTasks();

            CheatToggles.completeMyTasks = false;
        }
    }

    public static void engineerCheats(EngineerRole engineerRole)
    {
        if (CheatToggles.endlessVentTime)
        {

            // Makes vent time so incredibly long (float.MaxValue) so that it never ends
            engineerRole.inVentTimeRemaining = float.MaxValue;

            // Vent time is reset to normal value after the cheat is disabled
        }
        else if (engineerRole.inVentTimeRemaining > engineerRole.GetCooldown())
        {

            engineerRole.inVentTimeRemaining = engineerRole.GetCooldown();

        }

        if (CheatToggles.noVentCooldown)
        {

            if (engineerRole.cooldownSecondsRemaining > 0f)
            {

                engineerRole.cooldownSecondsRemaining = 0f;

                DestroyableSingleton<HudManager>.Instance.AbilityButton.ResetCoolDown();
                DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);

            }

        }
    }

    public static void shapeshifterCheats(ShapeshifterRole shapeshifterRole)
    {
        if (CheatToggles.endlessSsDuration)
        {

            // Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
            shapeshifterRole.durationSecondsRemaining = float.MaxValue;

            // Shapeshift duration is reset to normal value after the cheat is disabled
        }
        else if (shapeshifterRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.ShapeshifterDuration))
        {

            shapeshifterRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.ShapeshifterDuration);

        }
    }

    public static void scientistCheats(ScientistRole scientistRole)
    {
        if (CheatToggles.noVitalsCooldown)
        {

            scientistRole.currentCooldown = 0f;
        }

        if (CheatToggles.endlessBattery)
        {

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            scientistRole.currentCharge = float.MaxValue;

            // Battery charge is reset to normal value after the cheat is disabled
        }
        else if (scientistRole.currentCharge > scientistRole.RoleCooldownValue)
        {

            scientistRole.currentCharge = scientistRole.RoleCooldownValue;

        }
    }

    public static void trackerCheats(TrackerRole trackerRole)
    {
        if (CheatToggles.noTrackingCooldown)
        {

            trackerRole.cooldownSecondsRemaining = 0f;
            trackerRole.delaySecondsRemaining = 0f;

            DestroyableSingleton<HudManager>.Instance.AbilityButton.ResetCoolDown();
            DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);

        }

        if (CheatToggles.noTrackingDelay)
        {

            MapBehaviour.Instance.trackedPointDelayTime = GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.TrackerDelay);

        }

        if (CheatToggles.endlessTracking)
        {

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            trackerRole.durationSecondsRemaining = float.MaxValue;

            // Battery charge is reset to normal value after the cheat is disabled
        }
        else if (trackerRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.TrackerDuration))
        {

            trackerRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.TrackerDuration);

        }
    }

    public static void phantomCheats(PhantomRole phantomRole)
    {
        return;
    }

    public static void useVentCheat(HudManager hudManager)
    {
        // try-catch to prevent errors when role is null
        try
        {

            // Engineers & Impostors don't need this cheat so it is disabled for them
            // Ghost venting causes issues so it is also disabled

            if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                hudManager.ImpostorVentButton.gameObject.SetActive(CheatToggles.useVents);
            }

        }
        catch { }
    }

    public static void sabotageCheat(ShipStatus shipStatus)
    {
        var currentMapID = Utils.getCurrentMapID();

        // Handle all sabotage systems
        MalumSabotageSystem.HandleReactor(shipStatus, currentMapID);
        MalumSabotageSystem.HandleOxygen(shipStatus, currentMapID);
        MalumSabotageSystem.HandleComms(shipStatus, currentMapID);
        MalumSabotageSystem.HandleElectrical(shipStatus, currentMapID);
        MalumSabotageSystem.HandleMushMix(shipStatus, currentMapID);
        MalumSabotageSystem.HandleDoors(shipStatus);
        MalumSabotageSystem.OpenSabotageMap();
    }

    public static void fungleSabotageCheat(FungleShipStatus shipStatus)
    {
        var currentMapID = Utils.getCurrentMapID();

        MalumSabotageSystem.HandleSpores(shipStatus, currentMapID);
    }

    public static void walkInVentCheat()
    {
        try
        {
            if (!CheatToggles.walkVent) return;

            PlayerControl.LocalPlayer.inVent = false;
            PlayerControl.LocalPlayer.moveable = true;
        }
        catch { }
    }

    public static void kickVentsCheat()
    {
        if (!CheatToggles.kickVents) return;

        foreach (var vent in ShipStatus.Instance.AllVents)
        {
            VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id);
        }

        CheatToggles.kickVents = false; // Button behaviour
    }

    public static void killAllCheat()
    {
        if (!CheatToggles.killAll) return;

        if (Utils.isLobby)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
        }
        else
        {
            // Kill all players by sending a successful MurderPlayer RPC call
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Utils.murderPlayer(player, MurderResultFlags.Succeeded);
            }
        }

        CheatToggles.killAll = false;
    }

    public static void killAllCrewCheat()
    {
        if (!CheatToggles.killAllCrew) return;

        if (Utils.isLobby)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
        }
        else
        {
            // Kill all players by sending a successful MurderPlayer RPC call
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Role.TeamType == RoleTeamTypes.Crewmate)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }
            }
        }

        CheatToggles.killAllCrew = false;
    }

    public static void killAllImpsCheat()
    {
        if (!CheatToggles.killAllImps) return;

        if (Utils.isLobby)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
        }
        else
        {
            // Kill all players by sending a successful MurderPlayer RPC call
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Role.TeamType == RoleTeamTypes.Impostor)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }
            }
        }

        CheatToggles.killAllImps = false;
    }

    public static void teleportCursorCheat()
    {
        if (!CheatToggles.teleportCursor) return;

        // Teleport player to cursor's in-world position on right-click
        if (Input.GetMouseButtonDown(1))
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public static void noClipCheat()
    {
        try
        {

            PlayerControl.LocalPlayer.Collider.enabled = !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);

        }
        catch { }
    }

    public static void speedBoostCheat()
    {
        const float speedMultiplier = 2.0f;

        try
        {
            // If the speedBoost cheat is enabled, the default speed is multiplied by the speed multiplier
            // Otherwise the default speed is used by itself

            var newSpeed = CheatToggles.speedBoost ? Utils.DefaultSpeed * speedMultiplier : Utils.DefaultSpeed;

            var newGhostSpeed = CheatToggles.speedBoost ? Utils.DefaultGhostSpeed * speedMultiplier : Utils.DefaultGhostSpeed;

            PlayerControl.LocalPlayer.MyPhysics.Speed = newSpeed;
            PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = newGhostSpeed;
        }
        catch { }
    }

    public static void ReviveCheat()
    {
        if (!CheatToggles.revive) return;

        PlayerControl.LocalPlayer.Revive();
        CheatToggles.revive = false;
    }

    private static bool _hasUsedScanCheatBefore;

    private static void ForceSetScanner(PlayerControl player, bool toggle)
    {
        var count = ++player.scannerCount;
        player.SetScanner(toggle, count);
        RpcSetScannerMessage rpcMessage = new(player.NetId, toggle, count);
        AmongUsClient.Instance.LateBroadcastReliableMessage(Unsafe.As<IGameDataMessage>(rpcMessage));
    }

    public static void ScanCheat()
    {
        if (CheatToggles.animScan && !_hasUsedScanCheatBefore)
        {
            ForceSetScanner(PlayerControl.LocalPlayer, true);
            _hasUsedScanCheatBefore = true;
        }
        else if (!CheatToggles.animScan && _hasUsedScanCheatBefore)
        {
            ForceSetScanner(PlayerControl.LocalPlayer, false);
            _hasUsedScanCheatBefore = false;
        }
    }

    private static void ForcePlayAnimation(byte animationType)
    {
        // PlayerControl.LocalPlayer.RpcPlayAnimation(1); wouldn't work if visual tasks are turned off
        // The below way makes sure it works regardless of visual task settings

        PlayerControl.LocalPlayer.PlayAnimation(animationType);
        RpcPlayAnimationMessage rpcMessage = new(PlayerControl.LocalPlayer.NetId, animationType);
        AmongUsClient.Instance.LateBroadcastUnreliableMessage(Unsafe.As<IGameDataMessage>(rpcMessage));
    }

    private static bool _hasUsedCamsCheatBefore;

    public static void AnimationCheat()
    {
        var map = (MapNames)Utils.getCurrentMapID();

        if (CheatToggles.animShields)
        {
            if (map is MapNames.Skeld or MapNames.Dleks)
            {
                ForcePlayAnimation((byte)TaskTypes.PrimeShields);
            }
            CheatToggles.animShields = false;
        }
        if (CheatToggles.animAsteroids)
        {
            if (map is MapNames.Skeld or MapNames.Dleks or MapNames.Polus)
            {
                ForcePlayAnimation((byte)TaskTypes.ClearAsteroids);
            }
            else
            {
                CheatToggles.animAsteroids = false;
            }
        }
        if (CheatToggles.animEmptyGarbage)
        {
            if (map is MapNames.Skeld or MapNames.Dleks)
            {
                ForcePlayAnimation((byte)TaskTypes.EmptyGarbage);
            }
            CheatToggles.animEmptyGarbage = false;
        }

        if (CheatToggles.animCamsInUse && !_hasUsedCamsCheatBefore)
        {
            // There is no cameras on Mira HQ and Fungle
            if (map is MapNames.MiraHQ or MapNames.Fungle)
            {
                CheatToggles.animCamsInUse = false;
            }
            else
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Security, 1);
                _hasUsedCamsCheatBefore = true;
            }
        }
        else if (!CheatToggles.animCamsInUse && _hasUsedCamsCheatBefore)
        {
            // Turn off cams if the cheat was used before and is now disabled
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Security, 0);
            _hasUsedCamsCheatBefore = false;
        }
    }

    // NEW: Shadow Clone cheat - Advanced teleportation between two points
    public static void ShadowCloneCheat()
    {
        // Save Point 1 - BUTTON BEHAVIOR
        if (CheatToggles.savePoint1)
        {
            if (Utils.isPlayer && Utils.isShip)
            {
                _shadowClonePoint1 = PlayerControl.LocalPlayer.transform.position;
                HudManager.Instance.Notifier.AddDisconnectMessage("Shadow Clone Point 1 saved!");
            }
            CheatToggles.savePoint1 = false; // Reset immediately
        }
        
        // Save Point 2 - BUTTON BEHAVIOR
        if (CheatToggles.savePoint2)
        {
            if (Utils.isPlayer && Utils.isShip)
            {
                _shadowClonePoint2 = PlayerControl.LocalPlayer.transform.position;
                HudManager.Instance.Notifier.AddDisconnectMessage("Shadow Clone Point 2 saved!");
            }
            CheatToggles.savePoint2 = false; // Reset immediately
        }
        
        // Reset Points - BUTTON BEHAVIOR
        if (CheatToggles.resetShadowPoints)
        {
            _shadowClonePoint1 = Vector3.zero;
            _shadowClonePoint2 = Vector3.zero;
            CheatToggles.shadowCloneActive = false;
            HudManager.Instance.Notifier.AddDisconnectMessage("Shadow Clone points reset!");
            CheatToggles.resetShadowPoints = false; // Reset immediately
        }
        
        // Handle teleportation between points
        UpdateShadowCloneTeleport();
    }

    private static Vector3 _shadowClonePoint1 = Vector3.zero;
    private static Vector3 _shadowClonePoint2 = Vector3.zero;
    private static float _shadowCloneTeleportTimer = 0f;
    private static bool _isAtPoint1 = true;

    private static void UpdateShadowCloneTeleport()
    {
        // Check if we have both points set
        bool hasValidPoints = _shadowClonePoint1 != Vector3.zero && 
                             _shadowClonePoint2 != Vector3.zero &&
                             _shadowClonePoint1 != _shadowClonePoint2;
        
        if (!hasValidPoints)
        {
            // Can't teleport without valid points
            if (CheatToggles.shadowCloneActive)
            {
                CheatToggles.shadowCloneActive = false;
                HudManager.Instance.Notifier.AddDisconnectMessage("Need both points set for Shadow Clone!");
            }
            return;
        }
        
        // Start/Stop teleportation - BUTTON BEHAVIOR
        if (CheatToggles.toggleShadowClone)
        {
            CheatToggles.shadowCloneActive = !CheatToggles.shadowCloneActive;
            
            if (CheatToggles.shadowCloneActive)
            {
                HudManager.Instance.Notifier.AddDisconnectMessage("Shadow Clone teleportation started!");
                
                // Initialize starting position
                _isAtPoint1 = Vector3.Distance(PlayerControl.LocalPlayer.transform.position, 
                                             _shadowClonePoint1) < 2f;
                _shadowCloneTeleportTimer = 0f;
            }
            else
            {
                HudManager.Instance.Notifier.AddDisconnectMessage("Shadow Clone teleportation stopped!");
            }
            
            CheatToggles.toggleShadowClone = false; // Reset immediately
        }
        
        // Handle continuous teleportation
        if (CheatToggles.shadowCloneActive && Utils.isPlayer && Utils.isShip)
        {
            _shadowCloneTeleportTimer += Time.deltaTime;
            
            // Teleport every 0.02 seconds (50 times per second)
            if (_shadowCloneTeleportTimer >= 0.02f)
            {
                // Alternate between points
                if (_isAtPoint1)
                {
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(_shadowClonePoint2);
                    _isAtPoint1 = false;
                }
                else
                {
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(_shadowClonePoint1);
                    _isAtPoint1 = true;
                }
                
                _shadowCloneTeleportTimer = 0f;
            }
        }
    }

    // NEW: Waypoint cheats
    public static void saveWaypointCheat()
    {
        if (!CheatToggles.saveWaypoint) return;
        
        if (Utils.isPlayer && Utils.isShip)
        {
            string name = $"WP_{System.DateTime.Now:HHmmss}";
            WaypointSystem.AddWaypoint(
                name,
                PlayerControl.LocalPlayer.transform.position,
                Utils.getCurrentMapID()
            );
        }
        
        CheatToggles.saveWaypoint = false;
    }

    public static void clearWaypointsCheat()
    {
        if (!CheatToggles.clearWaypoints) return;
        
        WaypointSystem.ClearAllWaypoints();
        CheatToggles.clearWaypoints = false;
    }

    // NEW: Rage Quit cheat - FIXED VERSION
    private static float rageQuitTimer = 0f;
    private static bool rageQuitActive = false;

    public static void RageQuitCheat()
    {
        if (CheatToggles.rageQuit && Utils.isShip) // Only activate in ship
        {
            // Activate rage quit mode
            rageQuitActive = true;
            rageQuitTimer = 0f;
            
            // Sabotage everything immediately
            SabotageEverything();
            
            CheatToggles.rageQuit = false; // Button behaviour
        }
    }

    public static void UpdateRageQuit()
    {
        // AUTO-RESET: If rage quit was active but we're no longer in a ship
        if (!Utils.isShip && rageQuitActive)
        {
            ResetRageQuit();
            return;
        }
        
        // Don't run if not active or not in ship
        if (!rageQuitActive || !Utils.isShip) return;

        rageQuitTimer += Time.deltaTime;
        
        // Every 5 seconds, close all doors
        if (rageQuitTimer >= 5f)
        {
            CloseAllDoors();
            rageQuitTimer = 0f;
        }
    }

    // NEW: Method to reset rage quit state
    public static void ResetRageQuit()
    {
        // Reset rage quit state
        rageQuitActive = false;
        rageQuitTimer = 0f;
        
        // Also reset door spam cheats (important!)
        CheatToggles.closeAllDoors = false;
        CheatToggles.openAllDoors = false;
        CheatToggles.spamCloseAllDoors = false;
        CheatToggles.spamOpenAllDoors = false;
        
        Debug.Log("Rage quit reset for new game!");
    }

    private static void SabotageEverything()
    {
        if (!Utils.isShip) return;

        var shipStatus = ShipStatus.Instance;
        var currentMapID = Utils.getCurrentMapID();
        
        // Sabotage lights
        CheatToggles.elecSab = true;
        MalumSabotageSystem.HandleElectrical(shipStatus, currentMapID);
        
        // Sabotage comms
        CheatToggles.commsSab = true;
        MalumSabotageSystem.HandleComms(shipStatus, currentMapID);
        
        // Sabotage reactor/oxygen based on map
        if (currentMapID == 2 || currentMapID == 4) // Polus or Airship
        {
            CheatToggles.reactorSab = true;
            MalumSabotageSystem.HandleReactor(shipStatus, currentMapID);
        }
        else if (currentMapID == 0 || currentMapID == 1) // Skeld or Dleks
        {
            CheatToggles.reactorSab = true;
            MalumSabotageSystem.HandleReactor(shipStatus, currentMapID);
            CheatToggles.oxygenSab = true;
            MalumSabotageSystem.HandleOxygen(shipStatus, currentMapID);
        }
        else if (currentMapID == 3) // Mira HQ
        {
            CheatToggles.oxygenSab = true;
            MalumSabotageSystem.HandleOxygen(shipStatus, currentMapID);
        }
        else if (currentMapID == 5) // Fungle
        {
            // Fungle has different systems
            CheatToggles.mushSab = true;
            MalumSabotageSystem.HandleMushMix(shipStatus, currentMapID);
        }
        
        // Close all doors immediately
        CloseAllDoors();
    }

    private static void CloseAllDoors()
    {
        if (!Utils.isShip) return;

        try
        {
            // Use the existing door system
            CheatToggles.closeAllDoors = true;
            MalumSabotageSystem.HandleDoors(ShipStatus.Instance);
            CheatToggles.closeAllDoors = false;
        }
        catch { }
    }
}

// ====== WAYPOINT SYSTEM ======
[System.Serializable]
public class WaypointData
{
    public string name;
    public Vector3 position;
    public int mapId;
    public string timestamp;
}

public static class WaypointSystem
{
    private static List<WaypointData> waypoints = new List<WaypointData>();
    private static string WaypointFilePath => Path.Combine(BepInEx.Paths.ConfigPath, "MalumWaypoints.json");
    
    public static List<WaypointData> Waypoints => waypoints;
    
    static WaypointSystem()
    {
        LoadWaypoints();
    }
    
    public static void AddWaypoint(string name, Vector3 position, int mapId)
    {
        // Remove existing waypoint with same name
        RemoveWaypoint(name);
        
        var waypoint = new WaypointData
        {
            name = name,
            position = position,
            mapId = mapId,
            timestamp = System.DateTime.Now.ToString("HH:mm")
        };
        
        waypoints.Add(waypoint);
        SaveWaypoints();
    }
    
    public static void RemoveWaypoint(string name)
    {
        // Remove from list
        waypoints.RemoveAll(w => w.name == name);
        SaveWaypoints();
    }
    
    public static void RenameWaypoint(string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            Debug.Log("Waypoint name cannot be empty");
            return;
        }
        
        // Check if new name already exists
        if (waypoints.Any(w => w.name == newName))
        {
            Debug.Log($"Waypoint with name '{newName}' already exists");
            return;
        }
        
        var waypoint = waypoints.FirstOrDefault(w => w.name == oldName);
        if (waypoint != null)
        {
            waypoint.name = newName;
            waypoint.timestamp = System.DateTime.Now.ToString("HH:mm");
            SaveWaypoints();
        }
    }
    
    public static void TeleportToWaypoint(string name)
    {
        var waypoint = waypoints.FirstOrDefault(w => w.name == name);
        if (waypoint != null && Utils.isPlayer && Utils.isShip && Utils.getCurrentMapID() == waypoint.mapId)
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(waypoint.position);
        }
    }
    
    public static void ClearAllWaypoints()
    {
        waypoints.Clear();
        SaveWaypoints();
    }
    
    public static List<WaypointData> GetWaypointsForCurrentMap()
    {
        if (!Utils.isShip) return new List<WaypointData>();
        
        int currentMapId = Utils.getCurrentMapID();
        return waypoints.Where(w => w.mapId == currentMapId).ToList();
    }
    
    private static void SaveWaypoints()
    {
        try
        {
            // Simple JSON serialization
            var jsonLines = new List<string>();
            jsonLines.Add("[");
            
            for (int i = 0; i < waypoints.Count; i++)
            {
                var wp = waypoints[i];
                var json = $@"    {{
        ""name"": ""{EscapeJsonString(wp.name)}"",
        ""position"": {{""x"": {wp.position.x.ToString(System.Globalization.CultureInfo.InvariantCulture)}, ""y"": {wp.position.y.ToString(System.Globalization.CultureInfo.InvariantCulture)}, ""z"": {wp.position.z.ToString(System.Globalization.CultureInfo.InvariantCulture)}}},
        ""mapId"": {wp.mapId},
        ""timestamp"": ""{EscapeJsonString(wp.timestamp)}""
    }}";
                
                if (i < waypoints.Count - 1)
                    json += ",";
                
                jsonLines.Add(json);
            }
            
            jsonLines.Add("]");
            
            File.WriteAllLines(WaypointFilePath, jsonLines);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save waypoints: {e.Message}");
        }
    }
    
    private static void LoadWaypoints()
    {
        try
        {
            if (File.Exists(WaypointFilePath))
            {
                string json = File.ReadAllText(WaypointFilePath);
                waypoints = ParseWaypointsJson(json);
                Debug.Log($"Loaded {waypoints.Count} waypoints from file");
            }
            else
            {
                Debug.Log("No waypoints file found, starting fresh");
                waypoints = new List<WaypointData>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load waypoints: {e.Message}");
            waypoints = new List<WaypointData>();
        }
    }
    
    private static List<WaypointData> ParseWaypointsJson(string json)
    {
        var result = new List<WaypointData>();
        
        try
        {
            if (string.IsNullOrWhiteSpace(json)) return result;
            
            json = json.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            
            if (!json.StartsWith("[") || !json.EndsWith("]"))
                return result;
            
            json = json.Substring(1, json.Length - 2);
            
            var objects = SplitJsonObjects(json);
            
            foreach (var obj in objects)
            {
                var waypoint = ParseWaypointObject(obj);
                if (waypoint != null)
                    result.Add(waypoint);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse waypoints JSON: {e.Message}");
        }
        
        return result;
    }
    
    private static List<string> SplitJsonObjects(string json)
    {
        var objects = new List<string>();
        int depth = 0;
        int start = 0;
        
        for (int i = 0; i < json.Length; i++)
        {
            if (json[i] == '{')
                depth++;
            else if (json[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    objects.Add(json.Substring(start, i - start + 1));
                    start = i + 2;
                }
            }
        }
        
        return objects;
    }
    
    private static WaypointData ParseWaypointObject(string json)
    {
        try
        {
            var waypoint = new WaypointData();
            
            var nameMatch = System.Text.RegularExpressions.Regex.Match(json, @"""name"":""([^""]*)""");
            if (nameMatch.Success)
                waypoint.name = UnescapeJsonString(nameMatch.Groups[1].Value);
            
            var posMatch = System.Text.RegularExpressions.Regex.Match(json, @"""position"":\{""x"":([^,]*),""y"":([^,]*),""z"":([^}]*)\}");
            if (posMatch.Success)
            {
                waypoint.position = new Vector3(
                    float.Parse(posMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(posMatch.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(posMatch.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture)
                );
            }
            
            var mapIdMatch = System.Text.RegularExpressions.Regex.Match(json, @"""mapId"":([^,]*)");
            if (mapIdMatch.Success)
                waypoint.mapId = int.Parse(mapIdMatch.Groups[1].Value);
            
            var timeMatch = System.Text.RegularExpressions.Regex.Match(json, @"""timestamp"":""([^""]*)""");
            if (timeMatch.Success)
                waypoint.timestamp = UnescapeJsonString(timeMatch.Groups[1].Value);
            
            return waypoint;
        }
        catch
        {
            return null;
        }
    }
    
    private static string EscapeJsonString(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
    
    private static string UnescapeJsonString(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
    }
}
