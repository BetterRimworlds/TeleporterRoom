/*
 * This file is part of Teleporter Room, a Better Rimworlds Project.
 *
 * Copyright © 2025 Theodore R. Smith
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
using Verse;

namespace BetterRimworlds.TeleporterRoom;

public static class TeleporterRoomValidator
{
    public const int MAX_ROOM_CELL_COUNT = 300;

    public static string ValidateRoom(Room? room, string? teleporterName)
    {
        var rejectReasons = new List<string>();

        if (room == null || room.CellCount >= 15_000)
        {
            rejectReasons.Add("The Teleporter must be placed inside a Room (use Room Stats tool to debug).");

            return String.Join("\n", rejectReasons);
        }

        if (String.IsNullOrEmpty(teleporterName))
        {
            teleporterName = "this Teleporter";
        }

        if (room.CellCount > MAX_ROOM_CELL_COUNT)
        {
            rejectReasons.Add($"The room of {teleporterName} is too big (12x25, or {MAX_ROOM_CELL_COUNT} max cells).");
        }

        if (room.OpenRoofCount > 0)
            rejectReasons.Add($"The room of {teleporterName} has {room.OpenRoofCount} missing roof tiles (use Room Stats tool to debug).");

        if (!PlaceWorker_OnlyOneTeleporterRoom.isPlasteelWall(room))
            rejectReasons.Add($"The walls of {teleporterName}'s room must be made completely of Plasteel.");

        if (!PlaceWorker_OnlyOneTeleporterRoom.isSterileFloor(room))
            rejectReasons.Add($"The floors of {teleporterName}'s room must be made completely of Sterile Tile.");

        return rejectReasons.Count == 0 ? "OK" : string.Join("\n", rejectReasons);
    }
}
