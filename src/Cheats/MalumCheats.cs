using System.Runtime.CompilerServices;
using AmongUs.GameOptions;
using AmongUs.InnerNet.GameDataMessages;
using Sentry.Internal.Extensions;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            Object.Destroy(MeetingHud.Instance.gameObject);

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

    // NEW: Waypoint cheats
    public static void saveWaypointCheat()
    {
        if (!CheatToggles.saveWaypoint) return;
        
        if (Utils.isPlayer)
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
    private static Dictionary<string, GameObject> waypointMarkers = new Dictionary<string, GameObject>();
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
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };
        
        waypoints.Add(waypoint);
        SaveWaypoints();
        
        // Create visual marker if in-game
        CreateWaypointMarker(name, position);
    }
    
    public static void RemoveWaypoint(string name)
    {
        // Remove from list
        waypoints.RemoveAll(w => w.name == name);
        
        // Remove visual marker
        if (waypointMarkers.ContainsKey(name))
        {
            Object.Destroy(waypointMarkers[name]);
            waypointMarkers.Remove(name);
        }
        
        SaveWaypoints();
    }
    
    public static void TeleportToWaypoint(string name)
    {
        var waypoint = waypoints.FirstOrDefault(w => w.name == name);
        if (waypoint != null && Utils.isPlayer)
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(waypoint.position);
        }
    }
    
    public static void ClearAllWaypoints()
    {
        // Clear all visual markers
        foreach (var marker in waypointMarkers.Values)
        {
            if (marker != null)
                Object.Destroy(marker);
        }
        waypointMarkers.Clear();
        
        // Clear data
        waypoints.Clear();
        SaveWaypoints();
    }
    
    public static void UpdateVisualMarkers()
    {
        // Only create markers for current map
        if (!Utils.isShip) return;
        
        int currentMapId = Utils.getCurrentMapID();
        
        foreach (var wp in waypoints)
        {
            if (wp.mapId == currentMapId && !waypointMarkers.ContainsKey(wp.name))
            {
                CreateWaypointMarker(wp.name, wp.position);
            }
        }
        
        // Remove markers for wrong map
        var toRemove = new List<string>();
        foreach (var kvp in waypointMarkers)
        {
            var wp = waypoints.FirstOrDefault(w => w.name == kvp.Key);
            if (wp == null || wp.mapId != currentMapId)
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (var name in toRemove)
        {
            if (waypointMarkers.ContainsKey(name))
            {
                Object.Destroy(waypointMarkers[name]);
                waypointMarkers.Remove(name);
            }
        }
    }
    
    private static void CreateWaypointMarker(string name, Vector3 position)
    {
        if (!Utils.isShip) return;
        
        // Create a text marker instead of sprite
        var markerObject = new GameObject($"WaypointMarker_{name}");
        markerObject.transform.position = position + new Vector3(0, 1f, 0); // Above position
        
        // Create a TextMesh for emoji/text display
        var textMesh = markerObject.AddComponent<TextMesh>();
        textMesh.text = "📍"; // Pin emoji
        textMesh.fontSize = 50;
        textMesh.characterSize = 0.1f;
        textMesh.color = Color.cyan;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        
        // Make it always face camera
        markerObject.AddComponent<Billboard>();
        
        waypointMarkers[name] = markerObject;
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
        catch { }
    }
    
    private static void LoadWaypoints()
    {
        try
        {
            if (File.Exists(WaypointFilePath))
            {
                string json = File.ReadAllText(WaypointFilePath);
                waypoints = ParseWaypointsJson(json);
            }
        }
        catch
        {
            waypoints = new List<WaypointData>();
        }
    }
    
    private static List<WaypointData> ParseWaypointsJson(string json)
    {
        var result = new List<WaypointData>();
        
        try
        {
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
        catch { }
        
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
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
    
    private static string UnescapeJsonString(string input)
    {
        return input.Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
    }
}

// Simple billboard script to make marker face camera
public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                           Camera.main.transform.rotation * Vector3.up);
        }
    }
}
