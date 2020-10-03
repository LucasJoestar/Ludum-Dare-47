// ===== Ludum Dare 47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ================================================================================= //

using UnityEngine;

namespace LudumDare47.Geometry
{
    public static class GeometryHelper3D
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
        /// Check if two segement intersect
        /// </summary>
        /// <param name="L1_start">start of the first segment</param>
        /// <param name="L1_end">end of the first segment</param>
        /// <param name="L2_start">start of the second segment</param>
        /// <param name="L2_end">end of the second segment</param>
        /// <param name="_intersection">the intersection point between the two ssegments</param>
        /// <returns>return true if segements intersect</returns>
        public static bool IsIntersecting(Vector3 _a, Vector3 _b, Vector3 _c, Vector3 _d, out Vector3 _intersection)
        {
            Vector3 _ab = _b - _a; // I
            float _anglesign = AngleSign(_a, _b, _c);
            Vector3 _cd = _anglesign > 0 ? _d - _c : _c - _d; // J

            Vector3 _pointLeft = _anglesign > 0 ? _c : _d;

            // P -> Intersection point 
            // P = _a + k * _ab = _c + m * _cd
            // A.x + k*_ab.x = _c.x + m *_cd.x
            // A.y + k*_ab.y = _c.y + m *_cd.y
            // A.z + k*_ab.z = _c.z + m *_cd.z

            float _denominator = Vector3.Cross(_ab, _cd).magnitude;

            if (_denominator != 0)
            {
                //  m =    (     -Ix*A.y      +      Ix*Cy     +      Iy*Ax     -      Iy*Cx )
                float _m = ((-_ab.x * _a.y) + (_ab.x * _pointLeft.y) + (_ab.y * _a.x) - (_ab.y * _pointLeft.x)) / _denominator;

                //  k =    (     Jy*Ax     -      Jy*Cx     -      Jx*Ay     +      Jx*Cy )
                float _k = ((_cd.y * _a.x) - (_cd.y * _pointLeft.x) - (_cd.x * _a.y) + (_cd.x * _pointLeft.y)) / _denominator;

                //Debug.Log(_m + " " + _k); 

                if ((_m >= 0 && _m <= 1 && _k >= 0 && _k <= 1))
                {

                    if (Vector3.Distance((_a + _k * _ab), (_pointLeft + _m * _cd)) > .1f)
                    {
                        _intersection = _a;
                        return false;
                    }

                    _intersection = _a + _k * _ab;
                    return true;
                }
            }
            _intersection = _a;
            return false;
        }

        /// <summary>
        /// Check if a point is between two endpoints of a segment
        /// </summary>
        /// <param name="_firstSegmentPoint">First endpoint of the segment</param>
        /// <param name="_secondSegmentPoint">Second endpoint of the segment</param>
        /// <param name="_comparedPoint">Compared point</param>
        /// <returns></returns>
        public static bool PointContainedInSegment(Vector3 _firstSegmentPoint, Vector3 _secondSegmentPoint, Vector3 _comparedPoint)
        {
            float _segmentLength = FlatDistance(_firstSegmentPoint, _secondSegmentPoint);
            float _a = FlatDistance(_firstSegmentPoint, _comparedPoint);
            float _b = FlatDistance(_secondSegmentPoint, _comparedPoint);
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
            //Debug.DrawRay(_start, _ref, Color.green, 10);
            //Debug.DrawRay(_start, _angle, Color.red, 10); 
            float _alpha = Vector2.SignedAngle(_ref, _angle);
            //Debug.Log(_start + " -> " + _point + " = " + _alpha); 
            
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

        #region float
        public static float FlatDistance(Vector3 _pos1, Vector3 _pos2)
        {
            return Mathf.Sqrt(Mathf.Pow((_pos2.x - _pos1.x), 2) + Mathf.Pow((_pos2.y - _pos1.y), 2)); 
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
        public static Vector3 GetNormalPoint(Vector3 _predictedPosition, Vector3 _previousPosition, Vector3 _nextPosition)
        {
            Vector3 _ap = _predictedPosition - _previousPosition;
            Vector3 _ab = (_nextPosition - _previousPosition).normalized;
            Vector3 _ah = _ab * (Vector3.Dot(_ap, _ab));
            Vector3 _normal = (_previousPosition + _ah);
            Vector3 _min = Vector3.Min(_previousPosition, _nextPosition);
            Vector3 _max = Vector3.Max(_previousPosition, _nextPosition);
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