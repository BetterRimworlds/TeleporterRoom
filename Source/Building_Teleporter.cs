/*
 * This file is part of Dematerializer, a Better Rimworlds Project.
 *
 * Copyright © 2024 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *   GPG Fingerprint: D8EA 6E4D 5952 159D 7759  2BB4 EEB6 CE72 F441 EC41
 *   https://github.com/BetterRimworlds/Dematerializer
 *
 * This file is licensed under the Creative Commons No-Derivations v4.0 License.
 * Most rights are reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using RimWorld;

namespace BetterRimworlds.TeleporterRoom
{
    [StaticConstructorOnStartup]
    #if RIMWORLD15
    public class Building_Teleporter : Building, IThingHolder, IRenameable
    #else
    public class Building_Teleporter : Building, IThingHolder
    #endif
    {
        const int ADDITION_DISTANCE = 3;

        private int? Countdown = null;
        public bool PoweringUp = true;

        private Building_Teleporter destination;

        // protected static List<Building_Teleporter> TeleporterNetwork = new List<Building_Teleporter>();
        protected TeleporterBuffer teleporterBuffer;
        protected TeleporterNetwork TeleporterNetwork;

        protected static Texture2D UI_ADD_RESOURCES;
        protected static Texture2D UI_ADD_COLONIST;

        protected static Texture2D UI_GATE_IN;
        protected static Texture2D UI_GATE_OUT;

        protected static Texture2D UI_POWER_UP;
        protected static Texture2D UI_POWER_DOWN;

        static Graphic graphicInactive;

        private bool isPowerInited = false;
        CompPowerTrader power;
        // CompProperties_Power powerProps;

        int currentCapacitorCharge = 1000;
        int requiredCapacitorCharge = 1000;
        int chargeSpeed = 1;

        protected Map currentMap;
        protected Room room;

        public string Name;

        static Building_Teleporter()
        {
            UI_ADD_RESOURCES = ContentFinder<Texture2D>.Get("UI/ADD_RESOURCES", true);
            UI_ADD_COLONIST = ContentFinder<Texture2D>.Get("UI/ADD_COLONIST", true);

            UI_GATE_IN = ContentFinder<Texture2D>.Get("UI/StargateGUI-In", true);
            UI_GATE_OUT = ContentFinder<Texture2D>.Get("UI/StargateGUI-Out", true );

            UI_POWER_UP = ContentFinder<Texture2D>.Get("UI/PowerUp", true);
            UI_POWER_DOWN = ContentFinder<Texture2D>.Get("UI/PowerDown", true);

        #if RIMWORLD12
            GraphicRequest requestInactive = new GraphicRequest(Type.GetType("Graphic_Single"), "Things/Buildings/Teleporter", ShaderDatabase.DefaultShader, new Vector2(3, 3), Color.white, Color.white, new GraphicData(), 0, null);
        #else
            GraphicRequest requestInactive = new GraphicRequest(Type.GetType("Graphic_Single"), "Things/Buildings/Teleporter", ShaderDatabase.DefaultShader, new Vector2(3, 3), Color.white, Color.white, new GraphicData(), 0, null, null);
        #endif

            graphicInactive = new Graphic_Single();
            graphicInactive.Init(requestInactive);
        }

        ~Building_Teleporter()
        {
            TeleporterNetwork.Remove(this);
        }

        public override string Label => this.Name ?? "Teleporter";

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            this.TeleporterNetwork = Find.World.GetComponent<TeleporterNetwork>();

            // if (respawningAfterLoad)
            // {
            //     return;
            // }

            this.currentMap = map;

            this.power = base.GetComp<CompPowerTrader>();
            this.teleporterBuffer ??= new TeleporterBuffer(this);

            // this.power = new CompPowerTrader();

            // this.room = RegionAndRoomQuery.RoomAt(this.Position, this.Map);

            if (this.Name == null)
            {
                int teleNum = 1;
                foreach (var t in TeleporterNetwork)
                {
                    if (t.Name != "Teleporter" + ++teleNum)
                    {
                        break;
                    }
                }
                // this.Name = "Teleporter " + TeleporterNetwork.Count;
                this.Name = "Teleporter " + teleNum;
                Find.WindowStack.Add(new Dialog_NameTeleporterRoom(this));
            }

            TeleporterNetwork.Add(this);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Log.Warning("Removing " + this.Name + " from the Teleporter Network.");
            TeleporterNetwork.Remove(this);

            base.DeSpawn(mode);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Log.Warning("Removing " + this.Name + " from the Teleporter Network. 2");
            TeleporterNetwork.Remove(this);

            base.Destroy(mode);
        }

        // For displaying contents to the user.
        public ThingOwner GetDirectlyHeldThings() => this.teleporterBuffer;

        public void GetChildHolders(List<IThingHolder> outChildren) => ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, (IList<Thing>) this.GetDirectlyHeldThings());

        // public override string GetInspectString()
        // {
        //     this.def.defName = this.Name;
        //     // float excessPower = this.power.PowerNet.CurrentEnergyGainRate() / CompPower.WattsToWattDaysPerTick;
        //     return "Capacitor Charge: " + this?.currentCapacitorCharge + " / " + this?.requiredCapacitorCharge + "\n"
        //          + "Power needed: " + Math.Round((decimal)(this?.power?.powerOutputInt * -1.0f)) + " W"
        //         // + "Gain Rate: " + excessPower + "\n"
        //         // + "Stored Energy: " + this.power.PowerNet.CurrentStoredEnergy()
        //         ;
        // }

        // Saving game
        public override void ExposeData()
        {
            Scribe_Values.Look<int>(ref currentCapacitorCharge, "currentCapacitorCharge");
            Scribe_Values.Look<int>(ref requiredCapacitorCharge, "requiredCapacitorCharge");
            Scribe_Values.Look<int>(ref chargeSpeed, "chargeSpeed");
            Scribe_Values.Look<string>(ref Name, "name");
            // Scribe_Values.Look<CompPowerTrader>(ref power, "power");

            Scribe_Deep.Look<TeleporterBuffer>(ref this.teleporterBuffer, "teleporterBuffer", new object[]
            {
                this
            });

            base.ExposeData();
        }

        private bool detectSolarFlare()
        {
            var solarFlareDef = DefDatabase<GameConditionDef>.GetNamed("SolarFlare");
            bool isSolarFlare = this.currentMap.gameConditionManager.ConditionIsActive(solarFlareDef);

            if (isSolarFlare)
            {
                Log.Error("A solar flare is occuring...");
            }

            return isSolarFlare;
        }
        public override void TickRare()
        {
            this.detectSolarFlare();

            if (!this.teleporterBuffer.Any())
            {
                if (this.fullyCharged == true)
                {
                    this.power.powerOutputInt = 0;
                    chargeSpeed = 0;
                    this.updatePowerDrain();
                }

                if (this.fullyCharged == false && this.power.PowerOn)
                {
                    currentCapacitorCharge += chargeSpeed;

                    float excessPower = this.power.PowerNet.CurrentEnergyGainRate() / CompPower.WattsToWattDaysPerTick;
                    if (excessPower + (this.power.PowerNet.CurrentStoredEnergy() * 1000) > 5000)
                    {
                        // chargeSpeed += 5 - (this.chargeSpeed % 5);
                        chargeSpeed = (int)Math.Round(this.power.PowerNet.CurrentStoredEnergy() * 0.25 / 10);
                        this.updatePowerDrain();
                    }
                    else if (excessPower + (this.power.PowerNet.CurrentStoredEnergy() * 1000) > 1000)
                    {
                        chargeSpeed += 1;
                        this.updatePowerDrain();
                    }
                }
            }

            if (this.fullyCharged == true)
            {
                bool hasNoPower = this.power.PowerNet == null || !this.power.PowerNet.HasActivePowerSource;
                bool hasInsufficientPower = this.power.PowerOn == false;
                if (hasNoPower || hasInsufficientPower)
                {
                    // if (hasNoPower)
                    // {
                    //     Log.Error("NO POWER");
                    // }
                    //
                    // if (hasInsufficientPower)
                    // {
                    //     Log.Error("INSUFFICIENT POWER");
                    // }

                    // Ignore power requirements during a solar flare.
                    #if RIMWORLD15
                    // Solar flares do not exist in Rimworld v1.5.
                    var solarFlareDef = DefDatabase<GameConditionDef>.GetNamed("SolarFlare");
                    bool isSolarFlare = this.currentMap.gameConditionManager.ConditionIsActive(solarFlareDef);
                    #else
                    bool isSolarFlare = this.currentMap.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
                    #endif
                    if (isSolarFlare)
                    {
                        return;
                    }

                    // Log.Error("========= NOT ENOUGH POWER +========");
                    return;
                }
                if (this.isPowerInited == false)
                {
                    this.isPowerInited = true;
                    this.power.PowerOutput = -1000;
                }
            }

            if (this.Countdown > 0)
            {
                --this.Countdown;
            }

            if (this.Countdown <= 1)
            {
                this.DoBlastVisual();
            }

            if (this.Countdown <= 0)
            {
                this.Teleport(this.destination);
                this.Countdown = null;
            }

            base.TickRare();
        }

        #region Commands

        private bool fullyCharged
        {
            get
            {
                return (this.currentCapacitorCharge >= this.requiredCapacitorCharge);
            }
        }

        protected IEnumerable<Gizmo> GetDefaultGizmos()
        {
            return base.GetGizmos();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // Add the stock Gizmoes
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            if (this.fullyCharged == true)
            {
                //var network = Find.Maps.

                foreach (Building_Teleporter teleporter in TeleporterNetwork)
                {
                    if (teleporter == this)
                    {
                        continue;
                    }

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.InitiateTeleport(teleporter);
                    act.icon = UI_GATE_IN;
                    act.defaultLabel = "Send to " + teleporter.Name;
                    act.defaultDesc = "Teleport";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;

                    yield return act;
                }
            }

            // +57 319-666-8030
        }

        public bool HasThingsInBuffer()
        {
            return this.teleporterBuffer.Count > 0;
        }

        protected void InitiateTeleport(Building_Teleporter teleporter)
        {
            if (this.fullyCharged == false)
            {
                return;
            }

            var localRoom = this.GetRoom();
            var remoteRoom = teleporter.GetRoom();

            var rejectReasons = this.validateTeleporterRooms(localRoom, remoteRoom, teleporter);
            if (rejectReasons != "OK")
            {
                Messages.Message(rejectReasons, MessageTypeDefOf.RejectInput);
                return;
            }

            this.Countdown ??= 2;
            this.destination = teleporter;
        }

        public Room GetRoom()
        {
            var offsetPosition = new IntVec3(2, 0, 2);
            var localRoom = RegionAndRoomQuery.RoomAt(this.Position + offsetPosition, this.Map, RegionType.Set_All);
            // Log.Warning("Offset Position: " + offsetPosition + " | Real Position: " + (this.Position + offsetPosition));

            return localRoom;
        }

        protected string validateTeleporterRooms(Room localRoom, Room remoteRoom, Building_Teleporter teleporter)
        {
            string rejectReasons = "";
            if (localRoom == null)
            {
                rejectReasons += "This Teleporter is not inside a Room (use Room Stats tool to debug).\n";
            }

            if (remoteRoom == null)
            {
                rejectReasons += $"The destination Teleporter ({teleporter.Name}) is not inside a Room (use Room Stats tool to debug).\n";
            }

            if (localRoom.CellCount > 288)
            {
                rejectReasons += "The room of this Teleporter is too big (12x24, or 288 max cells).\n";
            }

            if (remoteRoom.CellCount > 288)
            {
                rejectReasons += $"The room of the destination Teleporter ({teleporter.Name}) is too big (12x24, or 288 max cells).\n";
            }

            if (localRoom.OpenRoofCount > 0)
            {
                rejectReasons += $"The room of this Teleporter has {localRoom.OpenRoofCount} missing roof tiles (use Room Stats tool to debug).\n";
            }

            if (remoteRoom.OpenRoofCount > 0)
            {
                rejectReasons += $"The room of the destination Teleporter ({teleporter.Name}) has {remoteRoom.OpenRoofCount} missing roof tiles (use Room Stats tool to debug).\n";
            }

            if (PlaceWorker_OnlyOneTeleporterRoom.isPlasteelWall(this.Map, localRoom) == false)
            {
                rejectReasons += "This teleporter room's walls are not made completely of Plasteel.\n";
            }

            if (PlaceWorker_OnlyOneTeleporterRoom.isPlasteelWall(teleporter.Map, remoteRoom) == false)
            {
                rejectReasons += "The remote teleporter room's walls are not made completely of Plasteel.\n";
            }

            if (PlaceWorker_OnlyOneTeleporterRoom.isSterileFloor(this.Map, localRoom) == false)
            {
                rejectReasons += "This teleporter room's floors are not made completely of Sterile Tile.\n";
            }

            if (PlaceWorker_OnlyOneTeleporterRoom.isSterileFloor(teleporter.Map, remoteRoom) == false)
            {
                rejectReasons += "The remote teleporter room's floors are not made completely of Sterile Tile.\n";
            }

            if (rejectReasons == "")
            {
                return "OK";
            }

            // Strip out the last \n.
            return rejectReasons.Remove(rejectReasons.Length - 1);
        }

        public void Teleport(Building_Teleporter teleporter, bool isRemoteTeleporter = false)
        {
            if (!this.fullyCharged && !isRemoteTeleporter)
            {
                Messages.Message("Insufficient Power for teleportation.", MessageTypeDefOf.RejectInput);
                return;
            }

            // var localRoom = RegionAndRoomQuery.RoomAt(new IntVec3(this.Position.x, this.Position.y, this.Position.z + 2), this.Map);
            // var remoteRoom = RegionAndRoomQuery.RoomAt(teleporter.Position + new IntVec3(0, 0, 2), teleporter.Map);

            // I have no idea why this.GetRoom() returns null...
            // Update: Now I know why: When a Building is set as Impassable, it becomes its own NULL Room.
            //         Because the Teleporter is bigger than other buildings (3x3), an offset of 2, 0, 2 needs to be made.
            //         Apparently, Rimworld uses the y-axis not for width but for depth and uses the z-axis for width.
            //         It seems completely counter-intuitive to me.
            // Hopefully, someone will see this comment on GitHub someday and it'll save them some time.

            var localRoom = this.GetRoom();
            var remoteRoom = teleporter.GetRoom();

            // var localRoom = GridsUtility.GetRoom(this.Position, this.Map);

            // Log.Error("Room ID: " + localRoom?.ID);
            // Log.Warning("Local Room info: " + localRoom + " | Remote Room info: " + remoteRoom);

            var rejectReasons = this.validateTeleporterRooms(localRoom, remoteRoom, teleporter);
            if (rejectReasons != "OK")
            {
                Messages.Message(rejectReasons, MessageTypeDefOf.RejectInput);
                return;
            }

            // End validation

            if (isRemoteTeleporter == false)
            {
                teleporter.Teleport(this, true);
            }

            var things = BetterRimworlds.Utilities.findThingsInRoom(localRoom);
            if (things.Count > 0)
            {
                foreach (Thing thing in things)
                {
                    teleporter.teleporterBuffer.TryAdd(thing);
                }
            }

            IEnumerable<Pawn> closePawns = BetterRimworlds.Utilities.findPawnsInRoom(localRoom);

            if (closePawns != null)
            {
                foreach (Pawn currentPawn in closePawns.ToList())
                {
                    if (currentPawn.Spawned)
                    {
                        // Fixes a bug w/ support for B19+ and later where colonists go *crazy*
                        // if they enter a Stargate after they've ever been drafted.
                        if (currentPawn.verbTracker != null)
                        {
                            currentPawn.verbTracker = new VerbTracker(currentPawn);
                        }

                        // Remove memories or they will go insane...
                        if (currentPawn.def.defName == "Human")
                        {
                            currentPawn.needs.mood.thoughts.memories = new MemoryThoughtHandler(currentPawn);
                        }

                        teleporter.teleporterBuffer.TryAdd(currentPawn);
                    }
                }
            }

            this.Rematerialize();

            // WHY do the teleporters seemingly share the same TeleporterBuffer?!?!
            // Teleport the stuff in the other teleporter room, too.
            if (isRemoteTeleporter == false)
            {
                teleporter.Rematerialize();
            }
        }

        private bool Rematerialize()
        {
            Log.Message("Number of teleporters on this planet: " + TeleporterNetwork.Count);

            /* Tuple<int, List<Thing>> **/
            var recallData = this.teleporterBuffer.ToList();
            this.teleporterBuffer.Clear();

            if (recallData.Count == 0)
            {
                Messages.Message("WARNING: The Teleporter buffer was empty!!", MessageTypeDefOf.ThreatBig);
                return false;
            }

            bool wasPlaced;
            foreach (Thing currentThing in recallData)
            {
                try
                {
                    // If it's just a teleport, destroy the thing first...
                    // Log.Warning("a1: is offworld? " + offworldEvent + " | Stargate Buffer count: " + this.stargateBuffer.Count);
                    wasPlaced = GenPlace.TryPlaceThing(currentThing, this.Position + new IntVec3(0, 0, -2),
                        this.currentMap, ThingPlaceMode.Near);
                    // Readd the unplaced Thing into the stargateBuffer.
                    if (!wasPlaced)
                    {
                        Log.Warning("Could not place " + currentThing.Label);
                        this.teleporterBuffer.TryAdd(currentThing);
                    }
                    else
                    {
                        this.currentCapacitorCharge = 0;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("=== COULD NOT SPAWN !!!! === " + e.Message);
                    continue;
                }
            }

            recallData.Clear();

            // Tell the MapDrawer that here is something that's changed
            #if RIMWORLD15
            Find.CurrentMap.mapDrawer.MapMeshDirty(Position, MapMeshFlagDefOf.Things, true, false);
            #else
            Find.CurrentMap.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things, true, false);
            #endif

            return !this.teleporterBuffer.Any();
        }

        private void updatePowerDrain()
        {
            this.power.powerOutputInt = -1000 * this.chargeSpeed;
        }

        #endregion

        public bool UpdateRequiredPower(float extraPower)
        {
            this.power.PowerOutput = -1 * extraPower;
            return true;
        }

        private void DoBlastVisual()
        {
            var cell = this.Position;
            // Mote mote = (Mote)ThingMaker.MakeThing(ThingDefOf.Mote_PsycastAreaEffect, null);
            Mote mote = (Mote)ThingMaker.MakeThing(ThingDefOf.Mote_Bombardment, null);
            // mote.Scale = (-80f * this.Countdown ?? 0) + 800f + 180f;
            mote.Scale = (-10f * this.Countdown ?? 0) + 100f + 10f;
            mote.rotationRate = Rand.Range(-3f, 3f);
            mote.exactPosition = cell.ToVector3Shifted() + new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);

            // mote.instanceColor = new Color(0, 120/255.0f, 1.0f);
            mote.instanceColor = new Color(0.9f, 0.9f, 0.7f);

            GenSpawn.Spawn((Thing) mote, cell, this.Map);
        }

        public string RenamableLabel { get => this.Name; set => this.Name = value; }
        public string BaseLabel => this.Name;
        public string InspectLabel => this.Name;
    }
}
