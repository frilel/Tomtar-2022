#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConstrainedPathObject))]
public class ConstrainedPathEditorView : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ConstrainedPathObject script = (ConstrainedPathObject)target;
        if (GUILayout.Button("Reset Path"))
        {
            script.path.position = script.obj.position;
            script.ResetLineRenderer();
        }
    }
}
#endif
