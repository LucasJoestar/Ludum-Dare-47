using UnityEditor;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredPropertyDrawer : PropertyDrawer
    {
        #region Methods
        /***************************
         *******   METHODS   *******
         **************************/

        private static readonly GUIContent getPropertyGUI = new GUIContent("Get Reference", "Get reference of the property.");

        private bool isMenuInitialized = false;
        private GenericMenu menu = null;

        // Specify how tall the GUI for this decorator is in pixels
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUIUtilityEnhanced.GetRequiredPropertyHeight(_property);
        }

        // Make your own IMGUI based GUI for the property
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            Event _current = Event.current;
            if ((_current.type == EventType.ContextClick) && _position.Contains(_current.mousePosition))
            {
                if (!isMenuInitialized)
                {
                    menu = new GenericMenu();
                    menu.AddItem(getPropertyGUI, false, GetReference, _property);
                }
                menu.ShowAsContext();
            }

            EditorGUIEnhanced.RequiredProperty(_position, _property, _label);
        }

        private void GetReference(object _object)
        {
            SerializedProperty _property = (SerializedProperty)_object;
            System.Type _type = EditorGUIUtilityEnhanced.FindSerializedObjectField(_property.serializedObject, _property.name).FieldType;

            _property.objectReferenceValue = ((Component)_property.serializedObject.targetObject).GetComponent(_type);
            _property.serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
