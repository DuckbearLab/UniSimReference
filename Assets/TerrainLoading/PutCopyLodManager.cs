using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class PutCopyLodManager
{
    public int PauseEvery = 500;
    public float CollidersRange = 1000;
    public float ViewRange = 3000;

    public long copiesToCheckCount;
    public long collidersToCheckCount;

    public long toAddToCopiesToCheckCount;
    public long toAddToCollidersToCheckCount;

    private List<GameObject> copiesToCheck;
    private List<Collider> collidersToCheck;

    private List<GameObject> toAddToCopiesToCheck;
    private List<Collider> toAddToCollidersToCheck;

    private float automaticRecalculateTime = -1; //if Time.time > this, contimue regular calculation. 

    public Vector3 Center
    {
        get
        {
            if (null == mainCameraTransform)
                mainCameraTransform = Camera.main.transform;
            return (CollidersCetner ?? mainCameraTransform).position;
        }
    }
    public static Transform CollidersCetner;
    private Transform mainCameraTransform;

    public PutCopyLodManager()
    {
        copiesToCheck = new List<GameObject>();
        collidersToCheck = new List<Collider>();

        toAddToCopiesToCheck = new List<GameObject>();
        toAddToCollidersToCheck = new List<Collider>();
    }

    public void AddCopy(GameObject copy)
    {
        toAddToCopiesToCheck.Add(copy);
        toAddToCopiesToCheckCount = toAddToCopiesToCheck.Count;

        var colliders = copy.GetComponentsInChildren<Collider>();
        toAddToCollidersToCheck.AddRange(colliders);
        toAddToCollidersToCheckCount = toAddToCollidersToCheck.Count;

        foreach (var collider in colliders)
            CheckCollider(collider);
        CheckCopy(copy);
    }

    public long i = 0;

    public IEnumerator Run()
    {
        while (true)
        {
            i = 0;

            copiesToCheck.AddRange(toAddToCopiesToCheck);
            copiesToCheckCount = copiesToCheck.Count;
            toAddToCopiesToCheck.Clear();
            toAddToCopiesToCheckCount = copiesToCheck.Count;

            foreach (var copy in copiesToCheck)
            {
                CheckCopy(copy);
                if (i++ % PauseEvery == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            collidersToCheck.AddRange(toAddToCollidersToCheck);
            collidersToCheckCount = collidersToCheck.Count;
            toAddToCollidersToCheck.Clear();
            toAddToCollidersToCheckCount = toAddToCollidersToCheck.Count;
            if (Time.time > automaticRecalculateTime)
            {
                foreach (var collider in collidersToCheck)
                {
                    CheckCollider(collider);
                    if (i++ % PauseEvery == 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckCopy(GameObject copy)
    {
        /*var copyPosition = copy.transform.position;
        var inLod = Mathf.Abs(copyPosition.x - camPos.x) < ViewRange
            && Mathf.Abs(copyPosition.z - camPos.z) < ViewRange;
        if (copy.activeSelf != inLod)
        {
            copy.SetActive(inLod);
        }*/
        /*foreach (Rigidbody rb in copy.GetComponentsInChildren<Rigidbody>())
            GameObject.Destroy(rb);*/

        if (copy == null)
            return;

        if (!copy.activeSelf)
            copy.SetActive(true);
    }

    #region One Without Duration

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// <para>
    /// This uses the Default Center, see <see cref="Center"/>.
    /// </para>
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    public void RecalculateColliders(float range = -1)
    {
        RecalculateColliders(Center, range);
    }

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenter">The center transform around which to activate the colliders. </param>
    public void RecalculateColliders(Transform CollidersCenter, float range = -1)
    {
        RecalculateColliders(CollidersCenter.position);
    }

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenter">The center position around which to activate the colliders. </param>
    public void RecalculateColliders(Vector3 CollidersCenter, float range = -1)
    {
        if (range == -1)
            range = CollidersRange;

        foreach (var collider in collidersToCheck)
        {
            var colliderPosition = collider.transform.position;
            var inLod = Mathf.Abs(colliderPosition.x - CollidersCenter.x) < range
                && Mathf.Abs(colliderPosition.z - CollidersCenter.z) < range;
            if (collider.enabled != inLod)
            {
                collider.enabled = inLod;
            }
        }
    }
    #endregion

    #region One With Duration

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// <para>
    /// The automatic recalculation won't happen again for "<paramref name="Duration"/>" Seconds. 
    /// </para>
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenter">The center transform around which to activate the colliders. </param>
    /// <param name="Duration">The amount, in seconds, for which to halt regular recalculation. </param>
    public void RecalculateCollidersForDuration(Transform CollidersCenter, float Duration, float range = -1)
    {
        RecalculateCollidersForDuration(CollidersCenter.position, Duration, range);
    }

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// <para>
    /// The automatic recalculation won't happen again for "<paramref name="Duration"/>" Seconds. 
    /// </para>
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenter">The center position around which to activate the colliders. </param>
    /// <param name="Duration">The amount, in seconds, for which to halt regular recalculation. </param>
    public void RecalculateCollidersForDuration(Vector3 CollidersCenter, float Duration, float range = -1)
    {
        RecalculateColliders(CollidersCenter, range);
        automaticRecalculateTime = Time.time + Duration;
    }
    #endregion

    #region Multiple Without Duration

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenters">The center transforms around which to activate the colliders. If the collider is in range of any of these, it will be activated. </param>
    public void RecalculateColliders(IEnumerable<Transform> CollidersCenters, float range = -1)
    {
        IEnumerable<Vector3> positions =
            from transform in CollidersCenters
            select transform.position;

        RecalculateColliders(positions, range);
    }

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenters">The center positions around which to activate the colliders. If the collider is in range of any of these, it will be activated. </param>
    public void RecalculateColliders(IEnumerable<Vector3> CollidersCenters, float range = -1)
    {
        if (range == -1)
            range = CollidersRange;

        foreach (var collider in collidersToCheck)
        {
            bool inLod = false;
            Vector3 colliderPosition = collider.transform.position;
            foreach (Vector3 center in CollidersCenters)
            {
                inLod = Mathf.Abs(colliderPosition.x - center.x) < range
                    && Mathf.Abs(colliderPosition.z - center.z) < range;
                if (inLod)
                    break;
            }
            if (collider.enabled != inLod)
            {
                collider.enabled = inLod;
            }
        }
    }
    #endregion

    #region Multiple With Duration

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// <para>
    /// The automatic recalculation won't happen again for "<paramref name="Duration"/>" Seconds. 
    /// </para>
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenters">The center transforms around which to activate the colliders. If the collider is in range of any of these, it will be activated. </param>
    /// <param name="Duration">The amount, in seconds, for which to halt regular recalculation. </param>
    public void RecalculateCollidersForDuration(IEnumerable<Transform> CollidersCenters, float Duration, float range = -1)
    {
        IEnumerable<Vector3> positions =
            from transform in CollidersCenters
            select transform.position;

        RecalculateCollidersForDuration(positions, Duration, range);
    }

    /// <summary>
    /// Recalculate whether every terrain building collider should be active for this frame. 
    /// <para>
    /// The automatic recalculation won't happen again for "<paramref name="Duration"/>" Seconds. 
    /// </para>
    /// </summary>
    /// <param name="range">The range at which to turn on the collider. Leave blank for default. </param>
    /// <param name="CollidersCenters">The center positions around which to activate the colliders. If the collider is in range of any of these, it will be activated. </param>
    /// <param name="Duration">The amount, in seconds, for which to halt regular recalculation. </param>
    public void RecalculateCollidersForDuration(IEnumerable<Vector3> CollidersCenters, float Duration, float range = -1)
    {
        RecalculateColliders(CollidersCenters, range);
        automaticRecalculateTime = Time.time + Duration;
    }
    #endregion

    private void CheckCollider(Collider collider)
    {
        if (collider == null)
            return;

        var colliderPosition = collider.transform.position;
        Vector3 center = Center;
        var inLod = Mathf.Abs(colliderPosition.x - center.x) < CollidersRange
            && Mathf.Abs(colliderPosition.z - center.z) < CollidersRange;
        if (collider.enabled != inLod)
        {
            collider.enabled = inLod;
        }

        //if (collider.enabled)
        //    collider.enabled = false;
    }

}
