//disables Variable not in use warning
#pragma warning disable 414 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrassPlanter : MonoBehaviour {

    public GrassModel GrassModel;

    public int CamSize = 50;
    public float PixelsPerMeter = 2f;

    public Camera myCam;
    public Light myLight;
    public RenderTexture camTexture;
    public Texture2D camTexture2D;

    public Color BestColor;
    public float BestColorMargin;
    public float GrassSpread;

    private HashSet<Point> scannedAreas;
    private List<Light> sceneLights;
    private List<Vector3> grassPoints;
    //private TerrainMinimap TerrainMinimap;

    private bool canStartScanningNextArea, shouldChangeLights;

    private const int MaxRaycatsPerFrame = 500;

    private struct Point
    {
        public int x;
        public int y;

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

	// Use this for initialization
	void Start () {
        scannedAreas = new HashSet<Point>();
        grassPoints = new List<Vector3>();
        camTexture = new RenderTexture(Mathf.FloorToInt(CamSize * PixelsPerMeter), Mathf.FloorToInt(CamSize * PixelsPerMeter), 0);
        camTexture2D = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, false);
        sceneLights = FindObjectsOfType<Light>().ToList();

        myCam.targetTexture = camTexture;

        myCam.aspect = 1;
        myCam.orthographicSize = CamSize / 2;
        myCam.orthographic = true;

        myLight.type = LightType.Directional;
        myLight.intensity = 1;
        myLight.color = Color.white;

        transform.eulerAngles = new Vector3(90, 0, 0);

        TerrainLoader.Instance.TerrainFullyLoaded += TerrainLoaded;
        canStartScanningNextArea = false;
	}

    void TerrainLoaded()
    {
        //TerrainMinimap = ObjectFinder.FindObjectOfType<TerrainMinimap>();
        canStartScanningNextArea = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (!canStartScanningNextArea)
            return;
        sceneLights.RemoveAll(x => null == x);
        if(Camera.main)
        {
            var pos = Camera.main.transform.position;

            int centerX = Mathf.RoundToInt(pos.x / CamSize);
            int centerY = Mathf.RoundToInt(pos.z / CamSize);

            var spiralStepper = new SpiralStepper(centerX, centerY);

            while(spiralStepper.Layer < 5)
            {
                Point cur = new Point() { x = spiralStepper.X, y = spiralStepper.Y };
                if (!scannedAreas.Contains(cur))
                {
                    scannedAreas.Add(cur);
                    StartCoroutine(Go(spiralStepper.X * CamSize, spiralStepper.Y * CamSize));
                    return;
                }

                spiralStepper.Step();
            }
        }
	}

    public IEnumerator Go(float x, float y)
    {
        canStartScanningNextArea = false;

        shouldChangeLights = true; //TurnOffLights();

        grassPoints.Clear();

        transform.position = new Vector3(
            x,
            1000,
            y
        );

        yield return ScanArea();
        PutGrassOnGrassPoints();

        //TurnOnLights();

        //yield return new WaitForEndOfFrame();

        canStartScanningNextArea = true;
    }

    void OnPreCull()
    {
        if (shouldChangeLights)
        {
            TurnOffLights();
            shouldChangeLights = false;
        }
    }

    void OnPostRender()
    {
        if (myLight.enabled)
            TurnOnLights();
    }

    private void TurnOffLights()
    {
        foreach (var sceneLight in sceneLights)
            sceneLight.enabled = false;
        myLight.enabled = true;
    }

    private void TurnOnLights()
    {
        foreach (var sceneLight in sceneLights)
            sceneLight.enabled = true;
        myLight.enabled = false;
    }

    private IEnumerator ScanArea()
    {
        int raycasts = 0;

        myCam.Render();

        var prevRenderTexture = RenderTexture.active;
        RenderTexture.active = camTexture;
        camTexture2D.ReadPixels(new Rect(0, 0, camTexture.width, camTexture.height), 0, 0, true);
        camTexture2D.Apply();
        RenderTexture.active = prevRenderTexture;

        var pixels = camTexture2D.GetPixels();
        for (int x = 0; x < camTexture2D.width; x++)
        {
            for (int y = 0; y < camTexture2D.height; y++)
            {
                var pixel = pixels[x + y * camTexture2D.width];

                float targetH, targetS, targetV;
                float curH, curS, curV;
                Color.RGBToHSV(BestColor, out targetH, out targetS, out targetV);
                Color.RGBToHSV(pixel, out curH, out curS, out curV);

                //float DistFromBest = Vector3.Distance(new Vector3(curH, curS, curV), new Vector3(targetH, targetS, targetV));
                float DistFromBest = Vector3.Distance(new Vector3(pixel.r, pixel.g, pixel.b), new Vector3(BestColor.r, BestColor.g, BestColor.g));


                //if (curH >= minH && curH <= maxH && curS >= minS && curS <= maxS && curV >= minV && curV <= maxV)
                if (DistFromBest <= BestColorMargin)
                {
                    float worldPosX = transform.position.x - CamSize / 2 + x / PixelsPerMeter;
                    float worldPosZ = transform.position.z - CamSize / 2 + y / PixelsPerMeter;
                    
                    var WorldPos = new Vector3(worldPosX, transform.position.y, worldPosZ);

                    int times = 1;
                    if (DistFromBest / BestColorMargin <= 0.5f)
                        times++;
                    if (DistFromBest / BestColorMargin <= 0.25f)
                        times++;

                    for (int i = 0; i < times; i++)
                    {
                        RaycastRandomGrassCheck(WorldPos);

                        if (raycasts++ == MaxRaycatsPerFrame)
                        {
                            //TurnOnLights();
                            yield return new WaitForEndOfFrame();
                            shouldChangeLights = true; //TurnOffLights();
                            raycasts = 0;
                        }
                    }
                }
            }
        }

    }

    private void RaycastRandomGrassCheck(Vector3 WorldPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(WorldPos + Random.insideUnitSphere * GrassSpread, Vector3.down, out hit))
            if(Layers.IsOnLayer(hit.transform.gameObject, Layer.TerrainTerrain))
                grassPoints.Add(hit.point);
    }

    private void PutGrassOnGrassPoints()
    {
        GrassModel.Plant(grassPoints);
    }
}
