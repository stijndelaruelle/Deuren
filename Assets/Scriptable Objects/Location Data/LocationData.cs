using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Custom/Location Data")]
public class LocationData : ScriptableObject
{
    [SerializeField]
    private string m_DisplayName;
    public string DisplayName
    {
        get { return m_DisplayName; }
    }

    [SerializeField]
    private List<InteractableObjectAndTransformTuple> m_InteractableObjects;
    public List<InteractableObjectAndTransformTuple> InteractableObjects
    {
        get { return m_InteractableObjects; }
    }

    public void Serialize(JSONArray rootArray)
    {
        JSONClass jsonClass = new JSONClass();

        jsonClass.Add("name", new JSONData(m_DisplayName));

        //Interactable Objects
        JSONArray interactableObjectsJsonArray = new JSONArray();
        foreach (InteractableObjectAndTransformTuple tuple in m_InteractableObjects)
        {
            tuple.Serialize(interactableObjectsJsonArray);
        }

        jsonClass.Add("interactable_objects", interactableObjectsJsonArray);

        rootArray.Add(jsonClass);
    }

    public void Deserialize(JSONNode rootNode)
    {

    }
}

[Serializable]
public struct InteractableObjectAndTransformTuple
{
    [SerializeField]
    private Vector2 m_Position;
    public Vector2 Position
    {
        get { return m_Position; }
    }

    [SerializeField]
    private InteractableObjectData m_Data;
    public InteractableObjectData Data
    {
        get { return m_Data; }
    }

    public void Serialize(JSONArray rootArray)
    {
        JSONClass jsonClass = new JSONClass();

        jsonClass.Add("position_x", new JSONData(m_Position.x));
        jsonClass.Add("position_y", new JSONData(m_Position.y));

        m_Data.Serialize(jsonClass);

        rootArray.Add(jsonClass);
    }

    public void Deserialize(JSONArray rootArray)
    {

    }
}