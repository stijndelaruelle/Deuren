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
    private LocationData m_CurrentLocation;

    //Pool, no need to generalize just yet
    [SerializeField]
    private ObjectPool m_InteractableObjectPool;

    public event LocationDelegate CreateLocationEvent;
    public event LocationDelegate LoadLocationEvent;
    public event LocationDelegate NameLocationEvent;

    private void Start()
    {
        LoadLocation(m_StartLocation);
    }

    //Locations
    public LocationData GetLocation(string name)
    {
        //Check if there is already a location with this name
        foreach(LocationData data in m_Locations)
        {
            if (data.DisplayName == name)
            {
                return data;
            }
        }

        return null;
    }

    public LocationData CreateLocation(string name)
    {
        LocationData newLocationData = ScriptableObject.CreateInstance<LocationData>();
        newLocationData.DisplayName = name;

        m_Locations.Add(newLocationData);

        if (CreateLocationEvent != null)
            CreateLocationEvent(newLocationData);

        return newLocationData;
    }

    public void LoadLocation(LocationData locationData)
    {
        if (locationData == null || locationData.DisplayName == "")
        {
            if (NameLocationEvent != null)
                NameLocationEvent(locationData);

            return;
        }

        //Deactivate all the old objects
        m_InteractableObjectPool.DeactivateAll();

        //Activate the new objects
        for (int i = 0; i < locationData.InteractableObjects.Count; ++i)
        {
            InteractableObjectAndTransformTuple tuple = locationData.InteractableObjects[i];
            LoadInteractableObject(tuple);
        }

        m_CurrentLocation = locationData;

        if (LoadLocationEvent != null)
            LoadLocationEvent(locationData);
    }


    //Interactable objects
    public void LoadInteractableObject(Vector2 position, InteractableObjectData data)
    {
        //Create it in the world (use pool)
        InteractableObject interactableObject = (InteractableObject)m_InteractableObjectPool.ActivateAvailableObjectNonDisruptive();

        if (interactableObject == null)
            return;

        interactableObject.Initialize(this, position, data);
    }

    public void LoadInteractableObject(InteractableObjectAndTransformTuple tuple)
    {
        LoadInteractableObject(tuple.Position, tuple.Data);
    }

    public void CreateInteractableObject(Vector2 position, InteractableObjectData data)
    {
        LoadInteractableObject(position, data);

        //Add it to the location data
        m_CurrentLocation.AddInteractableObject(position, data);
    }

    public void CreateInteractableObject(InteractableObjectAndTransformTuple tuple)
    {
        CreateInteractableObject(tuple.Position, tuple.Data);
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
