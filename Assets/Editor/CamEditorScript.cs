using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkCamera), true)]
public class CamEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        //  DrawDefaultInspector();
        base.OnInspectorGUI();

        NetworkCamera Target = (NetworkCamera)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Change Angle"))
        {
            //Target._GenratePathLines();
        }
        //GUILayout.Space(10);
    }
}
