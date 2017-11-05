using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class InteractableObject : MonoBehaviour
{
    private InteractableObjectData m_Data;
    private Button m_Button;

    private LocationManager m_LocationManager;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    public void Initialize(LocationManager locationManager, InteractableObjectAndTransformTuple tuple)
    {
        m_LocationManager = locationManager;

        //Position
        transform.localPosition = tuple.Position;

        //Data
        m_Data = tuple.Data;

        if (m_Data != null)
            m_Data.InitializeButton(m_Button);
    }

    public void Interact()
    {
        if (m_Data != null)
            m_Data.Interact(m_LocationManager);
    }
}
