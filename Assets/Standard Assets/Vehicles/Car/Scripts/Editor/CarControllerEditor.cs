using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/* ===================================================================================
 * CarControllerEditor -
 * DESCRIPTION -
 * =================================================================================== */
namespace UnityStandardAssets.Vehicles.Car.EditorScripts
{
    [CustomEditor(typeof(CarController))]
    public class CarControllerEditor : Editor
    {
        private SerializedProperty useDamp;
        private MonoScript script;
        void OnEnable()
        {
            useDamp = serializedObject.FindProperty("useChangingDampeningRate");
            script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", script, typeof(CarController), false);
            EditorGUI.EndDisabledGroup();
            SerializedProperty property = serializedObject.GetIterator();
            property.Next(true);
            property.Next(false); //skip many properties that should not be displayed
            property.Next(false);
            property.Next(false);
            property.Next(false);
            property.Next(false);
            property.Next(false);
            property.Next(false);
            property.Next(false);
            property.Next(false);
            bool next = true;
            while (next)
            {
                if (!property.name.Contains("dampRate") || useDamp.boolValue)
                    DrawDefault(property);
                next = property.Next(false);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDefault(SerializedProperty property)
        {
            if (property.hasChildren)
                EditorGUILayout.PropertyField(property, true);
            else
                EditorGUILayout.PropertyField(property);
        }
    }
}