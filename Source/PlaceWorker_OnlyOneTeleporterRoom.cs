/*
 * This file is part of Teleporter Room, a Better Rimworlds Project.
 *
 * Copyright © 2024-2025 Theodore R. Smith
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

namespace BetterRimworlds.TeleporterRoom;

class PlaceWorker_OnlyOneTeleporterRoom : PlaceWorker_OnlyOneBuilding
{
    private static bool ShowDebugMsg = new Settings().showDebugMessages;

    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing? thingToIgnore = null, Thing thing = null)
    {
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

        string rejectReasons = TeleporterRoomValidator.ValidateRoom(room, null);

        return (rejectReasons == "OK") ? true : rejectReasons;
    }

    public static bool isPlasteelWall(Room room)
    {
        // Log.Warning("Border Cells: " + String.Join(", ", room.BorderCells));
        foreach (IntVec3 borderPosition in room.BorderCells)
        {
            var wall = borderPosition.GetEdifice(room.Map);

            if (wall == null)
            {
                continue;
            }

            if ((wall.def != ThingDefOf.Wall || wall.def != ThingDefOf.Door || wall.def.defName != "Teleporter")
                && (wall.def.defName != "Teleporter" && wall.Stuff != ThingDefOf.Plasteel && wall.def.defName != "Stargate"))
            {
                if (ShowDebugMsg) Log.Warning(borderPosition + " : " + wall?.def + " (" + wall?.def?.defName + ") Stuff: " + wall?.Stuff?.defName);
                return false;
            }

        }

        return true;
    }

    public static bool isSterileFloor(Room room)
    {
        // Log.Warning("Floor Cells: " + String.Join(", ", room.Cells));
        foreach (IntVec3 floorCell in room.Cells)
        {
            if (floorCell.GetTerrain(room.Map).defName != "SterileTile")
            {
                if (ShowDebugMsg) Log.Warning(floorCell + " Terrain Def Name: " + floorCell.GetTerrain(room.Map).defName);
                return false;
            }
        }

        return true;
    }
}
