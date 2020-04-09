using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassModel : MonoBehaviour
{
    private const int MaxVerts = 16250;

    public Material[] mats;
    public int[] RelativeChances; //must be of same length as mats
    
    private Transform MainCamera { get { return mainCamera ?? (mainCamera = Camera.main.transform); } }
    private Transform mainCamera;
    private List<int> materialIndex;
    private List<Tuple3<Mesh, Vector3, Material>> meshes = new List<Tuple3<Mesh, Vector3, Material>>();
    private Matrix4x4 matrix = Matrix4x4.identity;

    void Start()
    {
        materialIndex = GetWeightedRandomList();
    }

    public void Plant(List<Vector3> grassPoints)
    {
        while (grassPoints.Count > 0)
        {
            int grassChunkCount = System.Math.Min(grassPoints.Count, MaxVerts);

            var mesh = GenerateGrassMesh(grassPoints.GetRange(0, grassChunkCount));
            Vector3 position = mesh.bounds.center;
            meshes.Add(new Tuple3<Mesh, Vector3, Material>(mesh, position, mats[LocationToMaterial(position)]));
            grassPoints.RemoveRange(0, grassChunkCount);
        }
    }

    private Mesh GenerateGrassMesh(List<Vector3> grassPoints)
    {
        var mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        foreach (var grassPoint in grassPoints)
        {
            AddGrass(vertices, uvs, triangles, grassPoint);
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }


    List<int> GetWeightedRandomList()
    {
        List<int> weightedList = new List<int>();
        for (int i = 0; i < RelativeChances.Length; i++)
            for (int j = 0; j < RelativeChances[i]; j++)
                weightedList.Add(i);
        return weightedList;
    }
    private void AddGrass(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, Vector3 position)
    {
        int i = vertices.Count;

        vertices.Add(position);
        vertices.Add(position);
        vertices.Add(position);
        vertices.Add(position);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));

        triangles.Add(i + 2); triangles.Add(i + 1); triangles.Add(i + 0);
        triangles.Add(i + 0); triangles.Add(i + 3); triangles.Add(i + 2);
        //triangles.Add(i + 3); triangles.Add(i + 2); triangles.Add(i + 1); triangles.Add(i + 0);

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Tuple3<Mesh, Vector3, Material> triplet in meshes)
        {
            if (Vector3.SqrMagnitude(MainCamera.position - triplet.Second) < 22500)
                Graphics.DrawMesh(triplet.First, matrix, triplet.Third,  0);
        }
    }
    int LocationToMaterial(Vector3 location)
    {
        if (materialIndex.Count == 0)
            return 0;
       int res = ((Mathf.RoundToInt(location.x + location.y + location.z) % materialIndex.Count) + materialIndex.Count) % materialIndex.Count;
                   //some no-rand (const) function from center of mesh     |  ((x % y) + y) % y returns between 0 (inclusive) 
                   //to item in materialIndex. can be any function really  |  and y (exclusive) for every (signed) integer x
        return materialIndex[res];
    }
}

