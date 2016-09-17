using System;
using System.Linq;
using GTA;
using GTA.Native;
using GTA.Math;
using Wildfire.Utility;

namespace Wildfire
{
    class GTAFireNode : IDisposable
    {
        public Vector3 Location { get; private set; }

        public int FireHealth { get; private set; }

        public int AliveTime { get { return Game.GameTime - startTime; } }

        public bool PreBurnCompleted { get { return AliveTime > 10000; } }

        public bool DidSpread { get; set; }

        private readonly LoopedPTFX fireFX, fireFXB;

        private int startTime, lastIncreaseTime;

        private int lastExtinguishCheckTime = 0;

        const int WaterDamageAmount = 20;

        private int soundID = -1;

        private Blip blip;

        /// <summary>
        /// The maximum fire health, set by ctor.
        /// </summary>
        public readonly int MaxFireHealth;

        /// <summary>
        /// The scale the fire FX should be at after pre-burning.
        /// </summary>
        public const float FirePostBurnFXScale = 0.6f;

        /// <summary>
        /// The scale the fire FX should be at after pre-burning.
        /// </summary>
        public const float FireBPostBurnFXScale = 0.8f;

        /// <summary>
        /// The scale the smoke FX should be at after pre-burning.
        /// </summary>
        public const float SmokePostBurnFXScale = 1.35f;

        /// <summary>
        /// The maximum allowed scale for the fire FX.
        /// </summary>
        public const float MaxFireFXScale = 0.6f;

        /// <summary>
        /// The maximum allowed scale for the fire FX.
        /// </summary>
        public const float MaxFireFXBScale = 1.4f;

        /// <summary>
        /// The maximum allowed scale for the smoke FX.
        /// </summary>
        public const float MaxSmokeFXScale = 1.8f;

        /// <summary>
        /// The minimum allowed scale for the fire FX.
        /// </summary>
        public const float MinFireFXScale = 0.0f;

        /// <summary>
        /// The maximum allowed scale for the fire FX.
        /// </summary>
        public const float MinFireFXBScale = 0.5f;

        /// <summary>
        /// The minimum allowed scal for the smoke FX.
        /// </summary>
        public const float MinSmokeFXScale = 0.4f;

       private readonly LoopedPTFX smokeFX = new LoopedPTFX("core", "ent_amb_smoke_foundry_white");

        public GTAFireNode(GTARegionNode regionNode)
        {
            Location = regionNode.Location;
            MaxFireHealth = Enumerable.Range(3800, 4600).Rand();
            FireHealth = MaxFireHealth / 2;
            fireFX = new LoopedPTFX("core", "ent_amb_fbi_fire_lg");
            fireFXB = new LoopedPTFX("cut_exile2", "scr_ex2_jeep_engine_fire");
            fireFXB.Load();
            fireFX.SetEvolution("LOD", 10000.0f);
            fireFXB.SetEvolution("LOD", 10000.0f);
            fireFXB.Start(Location + new Vector3(0, 0,1.033f), new Vector3(0, 0,0), 0.0f);
            fireFX.Start(Location + new Vector3(0,0, 0.87f), new Vector3(0, 90, 0), 0.0f);
            smokeFX.Start(Location + new Vector3(0, 0, 10.0f), 1.0f);
            smokeFX.SetEvolution("LOD", 10000.0f);
            soundID = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FROM_COORD, soundID, "HOUSE_FIRE", 
                Location.X, Location.Y, Location.Z + 1.2f, 
                "JOSH_03_SOUNDSET", 0, 0, 0);
            blip = World.CreateBlip(Location);
            blip.Color = BlipColor.Red;
            startTime = Game.GameTime;
        }

        public void RegisterDamage()
        {
            if (FireHealth > 0)
            {
                FireHealth = (FireHealth - WaterDamageAmount).Clamp(0, MaxFireHealth);
           }       
        }

        public void Update()
        {
            if (FireHealth > 0)
            {
                if ((FireHealth / MaxFireHealth) < 0.3f && (Game.GameTime - lastExtinguishCheckTime > 2200))
                {
                    float random = ((float)new Random(Environment.TickCount).Next(0, 10000)) / 10000.0f;

                    if (random > 0.86f)
                    {
                        RegisterDamage();
                    }

                    lastExtinguishCheckTime = Game.GameTime;
                }

                float fireFXScale = 0.0f, fireFXBScale, smokeFXScale = 0.0f;

                if (!PreBurnCompleted)
                {
                    fireFXScale = MinFireFXScale + (float)AliveTime / (float)10000 * FirePostBurnFXScale;
                    fireFXBScale = MinFireFXBScale + (float)AliveTime / (float)10000 * FireBPostBurnFXScale;
                    smokeFXScale = MinSmokeFXScale + (float)AliveTime / (float)10000 * SmokePostBurnFXScale;
                }

                else
                {
                    fireFXScale = MinFireFXScale + ((float)FireHealth / (float)MaxFireHealth * MaxFireFXScale);
                    fireFXBScale = MinFireFXBScale + ((float)FireHealth / (float)MaxFireHealth * MaxFireFXBScale);
                    smokeFXScale = MinSmokeFXScale + ((float)FireHealth / (float)MaxFireHealth * MaxSmokeFXScale);
                }

                fireFX.Scale = fireFXScale;

                fireFXB.Scale = fireFXBScale;

                smokeFX.Scale = smokeFXScale;

                if (FireHealth < MaxFireHealth && Game.GameTime - lastIncreaseTime > 1000)
                {
                    FireHealth += 40;
                    lastIncreaseTime = Game.GameTime;
                }
            }
        }

        public void Dispose()
        {
            Function.Call(Hash.STOP_SOUND, soundID);

            blip?.Remove();

            fireFX?.Remove();

            fireFXB?.Remove();

            smokeFX?.Remove();
        }
    }
}
