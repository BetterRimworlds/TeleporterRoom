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

using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace BetterRimworlds.TeleporterRoom
{
    #if RIMWORLD15
    public class Dialog_NameTeleporterRoom : Dialog_Rename<Building_Teleporter>
    #else
    public class Dialog_NameTeleporterRoom : Dialog_Rename
    #endif
    {
        private readonly Building_Teleporter teleporter;
        private static readonly Regex ValidNameRegex = new Regex(@"^[\w ]+$");

        #if RIMWORLD15
        public Dialog_NameTeleporterRoom(Building_Teleporter renaming): base(renaming)
        {
        }
        #else
        public Dialog_NameTeleporterRoom(Building_Teleporter teleporter)
        {
            this.teleporter = teleporter;
            base.curName = this.teleporter.Name;
        }
        #endif

        protected override int MaxNameLength => 24;

        public override void DoWindowContents(Rect inRect)
        {
            var titleRect = new Rect(inRect.x, inRect.y, inRect.width, 40);
            Widgets.Label(titleRect, "Name this Teleporter");

            base.DoWindowContents(inRect);
        }
        public static AcceptanceReport IsValidName(string name)
        {
            if (!ValidNameRegex.IsMatch(name))
            {
                return new AcceptanceReport("This is not a valid Teleporter Room name.");
            }

            // // TODO: figure out why this doesn't work
            // if (BlueprintController.FindBlueprint(name) != null)
            // {
            //     return new AcceptanceReport("That Teleporter name has already been used.");
            // }

            return true;
        }

        protected override AcceptanceReport NameIsValid(string newName)
        {
            // always ok if we didn't change anything
            if (newName == this.teleporter.Name)
            {
                return true;
            }

            // otherwise check for used symbols and uniqueness
            AcceptanceReport validName = IsValidName(newName);
            if (!validName.Accepted)
            {
                return validName;
            }

            // if all checks are passed, return true.
            return true;
        }

        #if !RIMWORLD15
        protected override void SetName( string name )
        {
            this.teleporter.Name = name;
        }
        #endif
    }
}
