// ===== Ludum Dare 47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ================================================================================= //

using System.Collections.Generic;
using UnityEngine;
using LudumDare47.Geometry;

namespace LudumDare47.Navigation
{
    public static class PathCalculator
    {
        #region Methods
        #region bool
        /// <summary>
        /// Calculate path from an origin to a destination 
        /// Set the path when it can be calculated 
        /// </summary>
        /// <param name="_origin">The Origin of the path </param>
        /// <param name="_destination">The Destination of the path</param>
        /// <param name="_path">The path to set</param>
        /// <returns>Return if the path can be calculated</returns>
        public static bool CalculatePath(Vector2 _origin, Vector2 _destination, out Vector2[] _path, List<Triangle> _trianglesDatas)
        {
            // GET TRIANGLES
            // Get the origin triangle and the destination triangle
            Triangle _originTriangle = NavMeshManager.Instance.GetTriangleContainingPosition(_origin);
            Triangle _targetedTriangle = NavMeshManager.Instance.GetTriangleContainingPosition(_destination);

            //Open list that contains all heuristically calculated triangles 
            List<Triangle> _openList = new List<Triangle>();
            //returned path
            Dictionary<int, int> _cameFrom = new Dictionary<int, int>();

            Triangle _currentTriangle;

            /* ASTAR: Algorithm*/
            // Add the origin point to the open and close List
            // Set its heuristic cost and its selection state
            _openList.Add(_originTriangle);
            _originTriangle.HeuristicCostFromStart = 0;
            float _cost = 0;
            while (_openList.Count > 0)
            {
                //Get the point with the best heuristic cost
                _currentTriangle = GetBestTriangle(_openList);
                //If this point is in the targeted triangle, 
                if (_currentTriangle == _targetedTriangle)
                {
                    //Build the path
                    _path = BuildPath(_cameFrom, _originTriangle, _targetedTriangle, _origin, _destination, _trianglesDatas);

                    return true;
                }
                //Get all linked points from the current point
                for (int i = 0; i < _currentTriangle.LinkedTriangles.Count; i++)
                {
                    Triangle _linkedTriangle = _trianglesDatas[_currentTriangle.LinkedTriangles[i]];

                    // If the linked points is not selected yet
                    if(!_cameFrom.ContainsKey(_linkedTriangle.Index))
                    {
                        // Calculate the heuristic cost from start of the linked point
                        _cost = _currentTriangle.HeuristicCostFromStart + HeuristicCost(_currentTriangle, _linkedTriangle);
                        if (!_openList.Contains(_linkedTriangle) || _cost < _linkedTriangle.HeuristicCostFromStart)
                        {
                            // Set the heuristic cost from start for the linked point
                            _linkedTriangle.HeuristicCostFromStart = _cost;
                            //Its heuristic cost is equal to its cost from start plus the heuristic cost between the point and the destination
                            _linkedTriangle.HeuristicPriority = HeuristicCost(_linkedTriangle, _targetedTriangle) + _cost;
                            //Set the point selected and add it to the open and closed list
                            _openList.Add(_linkedTriangle);
                            _cameFrom.Add(_linkedTriangle.Index, _currentTriangle.Index);
                        }
                    }
                }
            }
            _path = new Vector2[] { };
            return false;
        }
        #endregion

        #region float 
        /// <summary>
        /// Return the heuristic cost between 2 triangles
        /// Heuristic cost is the distance between 2 points
        /// => Can add a multiplier to change the cost of the movement depending on the point 
        /// </summary>
        /// <param name="_a">First Triangle</param>
        /// <param name="_b">Second Triangle</param>
        /// <returns>Heuristic Cost between 2 points</returns>
        static float HeuristicCost(Triangle _a, Triangle _b)
        {
            return Vector2.Distance(NavMeshManager.Instance.GetCenterPosition(_a), NavMeshManager.Instance.GetCenterPosition(_b));
        }
        #endregion

        #region Triangle
        /// <summary>
        /// Get the triangle with the best heuristic cost from a list 
        /// Remove this point from the list and return it
        /// </summary>
        /// <param name="_triangles">list where the points are</param>
        /// <returns>point with the best heuristic cost</returns>
        static Triangle GetBestTriangle(List<Triangle> _triangles)
        {
            int _bestIndex = 0;
            for (int i = 0; i < _triangles.Count; i++)
            {
                if (_triangles[i].HeuristicPriority < _triangles[_bestIndex].HeuristicPriority)
                {
                    _bestIndex = i;
                }
            }

            Triangle _bestNavTriangle = _triangles[_bestIndex];
            _triangles.RemoveAt(_bestIndex);
            return _bestNavTriangle;
        }
        #endregion

