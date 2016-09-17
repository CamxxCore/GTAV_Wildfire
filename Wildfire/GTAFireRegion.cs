using System;
using System.Collections.Generic;
using GTA.Math;
using GTA.Native;
using System.Linq;
using Wildfire.Utility;

namespace Wildfire
{
    public class GTAFireRegion
    {
        public bool bDebug = false;

        public string Alias { get; private set; }

        public Vector3 Center { get; private set; }

        public GTARegionNode[] Nodes { get; private set; }

        public GTARegionPerimeter Perimeter { get; private set; }

        public GTARegionNode StartNode { get { return startNode; } }

        public int Probability { get; set; }

        public int Density { get; set; }

        public bool DebugDraw { get; set; }

        public bool IsBurning { get; private set; }

        public int AmountBurned { get; private set; }

        private List<LoopedPTFX> activePTFX =
            new List<LoopedPTFX>();

        private List<GTAFireNode> activeNodes =
            new List<GTAFireNode>();

        private Queue<GTARegionNode> potentialNodes = 
            new Queue<GTARegionNode>();

        private Queue<GTAFireNode> extinguishedNodes = 
            new Queue<GTAFireNode>();

        private GTAFireSpread fireSpread;

        private const int MaxActiveNodes = 34;

        private int lastSpreadCheckTime = 0;

        private GTARegionNode startNode;

        public GTAFireRegion(FireRegionParams paramsObj)
        {
            Alias = paramsObj.Alias;
            Perimeter = paramsObj.Perimeter;
            Nodes = paramsObj.Nodes;
            Density = paramsObj.Density;
            Probability = paramsObj.Probability;
            Center = Helpers.GetCentroid(Perimeter.Vertices);
            fireSpread = new GTAFireSpread(this);
        }

        public void StartBurn(GTARegionNode regionNode)
        {
            if (!IsBurning)
            {
                GTA.Game.Player.Character.Position = regionNode.Location;

                startNode = regionNode;

                activeNodes.Add(new GTAFireNode(regionNode));

                fireSpread.StartBurn(startNode.Location);

                IsBurning = true;

                GTA.UI.Notify("Fire started.");
            }
        }

        public void StartBurn()
        {
            GTA.UI.Notify("Finding start node for fire routine...");
            StartBurn(Nodes.Rand());
        }

        public void StopBurn()
        {
            fireSpread?.StopBurn();
            activeNodes.ForEach(x => x.Dispose());
        }

        public void Update()
        {
            if (IsBurning)
            {
                if (activeNodes.Count < 1)
                {
                    GTA.UI.Notify("All nodes in the region have burned");
                    IsBurning = false;
                    return;
                }

                fireSpread?.Update();

                float random = 0.0f;

                if (GTA.Game.GameTime - lastSpreadCheckTime > 2200)
                {
                    random = ((float)new Random(Environment.TickCount).Next(0, 10000)) / 10000.0f;

                    lastSpreadCheckTime = GTA.Game.GameTime;
                }

                foreach (var node in activeNodes)
                {
                    if (node.FireHealth <= 0)
                    {
                        extinguishedNodes.Enqueue(node);
                    }

                    else
                    {
                        node.Update();

                        if (random > 0.86f && !node.DidSpread && (bDebug || (node.FireHealth / node.MaxFireHealth) > 0.52f))
                        {
                            if (activeNodes.Count < MaxActiveNodes)
                            {
                                var nearbyNodes = Nodes.Where(x => x.Active && x.Location.DistanceTo(node.Location) < 3.5f * Density);

                                if (nearbyNodes.Count() > 5)
                                {
                                    nearbyNodes = nearbyNodes.Take(5);
                                }

                                foreach (var vNode in nearbyNodes)
                                {
                                    potentialNodes.Enqueue(vNode);
                                }

                                GTA.UI.Notify(string.Format("Spreading to {0} nearby nodes.", nearbyNodes.Count()));

                                node.DidSpread = true;
                            }
                        }

                        var wcBaseVehicle = MemoryAccess.GetWaterCannonActiveVehicleHash();

                        if (wcBaseVehicle != -1)
                        {
                            if (Function.Call<bool>(Hash.IS_THIS_MODEL_A_CAR, wcBaseVehicle))
                            {
                                if (MemoryAccess.WaterCannonInRadius(node.Location.X, node.Location.Y, node.Location.Z, 15.0f))
                                {
                                    node.RegisterDamage();                           
                                }
                            }

                            else
                            {
                                if (MemoryAccess.WaterCannonInVerticalRadius(node.Location.X, node.Location.Y, node.Location.Z, 19.6f, 20000.0f))
                                {
                                    node.RegisterDamage();
                                }
                            }

                            GTA.UI.ShowSubtitle(node.FireHealth.ToString());
                        }              

                    }
                }

                while (potentialNodes.Count > 0)
                {
                    GTARegionNode regionNode = potentialNodes.Dequeue();

                    GTAFireNode fireNode = new GTAFireNode(regionNode);

                    activeNodes.Add(fireNode);

                    regionNode.Active = false;
                }

                while (extinguishedNodes.Count > 0)
                {
                    GTAFireNode node = extinguishedNodes.Dequeue();

                    GTA.UI.Notify("Fire node extinguished");

                    node.Dispose();

                    activeNodes.Remove(node);
                }
            }

            if (DebugDraw)
            {
                Perimeter.Draw();

                for (int i = 0; i < Nodes.Length; i++)
                {
                    Nodes[i].Draw();
                }
            }
        }
    }
}
