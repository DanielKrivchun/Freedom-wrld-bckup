using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathPointManager), true)]
public class PathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //  DrawDefaultInspector();
        base.OnInspectorGUI();

        PathPointManager Target = (PathPointManager)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Genrate Path"))
        {
            Target._GenratePathLines();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Delete Path"))
        {
            Target._Reset();
        }

        if (GUILayout.Button("Genrate Names"))
        {
            RaceManager race = FindObjectOfType<RaceManager>();

            race.namesJson._SetMyData();
        }

    }
}
