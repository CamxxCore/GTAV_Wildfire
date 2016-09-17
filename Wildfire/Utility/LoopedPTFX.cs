using GTA;
using GTA.Native;
using GTA.Math;

namespace Wildfire.Utility
{
    public class LoopedPTFX
    {
        private float scale;

        public int Handle { get; private set; }
        public string AssetName { get; private set; }
        public string FXName { get; private set; }

        /// <summary>
        /// If the particle FX is spawned.
        /// </summary>
        public bool Exists { get { if (Handle == -1) return false; return Function.Call<bool>(Hash.DOES_PARTICLE_FX_LOOPED_EXIST, Handle); } }

        /// <summary>
        /// If the particle FX asset is loaded.
        /// </summary>
        public bool IsLoaded { get { return Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, AssetName); } }

        /// <summary>
        /// Set the particle FX scale.
        /// </summary>
        public float Scale { get { return scale; } set { Function.Call(Hash.SET_PARTICLE_FX_LOOPED_SCALE, Handle, scale = value); } }

        /// <summary>
        /// Set the particle FX looped colour.
        /// </summary>
       // public Color Colour { set { Function.Call(Hash.SET_PARTICLE_FX_LOOPED_COLOUR, Handle, value.R, value.G, value.B, 0); } }

        public LoopedPTFX(string assetName, string fxName)
        {
            this.Handle = -1;
            this.AssetName = assetName;
            this.FXName = fxName;
            Load();
        }

        /// <summary>
        /// Load the particle FX asset.
        /// </summary>
        public void Load()
        {
            if (!IsLoaded)
            {
                Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, AssetName);

                while (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, AssetName))
                {
                    Script.Yield();
                }
            }
        }

        /// <summary>
        /// Start particle FX on the specified entity.
        /// </summary>
        /// <param name="entity">Entity to attach to.</param>
        /// <param name="scale">Scale of the fx.</param>
        /// <param name="offset">Optional offset.</param>
        /// <param name="rotation">Optional rotation.</param>
        /// <param name="bone">Entity bone.</param>
        public void Start(Entity entity, Vector3 offset, Vector3 rotation, float scale, Bone? bone)
        {
            if (Handle != -1) return;

            this.scale = scale;

            Function.Call(Hash._SET_PTFX_ASSET_NEXT_CALL, AssetName);

            Handle = bone == null ?
                Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY, FXName,
                entity, offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z, scale, 0, 0, 1) :
                Function.Call<int>(Hash._START_PARTICLE_FX_LOOPED_ON_ENTITY_BONE, FXName,
                entity, offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z, (int)bone, scale, 0, 0, 0);
        }

        /// <summary>
        /// Start particle FX on the specified entity.
        /// </summary>
        /// <param name="entity">Entity to attach to.</param>
        /// <param name="scale">Scale of the fx.</param>
        public void Start(Entity entity, float scale)
        {
            Start(entity, Vector3.Zero, Vector3.Zero, scale, null);
        }

        /// <summary>
        /// Start particle FX at the specified position.
        /// </summary>
        /// <param name="position">Position in world space.</param>
        /// <param name="scale">Scale of the fx.</param>
        /// <param name="rotation">Optional rotation.</param>
        public void Start(Vector3 position, Vector3 rotation, float scale)
        {
            if (Handle != -1) return;

            this.scale = scale;

            Function.Call(Hash._SET_PTFX_ASSET_NEXT_CALL, AssetName);

            Handle = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_AT_COORD, FXName,
             position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, scale, 0, 0, 0, 0);
        }

        /// <summary>
        /// Start particle FX at the specified position.
        /// </summary>
        /// <param name="position">Position in world space.</param>
        /// <param name="scale">Scale of the fx.</param>
        public void Start(Vector3 position, float scale)
        {
            Start(position, Vector3.Zero, scale);
        }

        /// <summary>
        /// Set position offsets.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rotOffset"></param>
        public void SetOffsets(Vector3 offset, Vector3 rotOffset)
        {
            Function.Call(Hash.SET_PARTICLE_FX_LOOPED_OFFSETS, Handle, offset.X, offset.Y, offset.Z, rotOffset.X, rotOffset.Y, rotOffset.Z);
        }

        /// <summary>
        /// Set custom PTFX evolution variables.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public void SetEvolution(string variableName, float value)
        {
            Function.Call(Hash.SET_PARTICLE_FX_LOOPED_EVOLUTION, Handle, variableName, value, 0);
        }

        /// <summary>
        /// Remove the particle FX.
        /// </summary>
        public void Remove()
        {
            if (Handle == -1) return;

            Function.Call(Hash.STOP_PARTICLE_FX_LOOPED, Handle, 0);

            Function.Call(Hash.REMOVE_PARTICLE_FX, Handle, 0);
            Handle = -1;
        }

        /// <summary>
        /// Remove the particle FX in range.
        /// </summary>
        public static void Remove(Vector3 position, float radius)
        {
            Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, position.X, position.Y, position.Z, radius);
        }

        /// <summary>
        /// Unload the loaded particle FX asset.
        /// </summary>
        public void Unload()
        {
            if (IsLoaded)
                Function.Call((Hash)0x5F61EBBE1A00F96D, AssetName);
        }
    }
}
