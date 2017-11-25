using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InteractableObject : PoolableObject, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler ,IPointerUpHandler, IDropHandler
{
    [SerializeField]
    private InteractableObjectData m_Data;
    public InteractableObjectData Data
    {
        get { return m_Data; }
    }

    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private AudioSource m_AudioSource;

    private LocationManager m_LocationManager;
    public LocationManager LocationManager
    {
        get { return m_LocationManager; }
    }

    private bool m_IsDragging = false;

    private RectTransform m_RectTransform;
    public TooltipDelegate TooltipEvent;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(LocationManager locationManager, Vector2 position, InteractableObjectData data)
    {
        m_LocationManager = locationManager;

        //Position
        transform.localPosition = position;

        SetData(data);
    }

    public void Initialize(LocationManager locationManager, InteractableObjectAndTransformTuple tuple)
    {
        Initialize(locationManager, tuple.Position, tuple.Data);
    }

    public void SetData(InteractableObjectData data)
    {
        //Data
        m_Data = data;

        if (m_Data == null)
        {
            m_Image.enabled = false;
            m_Text.enabled = false;
            return;
        }

        //Use data
        m_Image.sprite = m_Data.DefaultSprite;
        m_Text.text = m_Data.DisplayName;

        //Display (only image, text as fallback)
        m_Image.enabled = (m_Data.DefaultSprite != null);
        m_Text.enabled = !m_Image.enabled;
    }

    public void SetTooltip(string text, float duration)
    {
        if (TooltipEvent != null)
            TooltipEvent(text, duration, new Vector2(m_RectTransform.position.x, m_RectTransform.position.y - (m_RectTransform.sizeDelta.y * 0.5f)));
    }

    public void PlaySound(AudioClip audioClip)
    {
        m_AudioSource.clip = audioClip;
        m_AudioSource.Play();
    }


    //Interactions
    public void Interact()
    {
        if (m_Data != null)
        {
            bool success = m_Data.Interact(this);

            if (success)
            {
                if (m_Data.InteractSprite != null)
                    m_Image.sprite = m_Data.InteractSprite;
            }
        }
    }

    public void DropInteract(InteractableObject interactableObject)
    {
        if (m_Data != null)
            m_Data.DropInteract(this, interactableObject);
    }


    //UI Interfaces
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_IsDragging == false)
        {
            Interact();
        }

        m_IsDragging = false;
    }


    //Dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_Data.AllowDragging == false)
            return;

        m_Image.raycastTarget = false;
        m_Text.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_Data.AllowDragging == false)
            return;

        m_IsDragging = true;

        Vector3 newPosition = transform.position;
        newPosition.x += eventData.delta.x;
        newPosition.y += eventData.delta.y;

        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_Data.AllowDragging == false)
            return;

        m_Image.raycastTarget = true;
        m_Text.raycastTarget = true;
    }


    //Dropping
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InteractableObject otherInteractableObject = eventData.pointerDrag.GetComponent<InteractableObject>();

            if (otherInteractableObject != null)
                DropInteract(otherInteractableObject);
        }
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
