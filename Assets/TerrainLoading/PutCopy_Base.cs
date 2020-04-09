using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * PutCopy_Base -
 * DESCRIPTION - A base class for PutCopy and PutCopyDynamic. 
 * This type has their common members. 
 * =================================================================================== */

public abstract class PutCopy_Base : MonoBehaviour
{
    #region Shared Public Variables

    /// <summary>
    /// A List of possible textures for buildings.
    /// </summary>
    /// <remarks>This is assigned from the inspector and saved in the terrain AssetBundle. </remarks>
    public List<TextureSwitch> TextureSwitches = new List<TextureSwitch>();

    public PutCopyLodManager LodManager { get; protected set; }

    #endregion


    #region Shared Protected Variables

    protected Dictionary<Material, List<Material>> textureSwitchesCache;
    /// <summary>
    /// A dictionay that holds a tile position and a list with the copies
    /// </summary>
    protected Dictionary<Point, List<Details>> orderedCopies;
    /// <summary>
    /// An HashSet that holds all the copies that were created (one of each type!)
    /// </summary>
    protected HashSet<GameObject> patchedToCopies = new HashSet<GameObject>();

    protected SpiralStepper spiralStepper;

    #endregion


    #region Shared Constants

    protected const int OrganizedChunkSize = 50;
    protected const int MaxInstantiationsPerFrame = 1;
    protected const float InstantiationRadius = 999000;

    #endregion


    #region Shared Events

    /// <summary>
    /// Invoked when the loading is done (of the terrain or of a tile). 
    /// </summary>
    public event System.Action Done;

    protected void Invoke_Done()
    {
        if (null != Done) Done();
    }

    #endregion


    #region Shared Methods

    protected void PrepareTextureSwitchesCaches()
    {
        textureSwitchesCache = new Dictionary<Material, List<Material>>();

        Dictionary<int, List<Material>> groups = new Dictionary<int, List<Material>>();
        foreach (var textureSwitch in TextureSwitches)
        {
            if (!groups.ContainsKey(textureSwitch.Group))
                groups[textureSwitch.Group] = new List<Material>();

            groups[textureSwitch.Group].Add(textureSwitch.Material);
            textureSwitchesCache[textureSwitch.Material] = groups[textureSwitch.Group];
        }
    }

    #endregion


    #region Shared Structs

    protected struct Point
    {
        public int x;
        public int y;

        public override int GetHashCode()
        {
            return x << 10 + y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point other = (Point)obj;
                return x == other.x && y == other.y;
            }
            return base.Equals(obj);
        }
    }

    [System.Serializable]
    public struct Details
    {
        public GameObject ToCopy;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    [System.Serializable]
    public struct TextureSwitch
    {
        public int Group;
        public Material Material;
    }

    #endregion
}