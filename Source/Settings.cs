/*
 * This file is part of ThermoVoltaic Generator, a Better Rimworlds Project.
 *
 * Copyright © 2025 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *   GPG Fingerprint: D8EA 6E4D 5952 159D 7759  2BB4 EEB6 CE72 F441 EC41
 *   https://github.com/BetterRimworlds/TeleporterRoom
 *
 * This file is licensed under the MIT License.
 */

using UnityEngine;
using Verse;

namespace BetterRimworlds.TeleporterRoom;

public class Settings : ModSettings
{
    public bool showDebugMessages = false;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref showDebugMessages, "brw.teleporterroom.showDebugMessages", false);
    }

    public void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);

        string[] labels =
        {
            "Show Debug Messages",
        };

        listing_Standard.CheckboxLabeled(labels[0], ref showDebugMessages);

        listing_Standard.End();
    }
}
