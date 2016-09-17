using GTA.Native;
using GTA.Math;
using Wildfire.Interfaces;

namespace Wildfire
{
    public sealed class GTARegionPerimeter : IDrawable
    {
        public Vector3[] Vertices { get; private set; }

        public GTARegionPerimeter(Vector3[] vertices)
        {
            Vertices = vertices;
        }

        public void Draw()
        {
            for (int j = 1; j < Vertices.Length; j++)
            {
                Function.Call(Hash.DRAW_LINE, Vertices[j - 1].X, Vertices[j - 1].Y, Vertices[j - 1].Z,
                     Vertices[j].X, Vertices[j].Y, Vertices[j].Z, 255, 255, 0, 255);
            }
        }
    }
}
