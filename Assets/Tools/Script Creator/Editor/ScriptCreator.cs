// ===== The Dreadful Show - https://github.com/LucasJoestar/The-Dreadful-Show ===== //
//
// Notes :
//
// ================================================================================= //

using System.IO;
using UnityEngine;
using UnityEditor;

namespace Tools
{
    public static class ScriptCreator
    {
        #region Content
        private static readonly string templatePath = "/Tools/Script Creator/Editor/Templates/";

        private static readonly string monoBehaviour = "MonoBehaviour.txt";
        private static readonly string stateMachineBehaviour = "StateMachineBehaviour.txt";

        [MenuItem("Assets/Create/Script/MonoBehaviour", false, 50)]
        public static void CreateMonoBehaviour() => ScriptCreatorWindow.GetWindow(monoBehaviour);

        [MenuItem("Assets/Create/Script/StateMachineBehaviour", false, 50)]
        public static void CreateStateMachineBehaviour() => ScriptCreatorWindow.GetWindow(stateMachineBehaviour);

        // -----------------------

        public static void CreateScript(string _template, string _name)
        {
            string _path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            string _fullPath = _path.Replace("Assets", Application.dataPath);
            if (File.Exists(_fullPath))
                _fullPath = Path.GetDirectoryName(_fullPath);

            _fullPath += "/";
            if (_template == stateMachineBehaviour)
                _fullPath += "ANI_";

            _fullPath += _name + ".cs";
            if (File.Exists(_fullPath))
            {
                EditorUtility.DisplayDialog("Error", "A script with this name already exist.", "Okay");
                return;
            }

            string[] _lines = File.ReadAllLines(Application.dataPath + templatePath + _template);
            string _file = string.Empty;
            for (int _i = 0; _i < _lines.Length; _i++)
            {
                _lines[_i] = _lines[_i].Replace("#SCRIPTNAME#", _name);
                _file += _lines[_i] + "\n";
            }

            File.WriteAllText(_fullPath, _file);
            AssetDatabase.Refresh();
        }
        #endregion
    }

    public class ScriptCreatorWindow : EditorWindow
    {
        #region Content
        private string template = string.Empty;

        public static void GetWindow(string _template)
        {
            ScriptCreatorWindow _launcher = (ScriptCreatorWindow)GetWindow(typeof(ScriptCreatorWindow));
            _launcher.template = _template;

            _launcher.maxSize = new Vector2(250, 50);
            _launcher.minSize = new Vector2(250, 50);
            _launcher.ShowUtility();
        }

        // -----------------------

        private readonly GUIContent nameGUI = new GUIContent("Name:", "Name of the script.");
        private readonly GUIContent createGUI = new GUIContent("Create", "Create a script with this name.");

        private string scriptName = "NewScript";

        private void OnGUI()
        {
            scriptName = EditorGUILayout.TextField(nameGUI, scriptName);

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(createGUI))
            {
                ScriptCreator.CreateScript(template, scriptName);
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }
}
