using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * WorleyNoiseGenerator3D -
 * DESCRIPTION - Generates a 3D worley texture. 
 * =================================================================================== */
namespace Simulation.Fog
{
    public class WorleyNoiseGenerator3D : MonoBehaviour
    {

        #region Create Texture
#if UNITY_EDITOR
        [ContextMenu("Create Texture")]
        private void CreateTexture()
        {
            if (Texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture3D>("Assets/Simulation/Fog/Noise/Noise3D.asset"))
                return;
            Texture = new Texture3D(400, 400, 400, TextureFormat.RGBAFloat, false);
            if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Simulation/Fog/Noise"))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets/Simulation/Fog", "Noise");
            }
            UnityEditor.AssetDatabase.CreateAsset(Texture, "Assets/Simulation/Fog/Noise/Noise3D.asset");
        }
#endif
        #endregion

        #region GeneratePoints
        public int CellsPerAxis;

        private Vector3[] GeneratePoints()
        {
            Vector3[] result = new Vector3[CellsPerAxis * CellsPerAxis * CellsPerAxis];
            int index = 0;
            float cellSize = 1f / CellsPerAxis;
            for (int x = 0; x < CellsPerAxis; x++)
            {
                for (int y = 0; y < CellsPerAxis; y++)
                {
                    for (int z = 0; z < CellsPerAxis; z++)
                    {
                        Vector3 offset = new Vector3(Random.value, Random.value, Random.value);
                        Vector3 normPosition = (new Vector3(x, y, z) + offset) * cellSize;
                        result[index++] = normPosition;
                    }
                }
            }
            return result;
        }

        private Vector3[] GeneratorSurrounds(Vector3[] Points)
        {
            Vector3[] result = new Vector3[6*CellsPerAxis*CellsPerAxis + 12*CellsPerAxis + 8]; //(n+2)^3-n^2
            int index = 0;
            float cellSize = 1f / CellsPerAxis;
            for (int x = -1; x < CellsPerAxis + 1; x++)
            {
                for (int y = -1; y < CellsPerAxis + 1; y++)
                {
                    for (int z = -1; z < CellsPerAxis + 1; z++)
                    {
                        if (!PointIsSurround(x, y, z))
                            continue; //not in surrounds
                        try
                        {
                            int cellX = SurroundCoordToCellCoord(x),
                                cellY = SurroundCoordToCellCoord(y),
                                cellZ = SurroundCoordToCellCoord(z);
                            result[index++] = (new Vector3(x, y, z) * cellSize) + Points[cellX * CellsPerAxis * CellsPerAxis + cellY * CellsPerAxis + cellZ];
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError(new Vector3(x, y, z));
                            throw e;
                        }
                    }
                }
            }
            return result;
        }

        private int SurroundCoordToCellCoord(int SurroundCoord)
        {
            return (CellsPerAxis + SurroundCoord) % CellsPerAxis;
        }
        #endregion

        #region Process
        public Texture3D Texture;
        public ComputeShader Worley3D;
        private ComputeBuffer pointsBuffer;

        private void ProcessPoints()
        {
            Vector3[] points = GeneratePoints();
            Vector3[] surrounds = GeneratorSurrounds(points);

            int cells = CellsPerAxis + 2;
            int topSurround = cells - 1;
            List<Vector3> all = new List<Vector3>();
            int surIndex = 0, poiIndex = 0;
            for (int x = -1; x < CellsPerAxis + 1; x++)
            {
                for (int y = -1; y < CellsPerAxis + 1; y++)
                {
                    for (int z = -1; z < CellsPerAxis + 1; z++)
                    {
                        if (PointIsSurround(x, y, z))
                            all.Add(surrounds[surIndex++]);
                        else 
                            all.Add(points[poiIndex++]);
                    }
                }
            }
            if (null != pointsBuffer)
                pointsBuffer.Release();
            pointsBuffer = new ComputeBuffer(surrounds.Length + points.Length, 24, ComputeBufferType.Default);

            pointsBuffer.SetData(all.ToArray());

        }
        #endregion

        [ContextMenu("Go")]
        public void Dispatch()
        {
            ProcessPoints();

            Worley3D.SetInt("CellsPerAxis", CellsPerAxis);
            int wKer = Worley3D.FindKernel("CSMain");
            Worley3D.SetBuffer(wKer, "Points", pointsBuffer);

            Worley3D.SetTexture(wKer, "Result", Texture);

            Worley3D.Dispatch(wKer, 50, 50, 50);
            Texture.Apply();
        }


        #region Helpers
        private bool PointIsSurround(int x, int y, int z)
        {
            if (-1 == x || -1 == y || -1 == z || CellsPerAxis == x || CellsPerAxis == y || CellsPerAxis == z)
                return true;
            return false;
        }
        #endregion
    }
}