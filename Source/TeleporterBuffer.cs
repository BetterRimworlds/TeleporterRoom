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
using Verse;

namespace BetterRimworlds.TeleporterRoom
{
    public class TeleporterBuffer : ThingOwner<Thing>, IList<Thing>
    {
        Thing IList<Thing>.this[int index]
        {
            get => this.GetAt(index);
            set => throw new InvalidOperationException("ThingOwner doesn't allow setting individual elements.");
        }

        private IntVec3 Position;

        public TeleporterBuffer(IThingHolder owner, IntVec3 position, bool oneStackOnly, LookMode contentsLookMode = LookMode.Deep) :
            base(owner, oneStackOnly, contentsLookMode)
        {
            this.maxStacks = 5000;
            this.contentsLookMode = LookMode.Deep;
            this.Position = position;
        }

        public TeleporterBuffer(IThingHolder owner): base(owner)
        {
            this.maxStacks = 5000;
            this.contentsLookMode = LookMode.Deep;
        }

        public override bool TryAdd(Thing item, bool canMergeWithExistingStacks = true)
        {
            // Increase the maxStacks size for every Pawn, as they don't affect the dispersion area.
            // if (item is Pawn)
            // {
            //     ++this.maxStacks;
            // }

            if (item is Pawn pawn)
            {
                ++this.maxStacks;
            }
            else
            {
                if (this.InnerListForReading.Count >= this.maxStacks)
                {
                    return false;
                }
            }

            // Clear its existing Holder (the actual Stargate).
            item.holdingOwner = null;
            if (!base.TryAdd(item, canMergeWithExistingStacks))
            {
                return false;
            }

            item.DeSpawn();

            return true;
        }

        public int getMaxStacks()
        {
            return this.maxStacks;
        }
    }
}
