using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LocationManager : MonoBehaviour
{
    //Temp, will come from save game
    [SerializeField]
    private LocationData m_StartLocation;

    [SerializeField]
    private string m_RootPath;

    [SerializeField]
    private List<LocationData> m_Locations;

    //Temp, will change to event
    [SerializeField]
    private Text m_LocationText;

    //Pool, no need to generalize just yet
    [SerializeField]
    private InteractableObject m_InteractableObjectPrefab;
    private List<InteractableObject> m_InteractableObjects;

    private void Awake()
    {
        m_InteractableObjects = new List<InteractableObject>();
    }

    private void Start()
    {
        LoadLocation(m_StartLocation);
    }

    public void LoadLocation(LocationData locationData)
    {
        m_LocationText.text = locationData.DisplayName;

        for (int i = 0; i < locationData.InteractableObjects.Count; ++i)
        {
            InteractableObjectAndTransformTuple tuple = locationData.InteractableObjects[i];
            InteractableObject interactableObject = null;

            //Reuse existing interactable objects
            if (i < m_InteractableObjects.Count)
            {
                interactableObject = m_InteractableObjects[i];
            }

            //Create a new object
            else
            {
                interactableObject = GameObject.Instantiate<InteractableObject>(m_InteractableObjectPrefab, this.transform);
                m_InteractableObjects.Add(interactableObject);
            }

            interactableObject.Initialize(this, tuple);
        }
    }


    public void Serialize(JSONNode rootNode)
    {
        //Interactable Objects
        JSONArray locationsJsonArray = new JSONArray();

        if (m_Locations.Count <= 0)
        {
            Debug.LogWarning("Save game warning: No locations present!");
            return;
        }

        foreach (LocationData location in m_Locations)
        {
            location.Serialize(locationsJsonArray);
        }

        rootNode.Add("locations", locationsJsonArray);
    }

    public void Deserialize()
    {

    }
}
