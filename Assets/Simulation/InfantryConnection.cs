using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetStructs;
using EventReports;


public class InfantryConnection : MonoBehaviour
{
    public event System.Action OnDeploy;
    public event System.Action OnFold;

    public ExerciseConnection ExerciseConnection;
    public EventReportsManager EventReportsManager;
    public ReflectedEntities ReflectedEntities;

    public DeployFoldDevice.DeviceType DeviceType; 
    public string OperatorMarkingText;
    public ReflectedEntity OperatorReflectedEntity;
    // Use this for initialization

    public PublishedEntity OperatorPublishedEntity;
    //either operator published or reflected entity must exist. if the deployable is deployed in infantry scene,
    //the published entity is used and is set through LoadDeployableScene.cs

    public bool DeployedInInfantryScene = false;

    private EntityId operatorEntityId = EntityId.NullId; 
    
    IEnumerator Start()
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (scene.name == "Infantry")
            {
                DeployedInInfantryScene = true;
            }
        }
        while (!gameObject.scene.isLoaded)
        {
            yield return null;
        }
        yield return null;
        ExerciseConnection = FindObjectOfType<ExerciseConnection>();
        EventReportsManager = ExerciseConnection.GetComponent<EventReportsManager>();
        yield return new WaitForSeconds(0.5f);
        if (EventReportsManager != null)
        {
            EventReportsManager.Subscribe<DeployFoldDevice>(DeployFoldDeviceCallback);
        }

        ReflectedEntities = ExerciseConnection.GetComponent<ReflectedEntities>();

        StartCoroutine(FindOperatorEntityId());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            OnDeploy?.Invoke();
        }
    }

    private void DeployFoldDeviceCallback(DeployFoldDevice dfd)
    {
        Debug.Log("Deploy Callback.");
        if (dfd.deviceType != DeviceType)
            return;

         if (!DeployedInInfantryScene)
                LookForOperatorEntityId();

        if (!dfd.SenderId.Equals(operatorEntityId))
            return;

        //success

        if (dfd.action == DeployFoldDevice.Action.END_DEPLOY)
        {
            OnDeploy();

        }
        else if (dfd.action == DeployFoldDevice.Action.START_FOLD)
        {
            OnFold();
        }
    }

    private IEnumerator FindOperatorEntityId()
    {
        while (operatorEntityId.IsNullId)
        {
            LookForOperatorEntityId();
            yield return new WaitForSeconds(1);
        }
    }

    private void LookForOperatorEntityId()
    {
        OperatorReflectedEntity = ReflectedEntities.LookUpByMarkingText(OperatorMarkingText, false);
        if (null != OperatorReflectedEntity)
            operatorEntityId = OperatorReflectedEntity.EntityId;
    }

    public EntityId OperatorEntityId
    {
        get
        {
            return operatorEntityId;
        }
        set 
        {
            operatorEntityId = value;
        }
    }
}
