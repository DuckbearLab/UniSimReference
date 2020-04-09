using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * Fog -
 * DESCRIPTION - Enables fog on the camera. 
 * =================================================================================== */
namespace Simulation.Fog
{
    [RequireComponent(typeof(Camera))]
    public class Fog : MonoBehaviour
    {
        [Range(0, 500)]
        public float FogStartDistance = 0f;
        [Range(100, 1500)]
        public float FogMaxDistance = 500f;
        public float FogCurveExponent = 0.35f;

        public Gradient Gradient;

        public Texture2D gradientTex;
        private Material fogMaterial;
        private new Camera camera;
        void Start()
        {
            fogMaterial = new Material(Shader.Find("Hidden/Fog"));
            camera = GetComponent<Camera>();
            camera.depthTextureMode = camera.depthTextureMode | DepthTextureMode.Depth;
            CreateTexture();
            SetShaderParams();
        }

        private void SetShaderParams()
        {
            if (Application.isPlaying && null != fogMaterial)
            {
                fogMaterial.SetFloat("FogStartDistance", FogStartDistance);
                fogMaterial.SetFloat("FogMaxDistance", FogMaxDistance);
                fogMaterial.SetFloat("CurveExponent", FogCurveExponent);
                fogMaterial.SetTexture("FogGradient", gradientTex);
            }
        }

        private void CreateTexture()
        {
            if (null != gradientTex)
                Destroy(gradientTex);
            gradientTex = new Texture2D(128, 1);
            gradientTex.wrapMode = TextureWrapMode.Clamp;
            Color[] pixels = new Color[128];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Gradient.Evaluate((float)i / 128);
            }
            gradientTex.SetPixels(pixels);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Resolution r = Screen.currentResolution;
            fogMaterial.SetVector("_ScreenSize", new Vector4(r.width, r.height, 1f / r.width, 1f / r.height));
            Graphics.Blit(src, dest, fogMaterial);
        }

        void OnValidate()
        {
            FogCurveExponent = Mathf.Max(0.001f, FogCurveExponent);
            if (Application.isPlaying)
                CreateTexture();
            SetShaderParams();
        }

        [ContextMenu("Print gradient")]
        public void Grad()
        {
            foreach (var x in Gradient.alphaKeys)
            {
                Debug.Log(x.time.ToString() +  ", " + x.alpha.ToString());
            }
            foreach (var x in Gradient.colorKeys)
            {
                Debug.Log(x.time.ToString() + ", "+ x.color.r.ToString() + ", " + x.color.g.ToString() + ", " + x.color.b.ToString());
            }
        }
        
    }
}