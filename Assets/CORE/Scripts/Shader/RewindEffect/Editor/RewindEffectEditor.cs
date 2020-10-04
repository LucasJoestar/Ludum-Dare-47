using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RewindEffect))]
    public class RewindEffectEditor : Editor
    {
        SerializedProperty scanLineJitter;
        SerializedProperty verticalJump;
        SerializedProperty colorDrift;

        void OnEnable()
        {
            scanLineJitter = serializedObject.FindProperty("scanLineJitter");
            verticalJump = serializedObject.FindProperty("verticalJump");
            colorDrift = serializedObject.FindProperty("colorDrift");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(scanLineJitter);
            EditorGUILayout.PropertyField(verticalJump);
            EditorGUILayout.PropertyField(colorDrift);

            serializedObject.ApplyModifiedProperties();
        }
    }