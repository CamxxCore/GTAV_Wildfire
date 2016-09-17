using System;
using System.Collections.Generic;
using System.Linq;
using Wildfire.Interfaces;

namespace Wildfire
{
    public class GTAFireController : IUpdatable, IDisposable
    {
        public List<GTAFireRegion> Regions { get; private set; }

        private bool bRandomFires;

        public GTAFireController(List<GTAFireRegion> regions)
        {
            Regions = regions;
        }

        public GTAFireController()
        {
            Regions = new List<GTAFireRegion>();
        }

        public void AddRegions(IEnumerable<GTAFireRegion> regions)
        {
            Regions.AddRange(regions);
        }

        public void AddRegion(GTAFireRegion region)
        {
            Regions.Add(region);
        }

        public string CreateRandomFire()
        {
            if (Regions.Count < 1) return null;

            float totalWeight = 0;

            Regions.ForEach(x => totalWeight += (x.Probability / 10.0f));

            float result = new Random(Environment.TickCount).Next(0, 10000) / 10000.0f;

            result *= totalWeight;

            float modifier = 0.0f;

            foreach (var region in Regions.OrderBy(x => Environment.TickCount))
            {
                float cacheValue = ((float)region.Probability / 10) + modifier;

                if (result <= cacheValue)
                {
                    region.StartBurn();
                    return region.Alias;
                }

                modifier = cacheValue;
            }

            var vRegion = Regions.Last();
            vRegion.StartBurn();
            return vRegion.Alias;
        }

        public bool CreateFireAtRegion(string alias)
        {
            var region = Regions.Find(x => x.Alias == alias);

            if (region != null)
            {
                region.StartBurn();
                return true;
            }

            else return false;
        }

        public void ToggleRandomFires(bool value)
        {
            bRandomFires = value;
        }

        public void Update()
        {
            foreach (var region in Regions)
            {
                region.Update();
            }
        }

        public void RemoveFires()
        {
            foreach (var region in Regions)
            {
                if (region.IsBurning)
                {
                    region.StopBurn();
                }
            }
        }

        public void Dispose()
        {
            RemoveFires();
        }
    }
}

