using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMinimap : MonoBehaviour {

    public Texture2D MapTexture;
    public Texture2D[] MapTextureParts;
    public int MapTexturePartsColumns;
    public int MapTexturePartsRows;
    public Vector2 MapMin;
    public Vector2 MapMax;

    private int mapTexturePartSize = 1024;

    void Awake()
    {
        // The map in xxxx map has a lower resolution (512x512) than all the other maps.
        if (TerrainLoader.Instance.TerrainBundlePath.ToLower().Contains("xxxx"))
            mapTexturePartSize = 512;

        MapTexture = new Texture2D(mapTexturePartSize * MapTexturePartsColumns, mapTexturePartSize * MapTexturePartsRows, TextureFormat.DXT1, false);
        int i = 0;
        for (int y = 0; y < MapTexturePartsRows; y++)
            for (int x = 0; x < MapTexturePartsColumns; x++)
                Graphics.CopyTexture(MapTextureParts[i++], 0, 0, 0, 0, mapTexturePartSize, mapTexturePartSize, MapTexture, 0, 0, mapTexturePartSize * x, mapTexturePartSize * y);


        //USE This Code To Get The map in png format form map asset bundel @sagi_dadon

        //Texture2D temp = new Texture2D(512 * MapTexturePartsColumns, 512 * MapTexturePartsRows, TextureFormat.RGBAFloat, false);
        //Graphics.ConvertTexture(MapTexture, temp);
        //temp.Apply();
        //Texture2D temp2D = null;
        //temp2D = new Texture2D(temp.width, temp.height);
        //temp2D.ReadPixels(new Rect(0,0,temp.width,temp.height) ,0,0);
        //temp2D.Apply();
        //byte[] data = temp2D.EncodeToPNG();
        //System.IO.File.WriteAllBytes("C:\\Users\\xxxx\\Desktop\\.png", data);           
    }


}
