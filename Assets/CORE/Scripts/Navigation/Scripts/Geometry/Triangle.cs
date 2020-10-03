// ===== Ludum Dare 47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ================================================================================= //

using System.Collections.Generic;
using UnityEngine;

namespace LudumDare47.Geometry
{
    public struct Triangle
    {
        #region Fields and properties
        public int Index { get; private set; }
        public int[] Vertices { get; private set; }

        public List<int> LinkedTriangles { get; set; }

        private float heuristicPriority;
        public float HeuristicPriority
        {
            get
            {
                return heuristicPriority;
            }
            set
            {
                heuristicPriority = value;
                HeuristicCostToDestination = heuristicPriority - HeuristicCostFromStart;
            }
        }

        public float HeuristicCostFromStart { get; set; }
        public float HeuristicCostToDestination { get; set; }
        #endregion

        #region Constructor 
        public Triangle(int _index, int[] _vertices)
        {
            Index = _index;
            Vertices = new int[3];
            LinkedTriangles = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                Vertices[i] = _vertices[i];
            }
            heuristicPriority = 0;
            HeuristicCostFromStart = 0;
            HeuristicCostToDestination = 0; 
        }
        #endregion

        #region Operators
        public static bool operator ==(Triangle _a, Triangle _b)
        {
            return _a.Index == _b.Index; 
        }

        public static bool operator !=(Triangle _a, Triangle _b)
        {
            return !(_a == _b); 
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    public struct Barycentric
    {
        public float u;
        public float v;
        public float w;

        /// <summary>
        /// Return if u, v and w are greater or equal than 0 and less or equal than 1
        /// That means the point is inside of the triangle
        /// </summary>
        public bool IsInside
        {
            get
            {
                return (u >= 0.0f) && (u <= 1.0f) && (v >= 0.0f) && (v <= 1.0f) && (w >= 0.0f); //(w <= 1.0f)
            }
        }

        /// <summary>
        /// Calculate 3 value u, v, w as:
        /// aP = u*aV1 + v*aV2 + w*aV3
        /// if u, v and w are greater than 0, that means that the point is in the triangle aV1 aV2 aV3.
        /// </summary>
        /// <param name="aV1">First point of the triangle</param>
        /// <param name="aV2">Second point of the triangle</param>
        /// <param name="aV3">Third point of the triangle</param>
        /// <param name="aP">Point to get the Barycentric coordinates</param>
        public Barycentric(Vector3 aV1, Vector3 aV2, Vector3 aV3, Vector3 aP)
        {
            Vector3 a = aV2 - aV3;
            Vector3 b = aV1 - aV3;
            Vector3 c = aP - aV3;
            float aLen = a.x * a.x + a.y * a.y + a.z * a.z;
            float bLen = b.x * b.x + b.y * b.y + b.z * b.z;
            float ab = a.x * b.x + a.y * b.y + a.z * b.z;
            float ac = a.x * c.x + a.y * c.y + a.z * c.z;
            float bc = b.x * c.x + b.y * c.y + b.z * c.z;
            float d = aLen * bLen - ab * ab;
            u = (aLen * bc - ab * ac) / d;
            v = (bLen * ac - ab * bc) / d;
            w = 1.0f - u - v;
        }
    }
}