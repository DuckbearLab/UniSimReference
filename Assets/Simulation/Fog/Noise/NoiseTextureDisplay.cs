using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * NoiseTextureDisplay -
 * DESCRIPTION -
 * =================================================================================== */


public class NoiseTextureDisplay : MonoBehaviour
{
    public bool Show;
    [Range(0, 1)]
    public float Layer;
    public Texture3D Tex;
    private Material mat;

    void Start()
    {
        mat = new Material(Shader.Find("Hidden/Display3dTexture"));
        mat.SetTexture("_Display", Tex);
    }
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Show)
        {
            mat.SetFloat("Layer", Layer);
            Graphics.Blit(src, dest, mat);
        }
        else Graphics.Blit(src, dest);
    }
}