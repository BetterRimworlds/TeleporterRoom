/*
 * This file is part of Teleporter Room, a Better Rimworlds Project.
 *
 * Copyright © 2024 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *   GPG Fingerprint: D8EA 6E4D 5952 159D 7759  2BB4 EEB6 CE72 F441 EC41
 *   https://github.com/BetterRimworlds/TeleporterRoom
 *   https://www.glitterworlds.dev/
 *
 * This file is licensed under the Creative Commons No-Derivations v4.0 License.
 * Most rights are reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterRimworlds.TeleporterRoom
{
    class PlaceWorker_OnlyOneTeleporterRoom : PlaceWorker_OnlyOneBuilding
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            List<string> rejectReasons = new List<string>();

            List<Thing> blueprints = map.listerThings.ThingsOfDef(checkingDef.blueprintDef);
            List<Thing> frames = map.listerThings.ThingsOfDef(checkingDef.frameDef);
            if (
                ((blueprints != null) && (blueprints.Count > 0))
               || ((frames != null) && (frames.Count > 0))
               || map.listerBuildings.ColonistsHaveBuilding(ThingDef.Named(checkingDef.defName))
               || map.listerBuildings.ColonistsHaveBuilding(ThingDef.Named("Teleporter"))
               )
            {
                return "You can only build one Teleporter per map.";
            }

            // Log.Warning("1");

            // Check for the Teleporter Room requirements.
            var room = RegionAndRoomQuery.RoomAt(new IntVec3(loc.x, loc.y, loc.z + 2), map);

            bool rejected = false;
            if (room == null || room?.CellCount > 15_000)
            {
                rejected = true;
                rejectReasons.Add("The Teleporter must be placed inside a Room (use Room Stats tool to debug).");
            }

            if (rejected || room?.CellCount > 300)
            {
                rejected = true;
                rejectReasons.Add($"The room of this Teleporter is too big (12x25, or 300 max cells).");
            }

            if (rejected || room?.OpenRoofCount > 0)
            {
                rejected = true;

                rejectReasons.Add($"The room of this Teleporter has {room?.OpenRoofCount} missing roof tiles (use Room Stats tool to debug).");
            }

            if (rejected || PlaceWorker_OnlyOneTeleporterRoom.isPlasteelWall(map, room) == false)
            {
                rejectReasons.Add("The room's walls must be made completely of Plasteel.");
            }

            if (rejected || PlaceWorker_OnlyOneTeleporterRoom.isSterileFloor(map, room) == false)
            {
                rejectReasons.Add("The room's floors must be made completely of Sterile Tile.");
            }

            return rejectReasons.Count == 0 ? true : String.Join("\n", rejectReasons);
        }

        public static bool isPlasteelWall(Map map, Room room)
        {
            // Log.Warning("Border Cells: " + String.Join(", ", room.BorderCells));
            foreach (IntVec3 borderPosition in room.BorderCells)
            {
                var wall = borderPosition.GetEdifice(map);

                if (wall == null) return false;

                if ((wall.def != ThingDefOf.Wall || wall.def != ThingDefOf.Door || wall.def.defName != "Teleporter")
                    && (wall.def.defName != "Teleporter" && wall.Stuff != ThingDefOf.Plasteel))
                {
                    // Log.Warning(borderPosition + " : " + wall?.def + " (" + wall?.def?.defName + ") Stuff: " + wall?.Stuff?.defName);
                    return false;
                }

            }

            return true;
        }

        public static bool isSterileFloor(Map map, Room room)
        {
            // Log.Warning("Floor Cells: " + String.Join(", ", room.Cells));
            foreach (IntVec3 floorCell in room.Cells)
            {
                if (floorCell.GetTerrain(map).defName != "SterileTile")
                {
                    Log.Warning(floorCell + " Terrain Def Name: " + floorCell.GetTerrain(map).defName);
                    return false;
                }
            }

            return true;
        }
    }
}
