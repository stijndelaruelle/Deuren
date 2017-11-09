using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreatePanelUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    [Tooltip("The time (in seconds) it takes for the menu to become visible.")]
    private float m_MinHoldTime;

    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private GameObject m_Visuals;

    private bool m_IsHolding = false;
    private float m_HoldTimer;

    private void Start()
    {
        Hide();
    }

    public void Update()
    {
        if (IsShowing() == true)
            return;

        if (m_IsHolding)
        {
            m_HoldTimer += Time.deltaTime;

            if (m_HoldTimer > m_MinHoldTime)
            {
                Show();
            }
        }
    }

    public void Show()
    {
        m_Visuals.SetActive(true);
        m_Visuals.transform.position = Input.mousePosition;
    }

    public void Hide()
    {
        m_Visuals.SetActive(false);
    }

    public bool IsShowing()
    {
        return m_Visuals.activeSelf;
    }

    //Callbacks from pointer
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsShowing())
            Hide();

        m_IsHolding = true;
        m_HoldTimer = 0.0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_IsHolding = false;
    }
}
