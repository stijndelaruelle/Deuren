using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NameLocationPanelUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private LocationManager m_LocationManager;

    [SerializeField]
    private GameObject m_Visuals;
    private bool m_IsFadingOut;

    [SerializeField]
    private InputField m_InputField;

    [SerializeField]
    private Image m_BackgroundRaycastTarget;

    private LocationData m_LocationData;

    private void Start()
    {
        Hide();
        m_LocationManager.NameLocationEvent += OnNameLocation;
    }

    private void OnDestroy()
    {
        if (m_LocationManager != null)
            m_LocationManager.NameLocationEvent -= OnNameLocation;
    }

    private void OnNameLocation(LocationData locationData)
    {
        m_LocationData = locationData;
        Show();
    }

    public void NameLocation()
    {
        string name = m_InputField.text;

        //The location doesn't yet exist, let's create/find one with the same name
        if (m_LocationData == null)
        {
            m_LocationData = m_LocationManager.GetLocation(name);

            if (m_LocationData == null)
                m_LocationData = m_LocationManager.CreateLocation(name);
        }

        //The location already exists, but didn't have a name yet.
        else
        {
            m_LocationData.DisplayName = name;
        }
    }

    public void GotoLocation()
    {
        m_LocationManager.LoadLocation(m_LocationData);
        m_LocationData = null;

        Hide();
    }
 
    private void Show()
    {
        if (m_IsFadingOut)
            return;

        //Reset text
        m_InputField.text = "";
        m_Visuals.SetActive(true);

        //Enable background raycast
        m_BackgroundRaycastTarget.enabled = true;

        //Show panel
        m_Visuals.transform.localScale = Vector3.zero;
        m_Visuals.transform.DOScale(Vector3.one, 0.25f);
    }

    private void Hide()
    {
        if (m_IsFadingOut)
            return;

        m_IsFadingOut = true;
        m_Visuals.transform.DOScale(Vector3.zero, 0.25f).OnComplete(OnHideComplete);
    }

    private void OnHideComplete()
    {
        m_IsFadingOut = false;

        //Disable raycasttarget
        m_BackgroundRaycastTarget.enabled = false;

        m_Visuals.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //If we click anywhere outside of this window, hide it
        Hide();
    }
}
