using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInteractableObjectButtonUI : MonoBehaviour
{
    [SerializeField]
    private InteractableObjectData m_Data;

    [SerializeField]
    private RectTransform m_RootTransform;

    [SerializeField]
    private RectTransform m_SpawnTransform;

    [SerializeField]
    private LocationManager m_LocationManager;

    public void CreateInteractableObject()
    {
        InteractableObjectData newData = ScriptableObject.Instantiate(m_Data);
        newData.Interaction = ScriptableObject.Instantiate(newData.Interaction); //Create a copy of it's interaction scriptable object

        Vector2 localPosition = m_SpawnTransform.position - m_RootTransform.position;
        m_LocationManager.CreateInteractableObject(localPosition, newData);
    }
}
