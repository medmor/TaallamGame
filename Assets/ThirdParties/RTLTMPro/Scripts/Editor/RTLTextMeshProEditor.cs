using UnityEditor;
using UnityEngine;

namespace RTLTMPro
{
    [CustomEditor(typeof(RTLTextMeshPro)), CanEditMultipleObjects]
    public class RTLTextMeshProEditor : Editor
    {
        private SerializedProperty originalTextProp;
        private SerializedProperty preserveNumbersProp;
        private SerializedProperty farsiProp;
        private SerializedProperty fixTagsProp;
        private SerializedProperty forceFixProp;

        private bool foldout;
        private RTLTextMeshPro tmpro;
    private bool _haveChanges;

        protected void OnEnable()
        {
            foldout = true;
            preserveNumbersProp = serializedObject.FindProperty("preserveNumbers");
            farsiProp = serializedObject.FindProperty("farsi");
            fixTagsProp = serializedObject.FindProperty("fixTags");
            forceFixProp = serializedObject.FindProperty("forceFix");
            originalTextProp = serializedObject.FindProperty("originalText");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            tmpro = (RTLTextMeshPro)target;

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(originalTextProp, new GUIContent("RTL Text Input Box"));

            ListenForZeroWidthNoJoiner();

            if (EditorGUI.EndChangeCheck())
                OnChanged();

            serializedObject.ApplyModifiedProperties();

            // Draw the default inspector for other TMP/RTL fields
            DrawDefaultInspector();

            // Use a version-agnostic foldout style to avoid TMP editor API mismatches
            foldout = EditorGUILayout.Foldout(foldout, "RTL Settings", EditorStyles.foldout);
            if (foldout)
            {
                DrawOptions();

                if (GUILayout.Button("Re-Fix"))
                    _haveChanges = true;

                if (EditorGUI.EndChangeCheck())
                    _haveChanges = true;
            }

            if (_haveChanges)
                OnChanged();

            serializedObject.ApplyModifiedProperties();
        }

        protected void OnChanged()
        {
            tmpro.UpdateText();
            _haveChanges = false;
            tmpro.havePropertiesChanged = true;
            tmpro.SetVerticesDirty();
            tmpro.SetLayoutDirty();
            EditorUtility.SetDirty(tmpro);
        }

        protected virtual void DrawOptions()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            farsiProp.boolValue = GUILayout.Toggle(farsiProp.boolValue, new GUIContent("Farsi"));
            forceFixProp.boolValue = GUILayout.Toggle(forceFixProp.boolValue, new GUIContent("Force Fix"));
            preserveNumbersProp.boolValue = GUILayout.Toggle(preserveNumbersProp.boolValue, new GUIContent("Preserve Numbers"));

            if (tmpro.richText)
                fixTagsProp.boolValue = GUILayout.Toggle(fixTagsProp.boolValue, new GUIContent("FixTags"));

            EditorGUILayout.EndHorizontal();
        }

        protected virtual void ListenForZeroWidthNoJoiner()
        {
            var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

            bool shortcutPressed = (Event.current.modifiers & EventModifiers.Control) != 0 &&
                                   (Event.current.modifiers & EventModifiers.Shift) != 0 &&
                                   Event.current.type == EventType.KeyUp &&
                                   Event.current.keyCode == KeyCode.Alpha2;

            if (!shortcutPressed) return;

            // U+200C Zero Width Non-Joiner
            originalTextProp.stringValue = originalTextProp.stringValue.Insert(editor.cursorIndex, "\u200C");
            editor.selectIndex++;
            editor.cursorIndex++;
            Event.current.Use();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }
}