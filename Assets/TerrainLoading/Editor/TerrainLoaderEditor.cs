using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainLoader_Full))]
public class TerrainLoaderEditor : Editor {

    private Dictionary<string, string> availableTerrains;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if(availableTerrains == null)
        {
            availableTerrains = new Dictionary<string, string>();
            availableTerrains["TerrainName"] = @"C:\DB\Unity\TerrainName";
        }

        TerrainLoader targetTerrainLoader = (TerrainLoader)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Available Terrains:");

        foreach (var terrain in availableTerrains)
        {
            if (GUILayout.Button(terrain.Key))
            {
                serializedObject.FindProperty("TerrainBundlePath").stringValue = terrain.Value;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

}
