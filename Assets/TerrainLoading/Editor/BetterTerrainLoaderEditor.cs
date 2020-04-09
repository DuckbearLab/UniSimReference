using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/* ===================================================================================
 * BetterTerrainLoaderEditor -
 * DESCRIPTION -
 * =================================================================================== */

[CustomEditor(typeof(TerrainLoader_Dynamic))]
public class BetterTerrainLoaderEditor : Editor
{
    /*SerializedProperty p_radiusMode;
    SerializedProperty p_useLocalRotation;
    SerializedProperty p_dynamicLoadingRadius;*/

    private Dictionary<string, DynamicTerrainBundles> availableTerrains;

    private struct DynamicTerrainBundles
    {
        public string TerrainBundlePath;
        public string OriginalsBundlePath;
        public string ImrovedTerrainBundlePath;
        public float MapOffsetX;
        public float MapOffsetZ;
    }

    void OnEnable()
    {
        /*p_radiusMode = serializedObject.FindProperty("RadiusMode");
        p_useLocalRotation = serializedObject.FindProperty("UseLocalRotation");
        p_dynamicLoadingRadius = serializedObject.FindProperty("DynamicLoadingRadius");*/
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // TODO: Fix that it will look good.

        //if (p_radiusMode.boolValue)
        //{
        //    p_useLocalRotation.boolValue = EditorGUILayout.Toggle("UseLocalRotation", p_useLocalRotation.boolValue);
        //}
        //else
        //{
        //    p_dynamicLoadingRadius.intValue = EditorGUILayout.IntField(p_dynamicLoadingRadius.intValue);
        //}

        if (availableTerrains == null)
        {
            availableTerrains = new Dictionary<string, DynamicTerrainBundles>();

            DynamicTerrainBundles TerrainName_Bundles = new DynamicTerrainBundles();
            TerrainName_Bundles.TerrainBundlePath = @"C:\DB\Unity\ImpovedAndDynamic\TerrainName\Improved_TerrainName\terrainwithmodels";
            TerrainName_Bundles.OriginalsBundlePath = @"C:\DB\Unity\ImpovedAndDynamic\TerrainName\Improved_TerrainName\terrainoriginalsnew";
            TerrainName_Bundles.ImrovedTerrainBundlePath = @"C:\DB\Unity\ImpovedAndDynamic\TerrainName\Improved_TerrainName\improved_TerrainName";
            TerrainName_Bundles.MapOffsetX = -3770;
            TerrainName_Bundles.MapOffsetZ = -6;

            availableTerrains["TerrainName"] = TerrainName_Bundles;
        }

        TerrainLoader_Dynamic targetTerrainLoader = (TerrainLoader_Dynamic)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Available Terrains:");

        foreach (var terrain in availableTerrains)
        {
            if (GUILayout.Button(terrain.Key))
            {
                serializedObject.FindProperty("TerrainBundlePath").stringValue = terrain.Value.TerrainBundlePath;
                serializedObject.FindProperty("TerrainOriginalsBundlePath").stringValue = terrain.Value.OriginalsBundlePath;
                serializedObject.FindProperty("ImprovedTerrainBundlePath").stringValue = terrain.Value.ImrovedTerrainBundlePath;
                serializedObject.FindProperty("MapOffsetX").floatValue = terrain.Value.MapOffsetX;
                serializedObject.FindProperty("MapOffsetZ").floatValue = terrain.Value.MapOffsetZ;
                serializedObject.ApplyModifiedProperties();
            }
        }

    }
}