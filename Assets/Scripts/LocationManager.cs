using GestureRecognizer;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public delegate void LocationDelegate(LocationData location);
public delegate void InteractableObjectDelegate(InteractableObject interactableObject);

public class LocationManager : MonoBehaviour
{
    //Temp, will come from save game
    [SerializeField]
    private LocationData m_StartLocation;

    [SerializeField]
    private ObjectPool m_InteractableObjectPool;
    public Transform InteractableObjectRoot
    {
        get
        {
            if (m_InteractableObjectPool != null)
                return m_InteractableObjectPool.transform;
            else
                return null;
        }
    }

    //Visuals
    [SerializeField]
    private ColorAnimator m_Fader;

    [SerializeField]
    private DrawDetector m_DrawDetector;

    //Don't know where else to store it
    [SerializeField]
    private List<LocationData> m_Locations;
    private LocationData m_CurrentLocation;

    public event LocationDelegate CreateLocationEvent;

    public event LocationDelegate StartLoadLocationEvent;
    public event LocationDelegate LoadLocationEvent;
    public event LocationDelegate EndLoadLocationEvent;
    
    public event LocationDelegate NameLocationEvent;

    public event InteractableObjectDelegate LoadInteractableObjectEvent;

    private void Start()
    {
        m_CurrentLocation = m_StartLocation;
        LoadLocationInternal();
    }

    //Locations
    public LocationData GetLocation(string name)
    {
        //Check if there is already a location with this name
        foreach(LocationData data in m_Locations)
        {
            if (data.DisplayName.ToLower() == name.ToLower())
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

        m_CurrentLocation = locationData;

        if (StartLoadLocationEvent != null)
            StartLoadLocationEvent(locationData);

        //Fade it
        m_Fader.FadeIn(LoadLocationInternal);
    }

    private void LoadLocationInternal()
    {
        //Deactivate all the old objects
        m_InteractableObjectPool.DeactivateAll();

        //Activate the new objects
        for (int i = 0; i < m_CurrentLocation.InteractableObjects.Count; ++i)
        {
            InteractableObjectAndTransformTuple tuple = m_CurrentLocation.InteractableObjects[i];
            LoadInteractableObject(tuple);
        }

        if (LoadLocationEvent != null)
            LoadLocationEvent(m_CurrentLocation);

        //Enable / disable drawing
        if (m_DrawDetector != null)
            m_DrawDetector.enabled = (m_CurrentLocation.AllowDrawing);

        //Fade out again
        m_Fader.FadeOut(OnFadeOutComplete);
    }

    private void OnFadeOutComplete()
    {
        if (EndLoadLocationEvent != null)
            EndLoadLocationEvent(m_CurrentLocation);
    }

    //Interactable objects
    public void LoadInteractableObject(Vector2 position, InteractableObjectData data)
    {
        //Create it in the world (use pool)
        InteractableObject interactableObject = (InteractableObject)m_InteractableObjectPool.ActivateAvailableObjectNonDisruptive();

        if (interactableObject == null)
            return;

        if (LoadInteractableObjectEvent != null)
            LoadInteractableObjectEvent(interactableObject);

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