        #region Vector2[] 
        /// <summary>
        /// Build a path using Astar resources
        /// Get the last point and get all its parent to build the path
        /// </summary>
        /// <param name="_pathToBuild">Astar resources</param>
        static Vector2[] BuildPath(Dictionary<int, int> _pathToBuild, Triangle _firstTriangle, Triangle _lastTriangle, Vector2 _origin, Vector2 _destination, List<Triangle> _trianglesDatas)
        {
            
            if (_pathToBuild.Count <= 1)
            {
                List<Vector2> _pathPoints = new List<Vector2>();
                _pathPoints.Add(_origin);
                _pathPoints.Add(_destination);
                return _pathPoints.ToArray();
            }

            #region BuildingAbsolutePath
            // Building absolute path -> Link all triangle's CenterPosition together
            // Adding _origin and destination to the path
            Triangle _currentTriangle = _trianglesDatas[_pathToBuild[_lastTriangle.Index]];
            List<Triangle> _absoluteTrianglePath = new List<Triangle>();
            while (_currentTriangle != _firstTriangle)
            {
                _absoluteTrianglePath.Add(_currentTriangle);
                _currentTriangle = _trianglesDatas[_pathToBuild[_currentTriangle.Index]];
            }
            _absoluteTrianglePath.Add(_currentTriangle);
            //Reverse the path to start at the origin 
            _absoluteTrianglePath.Reverse();
            #endregion

            //Create the simplifiedPath
            List<Vector2> _simplifiedPath = new List<Vector2>() { _origin };

            if (_absoluteTrianglePath.Count <= 1)
            {
                _simplifiedPath.Add(_destination);
                return _simplifiedPath.ToArray(); 
            }

            //Simplify the path with Funnel Algorithm

            //Create both portals vertices arrays
            Vector2[] _leftVertices = new Vector2[_absoluteTrianglePath.Count - 1];
            Vector2[] _rightVertices = new Vector2[_absoluteTrianglePath.Count - 1];

            //Create the apex
            Vector2 _apex = _origin;

            //Initialize portal vertices
            Vector2 _startLinePoint = Vector2.zero;
            Vector2 _endLinePoint = Vector2.zero;
            Vector2 _vertex1 = Vector2.zero;
            Vector2 _vertex2 = Vector2.zero;


            #region Initialise Portal Vertices
            
            //Initialize portal vertices between each triangles
            for (int i = 1; i < _absoluteTrianglePath.Count - 1; i++)
            {
                _currentTriangle = _absoluteTrianglePath[i];
                _startLinePoint = NavMeshManager.Instance.GetCenterPosition(_currentTriangle);
                _endLinePoint = NavMeshManager.Instance.GetCenterPosition(_absoluteTrianglePath[i + 1]);
                for (int j = 0; j < _currentTriangle.Vertices.Length; j++)
                {
                    int k = j + 1 >= _currentTriangle.Vertices.Length ? 0 : j + 1;
                    _vertex1 = NavMeshManager.Instance.NavigationDatas.Vertices[_currentTriangle.Vertices[j]];
                    _vertex2 = NavMeshManager.Instance.NavigationDatas.Vertices[_currentTriangle.Vertices[k]];

                    if (GeometryHelper3D.IsIntersecting(_startLinePoint, _endLinePoint, _vertex1, _vertex2))
                    {
                        //Debug.Log(_startLinePoint + "///" + _endLinePoint + " intersect with " + _vertex1 + "///" + _vertex2); 
                        if (GeometryHelper3D.AngleSign(_startLinePoint, _endLinePoint, _vertex1) > 0)
                        {
                            _leftVertices[i] = _vertex2;
                            _rightVertices[i] = _vertex1;
                        }
                        else
                        {
                            _leftVertices[i] = _vertex1;
                            _rightVertices[i] = _vertex2;
                        }
                        break;

                    }
                }
            }
            
            //Initialize start portal vertices
            _startLinePoint = _origin;
            _endLinePoint = NavMeshManager.Instance.GetCenterPosition(_absoluteTrianglePath[1]);
            _currentTriangle = _absoluteTrianglePath[0];

            for (int j = 0; j < _currentTriangle.Vertices.Length; j++)
            {
                int k = j + 1 >= _currentTriangle.Vertices.Length ? 0 : j + 1;
                _vertex1 = NavMeshManager.Instance.NavigationDatas.Vertices[_currentTriangle.Vertices[j]];
                _vertex2 = NavMeshManager.Instance.NavigationDatas.Vertices[_currentTriangle.Vertices[k]];
                if (GeometryHelper3D.IsIntersecting(_startLinePoint, _endLinePoint, _vertex1, _vertex2))
                {
                    if (GeometryHelper3D.AngleSign(_startLinePoint, _endLinePoint, _vertex1) > 0)
                    {
                        _leftVertices[0] = _vertex2;
                        _rightVertices[0] = _vertex1;
                    }
                    else
                    {
                        _leftVertices[0] = _vertex1;
                        _rightVertices[0] = _vertex2;
                    }
                    break;
                }
            }

            // Initialise end portal vertices -> Close the funnel

            _leftVertices[_leftVertices.Length - 1] = _destination;
            _rightVertices[_rightVertices.Length - 1] = _destination;
            #endregion

            //Step through the channel
            Vector2 _currentLeftVertex;
            Vector2 _nextLeftVertex;
            Vector2 _currentRightVertex;
            Vector2 _nextRightVertex;

            //Set left and right indexes
            int _leftIndex = 0;
            int _rightIndex = 0;


            for (int i = 1; i < _absoluteTrianglePath.Count - 1; i++)
            {
                _currentLeftVertex = _leftVertices[_leftIndex];
                _nextLeftVertex = _leftVertices[i];

                _currentRightVertex = _rightVertices[_rightIndex];
                _nextRightVertex = _rightVertices[i];

                //If the new left vertex is different process
                if (_nextLeftVertex != _currentLeftVertex && i > _leftIndex)
                {
                    //If the next point does not widden funnel, update 
                    if (GeometryHelper3D.AngleSign(_apex, _currentLeftVertex, _nextLeftVertex) >= 0)
                    {
                        //if next side cross the other side, place new apex
                        if (GeometryHelper3D.AngleSign(_apex, _currentRightVertex, _nextLeftVertex) > 0)
                        {
                            // Set the new Apex
                            _apex = _currentRightVertex;
                            _simplifiedPath.Add(_apex);

                            //Set i to the apex index to be at the good index on the next loop 
                            i = _rightIndex;
                            // Find new right vertex.
                            for (int j = _rightIndex; j < _rightVertices.Length; j++)
                            {
                                if (_rightVertices[j] != _apex)
                                {
                                    _rightIndex = j;
                                    break;
                                }
                            }
                            _leftIndex = i;
                            i--;
                            continue;

                        }
                        // else skip to the next vertex
                        else
                        {
                            _leftIndex = i;
                        }
                    }
                    //else skip
                }
                //else skip


                // If the right vertex is different process
                if (_nextRightVertex != _currentRightVertex && i > _rightIndex)
                {
                    //If the next point does not widden funnel, update 
                    if (GeometryHelper3D.AngleSign(_apex, _currentRightVertex, _nextRightVertex) <= 0)
                    {
                        //if next side cross the other side, place new apex
                        if (GeometryHelper3D.AngleSign(_apex, _currentLeftVertex, _nextRightVertex) < 0)
                        {
                            //Set the new Apex
                            _apex = _currentLeftVertex;
                            _simplifiedPath.Add(_apex);

                            //Set i to the apex index to be at the good index on the next loop 
                            i = _leftIndex;
                            // Find next Left Index
                            for (int j = _leftIndex; j < _leftVertices.Length; j++)
                            {
                                if (_leftVertices[j] != _apex)
                                {
                                    _leftIndex = j;
                                    break;
                                }
                            }
                            _rightIndex = i;
                            i--;
                            continue;


                        }
                        //else skip to the next vertex
                        else
                        {
                            _rightIndex = i;
                        }
                    }
                    //else skip
                }
                //else skip

            }

            _simplifiedPath.Add(_destination);

            //Set the simplifiedPath
            return _simplifiedPath.ToArray();
        }
        #endregion

        #endregion
    }
}