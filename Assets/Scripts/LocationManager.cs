using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public delegate void LocationDelegate(LocationData location);

public class LocationManager : MonoBehaviour
{
    //Temp, will come from save game
    [SerializeField]
    private LocationData m_StartLocation;

    [SerializeField]
    private List<LocationData> m_Locations;

    //Pool, no need to generalize just yet
    [SerializeField]
    private InteractableObject m_InteractableObjectPrefab;
    private List<InteractableObject> m_InteractableObjects;

    public event LocationDelegate LoadLocationEvent;
    public event LocationDelegate NameLocationEvent;

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
        if (locationData.DisplayName == "")
        {
            if (NameLocationEvent != null)
                NameLocationEvent(locationData);

            return;
        }

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

        if (LoadLocationEvent != null)
            LoadLocationEvent(locationData);
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
