// ===== Ludum Dare 47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ================================================================================= //

using UnityEngine;

namespace LudumDare47.Geometry
{
    public static class GeometryHelper2D
    {
        #region Methods

        #region bool 
        /// <summary>
        /// Check if two segement intersect
        /// </summary>
        /// <param name="L1_start">start of the first segment</param>
        /// <param name="L1_end">end of the first segment</param>
        /// <param name="L2_start">start of the second segment</param>
        /// <param name="L2_end">end of the second segment</param>
        /// <returns>return true if segements intersect</returns>
        public static bool IsIntersecting(Vector2 _a, Vector2 _b, Vector2 _c, Vector2 _d)
        {
            Vector2 _ab = _b - _a; // I
            float _anglesign = AngleSign(_a, _b, _c);
            Vector2 _cd = _anglesign > 0 ? _d - _c : _c - _d; // J 

            Vector2 _pointLeft = _anglesign > 0 ? _c : _d;
            float _denominator = Vector3.Cross(_ab, _cd).magnitude;


            if (_denominator != 0)
            {
            //  m =    (     -Ix*A.y      +      Ix*Cy     +      Iy*Ax     -      Iy*Cx )
            float _m = ((-_ab.x * _a.y) + (_ab.x * _pointLeft.y) + (_ab.y * _a.x) - (_ab.y * _pointLeft.x)) / _denominator;

            //  k =    (     Jy*Ax     -      Jy*Cx     -      Jx*Ay     +      Jx*Cy )
            float _k = ((_cd.y * _a.x) - (_cd.y * _pointLeft.x) - (_cd.x * _a.y) + (_cd.x * _pointLeft.y)) / _denominator;


                if ((_m >= 0 && _m <= 1 && _k >= 0 && _k <= 1))
                {

                    if (Vector3.Distance((_a + _k * _ab), (_pointLeft + _m * _cd)) > .1f)
                    {
                        return false;
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if a point is between two endpoints of a segment
        /// </summary>
        /// <param name="_firstSegmentPoint">First endpoint of the segment</param>
        /// <param name="_secondSegmentPoint">Second endpoint of the segment</param>
        /// <param name="_comparedPoint">Compared point</param>
        /// <returns></returns>
        public static bool PointContainedInSegment(Vector2 _firstSegmentPoint, Vector2 _secondSegmentPoint, Vector2 _comparedPoint)
        {
            float _segmentLength = Vector2.Distance(_firstSegmentPoint, _secondSegmentPoint);
           
            float _a = Vector2.Distance(_firstSegmentPoint, _comparedPoint);
            float _b = Vector2.Distance(_secondSegmentPoint, _comparedPoint);
            return _segmentLength > _a && _segmentLength > _b;
        }
        #endregion

        #region int
        /// <summary>
        /// Return the sign of the angle between [StartPoint EndPoint] and [StartPoint Point]
        /// </summary>
        /// <param name="_start">Start point</param>
        /// <param name="_end">End Point</param>
        /// <param name="_point">Point</param>
        /// <param name="debug"></param>
        /// <returns>Sign of the angle between the three points (if 0 or 180 or -180, return 0)</returns>
        public static int AngleSign(Vector2 _start, Vector2 _end, Vector2 _point)
        {
            Vector2 _ref = _end - _start;
            Vector2 _angle = _point - _start;
            float _alpha = Vector2.SignedAngle(_ref, _angle);           
            if (_alpha == 0 || _alpha == 180 || _alpha == -180) return 0;
            if (_alpha > 0) return 1;
            return -1;
        }

        /// <summary>
        /// Compare triangles
        /// And return the number of vertices in common
        /// </summary>
        /// <param name="_triangle1">First triangle to compare</param>
        /// <param name="_triangle2">Second triangle to compare</param>
        /// <returns>return the number of vertices in common</returns>
        public static int VerticesInCommon(Triangle _triangle1, Triangle _triangle2)
        {
            int _verticesCount = 0;
            for (int i = 0; i < _triangle1.Vertices.Length; i++)
            {
                for (int j = 0; j < _triangle2.Vertices.Length; j++)
                {
                    if (_triangle1.Vertices[i] == _triangle2.Vertices[j])
                        _verticesCount++;
                }
            }
            return _verticesCount;
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Get the transposed point of the predicted position on a segement between the previous and the next position
        /// Check if the targeted point is on the segment between the previous and the next points
        /// If it doesn't the normal point become the _nextPosition
        /// </summary>
        /// <param name="_predictedPosition">Predicted Position</param>
        /// <param name="_previousPosition">Previous Position</param>
        /// <param name="_nextPosition">Next Position</param>
        /// <returns></returns>
        public static Vector2 GetNormalPoint(Vector2 _predictedPosition, Vector2 _previousPosition, Vector2 _nextPosition)
        {
            Vector2 _ap = _predictedPosition - _previousPosition;
            Vector2 _ab = (_nextPosition - _previousPosition).normalized;
            Vector2 _ah = _ab * (Vector2.Dot(_ap, _ab));
            Vector2 _normal = (_previousPosition + _ah);
            Vector2 _min = Vector2.Min(_previousPosition, _nextPosition);
            Vector2 _max = Vector2.Max(_previousPosition, _nextPosition);
            if (_normal.x < _min.x || _normal.y < _min.y || _normal.x > _max.x || _normal.y > _max.y)
            {
                return _nextPosition;
            }
            return _normal;
        }
        #endregion

        #endregion
    }
}