using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersMarker : MonoBehaviour {

    public LayerMarking[] LayersMarking;

    [System.Serializable]
    public struct LayerMarking
    {
        public string GameObjectContains;
        public Layer Layer;
    }

	// Use this for initialization
	void Start () {
        TerrainLoader.Instance.TerrainOnlyLoaded += OnTerrainLoaded;
	}

    public void OnDestroy()
    {
        TerrainLoader.Instance.TerrainOnlyLoaded -= OnTerrainLoaded;
    }

    void OnTerrainLoaded()
    {
        TerrainLoader.Instance.TerrainOnlyLoaded -= OnTerrainLoaded;

        var allTransforms = TerrainLoader.Instance.LoadedScene.GetRootGameObjects()[0].GetComponentsInChildren<Transform>(true);
        foreach (var layerMarking in LayersMarking)
        {
            foreach (var transform in allTransforms)
            {
                string nameLower = transform.gameObject.name.ToLower();
                if (nameLower.Contains(layerMarking.GameObjectContains.ToLower()))
                    MarkLayer(transform.gameObject, Layers.LayerToId(layerMarking.Layer));
            }
        }
    }

    private void MarkLayer(GameObject gameObject, int layerId)
    {
        gameObject.layer = layerId;
        foreach (Transform child in gameObject.transform)
            MarkLayer(child.gameObject, layerId);
    }
	
}
