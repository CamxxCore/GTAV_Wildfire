using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;

namespace Wildfire.Utility
{
    public static class Helpers
    {
        /// <summary>
        /// Extension for getting a random item from a collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T Rand<T>(this IEnumerable<T> items)
        {
            if (items.Count() < 1) return default(T);
            var arr = items.ToArray();
            var random = new Random(Guid.NewGuid().GetHashCode()).Next(0, arr.Length - 1);
            return (T)(object)arr[random];
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static double ToRadians(this float val)
        {
            return (Math.PI / 180) * val;
        }

        public static Vector3 GetCentroid(Vector3[] poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Length - 1; i < poly.Length - 1; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (accumulatedArea < 1E-7f)
                return Vector3.Zero;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new Vector3(centerX / accumulatedArea, centerY / accumulatedArea, poly[0].Z);
        }

        public static Vector2 GetCentroid(Vector2[] poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Length - 1; i < poly.Length - 1; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (accumulatedArea < 1E-7f)
                return Vector2.Zero;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
        }

        /// <summary>
        /// Returns a 3D coordinate on a circle on the given point with the specified center, radius and total amount of points
        /// </summary>
        /// <param name="center">Center of the circle</param>
        /// <param name="radius">Total radius of the circle</param>
        /// <param name="totalPoints">Total points around circumference</param>
        /// <param name="currentPoint">The point on the circle for which to return a coordinate</param>
        /// <returns></returns>
        public static Vector3 Radius(this Vector3 center, float radius, float totalPoints, float currentPoint)
        {
            float ptRatio = currentPoint / totalPoints;
            float pointX = center.X + (float)(Math.Cos(ptRatio * 2 * Math.PI)) * radius;
            float pointY = center.Y + (float)(Math.Sin(ptRatio * 2 * Math.PI)) * radius;
            GTA.Math.Vector3 panelCenter = new GTA.Math.Vector3(pointX, pointY, center.Z);
            return panelCenter;
        }

        public static Vector3 RotationToDirection(Vector3 Rotation)
        {
            double rotZ = Rotation.Z.ToRadians();
            double rotX = Rotation.X.ToRadians();
            double multiXY = Math.Abs(Convert.ToDouble(Math.Cos(rotX)));
            Vector3 res = default(Vector3);
            res.X = (float)(Convert.ToDouble(-Math.Sin(rotZ)) * multiXY);
            res.Y = (float)(Convert.ToDouble(Math.Cos(rotZ)) * multiXY);
            res.Z = (float)(Convert.ToDouble(Math.Sin(rotX)));
            return res;
        }

        public static Vector3 GetRandomPositionFromCoords(Vector3 position, float multiplier)
        {
            float randX, randY;

            int v1 = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 3999) / 1000;

            if (v1 == 0)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            else if (v1 == 1)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            else if (v1 == 2)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, -200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 50.0f) * multiplier;
            }
            else
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, -200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            return new Vector3(randX + position.X, randY + position.Y, position.Z);
        }


        public static Vector3 DirectionToRotation(Vector3 direction)
        {
            direction.Normalize();

            var x = Math.Atan2(direction.Z, Math.Sqrt(direction.Y * direction.Y + direction.X * direction.X));
            var y = 0;
            var z = -Math.Atan2(direction.X, direction.Y);

            return new Vector3
            {
                X = (float)RadToDeg(x),
                Y = (float)RadToDeg(y),
                Z = (float)RadToDeg(z)
            };
        }

        public static IEnumerable<float> FloatRange(float min, float max, float step)
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                float value = min + step * i;
                if (value > max)
                {
                    break;
                }
                yield return value;
            }
        }

        public static double RadToDeg(double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        public static Vector3 GetPositionInPoly(List<Vector3> vertices)
        {
            Vector3 vec = vertices.Rand(), vecA = vertices.Rand();
            return Vector3.Lerp(vec, vecA, (float)Enumerable.Range(30, 70).Rand() / 100);
        }

        public static bool InsidePolygon(Vector3 point, Vector3[] vertices)
        {
            int j = vertices.Length - 1;
            bool c = false;
            for (int i = 0; i < vertices.Length; j = i++)
                c ^= vertices[i].Y > point.Y ^ vertices[j].Y > point.Y && point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X;
            return c;
        }

        public static Vector3 RightVector(this Vector3 position, Vector3 up)
        {
            position.Normalize();
            up.Normalize();
            return Vector3.Cross(position, up);
        }
    }
}
