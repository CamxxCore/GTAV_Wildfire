using System;
using System.Linq;
using GTA;
using GTA.Math;
using System.Collections.Generic;
using Wildfire.Utility;

namespace Wildfire
{
    public class GTAFireSpreadNode : IDisposable
    {
        private readonly Vector3 SpreadSeperation = new Vector3(4f, 0.0f, 4f);

        private int lastExtinguishCheckTime = 0;
        private int nextSpreadTime = 0;
        private List<GTAFireSpreadNode> Children = new List<GTAFireSpreadNode>();
        private readonly LoopedPTFX fireFX;
        private int startTime;
        private int lastIncreaseTime;
        private const int WaterDamageAmount = 20;
        public readonly int MaxFireHealth;
        public const float FirePostBurnFXScale = 0.6f;
        public const float MaxFireFXScale = 0.6f;
        public const float MinFireFXScale = 0.0f;

        public Vector3 Location { get; private set; }

        public int FireHealth { get; private set; }

        public int AliveTime
        {
            get
            {
                return Game.GameTime - startTime;
            }
        }

        public bool PreBurnCompleted
        {
            get
            {
                return AliveTime > 10000;
            }
        }

        public bool DidSpread { get; set; }

        public GTAFireSpreadNode Parent { get; set; }

        private GTAFireRegion ParentRegion { get; set; }

        public GTAFireSpreadNode(GTAFireSpreadNode parent, GTAFireRegion parentRegion, Vector3 position)
        {
            Parent = parent;
            ParentRegion = parentRegion;
            Location = position;
            MaxFireHealth = Helpers.Rand<int>(Enumerable.Range(3800, 4600));
            FireHealth = MaxFireHealth / 2;
            fireFX = new LoopedPTFX("core", "ent_amb_fbi_fire_lg");
            fireFX.Load();
            fireFX.SetEvolution("LOD", 10000f);
            fireFX.Start(Location + new Vector3(0.0f, 0.0f, 0.87f), new Vector3(0.0f, 90f, 0.0f), 1f);
            startTime = Game.GameTime;
        }

        public void RegisterDamage()
        {
            if (FireHealth <= 0)
                return;
            FireHealth = Helpers.Clamp<int>(FireHealth - 20, 0, MaxFireHealth);
        }

        public IEnumerable<GTAFireSpreadNode> GetNestedNodes()
        {
            yield return this;
            foreach (GTAFireSpreadNode gtaFireSpreadNode in Children)
            {
                foreach (GTAFireSpreadNode nestedNode in gtaFireSpreadNode.GetNestedNodes())
                {
                    yield return nestedNode;
                }
            }
        }

        public void Update()
        {
            Children.ForEach(x => x.Update());

            if (FireHealth <= 0)
                return;

            if (FireHealth / MaxFireHealth < 0.300000011920929 && Game.GameTime - lastExtinguishCheckTime > 2200)
            {
                if (new Random(Environment.TickCount).Next(0, 10000) / 10000f > 0.860000014305115)
                    RegisterDamage();
                lastExtinguishCheckTime = Game.GameTime;
            }

            if (FireHealth < MaxFireHealth && Game.GameTime - lastIncreaseTime > 1000)
            {
                FireHealth = FireHealth + 40;
                lastIncreaseTime = Game.GameTime;
            }

            if (Game.GameTime > nextSpreadTime && !DidSpread)
            {
                int num1 = new Random(Guid.NewGuid().GetHashCode()).Next(1, 6);

                var nestedNodes = GetNestedNodes();

                for (int index = 0; index < num1; ++index)
                {
                    Vector3 newFirePos = new Vector3();
                    int count = 0;
                    while (true)
                    {
                        newFirePos = Location.Around(5f);

                        if (Enumerable.Any(nestedNodes, y => y.Location.DistanceTo(newFirePos) < 5.0))
                        {
                            if (count <= 10)
                            {
                                Script.Wait(0);
                                ++count;
                            }

                            else break;
                        }

                        else break;
                    }

                    newFirePos.Z = World.GetGroundHeight(newFirePos) - 1f;

                    var newNode = new GTAFireSpreadNode(this, ParentRegion, newFirePos);

                    if (newNode.fireFX.Exists)
                    {
                        Children.Add(newNode);
                        UI.ShowSubtitle("spreading " + Children.Count.ToString() + " " + newNode.Location.ToString());
                        DidSpread = true;
                    }

                    else
                    {
                        GTAFireSpreadNode parent = Parent;

                        GTAFireSpreadNode gtaFireSpreadNode = Parent != null ? Helpers.Rand(parent.Children) : null;

                        if (gtaFireSpreadNode != null)
                        {
                            gtaFireSpreadNode.Dispose();
                            Children.Remove(gtaFireSpreadNode);
                        }

                        UI.ShowSubtitle("Couldn't create a new node..");
                    }
                }
            }
        }

        public void Dispose()
        {
            LoopedPTFX loopedPtfx = fireFX;
            if (loopedPtfx != null)
                loopedPtfx.Remove();
            foreach (GTAFireSpreadNode gtaFireSpreadNode in Children)
                gtaFireSpreadNode.Dispose();
        }
    }
}


                                                                                                                                                                                                                                                                          