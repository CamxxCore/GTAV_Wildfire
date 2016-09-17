using GTA.Math;

namespace Wildfire
{
    public class GTAFireSpread
    {
        public const int MinSpreadTime = 5000;

        public const int MaxSpreadTime = 15000;

        GTAFireSpreadNode baseNode;

        GTAFireRegion parentRegion;

        public GTAFireSpread(GTAFireRegion parent)
        {
            parentRegion = parent;
        }

       /* public void SetFireDirection(Vector3 direction)
        {
            this.direction = direction;
        }*/

        public void StopBurn()
        {
            baseNode?.Dispose();
        }

        public void StartBurn(Vector3 position)
        {
            if (baseNode == null)
            {
               baseNode = new GTAFireSpreadNode(null, parentRegion, position);


            }
        }

        public void Update()
        {
            baseNode?.Update();
           
            /*       var rotation = Utility.Helpers.DirectionToRotation(direction);

                   var ray = Function.Call<int>(Hash._0xFE466162C4401D18, 
                       position.X, position.Y, position.Z, 40.0f, 40.0f, 40.0f, rotation.X, rotation.Y, rotation.Z, 2, 346, 
                       Game.Player.Character.Handle, 4);  // SHAPE_TEST_BOX

                   OutputArgument didHit = new OutputArgument(), 
                       endCoords = new OutputArgument(), 
                       surfaceNormal = new OutputArgument(), 
                       entityHit = new OutputArgument();

                   int result = Function.Call<int>((Hash)0x3D87450E15D98694, ray, didHit, endCoords, surfaceNormal, entityHit);

                   if (didHit.GetResult<bool>())
                   {
                       UI.ShowSubtitle(entityHit.GetResult<int>().ToString());
                   }

                   else
                   {
                       UI.ShowSubtitle("no hit");
                   }
                   */
            // UI.ShowSubtitle(arg1.GetResult<Vector3>().ToString());
        }
    }
}
