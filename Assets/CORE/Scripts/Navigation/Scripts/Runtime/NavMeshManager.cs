// ===== Ludum Dare 47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ================================================================================= //

using System; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using LudumDare47.Geometry;

namespace LudumDare47.Navigation
{
    public class NavMeshManager : MonoBehaviour
    {
        #region Fields and properties
        public static NavMeshManager Instance = null;

        private static readonly string addressablesAssetsLabel = "Navigation";
        public List<Triangle> Triangles { get; private set; }
        public NavData NavigationDatas { get; private set; }

        [SerializeField] private string dataName = string.Empty;
        #endregion

        #region Methods
        /// <summary>
        /// Load the addressables assets of the Navigations Datas
        /// /// </summary>
        private void InitManager()
        {
            Addressables.LoadAssetsAsync<TextAsset>(addressablesAssetsLabel, null).Completed += OnNavigationsDatasLoaded; 
        }

        /// <summary>
        /// Called when the addressables assets of the Navigations datas are loaded
        /// </summary>
        /// <param name="_loadedAssets"></param>
        private void OnNavigationsDatasLoaded(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<TextAsset>> _loadedAssets)
        {
            for (int i = 0; i < _loadedAssets.Result.Count; i++)
            {
                if(_loadedAssets.Result[i].name == dataName)
                {
                    NavigationDatas = JsonUtility.FromJson<NavData>(_loadedAssets.Result[i].text);
                    GenerateTriangles();
                    return; 
                }
            }
        }

        /// <summary>
        /// Generate Triangles from the navigation Datas 
        /// Then link triangles when they share 2 vertices
        /// </summary>
        private void GenerateTriangles()
        {
            Triangles = new List<Triangle>();
            int _i, _j = 0;
            for (_i = 0; _i < NavigationDatas.Indices.Length; _i += 3)
            {
                Triangles.Add(new Triangle(_j, new int[3] { NavigationDatas.Indices[_i], NavigationDatas.Indices[_i + 1], NavigationDatas.Indices[_i + 2] }));
                _j++;
            }
            for (_i = 0; _i < Triangles.Count; _i++)
            {
                for (_j = 0; _j < Triangles.Count; _j++)
                {
                    if (_i == _j) continue;
                    if (GeometryHelper2D.VerticesInCommon(Triangles[_i], Triangles[_j]) == 2)
                    {
                        Triangles[_i].LinkedTriangles.Add(_j);
                    }
                }
            }
        }

        #region Utils Methods
        /// <summary>
        /// Return if the position is inside of the triangle
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        public bool IsInTriangle(Vector3 _position, Triangle _triangle)
        {

            Barycentric _barycentric = new Barycentric(NavigationDatas.Vertices[_triangle.Vertices[0]],
                                                       NavigationDatas.Vertices[_triangle.Vertices[1]],
                                                       NavigationDatas.Vertices[_triangle.Vertices[2]],
                                                       _position);
            return _barycentric.IsInside;
        }

        /// <summary>
        /// Get the center position of the triangle
        /// This point is the average point of the 3 vertices of the triangle
        /// </summary>
        /// <param name="_triangle"></param>
        /// <returns></returns>
        public Vector3 GetCenterPosition(Triangle _triangle)
        {
            return (NavigationDatas.Vertices[_triangle.Vertices[0]] +
                    NavigationDatas.Vertices[_triangle.Vertices[1]] +
                    NavigationDatas.Vertices[_triangle.Vertices[2]]) / 3;
        }


        #region Triangle
        /// <summary>
        /// Get the triangle where the position is contained 
        /// If triangle can't be found, get the closest triangle
        /// </summary>
        /// <param name="_position">Position</param>
        /// <returns>Triangle where the position is contained</returns>
        public Triangle GetTriangleContainingPosition(Vector3 _position)
        {
            int _closestIndex = 0;
            float _minDist = -1, _currentDist = -1;
            foreach (Triangle triangle in Triangles)
            {
                if (IsInTriangle(_position, triangle))
                {
                    return triangle;
                }
                if (_minDist < 0)
                {
                    _minDist = Vector3.Distance(_position, GetCenterPosition(triangle));
                    _closestIndex = triangle.Index;
                }
                _currentDist = Vector3.Distance(_position, GetCenterPosition(triangle));
                if (_currentDist < _minDist)
                {
                    _currentDist = _minDist;
                    _closestIndex = triangle.Index;
                }
            }
            return Triangles[_closestIndex];
        }
        #endregion
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                InitManager();
            }
            else Destroy(this);
        }
        #endregion

        #endregion

    }

    [Serializable]
    public struct NavData
    {
        #region Field and Properties
        [SerializeField] private Vector2[] vertices;
        [SerializeField] private int[] indices;

        public Vector2[] Vertices { get { return vertices; } }
        public int[] Indices { get { return indices; } }
        #endregion

        #region Constructor
        public NavData(Vector3[] _vertices, int[] _indices)
        {
            List<Vector3> _cleanedVertices = new List<Vector3>();
            int _duplicatedIndex = 0, currentIndex = 0;
            bool _isFirstOne = true; 
            for (int i = 0; i < _vertices.Length; i++)
            {
                _isFirstOne = true; 
                for (int j = 0; j < _cleanedVertices.Count; j++)
                {
                    if(_vertices[i] == _cleanedVertices[j])
                    {
                        _duplicatedIndex = j;
                        _isFirstOne = false; 
                        for (int k = 0; k < _indices.Length; k++)
                        {
                            if (_indices[k] == i)
                                _indices[k] = _duplicatedIndex; 
                        }

                    }
                }
                if (_isFirstOne)
                {
                    _cleanedVertices.Add(_vertices[i]);
                    for (int k = 0; k < _indices.Length; k++)
                    {
                        if (_indices[k] == i)
                            _indices[k] = currentIndex;
                    }
                    currentIndex++; 
                }
            }
            vertices = new Vector2[_cleanedVertices.Count];
            for (int i = 0; i < _cleanedVertices.Count; i++)
            {
                vertices[i] = new Vector2(_cleanedVertices[i].x, _cleanedVertices[i].z);
            }
            //vertices = _cleanedVertices.ToArray();
            indices = _indices;
        }
        #endregion
    }
}