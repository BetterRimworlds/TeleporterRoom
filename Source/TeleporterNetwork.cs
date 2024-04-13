using System.Collections;
using System.Collections.Generic;
using RimWorld.Planet;

namespace BetterRimworlds.TeleporterRoom
{
    public class TeleporterNetwork: WorldComponent, IEnumerable
    {
        private List<Building_Teleporter> network = new List<Building_Teleporter>();

        public TeleporterNetwork(World world) : base(world)
        {
        }

        // Add an item to the network
        public void Add(Building_Teleporter item) => network.Add(item);

        // Remove an item from the network
        public bool Remove(Building_Teleporter item) => network.Remove(item);

        // Get the count of teleporters in the network
        public int Count => network.Count;

        // Indexer to access items in the network
        public Building_Teleporter this[int index]
        {
            get => network[index];
            set => network[index] = value;
        }

        // Expose the enumerator to allow foreach loops
        public IEnumerator<Building_Teleporter> GetEnumerator() => network.GetEnumerator();

        // Non-generic enumerator (needed for IEnumerable interface)
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
