using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PetCareData), true)]

public class PetCareDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //  DrawDefaultInspector();
        base.OnInspectorGUI();

        PetCareData Target = (PetCareData)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Get Json"))
        {
            Target._GetMyJsonData();
        }

        if (GUILayout.Button("Set Json"))
        {
            Target._SetData();
        }
    }
}
