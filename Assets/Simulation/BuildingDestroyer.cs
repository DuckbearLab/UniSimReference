using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventReports;

public class BuildingDestroyer : MonoBehaviour
{
    [System.Serializable]
    public struct BuildingDestroyedModel
    {
        public string BuildingOriginalName;
        public GameObject DestroyedModel;
        public Vector3 Rotation;
    }

    public EventReportsManager EventReportsManager;
    public TerrainLoader TerrainLoader;
    public List<BuildingDestroyedModel> DestroyedModels;

    private Transform copiesParent;
    private bool terrainFullyLoaded = false;
    private bool initialized = false;

    private const string CopiesGameObjectName = "copies";
    private const float SquaredDistanceApproximationThreshold = 0.5f;

    private void Start()
    {
        TerrainLoader.TerrainFullyLoaded += () => terrainFullyLoaded = true;

        EventReportsManager.Subscribe<BuildingDamageResult>(BuildingDamageHandler);
        EventReportsManager.Subscribe<DamagedBuildingsReport>(x => StartCoroutine(DamagedBuildingsReportHandler(x)));

        EventReportsManager.Send(new DamagedBuildingsRequest());
    }

    private void BuildingDamageHandler(BuildingDamageResult bdr)
    {
        switch (bdr.Result)
        {
            case BuildingDamageResult.DamageResult.None:
                break;
            case BuildingDamageResult.DamageResult.PartiallyDestroyed:
                break;
            case BuildingDamageResult.DamageResult.Destroyed:
                Vector3 local = CoordConverter.GeocToLocal(bdr.Location);
                StartCoroutine(DestroyBuildingAtPosition(local));
                break;
            default:
                break;
        }
    }

    private IEnumerator DamagedBuildingsReportHandler(DamagedBuildingsReport dbr)
    {
        if (initialized)
            yield break;

        while (CopiesParent == null || !terrainFullyLoaded)
            yield return null;

        DamagedBuildingsReport.DamagedBuildings[] buildings = dbr.Buildings;

        for (int i = 0; i < buildings.Length; i++)
        {
            switch (buildings[i].State)
            {
                case DamagedBuildingsReport.DamageState.None:
                    break;
                case DamagedBuildingsReport.DamageState.PartiallyDestroyed:
                    break;
                case DamagedBuildingsReport.DamageState.Destroyed:
                    Vector3 local = CoordConverter.GeocToLocal(buildings[i].Location);
                    StartCoroutine(DestroyBuildingAtPosition(local));
                    break;
                default:
                    break;
            }
        }

        initialized = true;
    }

    private IEnumerator DestroyBuildingAtPosition(Vector3 position)
    {
        while (true)
        {
            Transform copies = CopiesParent;
            if (copies == null)
                yield break;

            Transform tran = null;

            for (int i = 0; i < copies.childCount; i++)
            {
                tran = copies.GetChild(i);

                float sqrMagnitude = (tran.position - position).sqrMagnitude;

                if (sqrMagnitude < SquaredDistanceApproximationThreshold)
                {
                    Destroy(tran);
                    yield break;
                }
            }

            yield return new WaitForSeconds(5);
        }
    }

    private void Destroy(Transform tran)
    {
        int index = DestroyedModels.FindIndex(x => tran.name.Contains(x.BuildingOriginalName));

        if (index != -1)
        {
            Instantiate(DestroyedModels[index].DestroyedModel, tran.position, Quaternion.Euler(DestroyedModels[index].Rotation), CopiesParent);
            Destroy(tran.gameObject);
        }
    }

    private void FindCopiesParent()
    {
        if (TerrainLoader.LoadedScene == default(UnityEngine.SceneManagement.Scene) || !TerrainLoader.LoadedScene.isLoaded)
            return;

        Transform root = TerrainLoader.LoadedScene.GetRootGameObjects()[0].transform;
        int childCount = root.childCount;
        Transform child = null;

        for (int i = 0; i < childCount; i++)
        {
            child = root.GetChild(i);

            if (child.name.Equals(CopiesGameObjectName))
            {
                copiesParent = child.transform;
                return;
            }
        }
    }

    private Transform CopiesParent
    {
        get
        {
            if (copiesParent == null)
                FindCopiesParent();
            return copiesParent;
        }
    }
}