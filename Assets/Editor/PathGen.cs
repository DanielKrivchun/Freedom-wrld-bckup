using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeneratePathExample), true)]
public class PathGen : Editor
{
    public override void OnInspectorGUI()
    {
        //  DrawDefaultInspector();
        base.OnInspectorGUI();

        GeneratePathExample Target = (GeneratePathExample)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Genrate Path"))
        {
            Target._update();
        }

        GUILayout.Space(10);

    }
}
