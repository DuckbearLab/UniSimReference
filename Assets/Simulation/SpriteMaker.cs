using UnityEngine;
using System.Collections;
using System.IO;

public static class SpriteMaker
{
    public static Sprite LoadSpriteFromFile(string filePath)
    {
        Texture2D tex = LoadTextureFromFile(filePath);

        Sprite sprite = Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(0,0));

        return sprite;
    }

    public static Texture2D LoadTextureFromFile(string filePath)
    {
        Texture2D tex;
        byte[] texData;

        if(File.Exists(filePath))
        {
            texData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            if (tex.LoadImage(texData))
                return tex;
        }
        return null;
    }
}
