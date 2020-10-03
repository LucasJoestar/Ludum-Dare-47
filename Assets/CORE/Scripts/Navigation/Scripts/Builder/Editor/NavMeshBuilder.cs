// ===== Ludum Dare 47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ================================================================================= //

using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace LudumDare47.Navigation.NavEditor
{
    public class NavMeshBuilder : EditorWindow
    {
        #region Fields/Properties
        public string SavingDirectory { get { return Application.dataPath + "/CORE/Datas/NavigationDatas"; } }
        private Material material;
        private NavData navigationDatas;
        #endregion

        #region Methods
        [MenuItem("Tools/Custom Nav Mesh")]
        public static void Init()
        {
            NavMeshBuilder _instance = (NavMeshBuilder)GetWindow(typeof(NavMeshBuilder));
            _instance.Show();
        }

        #region void
        /// <summary>
        /// Get all nav mesh surfaces
        /// Update all navmesh datas for each NavMeshSurfaces
        /// Calculate triangulation 
        /// Add each triangle in the triangulation to a list of triangles
        /// Link Triangles with its neighbors
        /// </summary>
        public void GetNavPointsFromNavSurfaces()
        {
            List<NavMeshSurface> _surfaces = NavMeshSurface.activeSurfaces;
            foreach (NavMeshSurface surface in _surfaces)
            {
                surface.BuildNavMesh();
            }
            NavMeshTriangulation _tr = NavMesh.CalculateTriangulation();
            SaveDatas(_tr);
        }

        /// <summary>
        /// Get the nav points and save them in binary in a selected directory
        /// 
        /// Sauvegarder les datas dans le dossier resources
        /// Au démarrage du jeu, si les datas ne sont pas dans le data path, on va les chercher dans le dosser resources pour les mettre dans le pdpath
        /// </summary>
        public void SaveDatas(NavMeshTriangulation _tr)
        {
            if (!Directory.Exists(SavingDirectory)) Directory.CreateDirectory(SavingDirectory);
            navigationDatas = new NavData(_tr.vertices, _tr.indices);
            string _jsonData = JsonUtility.ToJson(navigationDatas);
            File.WriteAllText(Path.Combine(SavingDirectory, SceneManager.GetActiveScene().name + ".json"), _jsonData);
            Process.Start(SavingDirectory);
        }

        /// <summary>
        /// Load the datas to get the triangles
        /// </summary>
        private void LoadDatas()
        {
            if (!Directory.Exists(SavingDirectory) || !File.Exists(Path.Combine(SavingDirectory, SceneManager.GetActiveScene().name + ".json"))) return;
            string _jsonData = File.ReadAllText(Path.Combine(SavingDirectory, SceneManager.GetActiveScene().name + ".json"));
            navigationDatas = JsonUtility.FromJson<NavData>(_jsonData);
        }
        #endregion

        #endregion

        #region UnityMethods
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("CUSTOM NAV MESH BUILDER", MessageType.None, true);
            EditorGUILayout.Space();

            if (NavMeshSurface.activeSurfaces.Count == 0)
            {
                EditorGUILayout.HelpBox("You must add nav mesh surfaces to build the datas", MessageType.Error, true);
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Build Navigation Datas")))
                {
                    GetNavPointsFromNavSurfaces();
                }
                if (GUILayout.Button(new GUIContent("Clear Nav Datas")))
                {
                    if (!Directory.Exists(SavingDirectory)) Directory.CreateDirectory(SavingDirectory);
                    if (File.Exists(Path.Combine(SavingDirectory, SceneManager.GetActiveScene().name + ".json")))
                        File.Delete(Path.Combine(SavingDirectory, SceneManager.GetActiveScene().name + ".json"));
                }
            }

            if (GUILayout.Button(new GUIContent("Open Directory")))
            {
                if (!Directory.Exists(SavingDirectory)) Directory.CreateDirectory(SavingDirectory);
                Process.Start(SavingDirectory);
            }

        }

        void OnEnable()
        {
            //Create the material necessary to draw the navMesh
            material = new Material(Shader.Find("Specular"));
            LoadDatas();
            //Implement the event to draw the navmesh on the scene
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (!material || navigationDatas.Indices.Length == 0 || navigationDatas.Vertices.Length == 0)
            {
                return;
            }
           
            for (int i = 0; i < navigationDatas.Vertices.Length; i++)
            {
                Handles.Label(navigationDatas.Vertices[i], i.ToString());
            }
            
            for (int i = 0; i < navigationDatas.Indices.Length; i+=3)
            {
                Handles.DrawLine(navigationDatas.Vertices[navigationDatas.Indices[i]], navigationDatas.Vertices[navigationDatas.Indices[i+1]]);
                Handles.DrawLine(navigationDatas.Vertices[navigationDatas.Indices[i+1]], navigationDatas.Vertices[navigationDatas.Indices[i + 2]]);
                Handles.DrawLine(navigationDatas.Vertices[navigationDatas.Indices[i+2]], navigationDatas.Vertices[navigationDatas.Indices[i]]);
            }
            
            Handles.BeginGUI();

            Handles.EndGUI();
        }

        
        #endregion
    }
}