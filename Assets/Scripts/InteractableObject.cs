using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class InteractableObject : PoolableObject
{
    [SerializeField]
    private InteractableObjectData m_Data;
    private Button m_Button;

    private LocationManager m_LocationManager;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    public void Initialize(LocationManager locationManager, Vector2 position, InteractableObjectData data)
    {
        m_LocationManager = locationManager;

        //Position
        transform.localPosition = position;

        //Data
        m_Data = data;

        if (m_Data != null)
            m_Data.InitializeButton(m_Button);
    }

    public void Initialize(LocationManager locationManager, InteractableObjectAndTransformTuple tuple)
    {
        Initialize(locationManager, tuple.Position, tuple.Data);
    }

    public void Interact()
    {
        if (m_Data != null)
            m_Data.Interact(m_LocationManager);
    }


    //PoolableObject
    public override void Initialize()
    {

    }

    public override void Activate()
    {
        gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public override bool IsAvailable()
    {
        return (!gameObject.activeSelf);
    }
}
