using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace MalumMenu;
public class MenuUI : MonoBehaviour
{

    public List<GroupInfo> groups = [];
    private bool isDragging = false;
    private Rect windowRect = new(10, 10, 320, 550);
    private Rect horizontalWindowRect = new(10, 10, 750, 600);
    private bool isGUIActive = false;
    public static bool isPanicked = false;
    public int selectedTab;

    // Styles
    private GUIStyle submenuButtonStyle;
    public GUIStyle tabTitleStyle;
    public GUIStyle tabSubtitleStyle;
    public GUIStyle separatorStyle;
    private GUIStyle textFieldStyle;
    private float hue; // For RGB mode

    // Waypoint variables
    private Vector2 waypointScrollPos;
    private string renameInput = "";
    private string waypointToRename = "";
    private bool isRenaming = false;

    // Create all groups (buttons) and their toggles on start
    private void Start()
    {
        groups.Add(new GroupInfo("Player", false, [
            new ToggleInfo(" NoClip", () => CheatToggles.noClip, x => CheatToggles.noClip = x),
            new ToggleInfo(" Fake Revive", () => CheatToggles.revive, x => CheatToggles.revive = x),
            new ToggleInfo(" Invert Controls", () => CheatToggles.invertControls, x => CheatToggles.invertControls = x),
        ], [
            new SubmenuInfo("Teleport", false, [
                new ToggleInfo(" to Cursor", () => CheatToggles.teleportCursor, x => CheatToggles.teleportCursor = x),
                new ToggleInfo(" to Player", () => CheatToggles.teleportPlayer, x => CheatToggles.teleportPlayer = x),
            ])

        ]));

        groups.Add(new GroupInfo("ESP", false, [
            new ToggleInfo(" Show Player Info", () => CheatToggles.showPlayerInfo, x => CheatToggles.showPlayerInfo = x),
            new ToggleInfo(" See Roles", () => CheatToggles.seeRoles, x => CheatToggles.seeRoles = x),
            new ToggleInfo(" See Ghosts", () => CheatToggles.seeGhosts, x => CheatToggles.seeGhosts = x),
            new ToggleInfo(" No Shadows", () => CheatToggles.fullBright, x => CheatToggles.fullBright = x),
            new ToggleInfo(" Reveal Votes", () => CheatToggles.revealVotes, x => CheatToggles.revealVotes = x),
            new ToggleInfo(" More Lobby Info", () => CheatToggles.moreLobbyInfo, x => CheatToggles.moreLobbyInfo = x)
        ], [
            new SubmenuInfo("Camera", false, [
                new ToggleInfo(" Zoom Out", () => CheatToggles.zoomOut, x => CheatToggles.zoomOut = x),
                new ToggleInfo(" Spectate", () => CheatToggles.spectate, x => CheatToggles.spectate = x),
                new ToggleInfo(" Freecam", () => CheatToggles.freecam, x => CheatToggles.freecam = x)
            ]),

            new SubmenuInfo("Tracers", false, [
                new ToggleInfo(" Crewmates", () => CheatToggles.tracersCrew, x => CheatToggles.tracersCrew = x),
                new ToggleInfo(" Impostors", () => CheatToggles.tracersImps, x => CheatToggles.tracersImps = x),
                new ToggleInfo(" Ghosts", () => CheatToggles.tracersGhosts, x => CheatToggles.tracersGhosts = x),
                new ToggleInfo(" Dead Bodies", () => CheatToggles.tracersBodies, x => CheatToggles.tracersBodies = x),
                new ToggleInfo(" Color-based", () => CheatToggles.colorBasedTracers,
                    x => CheatToggles.colorBasedTracers = x)

            ]),

            new SubmenuInfo("Minimap", false, [
                new ToggleInfo(" Crewmates", () => CheatToggles.mapCrew, x => CheatToggles.mapCrew = x),
                new ToggleInfo(" Impostors", () => CheatToggles.mapImps, x => CheatToggles.mapImps = x),
                new ToggleInfo(" Ghosts", () => CheatToggles.mapGhosts, x => CheatToggles.mapGhosts = x),
                new ToggleInfo(" Color-based", () => CheatToggles.colorBasedMap, x => CheatToggles.colorBasedMap = x)
            ])

        ]));

        groups.Add(new GroupInfo("Roles", false, [
                new ToggleInfo(" Set Fake Role", () => CheatToggles.changeRole, x => CheatToggles.changeRole = x)
            ],
            [
                new SubmenuInfo("Impostor", false, [
                    new ToggleInfo(" Kill Reach", () => CheatToggles.killReach, x => CheatToggles.killReach = x)
                ]),

                new SubmenuInfo("Shapeshifter", false, [
                    new ToggleInfo(" No Ss Animation", () => CheatToggles.noShapeshiftAnim,
                        x => CheatToggles.noShapeshiftAnim = x),

                    new ToggleInfo(" Endless Ss Duration", () => CheatToggles.endlessSsDuration,
                        x => CheatToggles.endlessSsDuration = x)

                ]),

                new SubmenuInfo("Crewmate", false, [
                    new ToggleInfo(" Show Tasks Menu", () => CheatToggles.showTasksMenu, x => CheatToggles.showTasksMenu = x)
                ]),

                new SubmenuInfo("Tracker", false, [
                    new ToggleInfo(" Endless Tracking", () => CheatToggles.endlessTracking,
                        x => CheatToggles.endlessTracking = x),

                    new ToggleInfo(" No Track Delay", () => CheatToggles.noTrackingDelay,
                        x => CheatToggles.noTrackingDelay = x),

                    new ToggleInfo(" No Track Cooldown", () => CheatToggles.noTrackingCooldown,
                        x => CheatToggles.noTrackingCooldown = x)

                ]),

                new SubmenuInfo("Engineer", false, [
                    new ToggleInfo(" Endless Vent Time", () => CheatToggles.endlessVentTime,
                        x => CheatToggles.endlessVentTime = x),

                    new ToggleInfo(" No Vent Cooldown", () => CheatToggles.noVentCooldown,
                        x => CheatToggles.noVentCooldown = x)

                ]),

                new SubmenuInfo("Scientist", false, [
                    new ToggleInfo(" Endless Battery", () => CheatToggles.endlessBattery,
                        x => CheatToggles.endlessBattery = x),

                    new ToggleInfo(" No Vitals Cooldown", () => CheatToggles.noVitalsCooldown,
                        x => CheatToggles.noVitalsCooldown = x)

                ])

            ]));

        groups.Add(new GroupInfo("Ship", false, [
            new ToggleInfo(" Unfixable Lights", () => CheatToggles.unfixableLights,
                x => CheatToggles.unfixableLights = x),
            new ToggleInfo(" Report Body", () => CheatToggles.reportBody, x => CheatToggles.reportBody = x),
            new ToggleInfo(" Close Meeting", () => CheatToggles.closeMeeting, x => CheatToggles.closeMeeting = x),
            new ToggleInfo(" Auto-Open Doors On Use", () => CheatToggles.autoOpenDoorsOnUse, x => CheatToggles.autoOpenDoorsOnUse = x)
        ], [
            new SubmenuInfo("Sabotage", false, [
                new ToggleInfo(" Reactor", () => CheatToggles.reactorSab, x => CheatToggles.reactorSab = x),
                new ToggleInfo(" Oxygen", () => CheatToggles.oxygenSab, x => CheatToggles.oxygenSab = x),
                new ToggleInfo(" Lights", () => CheatToggles.elecSab, x => CheatToggles.elecSab = x),
                new ToggleInfo(" Comms", () => CheatToggles.commsSab, x => CheatToggles.commsSab = x),
                new ToggleInfo(" Show Doors Menu", () => CheatToggles.showDoorsMenu, x => CheatToggles.showDoorsMenu = x),
                new ToggleInfo(" MushroomMixup", () => CheatToggles.mushSab, x => CheatToggles.mushSab = x),
                new ToggleInfo(" Trigger Spores", () => CheatToggles.mushSpore, x => CheatToggles.mushSpore = x),
                new ToggleInfo(" Open Sabotage Map", () => CheatToggles.sabotageMap, x => CheatToggles.sabotageMap = x)
            ]),

            new SubmenuInfo("Vents", false, [
                new ToggleInfo(" Unlock Vents", () => CheatToggles.useVents, x => CheatToggles.useVents = x),
                new ToggleInfo(" Kick All From Vents", () => CheatToggles.kickVents, x => CheatToggles.kickVents = x),
                new ToggleInfo(" Walk In Vents", () => CheatToggles.walkVent, x => CheatToggles.walkVent = x)
            ])

        ]));

        groups.Add(new GroupInfo("Chat", false, [
            new ToggleInfo(" Enable Chat", () => CheatToggles.alwaysChat, x => CheatToggles.alwaysChat = x),
            new ToggleInfo(" Unlock Textbox", () => CheatToggles.chatJailbreak, x => CheatToggles.chatJailbreak = x)
        ], []));

        // Console is temporarly disabled until we implement some features for it

        //groups.Add(new GroupInfo("Console", false, new List<ToggleInfo>() {
        //    new ToggleInfo(" ConsoleUI", () => MalumMenu.consoleUI.isVisible, x => MalumMenu.consoleUI.isVisible = x),
        //}, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo("Host-Only", false,
            [
                new ToggleInfo(" Kill While Vanished", () => CheatToggles.killVanished,
                    x => CheatToggles.killVanished = x),
                new ToggleInfo(" Kill Anyone", () => CheatToggles.killAnyone, x => CheatToggles.killAnyone = x),
                new ToggleInfo(" No Kill Cooldown", () => CheatToggles.zeroKillCd, x => CheatToggles.zeroKillCd = x),
                new ToggleInfo(" Protect Player", () => CheatToggles.protectPlayer, x => CheatToggles.protectPlayer = x),
                new ToggleInfo(" No Options Limits", () => CheatToggles.noOptionsLimits, x => CheatToggles.noOptionsLimits = x)
            ],
            [
                new SubmenuInfo("Murder", false, [
                    new ToggleInfo(" Kill Player", () => CheatToggles.killPlayer, x => CheatToggles.killPlayer = x),
                    new ToggleInfo(" Telekill Player", () => CheatToggles.telekillPlayer, x => CheatToggles.telekillPlayer = x),
                    new ToggleInfo(" Kill All Crewmates", () => CheatToggles.killAllCrew,
                        x => CheatToggles.killAllCrew = x),

                    new ToggleInfo(" Kill All Impostors", () => CheatToggles.killAllImps,
                        x => CheatToggles.killAllImps = x),

                    new ToggleInfo(" Kill Everyone", () => CheatToggles.killAll, x => CheatToggles.killAll = x)

                ]),

                new SubmenuInfo("Game State", false, [
                    new ToggleInfo(" Force Start Game", () => CheatToggles.forceStartGame, x => CheatToggles.forceStartGame = x),
                    new ToggleInfo(" No Game End", () => CheatToggles.noGameEnd, x => CheatToggles.noGameEnd = x)
                ]),

                new SubmenuInfo("Meetings", false, [
                    new ToggleInfo(" Call Meeting", () => CheatToggles.callMeeting, x => CheatToggles.callMeeting = x),
                    new ToggleInfo(" Skip Meeting", () => CheatToggles.skipMeeting, x => CheatToggles.skipMeeting = x),
                    new ToggleInfo(" VoteImmune", () => CheatToggles.voteImmune, x => CheatToggles.voteImmune = x),
                    new ToggleInfo(" Eject Player", () => CheatToggles.ejectPlayer, x => CheatToggles.ejectPlayer = x),
                ])
            ]));

        groups.Add(new GroupInfo("Passive", false, [
            new ToggleInfo(" Free Cosmetics", () => CheatToggles.freeCosmetics, x => CheatToggles.freeCosmetics = x),
            new ToggleInfo(" Avoid Penalties", () => CheatToggles.avoidBans, x => CheatToggles.avoidBans = x),
            new ToggleInfo(" Unlock Extra Features", () => CheatToggles.unlockFeatures, x => CheatToggles.unlockFeatures = x),
            new ToggleInfo(" Spoof Date to April 1st", () => CheatToggles.spoofAprilFoolsDate, x => CheatToggles.spoofAprilFoolsDate = x),
            new ToggleInfo(" Panic (Disable MalumMenu)", () => CheatToggles.panic, x => CheatToggles.panic = x)
        ], []));

        groups.Add(new GroupInfo("Animations", false, [
            new ToggleInfo(" Shields", () => CheatToggles.animShields, x => CheatToggles.animShields = x),
            new ToggleInfo(" Asteroids", () => CheatToggles.animAsteroids, x => CheatToggles.animAsteroids = x),
            new ToggleInfo(" Empty Garbage", () => CheatToggles.animEmptyGarbage, x => CheatToggles.animEmptyGarbage = x),
            new ToggleInfo(" Medbay Scan", () => CheatToggles.animScan, x => CheatToggles.animScan = x),
            new ToggleInfo(" Fake Cams In Use", () => CheatToggles.animCamsInUse, x => CheatToggles.animCamsInUse = x),
            new ToggleInfo(" Moonwalk", () => CheatToggles.moonwalk, x => CheatToggles.moonwalk = x)
        ], []));

        groups.Add(new GroupInfo("Config", false, [
            new ToggleInfo(" Open plugin config", () => false, x => Utils.OpenConfigFile()),
            new ToggleInfo(" Reload plugin config", () => CheatToggles.reloadConfig, x => CheatToggles.reloadConfig = x),
            new ToggleInfo(" Save to Profile", () => false, x => CheatToggles.SaveTogglesToProfile()),
            new ToggleInfo(" Load from Profile", () => false, x => CheatToggles.LoadTogglesFromProfile()),
            new ToggleInfo(" RGB Mode", () => CheatToggles.RGBMode, x => CheatToggles.RGBMode = x)
        ], []));

        // Waypoints group - SIMPLIFIED (will be handled manually in WindowFunction)
        groups.Add(new GroupInfo("Waypoints", false, [], []));
    }

    public void InitStyles()
    {
        if (!MalumMenu.useHorizontalUI.Value && (GUI.skin.toggle.fontSize == 13 || GUI.skin.toggle.fontSize == 0))
        {
            GUI.skin.toggle.fontSize = GUI.skin.button.fontSize = 22;
        }
        else if (MalumMenu.useHorizontalUI.Value && GUI.skin.toggle.fontSize != 13)
        {
            GUI.skin.toggle.fontSize = GUI.skin.button.fontSize = GUI.skin.label.fontSize = 14;
        }

        if (submenuButtonStyle != null) return;

        submenuButtonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { textColor = Color.white, background = Texture2D.grayTexture },
            fontSize = 20
        };
        submenuButtonStyle.normal.background.Apply();

        tabTitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };

        tabSubtitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 17,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };

        // Style for the vertical separator line between tab selector buttons and the actual tab content
        separatorStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = Texture2D.whiteTexture },
            margin = new RectOffset { top = 4, bottom = 4 },
            padding = new RectOffset(),
            border = new RectOffset()
        };

        // Text field style for rename input
        textFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(5, 5, 5, 5)
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(Utils.stringToKeycode(MalumMenu.menuKeybind.Value)))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
            horizontalWindowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        if (CheatToggles.RGBMode)
        {
            hue += Time.deltaTime * 0.3f; // Adjust speed of color change, higher multiplier = faster
            if (hue > 1f) hue -= 1f; // Loop hue back to 0 when it exceeds 1
        }

        if (CheatToggles.panic)
        {
            Utils.Panic();
            isPanicked = true;
            CheatToggles.panic = false;
        }

        //Passive cheats are always on to avoid problems
        //CheatToggles.unlockFeatures = CheatToggles.freeCosmetics = CheatToggles.avoidBans = true;

        if(!Utils.isPlayer){
            CheatToggles.changeRole = CheatToggles.killAll = CheatToggles.telekillPlayer = CheatToggles.killAllCrew = CheatToggles.killAllImps = CheatToggles.teleportCursor = CheatToggles.teleportPlayer = CheatToggles.spectate = CheatToggles.freecam = CheatToggles.killPlayer = CheatToggles.protectPlayer = false;
        }

        if(!Utils.isHost && !Utils.isFreePlay){
            CheatToggles.killAll = CheatToggles.telekillPlayer = CheatToggles.killAllCrew = CheatToggles.killAllImps = CheatToggles.killPlayer = CheatToggles.protectPlayer = CheatToggles.ejectPlayer = CheatToggles.zeroKillCd = CheatToggles.killAnyone = CheatToggles.killVanished = CheatToggles.forceStartGame = CheatToggles.noGameEnd = CheatToggles.skipMeeting = CheatToggles.callMeeting = false;
        }

        //Some cheats only work if the ship is present, so they are turned off if it is not
        if(!Utils.isShip){
            CheatToggles.revive = CheatToggles.sabotageMap = CheatToggles.unfixableLights = CheatToggles.completeMyTasks = CheatToggles.kickVents = CheatToggles.reportBody = CheatToggles.ejectPlayer = CheatToggles.closeMeeting = CheatToggles.skipMeeting = CheatToggles.callMeeting = CheatToggles.reactorSab = CheatToggles.oxygenSab = CheatToggles.commsSab = CheatToggles.elecSab = CheatToggles.mushSab = CheatToggles.closeAllDoors = CheatToggles.openAllDoors = CheatToggles.spamCloseAllDoors = CheatToggles.spamOpenAllDoors = CheatToggles.autoOpenDoorsOnUse = CheatToggles.mushSpore = CheatToggles.animShields = CheatToggles.animAsteroids = CheatToggles.animEmptyGarbage = CheatToggles.animScan = CheatToggles.animCamsInUse = false;
        }

        // Update waypoint visual markers
        WaypointSystem.UpdateVisualMarkers();
    }

    public void OnGUI()
    {
        if (!isGUIActive || isPanicked) return;

        InitStyles();

        // Only change the window height while the user is not dragging it, or else dragging breaks
        if (!isDragging)
        {
            var windowHeight = CalculateWindowHeight();
            windowRect.height = windowHeight;
        }

        if (CheatToggles.RGBMode)
        {
            GUI.backgroundColor = Color.HSVToRGB(hue, 1f, 1f); // Set background color based on hue
        }
        else
        {
            var configHtmlColor = MalumMenu.menuHtmlColor.Value;

            if (!ColorUtility.TryParseHtmlString(configHtmlColor, out var uiColor))
            {
                if (!configHtmlColor.StartsWith("#"))
                {
                    if (ColorUtility.TryParseHtmlString("#" + configHtmlColor, out uiColor))
                    {
                        GUI.backgroundColor = uiColor;
                    }
                }
            }
            else
            {
                GUI.backgroundColor = uiColor;
            }
        }

        if (MalumMenu.useHorizontalUI.Value)
        {
            horizontalWindowRect = GUI.Window(0, horizontalWindowRect, (GUI.WindowFunction)HorizontalWindowFunction, "MalumMenu v" + MalumMenu.malumVersion);
        }
        else
        {
            windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)WindowFunction, "MalumMenu v" + MalumMenu.malumVersion);
        }
    }

    public void WindowFunction(int windowID)
    {
        int groupSpacing = 55;
        int toggleSpacing = 44;
        int submenuSpacing = 44;
        int currentYPosition = 25;

        for (int groupId = 0; groupId < groups.Count; groupId++)
        {
            GroupInfo group = groups[groupId];

            if (GUI.Button(new Rect(10, currentYPosition, 300, 44), group.name))
            {
                group.isExpanded = !group.isExpanded;
                groups[groupId] = group;
                CloseAllGroupsExcept(groupId); // Close all other groups when one is expanded
                
                // Reset rename state when closing/opening groups
                if (!group.isExpanded)
                {
                    isRenaming = false;
                    waypointToRename = "";
                    renameInput = "";
                }
            }
            currentYPosition += groupSpacing;

            if (!group.isExpanded) continue;
            
            // SPECIAL HANDLING FOR WAYPOINTS GROUP
            if (group.name == "Waypoints")
            {
                // Save waypoint button - CENTERED
                if (GUI.Button(new Rect(10, currentYPosition, 300, 34), "Save Current Position"))
                {
                    if (Utils.isPlayer && Utils.isShip)
                    {
                        string name = $"WP_{System.DateTime.Now:HHmmss}";
                        WaypointSystem.AddWaypoint(
                            name,
                            PlayerControl.LocalPlayer.transform.position,
                            Utils.getCurrentMapID()
                        );
                    }
                }
                currentYPosition += 44;
                
                // Clear all button - CENTERED
                if (GUI.Button(new Rect(10, currentYPosition, 300, 34), "Clear All Waypoints"))
                {
                    WaypointSystem.ClearAllWaypoints();
                    // Reset rename state
                    isRenaming = false;
                    waypointToRename = "";
                    renameInput = "";
                }
                currentYPosition += 44;
                
                // Waypoints list header
                GUI.Label(new Rect(10, currentYPosition, 300, 34), "Saved Waypoints (Current Map Only):");
                currentYPosition += 34;
                
                // Simple list display
                int waypointY = currentYPosition;
                bool hasWaypoints = false;
                
                // Get waypoints for current map - but only if we're in a ship/game
                if (Utils.isShip)
                {
                    foreach (var wp in WaypointSystem.GetWaypointsForCurrentMap())
                    {
                        hasWaypoints = true;
                        
                        if (isRenaming && waypointToRename == wp.name)
                        {
                            // Show rename input field
                            GUI.SetNextControlName("RenameField");
                            renameInput = GUI.TextField(new Rect(20, waypointY, 130, 34), renameInput, textFieldStyle);
                            
                            // Save button
                            if (GUI.Button(new Rect(155, waypointY, 44, 34), "Save"))
                            {
                                if (!string.IsNullOrWhiteSpace(renameInput) && renameInput != wp.name)
                                {
                                    WaypointSystem.RenameWaypoint(wp.name, renameInput);
                                }
                                isRenaming = false;
                                waypointToRename = "";
                                renameInput = "";
                            }
                            
                            // Cancel button
                            if (GUI.Button(new Rect(204, waypointY, 44, 34), "Cancel"))
                            {
                                isRenaming = false;
                                waypointToRename = "";
                                renameInput = "";
                            }
                            
                            // Remove button (smaller during rename)
                            if (GUI.Button(new Rect(253, waypointY, 34, 34), "X"))
                            {
                                WaypointSystem.RemoveWaypoint(wp.name);
                                isRenaming = false;
                                waypointToRename = "";
                                renameInput = "";
                            }
                            
                            // Focus the text field when starting rename
                            if (Event.current.type == EventType.Repaint)
                            {
                                GUI.FocusControl("RenameField");
                            }
                        }
                        else
                        {
                            // Normal display
                            // Waypoint info label
                            GUI.Label(new Rect(20, waypointY, 130, 34), $"{wp.name} ({wp.timestamp})");
                            
                            // Teleport button
                            if (GUI.Button(new Rect(155, waypointY, 34, 34), "TP"))
                            {
                                WaypointSystem.TeleportToWaypoint(wp.name);
                            }
                            
                            // Rename button
                            if (GUI.Button(new Rect(194, waypointY, 44, 34), "Edit"))
                            {
                                // Start rename mode
                                isRenaming = true;
                                waypointToRename = wp.name;
                                renameInput = wp.name;
                            }
                            
                            // Remove button
                            if (GUI.Button(new Rect(243, waypointY, 34, 34), "X"))
                            {
                                WaypointSystem.RemoveWaypoint(wp.name);
                            }
                        }
                        
                        waypointY += 38;
                    }
                }
                
                if (!hasWaypoints)
                {
                    if (!Utils.isShip)
                    {
                        GUI.Label(new Rect(20, currentYPosition, 270, 34), "Enter a game to save waypoints");
                    }
                    else
                    {
                        GUI.Label(new Rect(20, currentYPosition, 270, 34), "No waypoints saved for this map");
                    }
                    currentYPosition += 38;
                }
                else
                {
                    currentYPosition = waypointY;
                }
                
                currentYPosition += 12;
                continue;
            }
            
            // Render direct toggles for the group (empty for waypoints)
            foreach (var toggle in group.toggles)
            {
                bool currentState = toggle.getState();
                bool newState = GUI.Toggle(new Rect(20, currentYPosition, 280, 34), currentState, toggle.label);
                if (newState != currentState)
                {
                    toggle.setState(newState);
                }
                currentYPosition += toggleSpacing;
            }

            if (group.name == "Player")
            {
                try
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = GUI.HorizontalSlider(new Rect(20, currentYPosition, 270, 34), PlayerControl.LocalPlayer.MyPhysics.GhostSpeed, 0f, 20f);
                        Utils.snapSpeedToDefault(0.05f, true);
                        GUI.Label(new Rect(20, currentYPosition + 12, 270, 22), $"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.GhostSpeed} {(Utils.isSpeedDefault(true) ? "(Default)" : "")}");
                        currentYPosition += toggleSpacing;
                    }
                    else
                    {
                        PlayerControl.LocalPlayer.MyPhysics.Speed = GUI.HorizontalSlider(new Rect(20, currentYPosition, 270, 34), PlayerControl.LocalPlayer.MyPhysics.Speed, 0f, 20f);
                        Utils.snapSpeedToDefault(0.05f);
                        GUI.Label(new Rect(20, currentYPosition + 12, 270, 22), $"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.Speed} {(Utils.isSpeedDefault() ? "(Default)" : "")}");
                        currentYPosition += toggleSpacing;
                    }
                }catch (NullReferenceException) {}
            }

            for (int submenuId = 0; submenuId < group.submenus.Count; submenuId++)
            {
                var submenu = group.submenus[submenuId];

                // Add a button for each submenu and toggle its expansion state when clicked
                if (GUI.Button(new Rect(20, currentYPosition, 280, 34), submenu.name, submenuButtonStyle))
                {
                    submenu.isExpanded = !submenu.isExpanded;
                    group.submenus[submenuId] = submenu;
                    if (submenu.isExpanded)
                    {
                        CloseAllSubmenusExcept(group, submenuId);
                    }
                }
                currentYPosition += submenuSpacing;

                if (!submenu.isExpanded) continue;
                // Show all the toggles in the expanded submenu
                foreach (var toggle in submenu.toggles)
                {
                    bool currentState = toggle.getState();
                    bool newState = GUI.Toggle(new Rect(30, currentYPosition, 270, 34), currentState, toggle.label);
                    if (newState != currentState)
                    {
                        toggle.setState(newState);
                    }
                    currentYPosition += toggleSpacing;
                }
            }
        }

        isDragging = Event.current.type switch
        {
            EventType.MouseDrag => true,
            EventType.MouseUp => false,
            _ => isDragging
        };

        GUI.DragWindow(); //Allows dragging the GUI window with mouse
    }

    // Dynamically calculate the window's height depending on
    // The number of toggles & group expansion
    private int CalculateWindowHeight()
    {
        int totalHeight = 80;
        int groupHeight = 55;
        int toggleHeight = 34;
        int submenuHeight = 44;

        foreach (GroupInfo group in groups)
        {
            totalHeight += groupHeight;

            if (!group.isExpanded) continue;
            
            // Special height for Waypoints group
            if (group.name == "Waypoints")
            {
                int waypointCount = 0;
                if (Utils.isShip)
                {
                    waypointCount = WaypointSystem.Waypoints.Count(w => w.mapId == Utils.getCurrentMapID());
                }
                
                if (waypointCount == 0)
                {
                    totalHeight += 160;
                }
                else
                {
                    totalHeight += 120 + (waypointCount * 38);
                }
                continue;
            }
            
            totalHeight += group.toggles.Count * toggleHeight;

            foreach (SubmenuInfo submenu in group.submenus)
            {
                totalHeight += submenuHeight;

                if (submenu.isExpanded)
                {
                    totalHeight += submenu.toggles.Count * toggleHeight;
                }
            }
        }

        return totalHeight;
    }

    // Closes all expanded groups other than indexToKeepOpen
    private void CloseAllGroupsExcept(int indexToKeepOpen)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (i == indexToKeepOpen) continue;
            GroupInfo group = groups[i];
            group.isExpanded = false;
            groups[i] = group;
        }
    }

    private void CloseAllSubmenusExcept(GroupInfo group, int submenuIndexToKeepOpen)
    {
        for (int i = 0; i < group.submenus.Count; i++)
        {
            if (i == submenuIndexToKeepOpen) continue;
            var submenu = group.submenus[i];
            submenu.isExpanded = false;
            group.submenus[i] = submenu;
        }
    }

    public void HorizontalWindowFunction(int windowID)
    {
        GUILayout.BeginHorizontal();

        // Left tab selector (15% width)
        GUILayout.BeginVertical(GUILayout.Width(horizontalWindowRect.width * 0.15f));
        for (var i = 0; i < groups.Count; i++)
        {
            if (GUILayout.Button(groups[i].name, GUILayout.Height(44)))
                selectedTab = i;
        }
        GUILayout.EndVertical();

        // Invisible vertical separator line to create some space between the tab selector and the content
        GUILayout.Box("", separatorStyle, GUILayout.Width(1f), GUILayout.ExpandHeight(true));
        GUILayout.Box("", GUIStyle.none, GUILayout.Width(10f), GUILayout.ExpandHeight(true));

        // Right tab content and controls (85% width)
        GUILayout.BeginVertical(GUILayout.Width(horizontalWindowRect.width * 0.85f));

        // Tab-specific content
        if (selectedTab >= 0 && selectedTab < groups.Count)
        {
            GUILayout.Label(groups[selectedTab].name, tabTitleStyle);
            HorizontalDrawContent(selectedTab);
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // Make the window draggable
        GUI.DragWindow();
    }

    /// <summary>
    /// Gets the hardcoded number of left column submenus for each tab
    /// </summary>
    /// <param name="groupId">The group (tab) index</param>
    /// <returns>The number of left column submenus</returns>
    private int GetLeftSubmenuCount(int groupId)
    {
        var name = groups[groupId].name;
        return name switch
        {
            "Player" => 1,
            "ESP" => 2,
            "Roles" => 4,
            "Ship" => 1,
            "Chat" => 1,
            "Host-Only" => 2,
            "Passive" => 1,
            "Animations" => 1,
            "Config" => 1,
            "Waypoints" => 1,
            _ => 2
        };
    }

    public void HorizontalDrawContent(int groupId)
    {
        var group = groups[groupId];

        var count = group.submenus.Count;
        if (count == 0)
        {
            HorizontalDrawToggles(group.toggles);
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(horizontalWindowRect.width * 0.425f));

        HorizontalDrawToggles(group.toggles);

        if (group.name == "Player")
        {
            try
            {
                if (PlayerControl.LocalPlayer.Data.IsDead)
                {
                    PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = GUILayout.HorizontalSlider(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed, 0f, 20f, GUILayout.Width(270f));
                    Utils.snapSpeedToDefault(0.05f, true);
                    GUILayout.Label($"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.GhostSpeed} {(Utils.isSpeedDefault(true) ? "(Default)" : "")}");
                }
                else
                {
                    PlayerControl.LocalPlayer.MyPhysics.Speed = GUILayout.HorizontalSlider(PlayerControl.LocalPlayer.MyPhysics.Speed, 0f, 20f, GUILayout.Width(270f));
                    Utils.snapSpeedToDefault(0.05f);
                    GUILayout.Label($"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.Speed} {(Utils.isSpeedDefault() ? "(Default)" : "")}");
                }
            }catch (NullReferenceException) {}
        }

        var desiredLeft = GetLeftSubmenuCount(groupId);
        var leftCount = Mathf.Clamp(desiredLeft, 0, count);

        // Left column submenus
        var leftSubmenus = group.submenus.GetRange(0, leftCount);
        foreach (var submenu in leftSubmenus)
        {
            GUILayout.Label(submenu.name, tabSubtitleStyle, GUILayout.Height(34));
            HorizontalDrawToggles(submenu.toggles);
        }
        GUILayout.EndVertical();

        // Right column submenus (if any)
        GUILayout.BeginVertical();
        if (count > leftCount)
        {
            var rightSubmenus = group.submenus.GetRange(leftCount, count - leftCount);
            foreach (var submenu in rightSubmenus)
            {
                GUILayout.Label(submenu.name, tabSubtitleStyle, GUILayout.Height(34));
                HorizontalDrawToggles(submenu.toggles);
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public void HorizontalDrawToggles(List<ToggleInfo> toggles)
    {
        foreach (var toggle in toggles)
        {
            var currentState = toggle.getState();
            var newState = GUILayout.Toggle(currentState, toggle.label, GUILayout.Height(24));
            if (newState != currentState)
                toggle.setState(newState);
        }
    }
}
