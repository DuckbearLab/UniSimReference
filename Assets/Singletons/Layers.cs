using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Layer
{
    None = -1,
    IgnoreRaycast = 2,
    Player = 8,
    Ladder = 9,
    ReflectedEntity = 11,
    TerrainTerrain = 12,
    TerrainBuildings = 13,
    TerrainTrees = 14,
    TerrainBushes = 15,
    TerrainWalls = 16,
    TerrainSceneAdds = 17,
    ForcesIcons = 19,
    TerrainNight = 27,
    Sky = 31
}

public static class Layers
{

    public static int IgnoreRaycast = 1 << LayerToId(Layer.IgnoreRaycast);
    public static int Player = 1 << LayerToId(Layer.Player);
    public static int Ladder = 1 << LayerToId(Layer.Ladder);

    public static int ReflectedEntity = 1 << LayerToId(Layer.ReflectedEntity);

    public static int TerrainTerrain = 1 << LayerToId(Layer.TerrainTerrain);
    public static int TerrainBuildings = 1 << LayerToId(Layer.TerrainBuildings);
    public static int TerrainTrees = 1 << LayerToId(Layer.TerrainTrees);
    public static int TerrainBushes = 1 << LayerToId(Layer.TerrainBushes);
    public static int TerrainWalls = 1 << LayerToId(Layer.TerrainWalls);
    public static int TerrainSceneAdds = 1 << LayerToId(Layer.TerrainSceneAdds);

    public static int ForcesIcons = 1 << LayerToId(Layer.ForcesIcons);

    public static int TerrainNight = 1 << LayerToId(Layer.TerrainNight);
    public static int AllTerrainLayers = TerrainTerrain | TerrainBuildings | TerrainTrees | TerrainBushes | TerrainWalls | TerrainSceneAdds;


    public static int Sky = 1 << LayerToId(Layer.Sky);

    public static int LayerToId(Layer layer)
    {
        switch (layer)
        {
            case Layer.IgnoreRaycast: return Physics.IgnoreRaycastLayer;
            case Layer.Player: return LayerMask.NameToLayer("Player");
            case Layer.Ladder: return LayerMask.NameToLayer("Ladder");
			
            case Layer.ReflectedEntity: return LayerMask.NameToLayer("ReflectedEntity");

            case Layer.TerrainTerrain: return LayerMask.NameToLayer("Terrain Terrain");
            case Layer.TerrainBuildings: return LayerMask.NameToLayer("Terrain Buildings");
            case Layer.TerrainTrees: return LayerMask.NameToLayer("Terrain Trees");
            case Layer.TerrainBushes: return LayerMask.NameToLayer("Terrain Bushes");
            case Layer.TerrainWalls: return LayerMask.NameToLayer("Terrain Walls");
            case Layer.TerrainSceneAdds: return LayerMask.NameToLayer("Terrain Scene Adds");

            case Layer.ForcesIcons: return LayerMask.NameToLayer("ForcesIcons");

            case Layer.TerrainNight: return LayerMask.NameToLayer("Terrain Night");

            case Layer.Sky: return LayerMask.NameToLayer("Sky");
        }
        throw new Exception("Invalid layer! " + layer.ToString());
    }

    public static bool IsOnLayer(GameObject gameObject, Layer layer)
    {
        return gameObject.layer == LayerToId(layer);
    }

}
