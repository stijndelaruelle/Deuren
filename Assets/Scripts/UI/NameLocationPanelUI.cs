using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameLocationPanelUI : MonoBehaviour
{
    [SerializeField]
    private LocationManager m_LocationManager;

    [SerializeField]
    private GameObject m_Visuals;

    [SerializeField]
    private InputField m_InputField;

    private LocationData m_LocationData;

    private void Start()
    {
        m_Visuals.SetActive(false);
        m_LocationManager.NameLocationEvent += OnNameLocation;
    }

    private void OnDestroy()
    {
        if (m_LocationManager != null)
            m_LocationManager.NameLocationEvent -= OnNameLocation;
    }

    private void OnNameLocation(LocationData locationData)
    {
        if (locationData == null)
            return;

        m_LocationData = locationData;

        //Reset text
        m_InputField.text = "";
        m_Visuals.SetActive(true);

        //Show panel
        m_Visuals.transform.localScale = Vector3.zero;
        m_Visuals.transform.DOScale(Vector3.one, 0.25f);
    }

    public void NameLocation(string name)
    {
        if(m_LocationData != null)
            m_LocationData.DisplayName = name;
    }

    public void GotoLocation()
    {
        m_LocationManager.LoadLocation(m_LocationData);
        m_LocationData = null;

        //Fade out menu
        m_Visuals.transform.DOScale(Vector3.zero, 0.25f).OnComplete(OnFadeOutComplete);
    }

    private void OnFadeOutComplete()
    {
        m_Visuals.SetActive(false);
    }
}
