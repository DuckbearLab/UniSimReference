using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PutCopy : PutCopy_Base
{

#if DYNAMIC_LOADING

    [HideInInspector]
    public List<TileDetails> Copies = new List<TileDetails>();


    public List<TextureSwitch> TextureSwitches = new List<TextureSwitch>();

    [System.Serializable]
    public struct TileDetails
    {
        public Vector2 Tile;
        public List<Details> Details;
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

    private Dictionary<Material, List<Material>> textureSwitchesCache;

    private struct Point
    {
        public int x;
        public int y;

        public override int GetHashCode()
        {
            return x << 10 + y;
        }

        public override bool Equals(object obj)
        {
            if(obj is Point)
            {
                Point other = (Point)obj;
                return this.x == other.x && this.y == other.y;
            }
            return base.Equals(obj);
        }
    }

    private Dictionary<Point, List<Details>> orderedCopies;
    private SpiralStepper spiralStepper;
    private const int OrganizedChunkSize = 50;
    private const int MaxInstantiationsPerFrame = 1;
    private const float InstantiationRadius = 999000;
    public PutCopyLodManager lodManager;

    public event Action Done;

    void Awake()
    {
        lodManager = new PutCopyLodManager();
    }

    // Use this for initialization
    void Start()
    {
        //PrepareTextureSwitchesCaches();

        ////Debug.Log(Copies.Count);

        //OrganizeCopies();

        //spiralStepper = new SpiralStepper();
        //StartCoroutine(SortCopiesPeriodically());
        //StartCoroutine(AddCopies());

        //lodManager = new PutCopyLodManager();
        //StartCoroutine(lodManager.Run());
    }

    private void OrganizeCopies()
    {
        orderedCopies = new Dictionary<Point,List<Details>>();

        //foreach(var copy in Copies)
        //{
        //    var p = new Point()
        //    {
        //        x = (int)(copy.Position.x / OrganizedChunkSize),
        //        y = (int)(copy.Position.y / OrganizedChunkSize)
        //    };
        //    List<Details> details;
        //    if(orderedCopies.TryGetValue(p, out details))
        //        details.Add(copy);
        //    else
        //        orderedCopies[p] = new List<Details>(new Details[] { copy });
        //}
    }

    public IEnumerator LoadCopies(Vector2 tileLocation, GameObject tile)
    {
        yield return null;

        bool finished = false;
        for (int i = 0; i < Copies.Count; i++)
        {
            if (tileLocation == Copies[i].Tile)
            {
                Debug.Log("Started loading: " + Copies[i].Details.Count + " copies in tile: " + tileLocation.ToString());

                foreach (var copy in Copies[i].Details)
                {
                    GameObject gameObj = Instantiate(copy.ToCopy, copy.Position, copy.Rotation, tile.transform);
                    gameObj.name = copy.ToCopy.name;

              //      lodManager.AddCopy(gameObj);
             //       ApplyTextureSwitches(gameObj);
                    yield return null;
                }

                finished = true;
            }

            if (finished)
                break;
        }

        Debug.Log("Finished loading copies in tile: " + tileLocation.ToString());

        if (Done != null)
            Done();
    }

    private IEnumerator SortCopiesPeriodically()
    {
        while (true)
        {
            var camPos = Camera.main.transform.position;
            spiralStepper.Reset((int)camPos.x / OrganizedChunkSize, (int) camPos.y / OrganizedChunkSize);
            yield return new WaitForSeconds(5);
        }
    }

    private void PrepareTextureSwitchesCaches()
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

    private HashSet<GameObject> patchedToCopies = new HashSet<GameObject>();

    private IEnumerator AddCopies()
    {
        var copies = new GameObject("copies");
        copies.transform.parent = transform;

        yield return null;

        int instantiations = 0;

        // Run only on build
        bool firstTime = !Application.isEditor;

        /*while(true)
        {*/
            while (spiralStepper.Layer * OrganizedChunkSize < InstantiationRadius && orderedCopies.Count > 0)
            {
                var p = new Point() { x = spiralStepper.X, y = spiralStepper.Y };
                List<Details> chunk;
                if(orderedCopies.TryGetValue(p, out chunk))
                {
                    while (chunk.Count > 0)
                    {
                        var copy = chunk[0];
                        if (copy.ToCopy)
                        {
                            if (!patchedToCopies.Contains(copy.ToCopy))
                            {
                                if (copy.ToCopy.activeSelf)
                                    copy.ToCopy.SetActive(false);

                                foreach (Rigidbody rb in copy.ToCopy.GetComponentsInChildren<Rigidbody>())
                                    DestroyImmediate(rb);

                                patchedToCopies.Add(copy.ToCopy);
                            }

                            GameObject gameObj = Instantiate(copy.ToCopy, copy.Position, copy.Rotation, copies.transform);
                            gameObj.name = copy.ToCopy.name;
                            ApplyTextureSwitches(gameObj);

                            lodManager.AddCopy(gameObj);
                            if(firstTime)
                            {
                                if(instantiations++ >= 500000)
                                {
                                    Debug.Log("Count: " + copies.transform.childCount);
                                    firstTime = false;
                                }
                            }
                            else if(instantiations++ >= MaxInstantiationsPerFrame)
                            {
                                yield return new WaitForEndOfFrame();
                                instantiations = 0;
                            }
                        }
                        chunk.RemoveAt(0);
                    }
                    orderedCopies.Remove(p);
                }
                spiralStepper.Step();
            }
            
            /*yield return new WaitForEndOfFrame();
        }*/

        //Debug.Log("DOME!");
        if(Done != null)
            Done();
    }

    private void ApplyTextureSwitches(GameObject gameObj)
    {
        foreach (var renderer in gameObj.GetComponentsInChildren<MeshRenderer>())
        {
            var materials = renderer.sharedMaterials;
            bool materialChanged = false;
            for (int i = 0; i < materials.Length; i++)
            {

                List<Material> replaceWith;
                if (textureSwitchesCache.TryGetValue(materials[i], out replaceWith))
                {
                    materials[i] = ChooseRandomTexture(replaceWith, gameObj);
                    materialChanged = true;
                }
            }
            if (materialChanged)
                renderer.sharedMaterials = materials;
        }
    }

    private Material ChooseRandomTexture(List<Material> replaceWith, GameObject gameObj)
    {
        return replaceWith[HashLocation(gameObj.transform.position, replaceWith.Count)];
    }

    private int HashLocation(Vector3 location, int mod)
    {
        int x = Mathf.FloorToInt(location.x);
        int y = Mathf.FloorToInt(location.y);
        int z = Mathf.FloorToInt(location.z);
        return Mathf.Abs(x ^ y ^ z) % mod;
    }

}

//#region with list

#else

    [HideInInspector]
    public List<Details> Copies = new List<Details>();

    //   Use this for initialization
    void Start()
    {
        PrepareTextureSwitchesCaches();

        //Debug.Log(Copies.Count);

        OrganizeCopies();
    
        spiralStepper = new SpiralStepper();
        StartCoroutine(SortCopiesPeriodically());
        StartCoroutine(AddCopies());

        LodManager = new PutCopyLodManager();
        StartCoroutine(LodManager.Run());
    }

    private void OrganizeCopies()
    {
        orderedCopies = new Dictionary<Point, List<Details>>();

        foreach (var copy in Copies)
        {
            var p = new Point()
            {
                x = (int)(copy.Position.x / OrganizedChunkSize),
                y = (int)(copy.Position.y / OrganizedChunkSize)
            };
            List<Details> details;
            if (orderedCopies.TryGetValue(p, out details))
                details.Add(copy);
            else
                orderedCopies[p] = new List<Details>(new Details[] { copy });
        }
    }

    private IEnumerator SortCopiesPeriodically()
    {
        while (Camera.main != null)
        {
            var camPos = Camera.main.transform.position;
            spiralStepper.Reset((int)camPos.x / OrganizedChunkSize, (int)camPos.y / OrganizedChunkSize);
            yield return new WaitForSeconds(5);
        }
    }



    private IEnumerator AddCopies()
    {
        var copies = new GameObject("copies");
        copies.transform.parent = transform;

        yield return null;

        int instantiations = 0;

        //       Run only on build
        bool firstTime = !Application.isEditor;

        /*while(true)
        {*/
        while (spiralStepper.Layer * OrganizedChunkSize < InstantiationRadius && orderedCopies.Count > 0)
        {
            var p = new Point() { x = spiralStepper.X, y = spiralStepper.Y };

            // A list that holds all the copies in a specific tile (p).
            List<Details> chunk;

            if (orderedCopies.TryGetValue(p, out chunk))
            {
                while (chunk.Count > 0)
                {
                    var copy = chunk[0];
                    if (copy.ToCopy)
                    {
                        if (!patchedToCopies.Contains(copy.ToCopy))
                        {
                            if (copy.ToCopy.activeSelf)
                                copy.ToCopy.SetActive(false);

                            foreach (Rigidbody rb in copy.ToCopy.GetComponentsInChildren<Rigidbody>())
                                DestroyImmediate(rb);

                            patchedToCopies.Add(copy.ToCopy);
                        }

                        GameObject gameObj = Instantiate(copy.ToCopy, copy.Position, copy.Rotation, copies.transform);
                        gameObj.name = copy.ToCopy.name;
                        ApplyTextureSwitches(gameObj);

                        LodManager.AddCopy(gameObj);
                        if (firstTime)
                        {
                            if (instantiations++ >= 500000)
                            {
                                Debug.Log("Count: " + copies.transform.childCount);
                                firstTime = false;
                            }
                        }
                        else if (instantiations++ >= MaxInstantiationsPerFrame)
                        {
                            yield return new WaitForEndOfFrame();
                            instantiations = 0;
                        }
                    }
                    chunk.RemoveAt(0);
                }
                orderedCopies.Remove(p);
            }
            spiralStepper.Step();
        }

        /*yield return new WaitForEndOfFrame();
    }*/

        Invoke_Done();
    }

    private void ApplyTextureSwitches(GameObject gameObj)
    {
        foreach (var renderer in gameObj.GetComponentsInChildren<MeshRenderer>())
        {
            var materials = renderer.sharedMaterials;
            bool materialChanged = false;
            for (int i = 0; i < materials.Length; i++)
            {

                List<Material> replaceWith;
                if (textureSwitchesCache.TryGetValue(materials[i], out replaceWith))
                {
                    materials[i] = ChooseRandomTexture(replaceWith, gameObj);
                    materialChanged = true;
                }
            }
            if (materialChanged)
                renderer.sharedMaterials = materials;
        }
    }

    private Material ChooseRandomTexture(List<Material> replaceWith, GameObject gameObj)
    {
        return replaceWith[HashLocation(gameObj.transform.position, replaceWith.Count)];
    }

    private int HashLocation(Vector3 location, int mod)
    {
        int x = Mathf.FloorToInt(location.x);
        int y = Mathf.FloorToInt(location.y);
        int z = Mathf.FloorToInt(location.z);
        return Mathf.Abs(x ^ y ^ z) % mod;
    }
}

#endif
//#endregion
