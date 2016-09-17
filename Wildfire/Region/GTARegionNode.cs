using System;
using GTA.Math;
using GTA.Native;
using TestScript.GTAV_Firefighter.Interfaces;

namespace TestScript.GTAV_Firefighter.Region
{
    public sealed class GTARegionNode : IDrawable
    {
        public bool Active { get; set; } = true;

        public Vector3 Location { get; set; }

        private Tuple<Vector3, Vector3>[] drawInfo;

        public GTARegionNode(Vector3 location)
        {
            Location = location;
            drawInfo = new Tuple<Vector3, Vector3>[6];
            ResetDraw();
        }

        public void ResetDraw()
        {
            for (int x = 0; x < 6; x++)
            {
                drawInfo[x] = new Tuple<Vector3, Vector3>(
                    Location.Radius(2.0f, 6, x - 1),
                    Location.Radius(2.0f, 6, x));
            }
        }

        public void Draw()
        {
           for (int x = 0; x < 6; x++)
            {
                Function.Call(Hash.DRAW_LINE, drawInfo[x].Item1.X, drawInfo[x].Item1.Y, drawInfo[x].Item1.Z,
                    drawInfo[x].Item2.X, drawInfo[x].Item2.Y, drawInfo[x].Item2.Z, 255, 0, 0, 255);
            }
        }

        public static explicit operator GTARegionNode(Vector3 vec)  // explicit byte to digit conversion operator
        {
            return new GTARegionNode(vec);  // explicit conversion
        }
    }
}
