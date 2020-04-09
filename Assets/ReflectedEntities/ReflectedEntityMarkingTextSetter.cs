using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReflectedEntityMarkingTextSetter : MonoBehaviour {

    public TextMesh TextMesh;
    public ReflectedEntity ReflectedEntity;

    private static ReflectedEntities reflectedEntities = null;

	// Use this for initialization
	public void Start() 
    {
        if (null == reflectedEntities)
            reflectedEntities = FindObjectOfType<ReflectedEntities>();

        if (reflectedEntities)
        {
            var publishedEntity = reflectedEntities.PublishedEntity;

            if (publishedEntity)
            {
                if (publishedEntity.ForceType == ReflectedEntity.ForceType)
                {
                    TextMesh.text = Simulation.SwitchMarkingTextsToNames.GetConfiguredName(ReflectedEntity.MarkingText);
                }
                else
                    TextMesh.text = "";
            }
            else
                TextMesh.text = "";
        }
        else
            TextMesh.text = "";
	}
}
