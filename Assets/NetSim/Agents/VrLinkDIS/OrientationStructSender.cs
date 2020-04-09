using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//infantry should insert PublishedEntity, and a Device should insert InfantryConnection
public class OrientationStructSender : MonoBehaviour {

    public EventReports.Orientation.Application Application;
    public Camera Camera;
    public EventReportsManager EventReportsManager;
    public PublishedEntity PublishedEntity;
    public InfantryConnection Connection;

    public float SendRate;

    private Coroutine sendRoutine;
	// Update is called once per frame
	void Start() 
    {
        sendRoutine = StartCoroutine(SendOrientationStructCoroutine());
	}

    void OnEnable()
    {
        sendRoutine = StartCoroutine(SendOrientationStructCoroutine());
    }

    void OnDisable()
    {
        StopCoroutine(sendRoutine);
    }

    private IEnumerator SendOrientationStructCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(SendRate);
            sendOrientationStruct();
        }
    }

    private void sendOrientationStruct()
    {
        float pitch = Camera.transform.rotation.eulerAngles.x;
        //if (pitch > 180)
        //    pitch -= 360;
        //pitch *= -1;

        EventReports.Orientation s = new EventReports.Orientation()
        {
            applicationType = Application,
            orientationUtm = new Vector3(
                pitch,
                Camera.transform.rotation.eulerAngles.y,//heading
                Camera.transform.rotation.eulerAngles.z),//roll
            vFov = Camera.fieldOfView,
            hFov = Mathf.Rad2Deg * (2 * Mathf.Atan(Mathf.Tan((Camera.fieldOfView / 2) * Mathf.Deg2Rad) * Camera.aspect))//google
        };
        if(PublishedEntity!=null)
            s.SenderId = PublishedEntity.MyEntityId;
        else if(Connection!=null)
            s.SenderId = Connection.OperatorReflectedEntity.EntityId;
        EventReportsManager.Send(s);
    }
}
